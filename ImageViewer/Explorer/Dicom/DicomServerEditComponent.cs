using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerEditComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerEditComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerEditComponentViewExtensionPoint))]
    public class DicomServerEditComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerEditComponent(NewServerTree dicomServerTree)
        {
            _serverTree = dicomServerTree;
            if (_serverTree.CurrentNode.IsServer)
            {
                _serverName = _serverTree.CurrentNode.Name;
                _serverPath = _serverTree.CurrentNode.Path;
                _serverParentPath = _serverTree.CurrentNode.ParentPath;
                _serverLocation = (_serverTree.CurrentNode as Server).Location;
                _serverAE = (_serverTree.CurrentNode as Server).AETitle;
                _serverHost = (_serverTree.CurrentNode as Server).Host;
                _serverPort = (_serverTree.CurrentNode as Server).Port.ToString();
            }
            else
            {
                _serverName = "";
                _serverPath = _serverTree.CurrentNode.Path;
                _serverParentPath = _serverTree.CurrentNode.ParentPath;
                _serverLocation = "";
                _serverAE = "";
                _serverHost = "";
                _serverPort = "";
            }
        }

        public void Accept()
        {
            if (!IsServerPropertyValid() || !this.Modified)
                return;

            Server newServer = new Server(_serverName, _serverLocation, _serverParentPath, _serverHost, _serverAE, int.Parse(_serverPort));

            // edit current server
            if (_serverTree.CurrentNode.IsServer)
            {
                _serverTree.ReplaceDicomServerInCurrentGroup(newServer);
            }
            // add new server
            else if (_serverTree.CurrentNode.IsServerGroup)
            {
                (_serverTree.CurrentNode as ServerGroup).AddChild(newServer);
            }

            _serverTree.CurrentNode = newServer;
            _serverTree.SaveDicomServers();
            _serverTree.FireServerTreeUpdatedEvent();
            
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();

            return;
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private bool IsServerPropertyEmpty()
        {
            if (_serverName == null || _serverName.Equals("") || _serverAE == null || _serverAE.Equals("")
                || _serverHost == null || _serverHost.Equals("") || _serverPort == null || _serverPort.Equals(""))
            {
                return false;
            }

            return true;
        }

        private bool IsServerPropertyValid()
        {
            int port = -1;
            try
            {
                port = int.Parse(_serverPort);
                if (_serverTree.CurrentNode.IsServer
                        && _serverName.Equals(_serverTree.CurrentNode.Name)
                        && _serverAE.Equals((_serverTree.CurrentNode as Server).AETitle)
                        && _serverHost.Equals((_serverTree.CurrentNode as Server).Host)
                        && port == (_serverTree.CurrentNode as Server).Port)
                    return true;
            }
            catch
            {
                this.Modified = false;
                StringBuilder msgText = new StringBuilder();
				msgText.AppendFormat(SR.MessageServerPortMustBePositiveInteger);
                throw new DicomServerException(msgText.ToString());
            }

            if (port <= 0)
            {
                this.Modified = false;
				throw new DicomServerException(SR.MessageServerPortMustBePositiveInteger);
            }

            string conflictingServerPath;
            bool isConflicted = _serverTree.IsDuplicateServerInGroup(_serverTree.CurrentNode, _serverName, _serverAE, _serverHost, int.Parse(_serverPort), out conflictingServerPath);

            if (isConflicted)
            {
                this.Modified = false;
                StringBuilder msgText = new StringBuilder();
				msgText.AppendFormat(SR.FormatServerNameConflict, _serverName, conflictingServerPath);
                throw new DicomServerException(msgText.ToString());
            }

            return true;
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public bool FieldReadonly
        {
            get 
            {
                return _serverTree.CurrentNode.IsServer && _serverName.Equals(AENavigatorComponent.MyDatastoreTitle) ? true : false; 
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Fields

        private NewServerTree _serverTree;
        private string _serverName = "";
        private string _serverPath = "";
        private string _serverLocation = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";
        private string _serverParentPath = "";

        public NewServerTree ServerTree
        {
          get { return _serverTree; }
          set { _serverTree = value; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }

        public string ParentPath
        {
            get { return _serverParentPath; }
            set { _serverParentPath = value; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set 
            { 
                _serverName = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        public string ServerLocation
        {
            get { return _serverLocation; }
            set { 
                _serverLocation = value;
                this.Modified = true;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { 
                _serverAE = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        public string ServerHost
        {
            get { return _serverHost; }
            set { 
                _serverHost = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { 
                _serverPort = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        #endregion

    }

    public class DicomServerException : Exception
    {
        public DicomServerException(string message) : base(message) { }
        public DicomServerException(string message, Exception inner) : base(message, inner) { }
    }

}
