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

    public partial class DeviceSelectCriteria : EntitySelectCriteria
    {
        public DeviceSelectCriteria()
        : base("Device")
        {}
        public DeviceSelectCriteria(DeviceSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new DeviceSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="Dhcp")]
        public ISearchCondition<Boolean> Dhcp
        {
            get
            {
              if (!SubCriteria.ContainsKey("Dhcp"))
              {
                 SubCriteria["Dhcp"] = new SearchCondition<Boolean>("Dhcp");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Dhcp"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="Enabled")]
        public ISearchCondition<Boolean> Enabled
        {
            get
            {
              if (!SubCriteria.ContainsKey("Enabled"))
              {
                 SubCriteria["Enabled"] = new SearchCondition<Boolean>("Enabled");
              }
              return (ISearchCondition<Boolean>)SubCriteria["Enabled"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AllowStorage")]
        public ISearchCondition<Boolean> AllowStorage
        {
            get
            {
              if (!SubCriteria.ContainsKey("AllowStorage"))
              {
                 SubCriteria["AllowStorage"] = new SearchCondition<Boolean>("AllowStorage");
              }
              return (ISearchCondition<Boolean>)SubCriteria["AllowStorage"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AcceptKOPR")]
        public ISearchCondition<Boolean> AcceptKOPR
        {
            get
            {
              if (!SubCriteria.ContainsKey("AcceptKOPR"))
              {
                 SubCriteria["AcceptKOPR"] = new SearchCondition<Boolean>("AcceptKOPR");
              }
              return (ISearchCondition<Boolean>)SubCriteria["AcceptKOPR"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AllowRetrieve")]
        public ISearchCondition<Boolean> AllowRetrieve
        {
            get
            {
              if (!SubCriteria.ContainsKey("AllowRetrieve"))
              {
                 SubCriteria["AllowRetrieve"] = new SearchCondition<Boolean>("AllowRetrieve");
              }
              return (ISearchCondition<Boolean>)SubCriteria["AllowRetrieve"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AllowQuery")]
        public ISearchCondition<Boolean> AllowQuery
        {
            get
            {
              if (!SubCriteria.ContainsKey("AllowQuery"))
              {
                 SubCriteria["AllowQuery"] = new SearchCondition<Boolean>("AllowQuery");
              }
              return (ISearchCondition<Boolean>)SubCriteria["AllowQuery"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AllowAutoRoute")]
        public ISearchCondition<Boolean> AllowAutoRoute
        {
            get
            {
              if (!SubCriteria.ContainsKey("AllowAutoRoute"))
              {
                 SubCriteria["AllowAutoRoute"] = new SearchCondition<Boolean>("AllowAutoRoute");
              }
              return (ISearchCondition<Boolean>)SubCriteria["AllowAutoRoute"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="ThrottleMaxConnections")]
        public ISearchCondition<Int16> ThrottleMaxConnections
        {
            get
            {
              if (!SubCriteria.ContainsKey("ThrottleMaxConnections"))
              {
                 SubCriteria["ThrottleMaxConnections"] = new SearchCondition<Int16>("ThrottleMaxConnections");
              }
              return (ISearchCondition<Int16>)SubCriteria["ThrottleMaxConnections"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="LastAccessedTime")]
        public ISearchCondition<DateTime> LastAccessedTime
        {
            get
            {
              if (!SubCriteria.ContainsKey("LastAccessedTime"))
              {
                 SubCriteria["LastAccessedTime"] = new SearchCondition<DateTime>("LastAccessedTime");
              }
              return (ISearchCondition<DateTime>)SubCriteria["LastAccessedTime"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="DeviceTypeEnum")]
        public ISearchCondition<DeviceTypeEnum> DeviceTypeEnum
        {
            get
            {
              if (!SubCriteria.ContainsKey("DeviceTypeEnum"))
              {
                 SubCriteria["DeviceTypeEnum"] = new SearchCondition<DeviceTypeEnum>("DeviceTypeEnum");
              }
              return (ISearchCondition<DeviceTypeEnum>)SubCriteria["DeviceTypeEnum"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="ServerPartitionGUID")]
        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("ServerPartitionKey"))
              {
                 SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="AeTitle")]
        public ISearchCondition<String> AeTitle
        {
            get
            {
              if (!SubCriteria.ContainsKey("AeTitle"))
              {
                 SubCriteria["AeTitle"] = new SearchCondition<String>("AeTitle");
              }
              return (ISearchCondition<String>)SubCriteria["AeTitle"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="Port")]
        public ISearchCondition<Int32> Port
        {
            get
            {
              if (!SubCriteria.ContainsKey("Port"))
              {
                 SubCriteria["Port"] = new SearchCondition<Int32>("Port");
              }
              return (ISearchCondition<Int32>)SubCriteria["Port"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="Description")]
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
        [EntityFieldDatabaseMappingAttribute(TableName="Device", ColumnName="IpAddress")]
        public ISearchCondition<String> IpAddress
        {
            get
            {
              if (!SubCriteria.ContainsKey("IpAddress"))
              {
                 SubCriteria["IpAddress"] = new SearchCondition<String>("IpAddress");
              }
              return (ISearchCondition<String>)SubCriteria["IpAddress"];
            } 
        }
    }
}
