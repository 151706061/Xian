using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="NavigatorComponent"/>
    /// </summary>
    public class NavigatorComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// An application component that acts as a container for other application components.
    /// The child components are treated as "pages", where each page is a node in a tree.
    /// Only one page is displayed at a time, however, a navigation tree is provided on the side
    /// to aid the user in navigating the set of pages.
    /// </summary>
    [AssociateView(typeof(NavigatorComponentContainerViewExtensionPoint))]
    public class NavigatorComponentContainer : PagedComponentContainer<NavigatorPage>
    {
        private bool _forwardEnabled;
        private event EventHandler _forwardEnabledChanged;

        private bool _backEnabled;
        private event EventHandler _backEnabledChanged;

        private bool _acceptEnabled;
        private event EventHandler _acceptEnabledChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NavigatorComponentContainer()
        {
        }

        #region Presentation Model


        /// <summary>
        /// Advances to the next page
        /// </summary>
        public void Forward()
        {
            MoveTo(this.CurrentPageIndex + 1);
        }

        /// <summary>
        /// Indicates whether it is possible to advance one page.  True unless the current
        /// page is the last page.
        /// </summary>
        public bool ForwardEnabled
        {
            get { return _forwardEnabled; }
            protected set
            {
                if (_forwardEnabled != value)
                {
                    _forwardEnabled = value;
                    EventsHelper.Fire(_forwardEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="ForwardEnabled"/> property has changed.
        /// </summary>
        public event EventHandler ForwardEnabledChanged
        {
            add { _forwardEnabledChanged += value; }
            remove { _forwardEnabledChanged -= value; }
        }

        /// <summary>
        /// Sets the current page back to the previous page.
        /// </summary>
        public void Back()
        {
            MoveTo(this.CurrentPageIndex - 1);
        }

        /// <summary>
        /// Indicates whether it is possible to go back one page.  True unless the current page
        /// is the first page.
        /// </summary>
        public bool BackEnabled
        {
            get { return _backEnabled; }
            protected set
            {
                if (_backEnabled != value)
                {
                    _backEnabled = value;
                    EventsHelper.Fire(_backEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="BackEnabled"/> property has changed.
        /// </summary>
        public event EventHandler BackEnabledChanged
        {
            add { _backEnabledChanged += value; }
            remove { _backEnabledChanged -= value; }
        }
        
        /// <summary>
        /// Causes the component to exit, accepting any changes made by the user. Override this method
        /// if desired.
        /// </summary>
        public virtual void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        /// <summary>
        /// Indicates whether the accept button should be enabled.
        /// </summary>
        public bool AcceptEnabled
        {
            get { return _acceptEnabled; }
            protected set
            {
                if (_acceptEnabled != value)
                {
                    _acceptEnabled = value;
                    EventsHelper.Fire(_acceptEnabledChanged, this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Notifies that the <see cref="AcceptEnabled"/> property has changed.
        /// </summary>
        public event EventHandler AcceptEnabledChanged
        {
            add { _acceptEnabledChanged += value; }
            remove { _acceptEnabledChanged -= value; }
        }

        /// <summary>
        /// Causes the component to exit, discarding any changes made by the user.  Override this method
        /// if desired.
        /// </summary>
        public virtual void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion


        /// <summary>
        /// Moves to the page at the specified index
        /// </summary>
        /// <param name="index"></param>
        protected override void MoveTo(int index)
        {
            base.MoveTo(index);

            this.ForwardEnabled = (this.CurrentPageIndex < this.Pages.Count - 1);
            this.BackEnabled = (this.CurrentPageIndex > 0);
        }

        protected override void OnComponentModifiedChanged(IApplicationComponent component)
        {
            base.OnComponentModifiedChanged(component);
            this.AcceptEnabled = this.Modified;
        }
    }
}
