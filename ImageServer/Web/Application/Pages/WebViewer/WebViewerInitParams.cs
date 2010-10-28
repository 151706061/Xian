#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.ObjectModel;

namespace ClearCanvas.ImageServer.Web.Application.Pages.WebViewer
{
    public class WebViewerInitParams
    {
        private Collection<string> _accessionNumbers = new Collection<string>();
        private Collection<string> _patientIds = new Collection<string>();
        private Collection<string> _studyInstanceUids = new Collection<string>();

        public Collection<string> AccessionNumbers
        {
            get { return _accessionNumbers; }
        }

        public Collection<string> PatientIds
        {
            get { return _patientIds; }
        }

        public Collection<string> StudyInstanceUids
        {
            get { return _studyInstanceUids; }
        }

        public string AeTitle
        {
            get;
            set;
        }

    }
}
