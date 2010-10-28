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

    public partial class ServerTransferSyntaxSelectCriteria : EntitySelectCriteria
    {
        public ServerTransferSyntaxSelectCriteria()
        : base("ServerTransferSyntax")
        {}
        public ServerTransferSyntaxSelectCriteria(ServerTransferSyntaxSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new ServerTransferSyntaxSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerTransferSyntax", ColumnName="Uid")]
        public ISearchCondition<String> Uid
        {
            get
            {
              if (!SubCriteria.ContainsKey("Uid"))
              {
                 SubCriteria["Uid"] = new SearchCondition<String>("Uid");
              }
              return (ISearchCondition<String>)SubCriteria["Uid"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerTransferSyntax", ColumnName="Description")]
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
        [EntityFieldDatabaseMappingAttribute(TableName="ServerTransferSyntax", ColumnName="Lossless")]
        public ISearchCondition<Boolean> Lossless
        {
            get
            {
              if (!SubCriteria.ContainsKey("Lossless"))
              {
                 SubCriteria["Lossless"] = new SearchCondition<Boolean>("Lossless");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Lossless"];
            } 
        }
    }
}
