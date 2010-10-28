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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	/// <summary>
	/// A simple wrapper class that implements the <see cref="IStudyRootQuery"/> service contract,
	/// but internally uses a <see cref="StudyRootFindScu"/>.
	/// </summary>
	public class DicomStudyRootQuery : IStudyRootQuery
	{
		private readonly string _localAE;
		private readonly string _remoteAE;
		private readonly string _remoteHost;
		private readonly int _remotePort;

		public DicomStudyRootQuery(string localAETitle, string remoteAETitle, string remoteHost, int remotePort)
		{
			_localAE = localAETitle;
			_remoteAE = remoteAETitle;
			_remoteHost = remoteHost;
			_remotePort = remotePort;
		}

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			return Query<StudyRootStudyIdentifier, StudyRootFindScu>(queryCriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			return Query<SeriesIdentifier, StudyRootFindScu>(queryCriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			return Query<ImageIdentifier, StudyRootFindScu>(queryCriteria);
		}

		#endregion

		private IList<TIdentifier> Query<TIdentifier, TFindScu>(TIdentifier queryCriteria)
			where TIdentifier : Identifier, new()
			where TFindScu : FindScuBase, new()
		{
			Platform.CheckForEmptyString(_localAE, "localAE");
			Platform.CheckForEmptyString(_remoteAE, "remoteAE");
			Platform.CheckForEmptyString(_remoteHost, "remoteHost");
			Platform.CheckArgumentRange(_remotePort, 1, 65535, "remotePort");

			if (queryCriteria == null)
			{
				string message = "The query identifier cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			IList<DicomAttributeCollection> scuResults;
			using (TFindScu scu = new TFindScu())
			{
				DicomAttributeCollection criteria;

				try
				{
					criteria = queryCriteria.ToDicomAttributeCollection();
				}
				catch (DicomException e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Failed to convert contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}
				catch (Exception e)
				{
					DataValidationFault fault = new DataValidationFault();
					fault.Description = "Unexpected exception when converting contract object to DicomAttributeCollection.";
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<DataValidationFault>(fault, fault.Description);
				}

				try
				{
					scuResults = scu.Find(_localAE, _remoteAE, _remoteHost, _remotePort, criteria);
					scu.Join();

					if (scu.Status == ScuOperationStatus.Canceled)
					{
						String message = String.Format("The remote server cancelled the query ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.ConnectFailed)
					{
						String message = String.Format("Connection failed ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.AssociationRejected)
					{
						String message = String.Format("Association rejected ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.Failed)
					{
						String message = String.Format("The query operation failed ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.TimeoutExpired)
					{
						String message = String.Format("The connection timeout expired ({0})",
													   scu.FailureDescription ?? "no failure description provided");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.NetworkError)
					{
						String message = String.Format("An unexpected network error has occurred.");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}
					if (scu.Status == ScuOperationStatus.UnexpectedMessage)
					{
						String message = String.Format("An unexpected message was received; aborted association.");
						QueryFailedFault fault = new QueryFailedFault();
						fault.Description = message;
						throw new FaultException<QueryFailedFault>(fault, fault.Description);
					}

				}
				catch (FaultException)
				{
					throw;
				}
				catch (Exception e)
				{
					QueryFailedFault fault = new QueryFailedFault();
					fault.Description = String.Format("An unexpected error has occurred ({0})",
													  scu.FailureDescription ?? "no failure description provided");
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}
			}

			List<TIdentifier> results = new List<TIdentifier>();
			foreach (DicomAttributeCollection result in scuResults)
			{
				TIdentifier identifier = Identifier.FromDicomAttributeCollection<TIdentifier>(result);
				if (String.IsNullOrEmpty(identifier.RetrieveAeTitle))
					identifier.RetrieveAeTitle = _remoteAE;

				results.Add(identifier);
			}

			return results;
		}

		public override string ToString()
		{
			return _remoteAE;
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
