#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    [DataContract]
    public class ResetUserPasswordRequest : DataContractBase
    {
        public ResetUserPasswordRequest(string userName)
        {
            this.UserName = userName;
        }

        [DataMember]
        public string UserName;
    }
}
