#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	internal abstract class ReconcileCommandBase : ServerCommand<ReconcileStudyProcessorContext>, IReconcileServerCommand, IDisposable
	{
	    

	    /// <summary>
		/// Creates an instance of <see cref="ServerCommand"/>
		/// </summary>
		/// <param name="description"></param>
		/// <param name="requiresRollback"></param>
		/// <param name="context"></param>
		public ReconcileCommandBase(string description, bool requiresRollback, ReconcileStudyProcessorContext context)
			: base(description, requiresRollback, context)
		{
		}

	    protected bool SeriesMappingUpdated { get; set;}

        protected UidMapper UidMapper { get; set; }

	    protected string GetReconcileUidPath(WorkQueueUid sop)
	    {
            // In 2.0: Path = \\Filesystem Path\Reconcile\GroupID\StudyInstanceUid\*.dcm
            WorkQueue workqueueItem = Context.WorkQueueItem;
            if (!String.IsNullOrEmpty(workqueueItem.GroupID))
            {
                StudyStorageLocation storageLocation = Context.WorkQueueItemStudyStorage;
                string path = Path.Combine(storageLocation.FilesystemPath, storageLocation.PartitionFolder);
                path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
                path = Path.Combine(path, workqueueItem.GroupID);
                path = Path.Combine(path, storageLocation.StudyInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ServerPlatform.DicomFileExtension);
                return path; 
            }
            else
            {
                #region BACKWARD-COMPATIBLE CODE
                // 1.5 SP1, RelativePath is NOT populated for Reconcile Study entry
                // Path = \\Filesystem Path\Reconcile\GUID\*.dcm
                // where \\Filesystem Path\Reconcile\GUID = Context.ReconcileWorkQueueData.StoragePath
                if (String.IsNullOrEmpty(sop.RelativePath))
                {
                    string path = Context.ReconcileWorkQueueData.StoragePath;
                    path = Path.Combine(path, sop.SopInstanceUid + ServerPlatform.DicomFileExtension);
                    return path;
                }

                // will this ever happen?
                return Path.Combine(Context.ReconcileWorkQueueData.StoragePath, sop.RelativePath);
                #endregion
            }
	    	
	    }

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				DirectoryUtility.DeleteIfEmpty(Context.ReconcileWorkQueueData.StoragePath);
			}
			catch(IOException ex)
			{
				Platform.Log(LogLevel.Warn, ex, "Unable to cleanup {0}", Context.ReconcileWorkQueueData.StoragePath);
			}
		}

		#endregion

		protected static StudyXml LoadStudyXml(StudyStorageLocation location)
		{
			// This method should be combined with StudyStorageLocation.LoadStudyXml()
			StudyXml theXml = new StudyXml(location.StudyInstanceUid);

			String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
			if (File.Exists(streamFile))
			{
				using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
				{
					XmlDocument theDoc = new XmlDocument();

					StudyXmlIo.Read(theDoc, fileStream);

					theXml.SetMemento(theDoc);

					fileStream.Close();
				}
			}

			return theXml;
		}

	    internal void UidMapper_SeriesMapUpdated(object sender, SeriesMapUpdatedEventArgs e)
        {
            SeriesMappingUpdated = true;
        }

	    protected void UpdateHistory(StudyStorageLocation location)
	    {
	        using(ServerCommandProcessor processor = new ServerCommandProcessor("Reconcile-CreateStudy-Update History"))
	        {
                processor.AddCommand(new SaveUidMapXmlCommand(location, UidMapper));
                processor.AddCommand(new UpdateHistorySeriesMappingCommand(Context.History, location, UidMapper));
                if (!processor.Execute())
	            {
	                throw new ApplicationException("Unable to update the history", processor.FailureException);
	            }
	        }	        
	    }

	    protected void EnsureStudyCanBeUpdated(StudyStorageLocation location)
	    {
	        string reason;
	        if (!location.CanUpdate(out reason))
	            throw new StudyIsInInvalidStateException(location, reason);
	    }
	}
}