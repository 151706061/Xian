#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
    //public interface IAdjustableDataLut : IDataLut, IBasicVoiLutLinear
    //{
    //    double BrightnessPercent { get; }
    //    double ContrastPercent { get; }
    //}

	/// <summary>
	/// A class that wraps a <see cref="DataLut"/> inside an <see cref="IBasicVoiLutLinear"/>, in
	/// order to allow 'window/levelling' of the <see cref="DataLut"/>.  
	/// </summary>
	/// <remarks>
	/// Internally, this will be treated like any other linear lut, except that
	/// <see cref="GetDescription"/> expresses the Window Width/Center as a percentage of 
	/// the full window, since the true values won't necessarily have any real meaning.
	/// </remarks>
	[Cloneable]
    public class AdjustableDataLut : ComposableVoiLut, IDataLut, IBasicVoiLutLinear
	{
		private class Memento
		{
			public readonly object DataLutMemento;
			public readonly object LinearLutMemento;
			
			public Memento(object dataLutMemento, object linearLutMemento)
			{
				DataLutMemento = dataLutMemento;
				LinearLutMemento = linearLutMemento;
			}

			public override int GetHashCode()
			{
				int dataLutHash = 0x7D60C4F1;
				int linearLutHash = 0x081B32C5;
				if (this.DataLutMemento != null)
					dataLutHash = this.DataLutMemento.GetHashCode();
				if (this.LinearLutMemento != null)
					linearLutHash = this.LinearLutMemento.GetHashCode();
				return dataLutHash ^ linearLutHash ^ 0x273FB457;
			}

			public override bool Equals(object obj)
			{
				if (Object.ReferenceEquals(obj, this))
					return true;

				if (obj is Memento)
				{
					Memento other = obj as Memento;
					return Object.Equals(other.DataLutMemento, DataLutMemento) && 
							Object.Equals(other.LinearLutMemento, LinearLutMemento);
				}

				return false;
			}
		}

		#region Private Fields

		private double _minInputValue;
		private double _maxInputValue;

		private readonly DataLut _dataLut;
		private readonly BasicVoiLutLinear _linearLut;

		[CloneIgnore]
		private int[] _lutDataCache = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public AdjustableDataLut(DataLut dataLut)
		{
			Platform.CheckForNullReference(dataLut, "dataLut");

			_dataLut = dataLut;
			_dataLut.LutChanged += OnDataLutChanged;

			_linearLut = new BasicVoiLutLinear();
			_linearLut.LutChanged += OnLinearLutChanged;

			Reset();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected AdjustableDataLut(AdjustableDataLut source, ICloningContext context)
		{
			context.CloneFields(source, this);

			Platform.CheckForNullReference(_dataLut, "_dataLut");
			Platform.CheckForNullReference(_linearLut, "_linearLut");

			_linearLut.LutChanged += OnLinearLutChanged;
			_dataLut.LutChanged += OnDataLutChanged;
		}

		#endregion

		#region Private Properties

		private double FullWindow
		{
			get { return _linearLut.MaxInputValue - _linearLut.MinInputValue + 1; }
		}

		#endregion

		#region Private Methods

		private void OnDataLutChanged(object sender, EventArgs e)
		{
			UpdateMinMaxInputLinear();
			this.OnLutChanged();
		}

		private void OnLinearLutChanged(object sender, EventArgs e)
		{
			this.OnLutChanged();
		}

		private void UpdateMinMaxInputLinear()
		{
			_linearLut.MinInputValue = _dataLut.MinOutputValue;
			_linearLut.MaxInputValue = _dataLut.MaxOutputValue;
		}

		#endregion

        #region IBasicVoiLutLinear Members

        /// <summary>
        /// Gets or sets the Window Width.
        /// </summary>
        double IBasicVoiLutLinear.WindowWidth
        {
            get { return _linearLut.WindowWidth; }
            set { _linearLut.WindowWidth = value; }
        }

        /// <summary>
        /// Gets or sets the Window Center.
        /// </summary>
        double IBasicVoiLutLinear.WindowCenter
        {
            get { return _linearLut.WindowCenter; }
            set { _linearLut.WindowCenter = value; }
        }

        #endregion

        #region IBasicVoiLutLinear Members

        /// <summary>
        /// Gets or sets the Window Width.
        /// </summary>
        double IVoiLutLinear.WindowWidth
        {
            get { return _linearLut.WindowWidth; }
        }

        /// <summary>
        /// Gets or sets the Window Center.
        /// </summary>
        double IVoiLutLinear.WindowCenter
        {
            get { return _linearLut.WindowCenter; }
        }

        #endregion

        #region Public Properties

		/// <summary>
		/// Gets the underlying data lut.
		/// </summary>
		public DataLut DataLut
		{
			get { return _dataLut; }	
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override double MinInputValue
		{
			get { return _minInputValue; }
			set { _dataLut.MinInputValue = (int) Math.Round(_minInputValue = value); }
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public override double MaxInputValue
		{
			get { return _maxInputValue; }
			set { _dataLut.MaxInputValue = (int) Math.Round(_maxInputValue = value); }
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		public override int MinOutputValue
		{
			get { return _linearLut.MinOutputValue; }
			protected set { throw new InvalidOperationException(SR.ExceptionMinimumOutputValueIsNotSettable); } 
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		public override int MaxOutputValue
		{
			get { return _linearLut.MaxOutputValue; }
			protected set { throw new InvalidOperationException(SR.ExceptionMaximumOutputValueIsNotSettable); }
		}

        //TODO: later, add IContrastBrightnessLut and allow the properties to be set.

        /// <summary>
        /// The brightness as a percentage.
        /// </summary>
        /// <remarks>
        /// This property is currently only settable by casting to <see cref="IBasicVoiLutLinear"/>.
        /// </remarks>
        public double BrightnessPercent
        {
            get { return 100 - (_linearLut.WindowCenter - _linearLut.MinInputValue) / FullWindow * 100; }
        }

        /// <summary>
        /// The contrast as a percentage.
        /// </summary>
        /// <remarks>
        /// This property is currently only settable by casting to <see cref="IBasicVoiLutLinear"/>.
        /// </remarks>
        public double ContrastPercent
        {
            get { return _linearLut.WindowWidth / FullWindow * 100; }
        }

		#endregion

		/// <summary>
		/// Gets the output value of the lut at a given input.
		/// </summary>
		public override int this[double input]
		{
			get { return _linearLut[_dataLut[(int) Math.Round(input)]]; }
		}

		#region Public Methods

		/// <summary>
		/// Resets the 
		/// </summary>
		public void Reset()
		{
			UpdateMinMaxInputLinear();

			_linearLut.WindowWidth = FullWindow;
			_linearLut.WindowCenter = _dataLut.MinOutputValue + FullWindow / 2;

			this.OnLutChanged();
		}

		/// <summary>
		/// Gets a string key that identifies this particular Lut's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		public override string GetKey()
		{
			return String.Format("{0}:{1}", _dataLut.GetKey(), _linearLut.GetKey());
		}

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return String.Format(SR.FormatAdjustableDataLutDescription, _dataLut.GetDescription(), ContrastPercent, BrightnessPercent);
		}

		/// <summary>
		/// Returns null.
		/// </summary>
		/// <remarks>
		/// Override this member only when necessary.  If this method is overridden, <see cref="ComposableLutBase.SetMemento"/> must also be overridden.
		///  </remarks>
		/// <returns>null, unless overridden.</returns>
		public override object CreateMemento()
		{
			return new Memento(_dataLut.CreateMemento(), _linearLut.CreateMemento());
		}

		/// <summary>
		/// Does nothing unless overridden.
		/// </summary>
		/// <remarks>
		/// If you override <see cref="ComposableLutBase.CreateMemento"/> to capture the Lut's state, you must also override this method
		/// to allow the state to be restored.
		/// </remarks>
		/// <param name="memento">The memento object from which to restore the Lut's state.</param>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="memento"/> is <B>not</B> null, 
		/// which would indicate that <see cref="ComposableLutBase.CreateMemento"/> has been overridden, but <see cref="ComposableLutBase.SetMemento"/> has not.</exception>
		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			Memento lutMemento = (Memento) memento;

			if (lutMemento.DataLutMemento != null)
				_dataLut.SetMemento(lutMemento.DataLutMemento);
			
			if (lutMemento.LinearLutMemento != null)
				_linearLut.SetMemento(lutMemento.LinearLutMemento);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Fires the <see cref="ComposableLutBase.LutChanged"/> event.
		/// </summary>
		/// <remarks>
		/// Inheritors should call this method when any property of the Lut has changed.
		/// </remarks>
		protected override void OnLutChanged()
		{
			// when something changes, wipe the cached lut array
			_lutDataCache = null;

			base.OnLutChanged();
		}

		#endregion

		#region IDataLut Members

		int IDataLut.this[int index]
		{
			get { return this[index]; }
		}

		int IDataLut.MinInputValue
		{
			get { return (int) Math.Round(MinInputValue); }
			set { MinInputValue = value; }
		}

		int IDataLut.MaxInputValue
		{
			get { return (int) Math.Round(MaxInputValue); }
			set { MaxInputValue = value; }
		}

		int IDataLut.FirstMappedPixelValue
		{
			get { return _dataLut.FirstMappedPixelValue; }
		}

		int[] IDataLut.Data
		{
			get
			{
				if (_lutDataCache == null)
				{
					int lutLength = _dataLut.Length;
					int[] lutData = new int[lutLength];

					unsafe
					{
						fixed (int* output = lutData)
						{
							fixed (int* input = _dataLut.Data)
							{
								for (int n = 0; n < lutLength; n++)
									output[n] = _linearLut[input[n]];
							}
						}
					}

					_lutDataCache = lutData;
				}

				return _lutDataCache;
			}
		}

		IDataLut IDataLut.Clone()
		{
			return (IDataLut) Clone();
		}

		#endregion
	}
}
