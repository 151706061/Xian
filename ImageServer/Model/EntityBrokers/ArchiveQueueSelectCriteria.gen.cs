#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;

    public partial class ArchiveQueueSelectCriteria : EntitySelectCriteria
    {
        public ArchiveQueueSelectCriteria()
        : base("ArchiveQueue")
        {}
        public ArchiveQueueSelectCriteria(ArchiveQueueSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new ArchiveQueueSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="PartitionArchiveGUID")]
        public ISearchCondition<ServerEntityKey> PartitionArchiveKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("PartitionArchiveKey"))
              {
                 SubCriteria["PartitionArchiveKey"] = new SearchCondition<ServerEntityKey>("PartitionArchiveKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["PartitionArchiveKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="ScheduledTime")]
        public ISearchCondition<DateTime> ScheduledTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("ScheduledTime"))
              {
                 SubCriteria["ScheduledTime"] = new SearchCondition<DateTime>("ScheduledTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["ScheduledTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="StudyStorageGUID")]
        public ISearchCondition<ServerEntityKey> StudyStorageKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("StudyStorageKey"))
              {
                 SubCriteria["StudyStorageKey"] = new SearchCondition<ServerEntityKey>("StudyStorageKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["StudyStorageKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="ArchiveQueueStatusEnum")]
        public ISearchCondition<ArchiveQueueStatusEnum> ArchiveQueueStatusEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("ArchiveQueueStatusEnum"))
              {
                 SubCriteria["ArchiveQueueStatusEnum"] = new SearchCondition<ArchiveQueueStatusEnum>("ArchiveQueueStatusEnum");
              }
              return (ISearchCondition<ArchiveQueueStatusEnum>)SubCriteria["ArchiveQueueStatusEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="ProcessorId")]
        public ISearchCondition<String> ProcessorId
        {
            get
            {
              if (!SubCriteria.ContainsKey("ProcessorId"))
              {
                 SubCriteria["ProcessorId"] = new SearchCondition<String>("ProcessorId");
              }
              return (ISearchCondition<String>)SubCriteria["ProcessorId"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ArchiveQueue", ColumnName="FailureDescription")]
        public ISearchCondition<String> FailureDescription
        {
            get
            {
              if (!SubCriteria.ContainsKey("FailureDescription"))
              {
                 SubCriteria["FailureDescription"] = new SearchCondition<String>("FailureDescription");
              }
              return (ISearchCondition<String>)SubCriteria["FailureDescription"];
            } 
        }
    }
}
