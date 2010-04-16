﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint]
	public class ViewerContextMenuFilterExtensionPoint : ExtensionPoint<IViewerActionFilter>
	{}

	//TODO (CR Mar 2010): Predicate!
	public interface IViewerActionFilter
	{
		void SetImageViewer(IImageViewer imageViewer);
        
		bool Evaluate(IAction action);
	}

	public abstract class ViewerActionFilter : IViewerActionFilter
	{
		private class AlwaysTrueFilter : IViewerActionFilter
		{
			#region IViewerActionFilter Members

			public void SetImageViewer(IImageViewer imageViewer)
			{
			}

			public bool Evaluate(IAction action)
			{
				return true;
			}

			#endregion
		}

		public static readonly IViewerActionFilter Null = new AlwaysTrueFilter();

		protected ViewerActionFilter()
		{}

		protected IImageViewer ImageViewer { get; private set; }

		#region IViewerActionFilter Members

		void IViewerActionFilter.SetImageViewer(IImageViewer imageViewer)
		{
			ImageViewer = imageViewer;
		}

		public abstract bool Evaluate(IAction action);

		#endregion

		public static IViewerActionFilter CreateContextMenuFilter()
		{
			try
			{
				return (IViewerActionFilter)new ViewerContextMenuFilterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return Null;
			}
		}
	}
}
