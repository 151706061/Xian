using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    internal class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		#region Private Fields

		private static DiskspaceManagerProcessor _instance;

		private object _settingsSyncLock = new object();
		private Thread _processingThread;
		private DiskspaceManagerData _diskspaceManagerData;
		private ClearCanvas.Common.Utilities.Timer _timer;
		private EventWaitHandle _stopSignal;
		private EventWaitHandle _checkSignal;

		#endregion

		public DiskspaceManagerProcessor()
        {
        }

        public static DiskspaceManagerProcessor Instance
		{
			get
			{
				if (_instance == null)
                    _instance = new DiskspaceManagerProcessor();

				return _instance;
			}
            set
            {
                _instance = value;
            }
		}

		public DMDriveInfo CurrentDriveInfo
		{
			get
			{
				foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
				{
					if (dmDriveInfo.DriveName.StartsWith(DiskspaceManagerSettings.Instance.DriveName, StringComparison.CurrentCultureIgnoreCase))
						return dmDriveInfo;
				}

				return null;
			}
		}

        public void StartProcessor()
        {
            try
            {
                _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
                _checkSignal = new EventWaitHandle(false, EventResetMode.AutoReset);
                
                _diskspaceManagerData = new DiskspaceManagerData();
                if (_diskspaceManagerData.DriveInfoList != null && _diskspaceManagerData.DriveInfoList.Count > 0)
                {
                    if (!_diskspaceManagerData.DriveInfoList[0].DriveName.Equals(DiskspaceManagerSettings.Instance.DriveName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        DiskspaceManagerSettings.Instance.DriveName = _diskspaceManagerData.DriveInfoList[0].DriveName;
                        DiskspaceManagerSettings.Save();
                    }
                }
                else
                {
                    if (!DiskspaceManagerSettings.Instance.DriveName.Equals(""))
                    {
                        DiskspaceManagerSettings.Instance.DriveName = "";
                        DiskspaceManagerSettings.Save();
                    }
                }

                if (!FindDiskspaceManagerDBAccessExtensionPoint())
                    return;

                _stopSignal.Reset();
                _checkSignal.Reset();
                // start up processing thread
                _processingThread = new Thread(new ThreadStart(StartDiskspaceManager));
				_processingThread.Start();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopProcessor()
        {
            try
            {
                _diskspaceManagerData.DeleteStudyInDBCompletedEvent -= DeleteStudyCompletedHandler;
                _diskspaceManagerData.OrderedStudyListReadyEvent -= ValidateStudyHandler;
                _stopSignal.Set();
                _checkSignal.Reset();
			
				// wait for processing thread to finish
				_processingThread.Join();
				_processingThread = null;
			}
            catch (Exception e)
            {
                throw e;
            }
        }

        private void StartDiskspaceManager()
        {
			_timer = new ClearCanvas.Common.Utilities.Timer(this.OnTimer, 60000, _diskspaceManagerData.CheckingFrequency);

            _diskspaceManagerData.OrderedStudyListReadyEvent += ValidateStudyHandler;
            _diskspaceManagerData.DeleteStudyInDBCompletedEvent += DeleteStudyCompletedHandler;

			do
			{
				lock (_settingsSyncLock)
				{
					CheckConfigurationSettings();
					CheckDiskspace();
				}

				if (!_checkSignal.WaitOne(0, true) || _diskspaceManagerData.IsProcessing)
					continue;

				if (_diskspaceManagerData.ReachHighWaterMark)
				{
					_diskspaceManagerData.IsProcessing = true;
					_diskspaceManagerData.FireOrderedStudyListRequired();
				}
			}
			while (!_stopSignal.WaitOne(1000, true));

			_timer.Dispose();
			_timer = null;
        }

        private void OnTimer()
        {
            _checkSignal.Set();
        }

        private void CheckConfigurationSettings()
        {
            if (_diskspaceManagerData.CheckingFrequency != DiskspaceManagerSettings.Instance.CheckFrequency * 60000)
            {
                _diskspaceManagerData.CheckingFrequency = DiskspaceManagerSettings.Instance.CheckFrequency * 60000;
                _timer.Dispose();
                _timer = null;
                _timer = new ClearCanvas.Common.Utilities.Timer(this.OnTimer, _diskspaceManagerData.CheckingFrequency, _diskspaceManagerData.CheckingFrequency);
                _checkSignal.Reset();
            }
            foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
            {
                if (dmDriveInfo.DriveName.StartsWith(DiskspaceManagerSettings.Instance.DriveName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (dmDriveInfo.LowWatermark != DiskspaceManagerSettings.Instance.LowWatermark)
                        dmDriveInfo.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
                    if (dmDriveInfo.HighWatermark != DiskspaceManagerSettings.Instance.HighWatermark)
                        dmDriveInfo.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
                }
            }
        }

        private void CheckDiskspace()
        {
            if (_diskspaceManagerData == null || _diskspaceManagerData.DriveInfoList == null || _diskspaceManagerData.DriveInfoList.Count <= 0)
                return;

            foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
            {
                dmDriveInfo.init();
                ManagementObject drive = new ManagementObject("win32_logicaldisk.deviceid='" + dmDriveInfo.DriveName + "'");
                drive.Get();
                dmDriveInfo.DriveSize = long.Parse(drive["Size"].ToString());
                dmDriveInfo.UsedSpace = dmDriveInfo.DriveSize - long.Parse(drive["FreeSpace"].ToString());

				// debug data
                // Platform.Log("    Checking diskspace on drive (" + dmDriveInfo.DriveName + ") : " + dmDriveInfo.UsedSpace + "/" + dmDriveInfo.DriveSize
                //    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
            }

        }

        private void DeleteStudyCompletedHandler(object sender, EventArgs args)
        {
            _diskspaceManagerData.IsProcessing = false;
        }

        private void ValidateStudyHandler(object sender, EventArgs args)
        {
            ValidateOrderedStudyList();
            _diskspaceManagerData.FireDeleteStudyInDBRequired();
        }

        private void ValidateOrderedStudyList()
        {
            //Platform.Log("    Validation for DICOM files on drive:");
            foreach (DMStudyItem studyItem in _diskspaceManagerData.OrderedStudyList)
            {
                CheckStudyItem(studyItem);
                if (_diskspaceManagerData.EnoughDeletedFiles)
                    break;
            }
            foreach (DMDriveInfo dmDriveInfo in _diskspaceManagerData.DriveInfoList)
            {
                if (!dmDriveInfo.ReachHighWaterMark)
                    continue;
                Platform.Log("    Validation for drive " + dmDriveInfo.DriveName + " Studies found: " + dmDriveInfo.DeletedStudyNumber + "; Used Space: " + dmDriveInfo.DeletedFileSpace
                    + " (" + dmDriveInfo.UsedSpacePercentage + "%) (Watermark: " + dmDriveInfo.LowWatermark + " ~ " + dmDriveInfo.HighWatermark + ")");
            }
            return;
        }

        private void CheckStudyItem(DMStudyItem studyItem)
        {
            long usedspace = 0;
            foreach (DMSopItem sopItem in studyItem.SopItemList)
            {
                if (usedspace <= 0)
                {
                    studyItem.DriveID = -1;
                    for (int i = 0; i < _diskspaceManagerData.DriveInfoList.Count; i++)
                    {
                        if (sopItem.LocationUri.StartsWith(_diskspaceManagerData.DriveInfoList[i].DriveName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (_diskspaceManagerData.DriveInfoList[i].ReachHighWaterMark)
                                studyItem.DriveID = i;
                            break;
                        }
                    }
                    if (studyItem.DriveID == -1 || _diskspaceManagerData.DriveInfoList[studyItem.DriveID].EnoughDeletedFiles)
                        return;
                }
                if (!File.Exists(sopItem.LocationUri))
                    return;
                FileInfo dfile = new FileInfo(sopItem.LocationUri);
                usedspace += dfile.Length;
            }
            studyItem.UsedSpace = usedspace;
            _diskspaceManagerData.DriveInfoList[studyItem.DriveID].DeletedFileSpace += usedspace;
            _diskspaceManagerData.DriveInfoList[studyItem.DriveID].DeletedStudyNumber += 1;
            studyItem.Status = DiskspaceManagerStatus.ExistsOnDrive;
            //Platform.Log("    A#: " + studyItem.AccessionNumber + "; UsedSpace: " + studyItem.UsedSpace + "; StoreTime: " + studyItem.StoreTime
            //    + "; DICOMFiles: " + studyItem.SopItemList.Count + "; StudyInstanceUID: " + studyItem.StudyInstanceUID);
            return;
        }

        private bool FindDiskspaceManagerDBAccessExtensionPoint()
        {
            DiskspaceManagerDBAccessExtensionPoint xp = new DiskspaceManagerDBAccessExtensionPoint();
            object[] dmObjects = xp.CreateExtensions();
            foreach (object dmObject in dmObjects)
            {
                if (dmObject is DiskspaceManagerDBAccess)
                {
                    DiskspaceManagerDBAccess studyDataAccess = (DiskspaceManagerDBAccess)dmObject;
                    studyDataAccess.SetComponent(_diskspaceManagerData);
                    return true;
                }
            }
            return false;
        }

        #region IDiskspaceManagerService Members

		public ServiceInformation GetServiceInformation()
        {
			// need to synchronize reading of the properties from currentDriveInfo object.
			lock (_settingsSyncLock)
			{
				DMDriveInfo currentDriveInfo = this.CurrentDriveInfo;
				if (currentDriveInfo == null || String.IsNullOrEmpty(currentDriveInfo.DriveName))
					throw new InvalidDataException(SR.ExceptionUnableToDetermineDrive);

				ServiceInformation returnInformation = new ServiceInformation();
				returnInformation.DriveName = DiskspaceManagerSettings.Instance.DriveName;
				returnInformation.DriveSize = currentDriveInfo.DriveSize;
				returnInformation.UsedSpace = currentDriveInfo.UsedSpace;
				returnInformation.LowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
				returnInformation.HighWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
				returnInformation.CheckFrequency = DiskspaceManagerSettings.Instance.CheckFrequency;

				return returnInformation;
			}
        }

		public void UpdateServiceConfiguration(ServiceConfiguration newConfiguration)
        {
			lock (_settingsSyncLock)
			{
				//DiskspaceManagerSettings.Instance.DriveName = newConfiguration.DriveName; //not configurable right now.
				DiskspaceManagerSettings.Instance.LowWatermark = newConfiguration.LowWatermark;
				DiskspaceManagerSettings.Instance.HighWatermark = newConfiguration.HighWatermark;
				DiskspaceManagerSettings.Instance.CheckFrequency = newConfiguration.CheckFrequency;
				DiskspaceManagerSettings.Save();
			}
        }

		#endregion
	}
}
