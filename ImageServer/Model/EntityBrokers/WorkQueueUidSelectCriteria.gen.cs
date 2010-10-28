#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;

    public partial class WorkQueueUidSelectCriteria : EntitySelectCriteria
    {
        public WorkQueueUidSelectCriteria()
        : base("WorkQueueUid")
        {}
        public WorkQueueUidSelectCriteria(WorkQueueUidSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new WorkQueueUidSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="WorkQueueGUID")]
        public ISearchCondition<ServerEntityKey> WorkQueueKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("WorkQueueKey"))
              {
                 SubCriteria["WorkQueueKey"] = new SearchCondition<ServerEntityKey>("WorkQueueKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["WorkQueueKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Failed")]
        public ISearchCondition<Boolean> Failed
        {
            get
            {
              if (!SubCriteria.ContainsKey("Failed"))
              {
                 SubCriteria["Failed"] = new SearchCondition<Boolean>("Failed");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Failed"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Duplicate")]
        public ISearchCondition<Boolean> Duplicate
        {
            get
            {
              if (!SubCriteria.ContainsKey("Duplicate"))
              {
                 SubCriteria["Duplicate"] = new SearchCondition<Boolean>("Duplicate");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Duplicate"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="FailureCount")]
        public ISearchCondition<Int16> FailureCount
        {
            get
            {
              if (!SubCriteria.ContainsKey("FailureCount"))
              {
                 SubCriteria["FailureCount"] = new SearchCondition<Int16>("FailureCount");
              }
              return (ISearchCondition<Int16>)SubCriteria["FailureCount"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="GroupID")]
        public ISearchCondition<String> GroupID
        {
            get
            {
              if (!SubCriteria.ContainsKey("GroupID"))
              {
                 SubCriteria["GroupID"] = new SearchCondition<String>("GroupID");
              }
              return (ISearchCondition<String>)SubCriteria["GroupID"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="RelativePath")]
        public ISearchCondition<String> RelativePath
        {
            get
            {
              if (!SubCriteria.ContainsKey("RelativePath"))
              {
                 SubCriteria["RelativePath"] = new SearchCondition<String>("RelativePath");
              }
              return (ISearchCondition<String>)SubCriteria["RelativePath"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="Extension")]
        public ISearchCondition<String> Extension
        {
            get
            {
              if (!SubCriteria.ContainsKey("Extension"))
              {
                 SubCriteria["Extension"] = new SearchCondition<String>("Extension");
              }
              return (ISearchCondition<String>)SubCriteria["Extension"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SeriesInstanceUid")]
        public ISearchCondition<String> SeriesInstanceUid
        {
            get
            {
              if (!SubCriteria.ContainsKey("SeriesInstanceUid"))
              {
                 SubCriteria["SeriesInstanceUid"] = new SearchCondition<String>("SeriesInstanceUid");
              }
              return (ISearchCondition<String>)SubCriteria["SeriesInstanceUid"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="WorkQueueUid", ColumnName="SopInstanceUid")]
        public ISearchCondition<String> SopInstanceUid
        {
            get
            {
              if (!SubCriteria.ContainsKey("SopInstanceUid"))
              {
                 SubCriteria["SopInstanceUid"] = new SearchCondition<String>("SopInstanceUid");
              }
              return (ISearchCondition<String>)SubCriteria["SopInstanceUid"];
            } 
        }
    }
}
