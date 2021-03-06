#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.Folders.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderOptionComponent"/>
    /// </summary>
    public partial class FolderOptionComponentControl : ApplicationComponentUserControl
    {
        private FolderOptionComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderOptionComponentControl(FolderOptionComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _refreshTime.DataBindings.Add("Value", _component, "RefreshTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
