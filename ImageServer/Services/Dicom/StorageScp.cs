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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Abstract class for handling Storage SCP connections.
    /// </summary>
    /// <remarks>
    /// <para>This class is an abstract base class for ImageServer plugins that process DICOM C-STORE
    /// request messages.  The class implements a number of common methods needed for SCP handlers.
    /// The class also implements the <see cref="IDicomScp{TContext}"/> interface.</para>
    /// </remarks>
    public abstract class StorageScp : BaseScp
    {
        #region Abstract Properties
        public abstract string StorageScpType { get; }
        #endregion

        #region Protected Members

        /// <summary>
        /// Converts a <see cref="DicomMessage"/> instance into a <see cref="DicomFile"/>.
        /// </summary>
        /// <remarks>This routine sets the Source AE title, </remarks>
        /// <param name="message"></param>
        /// <param name="filename"></param>
        /// <param name="assocParms"></param>
        /// <returns></returns>
        protected static DicomFile ConvertToDicomFile(DicomMessage message, string filename, AssociationParameters assocParms)
        {
            // This routine sets some of the group 0x0002 elements.
            DicomFile file = new DicomFile(message, filename);

            file.SourceApplicationEntityTitle = assocParms.CallingAE;
            if (message.TransferSyntax.Encapsulated)
                file.TransferSyntax = message.TransferSyntax;
            else
                file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            return file;
        }

        /// <summary>
        /// Load from the database the configured transfer syntaxes
        /// </summary>
        /// <param name="read">a Read context</param>
        /// <param name="partitionKey">The partition to retrieve the transfer syntaxes for</param>
        /// <param name="encapsulated">true if searching for encapsulated syntaxes only</param>
        /// <returns>The list of syntaxes</returns>
		protected static IList<PartitionTransferSyntax> LoadTransferSyntaxes(IReadContext read, ServerEntityKey partitionKey, bool encapsulated)
        {
            IList<PartitionTransferSyntax> list;

			IQueryServerPartitionTransferSyntaxes broker = read.GetBroker<IQueryServerPartitionTransferSyntaxes>();

			PartitionTransferSyntaxQueryParameters criteria = new PartitionTransferSyntaxQueryParameters();

            criteria.ServerPartitionKey = partitionKey;

            list = broker.Find(criteria);


			List<PartitionTransferSyntax> returnList = new List<PartitionTransferSyntax>();
			foreach (PartitionTransferSyntax syntax in list)
            {
				if (!syntax.Enabled) continue;

                TransferSyntax dicomSyntax = TransferSyntax.GetTransferSyntax(syntax.Uid);
                if (dicomSyntax.Encapsulated == encapsulated)
                    returnList.Add(syntax);
            }
            return returnList;
        }
        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {
			if (Device == null)
				return DicomPresContextResult.Accept;

            if (!Device.AllowStorage)
            {
                return DicomPresContextResult.RejectUser;
            }

			if (Device.AcceptKOPR)
			{
				DicomPresContext context = association.GetPresentationContext(pcid);
				if (context.AbstractSyntax.Equals(SopClass.KeyObjectSelectionDocumentStorage)
				  ||context.AbstractSyntax.Equals(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass)
				  ||context.AbstractSyntax.Equals(SopClass.BlendingSoftcopyPresentationStateStorageSopClass)
				  ||context.AbstractSyntax.Equals(SopClass.ColorSoftcopyPresentationStateStorageSopClass)
				  ||context.AbstractSyntax.Equals(SopClass.PseudoColorSoftcopyPresentationStateStorageSopClass))
					return DicomPresContextResult.Accept;

				return DicomPresContextResult.RejectUser;
			}

            return DicomPresContextResult.Accept;
        }

        #endregion Overridden BaseSCP methods

        #region IDicomScp Members

        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            try
            {
                SopInstanceImporterContext context = new SopInstanceImporterContext(
                    String.Format("{0}_{1}", association.CallingAE, association.TimeStamp.ToString("yyyyMMddhhmmss")),
                    association.CallingAE, association.CalledAE);

                SopInstanceImporter importer = new SopInstanceImporter(context);
                DicomProcessingResult result = importer.Import(message);

                if (result.Successful)
                {
					if (!String.IsNullOrEmpty(result.AccessionNumber))
						Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (A#:{3} StudyUid:{4})",
						             result.SopInstanceUid, association.CallingAE, association.CalledAE, result.AccessionNumber,
						             result.StudyInstanceUid);
					else
						Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (StudyUid:{3})",
									 result.SopInstanceUid, association.CallingAE, association.CalledAE,
									 result.StudyInstanceUid);                
                }
				else 
					Platform.Log(LogLevel.Warn, "Failure importing sop: {0}", result.ErrorMessage);

                server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, result.DicomStatus);
                return true;
            }
            catch(DicomDataException ex)
            {
                Platform.Log(LogLevel.Error, ex);
                return false;  // caller will abort the association
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
                return false;  // caller will abort the association
            }
        }

        #endregion
    }
}