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
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class Filesystem: ServerEntity
    {
        #region Constructors
        public Filesystem():base("Filesystem")
        {}
        public Filesystem(
             String _filesystemPath_
            ,Boolean _enabled_
            ,Boolean _readOnly_
            ,Boolean _writeOnly_
            ,FilesystemTierEnum _filesystemTierEnum_
            ,Decimal _lowWatermark_
            ,Decimal _highWatermark_
            ,String _description_
            ):base("Filesystem")
        {
            FilesystemPath = _filesystemPath_;
            Enabled = _enabled_;
            ReadOnly = _readOnly_;
            WriteOnly = _writeOnly_;
            FilesystemTierEnum = _filesystemTierEnum_;
            LowWatermark = _lowWatermark_;
            HighWatermark = _highWatermark_;
            Description = _description_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="FilesystemPath")]
        public String FilesystemPath
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="Enabled")]
        public Boolean Enabled
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="ReadOnly")]
        public Boolean ReadOnly
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="WriteOnly")]
        public Boolean WriteOnly
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="FilesystemTierEnum")]
        public FilesystemTierEnum FilesystemTierEnum
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="LowWatermark")]
        public Decimal LowWatermark
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="HighWatermark")]
        public Decimal HighWatermark
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="Filesystem", ColumnName="Description")]
        public String Description
        { get; set; }
        #endregion

        #region Static Methods
        static public Filesystem Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public Filesystem Load(IPersistenceContext read, ServerEntityKey key)
        {
            IFilesystemEntityBroker broker = read.GetBroker<IFilesystemEntityBroker>();
            Filesystem theObject = broker.Load(key);
            return theObject;
        }
        static public Filesystem Insert(Filesystem entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Filesystem newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public Filesystem Insert(IUpdateContext update, Filesystem entity)
        {
            IFilesystemEntityBroker broker = update.GetBroker<IFilesystemEntityBroker>();
            FilesystemUpdateColumns updateColumns = new FilesystemUpdateColumns();
            updateColumns.FilesystemPath = entity.FilesystemPath;
            updateColumns.Enabled = entity.Enabled;
            updateColumns.ReadOnly = entity.ReadOnly;
            updateColumns.WriteOnly = entity.WriteOnly;
            updateColumns.FilesystemTierEnum = entity.FilesystemTierEnum;
            updateColumns.LowWatermark = entity.LowWatermark;
            updateColumns.HighWatermark = entity.HighWatermark;
            updateColumns.Description = entity.Description;
            Filesystem newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
