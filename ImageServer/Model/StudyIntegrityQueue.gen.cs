#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class StudyIntegrityQueue: ServerEntity
    {
        #region Constructors
        public StudyIntegrityQueue():base("StudyIntegrityQueue")
        {}
        public StudyIntegrityQueue(
             ServerEntityKey _serverPartitionKey_
            ,ServerEntityKey _studyStorageKey_
            ,DateTime _insertTime_
            ,XmlDocument _studyData_
            ,StudyIntegrityReasonEnum _studyIntegrityReasonEnum_
            ,String _groupID_
            ,XmlDocument _details_
            ,String _description_
            ):base("StudyIntegrityQueue")
        {
            ServerPartitionKey = _serverPartitionKey_;
            StudyStorageKey = _studyStorageKey_;
            InsertTime = _insertTime_;
            StudyData = _studyData_;
            StudyIntegrityReasonEnum = _studyIntegrityReasonEnum_;
            GroupID = _groupID_;
            Details = _details_;
            Description = _description_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="StudyStorageGUID")]
        public ServerEntityKey StudyStorageKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="InsertTime")]
        public DateTime InsertTime
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="StudyData")]
        public XmlDocument StudyData
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="StudyIntegrityReasonEnum")]
        public StudyIntegrityReasonEnum StudyIntegrityReasonEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="GroupID")]
        public String GroupID
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="Details")]
        public XmlDocument Details
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="StudyIntegrityQueue", ColumnName="Description")]
        public String Description
        { get; set; }
        #endregion

        #region Static Methods
        static public StudyIntegrityQueue Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public StudyIntegrityQueue Load(IPersistenceContext read, ServerEntityKey key)
        {
            IStudyIntegrityQueueEntityBroker broker = read.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueue theObject = broker.Load(key);
            return theObject;
        }
        static public StudyIntegrityQueue Insert(StudyIntegrityQueue entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyIntegrityQueue newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public StudyIntegrityQueue Insert(IUpdateContext update, StudyIntegrityQueue entity)
        {
            IStudyIntegrityQueueEntityBroker broker = update.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueUpdateColumns updateColumns = new StudyIntegrityQueueUpdateColumns();
            updateColumns.ServerPartitionKey = entity.ServerPartitionKey;
            updateColumns.StudyStorageKey = entity.StudyStorageKey;
            updateColumns.InsertTime = entity.InsertTime;
            updateColumns.StudyData = entity.StudyData;
            updateColumns.StudyIntegrityReasonEnum = entity.StudyIntegrityReasonEnum;
            updateColumns.GroupID = entity.GroupID;
            updateColumns.Details = entity.Details;
            updateColumns.Description = entity.Description;
            StudyIntegrityQueue newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
