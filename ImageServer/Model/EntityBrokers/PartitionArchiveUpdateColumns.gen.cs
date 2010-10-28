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
    using ClearCanvas.ImageServer.Enterprise;

   public class PartitionArchiveUpdateColumns : EntityUpdateColumns
   {
       public PartitionArchiveUpdateColumns()
       : base("PartitionArchive")
       {}
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartitionKey"] = new EntityUpdateColumn<ServerEntityKey>("ServerPartitionKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ArchiveTypeEnum")]
        public ArchiveTypeEnum ArchiveTypeEnum
        {
            set { SubParameters["ArchiveTypeEnum"] = new EntityUpdateColumn<ArchiveTypeEnum>("ArchiveTypeEnum", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="Description")]
        public String Description
        {
            set { SubParameters["Description"] = new EntityUpdateColumn<String>("Description", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="Enabled")]
        public Boolean Enabled
        {
            set { SubParameters["Enabled"] = new EntityUpdateColumn<Boolean>("Enabled", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ReadOnly")]
        public Boolean ReadOnly
        {
            set { SubParameters["ReadOnly"] = new EntityUpdateColumn<Boolean>("ReadOnly", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ArchiveDelayHours")]
        public Int32 ArchiveDelayHours
        {
            set { SubParameters["ArchiveDelayHours"] = new EntityUpdateColumn<Int32>("ArchiveDelayHours", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionArchive", ColumnName="ConfigurationXml")]
        public XmlDocument ConfigurationXml
        {
            set { SubParameters["ConfigurationXml"] = new EntityUpdateColumn<XmlDocument>("ConfigurationXml", value); }
        }
    }
}
