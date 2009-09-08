﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client;
using System;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="OrderNoteConversationComponent"/>.
	/// </summary>
	public partial class OrderNoteConversationComponentControl : ApplicationComponentUserControl
	{
		private readonly OrderNoteConversationComponent _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor.
		/// </summary>
		public OrderNoteConversationComponentControl(OrderNoteConversationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			Control orderNotes = (Control)_component.OrderNotesHost.ComponentView.GuiElement;
			orderNotes.Dock = DockStyle.Fill;
			_orderNotesPanel.Controls.Add(orderNotes);

			_replyBody.DataBindings.Add("Text", _component, "Body", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_replyBody, _component.CannedTextLookupHandler);

			_urgent.DataBindings.Add("Checked", _component, "Urgent", true, DataSourceUpdateMode.OnPropertyChanged);

			_onBehalf.DataSource = _component.OnBehalfOfGroupChoices;
			_onBehalf.DataBindings.Add("Value", _component, "OnBehalfOf", true, DataSourceUpdateMode.OnPropertyChanged);

			_recipients.Table = _component.Recipients;
			_recipients.MenuModel = _component.RecipientsActionModel;
			_recipients.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);

			_staffRecipientLookup.LookupHandler = _component.StaffRecipientLookupHandler;
			_staffRecipientLookup.DataBindings.Add("Value", _component, "SelectedStaffRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_staffRecipientAddButton.DataBindings.Add("Enabled", _component, "AddStaffRecipientEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_groupRecipientLookup.LookupHandler = _component.GroupRecipientLookupHandler;
			_groupRecipientLookup.DataBindings.Add("Value", _component, "SelectedGroupRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_groupRecipientAddButton.DataBindings.Add("Enabled", _component, "AddGroupRecipientEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_completeButton.DataBindings.Add("Text", _component, "CompleteLabel", true, DataSourceUpdateMode.OnPropertyChanged);
			_completeButton.DataBindings.Add("Enabled", _component, "CompleteEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_replyCheckBox.Visible = !_component.IsCreatingNewNote;
			_replyCheckBox.DataBindings.Add("Checked", _component, "Reply", true, DataSourceUpdateMode.OnPropertyChanged);

			_notesGroupBox.Text = _component.OrderNotesLabel;

			int reducedHeight = this.Height - _componentSplitContainer.Panel1.Height;

			_componentSplitContainer.Panel1Collapsed = !_component.HasExistingNotes;
			_componentSplitContainer.DataBindings.Add("Panel2Collapsed", _component, "HideReply", true, DataSourceUpdateMode.OnPropertyChanged);

			if (_componentSplitContainer.Panel1Collapsed)
				this.Size = new Size(this.Size.Width, reducedHeight);

			_component.PropertyChanged += _component_propertyChanged;
		}

		private void _component_propertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Recipients")
			{
				_recipients.Table = _component.Recipients;
			}
			else if (e.PropertyName == "CompleteLabel")
			{
				_completeButton.Text = _component.CompleteLabel;
			}
		}

		private void _staffRecipientAddButton_Click(object sender, System.EventArgs e)
		{
			using(new CursorManager(Cursors.WaitCursor))
			{
				_component.AddStaffRecipient();
			}
		}

		private void _groupRecipientAddButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.AddGroupRecipient();
			}
		}

		private void _completeButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.OnComplete();
			}
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			_component.OnCancel();
		}
	}
}
