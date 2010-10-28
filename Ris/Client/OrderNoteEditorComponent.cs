#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class OrderNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(OrderNoteEditorComponentViewExtensionPoint))]
    public class OrderNoteEditorComponent : ApplicationComponent
    {
        private readonly OrderNoteDetail _note;
		private ICannedTextLookupHandler _cannedTextLookupHandler;

        public OrderNoteEditorComponent(OrderNoteDetail noteDetail)
        {
            _note = noteDetail;
        }

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

        #region Presentation Model

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}
		
		[ValidateNotNull]
		public string Comment
        {
            get { return _note.NoteBody; }
            set
            {
                _note.NoteBody = value;
                this.Modified = true;
            }
        }

        public bool IsNewItem
        {
            get { return _note.OrderNoteRef == null; }
        }

        public void Accept()
        {
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

            this.ExitCode = ApplicationComponentExitCode.Accepted;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified && this.IsNewItem; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}
