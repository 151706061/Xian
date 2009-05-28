﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Interactive builder class that interprets mouse clicks as ordered
	/// vertices to setup a closed <see cref="IPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// This builder takes input until the the maximum number of
	/// vertices is reached, the user clicks near the first point
	/// or the user double clicks anywhere, after which the graphic is complete
	/// and control is released. A visual cue is shown when the cursor is close
	/// enough to the first point to snap and close the polygon.
	/// </remarks>
	public class InteractivePolygonGraphicBuilder : InteractiveGraphicBuilder
	{
		private readonly int _maximumVertices;
		private float _snapRadius = 15f;
		private int _numberOfPointsAnchored = 0;

		private SnapPointGraphic _snapPoint;

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractivePolygonGraphicBuilder(IPointsGraphic pointsGraphic)
			: this(int.MaxValue, pointsGraphic) {}

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="maximumVertices">The maximum number of vertices to accept.</param>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maximumVertices"/> is less than 3.</exception>
		public InteractivePolygonGraphicBuilder(int maximumVertices, IPointsGraphic pointsGraphic)
			: base(pointsGraphic)
		{
			Platform.CheckArgumentRange(maximumVertices, 3, int.MaxValue, "maximumVertices");
			_maximumVertices = maximumVertices;
		}

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IPointsGraphic Graphic
		{
			get { return (IPointsGraphic) base.Graphic; }
		}

		/// <summary>
		/// Gets or the sets the radius of the snap-to feature in destination pixels.
		/// </summary>
		public float SnapRadius
		{
			get { return _snapRadius; }
			set { _snapRadius = value; }
		}

		protected override void OnGraphicCancelled()
		{
			base.OnGraphicCancelled();
			InstallSnapPointGraphic(false);
		}

		protected override void NotifyGraphicComplete()
		{
			InstallSnapPointGraphic(false);
			base.NotifyGraphicComplete();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_numberOfPointsAnchored++;

			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}
			// We're done creating
			else if (_numberOfPointsAnchored >= 5 && mouseInformation.ClickCount >= 2)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					this.Graphic.Points.RemoveAt(--_numberOfPointsAnchored);
					this.Graphic.Points[--_numberOfPointsAnchored] = this.Graphic.Points[0];
				}
				finally
				{
					this.Graphic.ResetCoordinateSystem();
				}

				this.NotifyGraphicComplete();
			}
			// We're done creating
			else if (_numberOfPointsAnchored >= 4 && AtOrigin(mouseInformation.Location) && mouseInformation.ClickCount == 1)
			{
				this.NotifyGraphicComplete();
			}
			// We're done creating
			else if (_numberOfPointsAnchored >= 3 && _numberOfPointsAnchored >= _maximumVertices && mouseInformation.ClickCount == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(this.Graphic.Points[0]);
				this.Graphic.ResetCoordinateSystem();
				this.NotifyGraphicComplete();
			}
			// We're in the middle of creating
			else if (_numberOfPointsAnchored >= 2 && mouseInformation.ClickCount == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}
			else if (mouseInformation.ClickCount > 1)
			{
				// removes the extra click if the user multi-clicks and it doesn't otherwise have any meaning
				_numberOfPointsAnchored--;
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;

			if (_numberOfPointsAnchored >= 3 && AtOrigin(mouseInformation.Location))
			{
				this.Graphic.Points[_numberOfPointsAnchored] = this.Graphic.Points[0];
				InstallSnapPointGraphic(true);
			}
			else
			{
				this.Graphic.Points[_numberOfPointsAnchored] = mouseInformation.Location;
				InstallSnapPointGraphic(false);
			}

			this.Graphic.ResetCoordinateSystem();
			this.Graphic.Draw();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		private bool AtOrigin(PointF point)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			double distanceToOrigin = Vector.Distance(this.Graphic.Points[0], point);
			this.Graphic.ResetCoordinateSystem();
			return distanceToOrigin < _snapRadius;
		}

		private void InstallSnapPointGraphic(bool install)
		{
			IPointsGraphic subject = this.Graphic;
			CompositeGraphic parent = subject.ParentGraphic as CompositeGraphic;
			if (parent == null)
				return;

			if (install && subject.Points.Count > 0)
			{
				if (_snapPoint == null)
					_snapPoint = new SnapPointGraphic();

				if (!parent.Graphics.Contains(_snapPoint))
					parent.Graphics.Add(_snapPoint);

				_snapPoint.CoordinateSystem = subject.CoordinateSystem;
				_snapPoint.Location = subject.Points[0];
				_snapPoint.ResetCoordinateSystem();

				parent.Draw();
			}
			else if (_snapPoint != null && parent.Graphics.Contains(_snapPoint))
			{
				parent.Graphics.Remove(_snapPoint);
				parent.Draw();
			}
		}

		/// <summary>
		/// A graphic for indicating that the cursor is close enough to an existing point to snap to it.
		/// </summary>
		private class SnapPointGraphic : CompositeGraphic
		{
			private readonly InvariantEllipsePrimitive _circle;
			private PointF _location;

			internal SnapPointGraphic()
			{
				_circle = new InvariantEllipsePrimitive();
				_circle.Color = Color.Tomato;
				_circle.InvariantTopLeft = new PointF(-6, -6);
				_circle.InvariantBottomRight = new PointF(6, 6);

				this.Graphics.Add(_circle);
			}

			protected override void Dispose(bool disposing)
			{
				_circle.Dispose();
				base.Dispose(disposing);
			}

			/// <summary>
			/// Gets the coordinates of the point.
			/// </summary>
			public PointF Location
			{
				get
				{
					if (base.CoordinateSystem == CoordinateSystem.Source)
						return _location;
					else
						return base.SpatialTransform.ConvertToDestination(_location);
				}
				internal set
				{
					if (!FloatComparer.AreEqual(this.Location, value))
					{
						Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");

						if (base.CoordinateSystem == CoordinateSystem.Source)
							_location = value;
						else
							_location = base.SpatialTransform.ConvertToSource(value);

						_circle.AnchorPoint = this.Location;
					}
				}
			}

			/// <summary>
			/// Gets or sets the radius of the snap graphic.
			/// </summary>
			public float Radius
			{
				get { return _circle.InvariantBottomRight.X; }
				set
				{
					_circle.InvariantTopLeft = new PointF(-value, -value);
					_circle.InvariantBottomRight = new PointF(value, value);
				}
			}

			/// <summary>
			/// Gets or sets the colour of the snap graphic.
			/// </summary>
			public Color Color
			{
				get { return _circle.Color; }
				set { _circle.Color = value; }
			}

			/// <summary>
			/// Performs a hit test on the snap graphic at given point.
			/// </summary>
			/// <param name="point">The mouse position in destination coordinates.</param>
			/// <returns>
			/// <b>True</b> if <paramref name="point"/> is inside the snap graphic's radius, <b>false</b> otherwise.
			/// </returns>
			public override bool HitTest(Point point)
			{
				base.CoordinateSystem = CoordinateSystem.Source;
				bool result = FloatComparer.IsLessThan((float) Vector.Distance(base.SpatialTransform.ConvertToSource(point), this.Location), this.Radius);
				base.ResetCoordinateSystem();

				return result;
			}
		}
	}
}