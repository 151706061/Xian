#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class WorkQueueItemNotFoundException : BaseWebException
    {
        public WorkQueueItemNotFoundException(string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.WorkQueueItemNotFound);
            ErrorDescription = ExceptionMessages.WorkQueueItemNotFoundDescription;
            LogMessage = logMessage;
        }

        public WorkQueueItemNotFoundException()
        {
            ErrorMessage = string.Format(ExceptionMessages.WorkQueueItemNotFound);
            ErrorDescription = ExceptionMessages.WorkQueueItemNotFoundDescription;
            LogMessage = string.Empty;
        }
    }
}
