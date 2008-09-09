#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using ClearCanvas.ImageServer.Enterprise;

   public class WorkQueueUpdateColumns : EntityUpdateColumns
   {
       public WorkQueueUpdateColumns()
       : base("WorkQueue")
       {}
        public System.Xml.XmlDocument Data
        {
            set { SubParameters["Data"] = new EntityUpdateColumn<System.Xml.XmlDocument>("Data", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey DeviceKey
        {
            set { SubParameters["DeviceKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("DeviceKey", value); }
        }
        public System.DateTime ExpirationTime
        {
            set { SubParameters["ExpirationTime"] = new EntityUpdateColumn<System.DateTime>("ExpirationTime", value); }
        }
        public System.Int32 FailureCount
        {
            set { SubParameters["FailureCount"] = new EntityUpdateColumn<System.Int32>("FailureCount", value); }
        }
        public System.String FailureDescription
        {
            set { SubParameters["FailureDescription"] = new EntityUpdateColumn<System.String>("FailureDescription", value); }
        }
        public System.DateTime InsertTime
        {
            set { SubParameters["InsertTime"] = new EntityUpdateColumn<System.DateTime>("InsertTime", value); }
        }
        public System.String ProcessorID
        {
            set { SubParameters["ProcessorID"] = new EntityUpdateColumn<System.String>("ProcessorID", value); }
        }
        public System.DateTime ScheduledTime
        {
            set { SubParameters["ScheduledTime"] = new EntityUpdateColumn<System.DateTime>("ScheduledTime", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey ServerPartitionKey
        {
            set { SubParameters["ServerPartitionKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("ServerPartitionKey", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyHistoryKey
        {
            set { SubParameters["StudyHistoryKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("StudyHistoryKey", value); }
        }
        public ClearCanvas.ImageServer.Enterprise.ServerEntityKey StudyStorageKey
        {
            set { SubParameters["StudyStorageKey"] = new EntityUpdateColumn<ClearCanvas.ImageServer.Enterprise.ServerEntityKey>("StudyStorageKey", value); }
        }
        public WorkQueuePriorityEnum WorkQueuePriorityEnum
        {
            set { SubParameters["WorkQueuePriorityEnum"] = new EntityUpdateColumn<WorkQueuePriorityEnum>("WorkQueuePriorityEnum", value); }
        }
        public WorkQueueStatusEnum WorkQueueStatusEnum
        {
            set { SubParameters["WorkQueueStatusEnum"] = new EntityUpdateColumn<WorkQueueStatusEnum>("WorkQueueStatusEnum", value); }
        }
        public WorkQueueTypeEnum WorkQueueTypeEnum
        {
            set { SubParameters["WorkQueueTypeEnum"] = new EntityUpdateColumn<WorkQueueTypeEnum>("WorkQueueTypeEnum", value); }
        }
    }
}
