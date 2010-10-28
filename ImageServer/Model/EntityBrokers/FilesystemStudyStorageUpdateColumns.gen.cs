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

   public class FilesystemStudyStorageUpdateColumns : EntityUpdateColumns
   {
       public FilesystemStudyStorageUpdateColumns()
       : base("FilesystemStudyStorage")
       {}
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="StudyStorageGUID")]
        public ServerEntityKey StudyStorageKey
        {
            set { SubParameters["StudyStorageKey"] = new EntityUpdateColumn<ServerEntityKey>("StudyStorageKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="FilesystemGUID")]
        public ServerEntityKey FilesystemKey
        {
            set { SubParameters["FilesystemKey"] = new EntityUpdateColumn<ServerEntityKey>("FilesystemKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="ServerTransferSyntaxGUID")]
        public ServerEntityKey ServerTransferSyntaxKey
        {
            set { SubParameters["ServerTransferSyntaxKey"] = new EntityUpdateColumn<ServerEntityKey>("ServerTransferSyntaxKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemStudyStorage", ColumnName="StudyFolder")]
        public String StudyFolder
        {
            set { SubParameters["StudyFolder"] = new EntityUpdateColumn<String>("StudyFolder", value); }
        }
    }
}
