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

    public partial class ServerSopClassSelectCriteria : EntitySelectCriteria
    {
        public ServerSopClassSelectCriteria()
        : base("ServerSopClass")
        {}
        public ServerSopClassSelectCriteria(ServerSopClassSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new ServerSopClassSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerSopClass", ColumnName="SopClassUid")]
        public ISearchCondition<String> SopClassUid
        {
            get
            {
              if (!SubCriteria.ContainsKey("SopClassUid"))
              {
                 SubCriteria["SopClassUid"] = new SearchCondition<String>("SopClassUid");
              }
              return (ISearchCondition<String>)SubCriteria["SopClassUid"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerSopClass", ColumnName="Description")]
        public ISearchCondition<String> Description
        {
            get
            {
              if (!SubCriteria.ContainsKey("Description"))
              {
                 SubCriteria["Description"] = new SearchCondition<String>("Description");
              }
              return (ISearchCondition<String>)SubCriteria["Description"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerSopClass", ColumnName="NonImage")]
        public ISearchCondition<Boolean> NonImage
        {
            get
            {
              if (!SubCriteria.ContainsKey("NonImage"))
              {
                 SubCriteria["NonImage"] = new SearchCondition<Boolean>("NonImage");
              }
              return (ISearchCondition<Boolean>)SubCriteria["NonImage"];
            } 
        }
    }
}
