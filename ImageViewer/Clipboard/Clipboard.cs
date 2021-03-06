#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// A global image clipboard.
	/// </summary>
	/// <remarks>
	/// The clipboard can be thought of as a "holding area" for images the user has deemed to
	/// be of interest. Clipboard tools can then operate on those images.
	/// </remarks>
	public static class Clipboard
	{
		/// <summary>
		/// Constant defining the action model site for the toolbar on the <see cref="ClipboardComponent"/>.
		/// </summary>
		public const string ClipboardSiteToolbar = "clipboard-toolbar";

		/// <summary>
		/// Constant defining the action model site for the context menu of the <see cref="ClipboardComponent"/>.
		/// </summary>
		public const string ClipboardSiteMenu = "clipboard-contextmenu";
		
		internal static readonly BindingList<IClipboardItem> Items = new BindingList<IClipboardItem>();

		/// <summary>
		/// Adds an <see cref="IPresentationImage"/> to the clipboard.
		/// </summary>
		/// <param name="image"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			Items.Add(ClipboardComponent.CreatePresentationImageItem(image));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IDisplaySet"/> is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IDisplaySet"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");

			Items.Add(ClipboardComponent.CreateDisplaySetItem(displaySet));
		}

		/// <summary>
		/// Adds an <see cref="IDisplaySet"/> to the clipboard.
		/// </summary>
		/// <param name="displaySet"></param>
		/// <param name="selectionStrategy"></param>
		/// <remarks>
		/// When called, a copy of the specified <see cref="IPresentationImage"/>s
		/// (as determined by the <paramref name="selectionStrategy"/>) is made and stored
		/// in the clipbaord.  This ensures that the <see cref="IPresentationImage"/> is in fact a
		/// snapshot and not a reference that could be changed in unpredictable ways.
		/// Pixel data, however, is not replicated.
		/// </remarks>
		public static void Add(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			Platform.CheckForNullReference(selectionStrategy, "selectionStrategy");

			Items.Add(ClipboardComponent.CreateDisplaySetItem(displaySet, selectionStrategy));
		}
	}
}
