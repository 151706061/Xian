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
    public partial class PartitionTransferSyntax: ServerEntity
    {
        #region Constructors
        public PartitionTransferSyntax():base("PartitionTransferSyntax")
        {}
        public PartitionTransferSyntax(
             ServerEntityKey _serverPartitionKey_
            ,ServerEntityKey _serverTransferSyntaxKey_
            ,Boolean _enabled_
            ):base("PartitionTransferSyntax")
        {
            ServerPartitionKey = _serverPartitionKey_;
            ServerTransferSyntaxKey = _serverTransferSyntaxKey_;
            Enabled = _enabled_;
        }
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionTransferSyntax", ColumnName="ServerPartitionGUID")]
        public ServerEntityKey ServerPartitionKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionTransferSyntax", ColumnName="ServerTransferSyntaxGUID")]
        public ServerEntityKey ServerTransferSyntaxKey
        { get; set; }
        [EntityFieldDatabaseMappingAttribute(TableName="PartitionTransferSyntax", ColumnName="Enabled")]
        public Boolean Enabled
        { get; set; }
        #endregion

        #region Static Methods
        static public PartitionTransferSyntax Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public PartitionTransferSyntax Load(IPersistenceContext read, ServerEntityKey key)
        {
            IPartitionTransferSyntaxEntityBroker broker = read.GetBroker<IPartitionTransferSyntaxEntityBroker>();
            PartitionTransferSyntax theObject = broker.Load(key);
            return theObject;
        }
        static public PartitionTransferSyntax Insert(PartitionTransferSyntax entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                PartitionTransferSyntax newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
        static public PartitionTransferSyntax Insert(IUpdateContext update, PartitionTransferSyntax entity)
        {
            IPartitionTransferSyntaxEntityBroker broker = update.GetBroker<IPartitionTransferSyntaxEntityBroker>();
            PartitionTransferSyntaxUpdateColumns updateColumns = new PartitionTransferSyntaxUpdateColumns();
            updateColumns.ServerPartitionKey = entity.ServerPartitionKey;
            updateColumns.ServerTransferSyntaxKey = entity.ServerTransferSyntaxKey;
            updateColumns.Enabled = entity.Enabled;
            PartitionTransferSyntax newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
        #endregion
    }
}
