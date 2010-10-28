#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
	/// Form used by the <see cref="DesktopWindowView"/> class.
    /// </summary>
    /// <remarks>
    /// This class may be subclassed.
    /// </remarks>
    public partial class DesktopForm : DotNetMagicForm
    {
        private ActionModelNode _menuModel;
        private ActionModelNode _toolbarModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopForm()
        {
#if !MONO
			SplashScreenManager.DismissSplashScreen(this);
#endif
			InitializeComponent();

			// manually subscribe this event handler *after* the call to InitializeComponent()
			_toolbar.ParentChanged += OnToolbarParentChanged;

            _dockingManager = new DockingManager(_toolStripContainer.ContentPanel, VisualStyle.IDE2005);
            _dockingManager.ActiveColor = SystemColors.Control;
            _dockingManager.InnerControl = _tabbedGroups;
			_dockingManager.TabControlCreated += OnDockingManagerTabControlCreated;

			_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
			_tabbedGroups.TabControlCreated += OnTabbedGroupsTabControlCreated;

			if (_tabbedGroups.ActiveLeaf != null)
			{
				InitializeTabControl(_tabbedGroups.ActiveLeaf.TabControl);
			}

			ToolStripSettings.Default.PropertyChanged += OnToolStripSettingsPropertyChanged;
			OnToolStripSettingsPropertyChanged(ToolStripSettings.Default, new PropertyChangedEventArgs("WrapLongToolstrips"));
			OnToolStripSettingsPropertyChanged(ToolStripSettings.Default, new PropertyChangedEventArgs("IconSize"));
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the menu model.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Menu, _mainMenu, _menuModel);
            }
        }

        /// <summary>
        /// Gets or sets the toolbar model.
        /// </summary>
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _toolbar, _toolbarModel);
            }
        }

        /// <summary>
        /// Gets the <see cref="TabbedGroups"/> object that manages workspace tab groups.
        /// </summary>
        public TabbedGroups TabbedGroups
        {
            get { return _tabbedGroups; }
        }

        /// <summary>
        /// Gets the <see cref="DockingManager"/> object that manages shelf docking windows.
        /// </summary>
        public DockingManager DockingManager
        {
            get { return _dockingManager; }
        }

        #endregion

        #region Form event handlers

        private void OnTabbedGroupsTabControlCreated(TabbedGroups tabbedGroups, Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

        private void OnDockingManagerTabControlCreated(Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

    	private void OnToolStripSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    	{
    		ToolStripSettings settings = ToolStripSettings.Default;
    		if (e.PropertyName == "WrapLongToolstrips" || e.PropertyName == "ToolStripDock")
    		{
				// handle both wrapping and docking together because both affect flow direction
    			bool verticalOrientation = ReferenceEquals(_toolbar.Parent, _toolStripContainer.LeftToolStripPanel)
    			                           || ReferenceEquals(_toolbar.Parent, _toolStripContainer.RightToolStripPanel);

    			_toolbar.SuspendLayout();
    			_toolbar.LayoutStyle = settings.WrapLongToolstrips ? ToolStripLayoutStyle.Flow : ToolStripLayoutStyle.StackWithOverflow;
    			if (settings.WrapLongToolstrips)
    				((FlowLayoutSettings) _toolbar.LayoutSettings).FlowDirection = verticalOrientation ? FlowDirection.TopDown : FlowDirection.LeftToRight;
    			_toolbar.ResumeLayout(true);

    			ToolStripPanel targetParent = ConvertToToolStripPanel(_toolStripContainer, settings.ToolStripDock);
    			if (targetParent != null && !ReferenceEquals(targetParent, _toolbar.Parent))
    			{
    				_toolStripContainer.SuspendLayout();
    				targetParent.Join(_toolbar);

    				// this keeps the main menu above the toolbar.
    				// very hacky, but until we have a better way of serializing the entire state of *all* toolstrips and their relationships with each other...
    				if (ReferenceEquals(targetParent, _toolStripContainer.TopToolStripPanel))
    					_toolStripContainer.TopToolStripPanel.Join(_mainMenu);

    				_toolStripContainer.ResumeLayout(true);
    			}
    		}
			else if (e.PropertyName == "IconSize")
			{
				ToolStripBuilder.ChangeIconSize(_toolbar, settings.IconSize);
			}
    	}

    	private void OnToolbarParentChanged(object sender, EventArgs e)
    	{
    		ToolStripDock dock = ConvertToToolStripDock(_toolStripContainer, _toolbar);
    		if (dock != ToolStripDock.None)
    		{
    			ToolStripSettings settings = ToolStripSettings.Default;
    			settings.ToolStripDock = dock;
    			settings.Save();
    		}
    	}

        #endregion

        #region Helper methods

        /// <summary>
        /// Called to initialize a <see cref="Crownwood.DotNetMagic.Controls.TabControl"/>. Override
        /// this method to perform custom initialization.
        /// </summary>
        /// <param name="tabControl"></param>
        protected virtual void InitializeTabControl(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			if (tabControl == null)
				return;

			tabControl.TextTips = true;
			tabControl.ToolTips = false;
			tabControl.MaximumHeaderWidth = 256;
        }

        /// <summary>
        /// Called to build menus and toolbars.  Override this method to customize menu and toolbar building.
        /// </summary>
        /// <remarks>
        /// The default implementation simply clears and re-creates the toolstrip using methods on the
        /// utility class <see cref="ToolStripBuilder"/>.
        /// </remarks>
        /// <param name="kind"></param>
        /// <param name="toolStrip"></param>
        /// <param name="actionModel"></param>
        protected virtual void BuildToolStrip(ToolStripBuilder.ToolStripKind kind, ToolStrip toolStrip, ActionModelNode actionModel)
        {
            // avoid flicker
            toolStrip.SuspendLayout();
            // very important to clean up the existing ones first
            ToolStripBuilder.Clear(toolStrip.Items);

            if (actionModel != null)
            {
				if (actionModel.ChildNodes.Count > 0)
				{
					// Toolstrip should only be visible if there are items on it
					toolStrip.Visible = true;

					if (kind == ToolStripBuilder.ToolStripKind.Toolbar)
						ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes, ToolStripBuilder.ToolStripBuilderStyle.GetDefault(), ToolStripSettings.Default.IconSize);
					else
						ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes);
				}
				else
				{
					toolStrip.Visible = false;
				}
            }

            toolStrip.ResumeLayout();
        }

    	private static ToolStripPanel ConvertToToolStripPanel(ToolStripContainer toolStripContainer, ToolStripDock dock)
    	{
    		switch (dock)
    		{
    			case ToolStripDock.Left:
    				return toolStripContainer.LeftToolStripPanel;
    			case ToolStripDock.Top:
    				return toolStripContainer.TopToolStripPanel;
    			case ToolStripDock.Right:
    				return toolStripContainer.RightToolStripPanel;
    			case ToolStripDock.Bottom:
    				return toolStripContainer.BottomToolStripPanel;
    			case ToolStripDock.None:
    			default:
    				return null;
    		}
    	}

    	private static ToolStripDock ConvertToToolStripDock(ToolStripContainer toolStripContainer, ToolStrip toolStrip)
    	{
    		ToolStripPanel parent = toolStrip.Parent as ToolStripPanel;
    		if (ReferenceEquals(parent, toolStripContainer.TopToolStripPanel))
    			return ToolStripDock.Top;
    		else if (ReferenceEquals(parent, toolStripContainer.LeftToolStripPanel))
    			return ToolStripDock.Left;
    		else if (ReferenceEquals(parent, toolStripContainer.BottomToolStripPanel))
    			return ToolStripDock.Bottom;
    		else if (ReferenceEquals(parent, toolStripContainer.RightToolStripPanel))
    			return ToolStripDock.Right;
			else
				return ToolStripDock.None;
    	}

        #endregion
    }
}
