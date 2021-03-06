#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public partial class ShredHostClient : System.ServiceModel.ClientBase<IShredHost>, IDisposable
    {

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
