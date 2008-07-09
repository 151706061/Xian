#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Codec;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.Shreds.DicomServer
{
    /// <summary>
    /// This class manages the DICOM SCP Shred for the ImageServer.
    /// </summary>
	public class DicomServerManager : ThreadedService
    {
        #region Private Members
        private readonly List<DicomScp<DicomScpContext>> _listenerList = new List<DicomScp<DicomScpContext>>();
    	private readonly object _syncLock = new object();
        private static DicomServerManager _instance;
		IList<ServerPartition> _partitions;
    	private bool _stop = false;
        #endregion

		#region Constructor
		public DicomServerManager(string name) : base(name)
		{}
		#endregion

		#region Properties
		/// <summary>
        /// Singleton instance of the class.
        /// </summary>
        public static DicomServerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DicomServerManager("DICOM Service Manager");

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion

		#region Private Methods
		private void LoadPartitions()
		{
			//Get partitions
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

			using (IReadContext read = store.OpenReadContext())
			{
				IServerPartitionEntityBroker broker = read.GetBroker<IServerPartitionEntityBroker>();
				ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
				_partitions = broker.Find(criteria);
			}
		}
		private void StartListeners(ServerPartition part, FilesystemMonitor monitor)
		{
			DicomScpContext parms =
				new DicomScpContext(part, monitor, new FilesystemSelector(monitor));

			if (ImageServerServicesShredSettings.Default.ListenIPV4)
			{
				DicomScp<DicomScpContext> ipV4Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify);

				ipV4Scp.ListenPort = part.Port;
				ipV4Scp.AeTitle = part.AeTitle;

				if (ipV4Scp.Start(IPAddress.Any))
					_listenerList.Add(ipV4Scp);
				else
				{
					Platform.Log(LogLevel.Error, "Unable to add IPv4 SCP handler for server partition {0}",
					             part.Description);
					Platform.Log(LogLevel.Error,
					             "Partition {0} will not accept IPv4 incoming DICOM associations.",
					             part.Description);
				}
			}

			if (ImageServerServicesShredSettings.Default.ListenIPV6)
			{
				DicomScp<DicomScpContext> ipV6Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify);

				ipV6Scp.ListenPort = part.Port;
				ipV6Scp.AeTitle = part.AeTitle;

				if (ipV6Scp.Start(IPAddress.IPv6Any))
					_listenerList.Add(ipV6Scp);
				else
				{
					Platform.Log(LogLevel.Error, "Unable to add IPv6 SCP handler for server partition {0}",
					             part.Description);
					Platform.Log(LogLevel.Error,
					             "Partition {0} will not accept IPv6 incoming DICOM associations.",
					             part.Description);
				}
			}
		}

    	private void CheckPartitions(FilesystemMonitor monitor)
		{
    	
			lock (_syncLock)
			{
				IList<DicomScp<DicomScpContext>> scpsToDelete = new List<DicomScp<DicomScpContext>>();

				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					bool bFound = false;
					foreach (ServerPartition part in _partitions)
					{
						if (part.Port == scp.ListenPort && part.AeTitle.Equals(scp.AeTitle) && part.Enabled)
						{
							bFound = true;
							break;
						}
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Partition was deleted, shutting down listener {0}:{1}", scp.AeTitle, scp.ListenPort);
						scp.Stop();
						scpsToDelete.Add(scp);
					}
				}

				foreach (DicomScp<DicomScpContext> scp in scpsToDelete)
					_listenerList.Remove(scp);

				foreach (ServerPartition part in _partitions)
				{
					if (!part.Enabled)
						continue;

					bool bFound = false;
					foreach (DicomScp<DicomScpContext> scp in _listenerList)
					{
						if (part.Port != scp.ListenPort || !part.AeTitle.Equals(scp.AeTitle))
							continue;

						// Reset the context partition, incase its changed.
						scp.Context.Partition = part;

						bFound = true;
						break;
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Detected partition was added, starting listener {0}:{1}", part.AeTitle, part.Port);
						StartListeners(part, monitor);
					}
				}
			}
		}
		#endregion

		#region Public Methods
		protected override void Initialize()
		{
			//Load Codecs
			DicomCodecHelper.RegisterCodecExtensions();

			LoadPartitions();
		}

        /// <summary>
        /// Method called when starting the DICOM SCP.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method starts a <see cref="DicomScp{DicomScpParameters}"/> instance for each server partition configured in
        /// the database.  It assumes that the combination of the configured AE Title and Port for the 
        /// partition is unique.  
        /// </para>
        /// </remarks>
        protected override void Run()
        {
        
            FilesystemMonitor monitor = new FilesystemMonitor();
            monitor.Load();
            
            foreach (ServerPartition part in _partitions)
            {
                if (part.Enabled)
                {
                	StartListeners(part, monitor);
                }
            }

			while (!CheckStop(60000))
			{
				LoadPartitions();

				CheckPartitions(monitor);
			}
        }

        /// <summary>
        /// Method called when stopping the DICOM SCP.
        /// </summary>
        protected override void Stop()
        {
			lock (_syncLock)
			{
				_stop = true;
				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					scp.Stop();
				}
			}
        }
        #endregion
    }
}