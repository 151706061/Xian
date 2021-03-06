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
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using System.Threading;

namespace ClearCanvas.Dicom.TestTools.TestScp
{
	public partial class Form1 : Form, IDicomServerHandler
	{
		
		private ServerAssociationParameters _parms;
		
		public Form1()
		{
			InitializeComponent();
		}

		private IDicomServerHandler StartAssociation(DicomServer server, ServerAssociationParameters assoc)
		{
			return this;
		}

		#region IDicomServerHandler Members

		public void OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association)
		{
			if (_delayAssociationAccept.Checked)
				Thread.Sleep(TimeSpan.FromSeconds(35));

			if (_rejectAssociation.Checked)
				server.SendAssociateReject(DicomRejectResult.Permanent, DicomRejectSource.ServiceUser, DicomRejectReason.CallingAENotRecognized);
			else
				server.SendAssociateAccept(association);
		}

		public void OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
		{
			foreach (byte pcid in association.GetPresentationContextIDs())
			{
				DicomPresContext context = association.GetPresentationContext(pcid);
				if (context.Result == DicomPresContextResult.Accept)
				{
					if (context.AbstractSyntax == SopClass.StudyRootQueryRetrieveInformationModelFind)
					{
						DicomMessage response = new DicomMessage();
						response.DataSet[DicomTags.StudyInstanceUid].SetStringValue("1.2.3");
						response.DataSet[DicomTags.PatientId].SetStringValue("1");
						response.DataSet[DicomTags.PatientsName].SetStringValue("test");
						response.DataSet[DicomTags.StudyId].SetStringValue("1");
						response.DataSet[DicomTags.StudyDescription].SetStringValue("dummy");
						server.SendCFindResponse(presentationID, message.MessageId, response, DicomStatuses.Pending);

						DicomMessage finalResponse = new DicomMessage();
						server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);
					}
					else if (context.AbstractSyntax == SopClass.VerificationSopClass)
					{
						server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
					}
				}
			}
		}

		public void OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
		{
			server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.UnexpectedPDU);
		}

		public void OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association)
		{
			if (_delayAssociationRelease.Checked)
				Thread.Sleep(TimeSpan.FromSeconds(35));
		}

		public void OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
		{
		}

		public void OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e)
		{
		}

		public void OnDimseTimeout(DicomServer server, ServerAssociationParameters association)
		{
		}

		#endregion

		private void _startStop_Click(object sender, EventArgs e)
		{
			if (_parms == null)
			{
				_parms = new ServerAssociationParameters("TEST", new IPEndPoint(IPAddress.Loopback, 105));
				_parms.AddPresentationContext(1, SopClass.VerificationSopClass);
				_parms.AddTransferSyntax(1, TransferSyntax.ExplicitVrLittleEndian);
				_parms.AddTransferSyntax(1, TransferSyntax.ImplicitVrLittleEndian);

				_parms.AddPresentationContext(2, SopClass.StudyRootQueryRetrieveInformationModelMove);
				_parms.AddTransferSyntax(2, TransferSyntax.ExplicitVrLittleEndian);
				_parms.AddTransferSyntax(2, TransferSyntax.ImplicitVrLittleEndian);

				_parms.AddPresentationContext(3, SopClass.StudyRootQueryRetrieveInformationModelFind);
				_parms.AddTransferSyntax(3, TransferSyntax.ExplicitVrLittleEndian);
				_parms.AddTransferSyntax(3, TransferSyntax.ImplicitVrLittleEndian);

				DicomServer.StartListening(_parms, StartAssociation);
				_startStop.Text = "Stop Listening";
			}
			else
			{
				DicomServer.StopListening(_parms);
				_parms = null;
				_startStop.Text = "Stop Listening";
			}
		}
	}
}