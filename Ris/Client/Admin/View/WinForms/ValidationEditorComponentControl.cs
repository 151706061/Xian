#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Reflection;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ValidationEditorComponent"/>
    /// </summary>
    public partial class ValidationEditorComponentControl : ApplicationComponentUserControl
    {
        private ValidationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponentControl(ValidationEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _propertiesTableView.Table = _component.Rules;
            _propertiesTableView.ToolbarModel = _component.RulesActionModel;
            _propertiesTableView.MenuModel = _component.RulesActionModel;
            _propertiesTableView.DataBindings.Add("Selection", _component, "SelectedRule", true, DataSourceUpdateMode.OnPropertyChanged);
            _testButton.DataBindings.Add("Enabled", _component, "CanTestRules");

            foreach (PropertyInfo item in _component.ComponentPropertyChoices)
            {
                _propertiesMenu.Items.Add(item.Name);
            }

            Control editor = (Control)_component.EditorComponentHost.ComponentView.GuiElement;
            editor.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(editor);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            _component.TestRules();
        }

        private void _macroButton_Click(object sender, EventArgs e)
        {
            _propertiesMenu.Show(_macroButton, new Point(0, _macroButton.Height), ToolStripDropDownDirection.BelowRight);
        }

        private void _propertiesMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _component.InsertText(e.ClickedItem.Text);
        }
    }
}
