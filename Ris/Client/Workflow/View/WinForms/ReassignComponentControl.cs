#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ReassignComponent"/>
    /// </summary>
    public partial class ReassignComponentControl : ApplicationComponentUserControl
    {
        private readonly ReassignComponent _component;

        public ReassignComponentControl(ReassignComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_radiologist.LookupHandler = _component.RadiologistLookupHandler;
			_radiologist.DataBindings.Add("Value", _component, "Radiologist", true, DataSourceUpdateMode.OnPropertyChanged);	
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
