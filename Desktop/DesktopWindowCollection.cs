#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="DesktopWindow"/> objects in the application.
    /// </summary>
    public sealed class DesktopWindowCollection : DesktopObjectCollection<DesktopWindow>
    {
        private Application _owner;
        private DesktopWindow _activeWindow;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DesktopWindowCollection(Application owner)
        {
            _owner = owner;
        }

        #region Public methods

        /// <summary>
        /// Gets the currently active window.
        /// </summary>
        public DesktopWindow ActiveWindow
        {
            get { return _activeWindow; }
        }

        /// <summary>
        /// Opens a new unnamed desktop window.
        /// </summary>
        public DesktopWindow AddNew()
        {
            return AddNew(new DesktopWindowCreationArgs(null, null));
        }

        /// <summary>
        /// Opens a new desktop window with the specified name.
        /// </summary>
        /// <remarks>
        /// <see cref="DesktopWindow"/> names must be unique within a collection or an exception will be thrown.
        /// </remarks>
        public DesktopWindow AddNew(string name)
        {
            return AddNew(new DesktopWindowCreationArgs(null, name));
        }

        /// <summary>
        /// Opens a new desktop window with the specified creation arguments.
        /// </summary>
        public DesktopWindow AddNew(DesktopWindowCreationArgs args)
        {
            DesktopWindow window = CreateWindow(args);
            Open(window);
            return window;
        }

        #endregion

        #region Protected overrides

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				_activeWindow = null;
		}

		/// <summary>
		/// Called when a <see cref="DesktopWindow"/> item's <see cref="DesktopObject.InternalActiveChanged"/> event
		/// has fired.
		/// </summary>
		protected sealed override void OnItemActivationChangedInternal(ItemEventArgs<DesktopWindow> args)
        {
            if (args.Item.Active)
            {
                // activated
                DesktopWindow lastActive = _activeWindow;

                // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                _activeWindow = args.Item;

                if (lastActive != null)
                {
                    lastActive.RaiseActiveChanged();
                }
                _activeWindow.RaiseActiveChanged();

            }
        }

		/// <summary>
		/// Called when a <see cref="DesktopWindow"/> item's <see cref="DesktopObject.Closed"/> event
		/// has fired.
		/// </summary>
		protected sealed override void OnItemClosed(ClosedItemEventArgs<DesktopWindow> args)
        {
            if (this.Count == 0)
            {
                // raise pending de-activation event for the last active workspace, before the closing event
                if (_activeWindow != null)
                {
                    DesktopWindow lastActive = _activeWindow;

                    // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                    _activeWindow = null;
                    lastActive.RaiseActiveChanged();
                }
            }

            base.OnItemClosed(args);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="DesktopWindow"/>.
        /// </summary>
        private DesktopWindow CreateWindow(DesktopWindowCreationArgs args)
        {
            IDesktopWindowFactory factory = CollectionUtils.FirstElement<IDesktopWindowFactory>(
                (new DesktopWindowFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDesktopWindowFactory();

            return factory.CreateWindow(args, _owner);
        }

    }
}
