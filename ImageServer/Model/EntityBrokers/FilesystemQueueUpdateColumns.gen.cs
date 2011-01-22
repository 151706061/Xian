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
    using ClearCanvas.Dicom;
    using ClearCanvas.ImageServer.Enterprise;

   public class FilesystemQueueUpdateColumns : EntityUpdateColumns
   {
       public FilesystemQueueUpdateColumns()
       : base("FilesystemQueue")
       {}
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="FilesystemQueueTypeEnum")]
        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        {
            set { SubParameters["FilesystemQueueTypeEnum"] = new EntityUpdateColumn<FilesystemQueueTypeEnum>("FilesystemQueueTypeEnum", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="StudyStorageGUID")]
        public ServerEntityKey StudyStorageKey
        {
            set { SubParameters["StudyStorageKey"] = new EntityUpdateColumn<ServerEntityKey>("StudyStorageKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="FilesystemGUID")]
        public ServerEntityKey FilesystemKey
        {
            set { SubParameters["FilesystemKey"] = new EntityUpdateColumn<ServerEntityKey>("FilesystemKey", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="ScheduledTime")]
        public DateTime ScheduledTime
        {
            set { SubParameters["ScheduledTime"] = new EntityUpdateColumn<DateTime>("ScheduledTime", value); }
        }
       [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="SeriesInstanceUid")]
        public String SeriesInstanceUid
        {
            set { SubParameters["SeriesInstanceUid"] = new EntityUpdateColumn<String>("SeriesInstanceUid", value); }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="FilesystemQueue", ColumnName="QueueXml")]
        public XmlDocument QueueXml
        {
            set { SubParameters["QueueXml"] = new EntityUpdateColumn<XmlDocument>("QueueXml", value); }
        }
    }
}
