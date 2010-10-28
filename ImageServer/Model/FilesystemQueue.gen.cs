#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Xml;
    using ClearCanvas.Dicom;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class FilesystemQueue: ServerEntity
    {
        #region Constructors
        public FilesystemQueue():base("FilesystemQueue")
        {}
        public FilesystemQueue(
             FilesystemQueueTypeEnum _filesystemQueueTypeEnum_
            ,ServerEntityKey _studyStorageKey_
            ,ServerEntityKey _filesystemKey_
            ,DateTime _scheduledTime_
            ,String _seriesInstanceUid_
            ,XmlDocument _queueXml_
            ):base("FilesystemQueue")
        {
            FilesystemQueueTypeEnum = _filesystemQueueTypeEnum_;
            StudyStorageKey = _studyStorageKey_;
            FilesystemKey = _filesystemKey_;
            ScheduledTime = _scheduledTime_;
            SeriesInstanceUid = _seriesInstanceUid_;
            QueueXml = _queueXml_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="FilesystemQueueTypeEnum")]
        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="StudyStorageGUID")]
        public ServerEntityKey StudyStorageKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="FilesystemGUID")]
        public ServerEntityKey FilesystemKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="ScheduledTime")]
        public DateTime ScheduledTime
        { get; set; }
        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="QueueXml")]
        public XmlDocument QueueXml
        { get; set; }
        #endregion

        #region Static Methods
        static public FilesystemQueue Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public FilesystemQueue Load(IPersistenceContext read, ServerEntityKey key)
        {
            IFilesystemQueueEntityBroker broker = read.GetBroker<IFilesystemQueueEntityBroker>();
            FilesystemQueue theObject = broker.Load(key);
            return theObject;
        }
        static public FilesystemQueue Insert(FilesystemQueue entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                FilesystemQueue newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public FilesystemQueue Insert(IUpdateContext update, FilesystemQueue entity)
        {
            IFilesystemQueueEntityBroker broker = update.GetBroker<IFilesystemQueueEntityBroker>();
            FilesystemQueueUpdateColumns updateColumns = new FilesystemQueueUpdateColumns();
            updateColumns.FilesystemQueueTypeEnum = entity.FilesystemQueueTypeEnum;
            updateColumns.StudyStorageKey = entity.StudyStorageKey;
            updateColumns.FilesystemKey = entity.FilesystemKey;
            updateColumns.ScheduledTime = entity.ScheduledTime;
            updateColumns.SeriesInstanceUid = entity.SeriesInstanceUid;
            updateColumns.QueueXml = entity.QueueXml;
            FilesystemQueue newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
