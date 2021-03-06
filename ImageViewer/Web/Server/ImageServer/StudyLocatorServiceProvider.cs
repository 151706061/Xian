#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class StudyLocatorServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IStudyRootQuery))
            {
                //just return an instance when in the same process/domain.
                return new StudyLocator();
            }

            return null;
        }

        #endregion
    }
}
	