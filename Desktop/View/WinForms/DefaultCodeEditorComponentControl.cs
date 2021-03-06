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

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CodeEditorComponent"/>
    /// </summary>
    public partial class DefaultCodeEditorComponentControl : ApplicationComponentUserControl
    {
        private DefaultCodeEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultCodeEditorComponentControl(DefaultCodeEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component.InsertTextRequested += new EventHandler<DefaultCodeEditorComponent.InsertTextEventArgs>(_component_InsertTextRequested);
            _editor.DataBindings.Add("Text", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _component_InsertTextRequested(object sender, DefaultCodeEditorComponent.InsertTextEventArgs e)
        {
            _editor.SelectedText = e.Text;
        }
    }
}
