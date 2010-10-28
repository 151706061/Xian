#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    public static class HtmlEncoder
    {
        static public string EncodeText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            return HttpUtility.HtmlEncode(text).Replace("\n", "<BR/>");
        }
    }
}
