#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using ClearCanvas.Desktop;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using System;

namespace ClearCanvas.Ris.Client
{
    public interface IFolderExplorerToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        IFolder SelectedFolder { get; set; }
        ISelection SelectedItems { get; }
        void RegisterSearchDataHandler(ISearchDataHandler handler);
    }

    public class FolderExplorerToolBase : Tool<IFolderExplorerToolContext>
    {
        protected IFolderSystem _folderSystem;

        public IFolderSystem FolderSystem
        {
            get { return _folderSystem; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_folderSystem != null) _folderSystem.Dispose();
            }
        }
    }

    public class HomePageContainer : SplitComponentContainer, ISearchDataHandler
    {
        #region IFolderExplorerToolContext implementation

        class FolderExplorerToolContext : ToolContext, IFolderExplorerToolContext
        {
            private readonly HomePageContainer _component;

            public FolderExplorerToolContext(HomePageContainer component)
            {
                _component = component;
            }

            #region IFolderExplorerToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public IFolder SelectedFolder
            {
                get { return (IFolder) _component.SelectedFolderExplorer.SelectedFolder.Item; }
                set { _component.SelectedFolderExplorer.SelectedFolder = new Selection(value); }
            }

            public ISelection SelectedItems
            {
                get { return _component._folderContentComponent.SelectedItems; }
            }

            public void RegisterSearchDataHandler(ISearchDataHandler handler)
            {
                _component.RegisterSearchDataHandler(handler);
            }

            #endregion
        }

        #endregion

        #region Search related

        private ISearchDataHandler _searchDataHandler;

        public void RegisterSearchDataHandler(ISearchDataHandler handler)
        {
            _searchDataHandler = handler;
        }

        public SearchData SearchData
        {
            set
            {
                if (_searchDataHandler != null)
                    _searchDataHandler.SearchData = value;
            }
        }

        #endregion

        private readonly Dictionary<IFolderSystem, FolderExplorerComponent> _folderExplorerComponents;
        private readonly FolderContentsComponent _folderContentComponent;
        private readonly IApplicationComponent _previewComponent;

        private FolderExplorerComponent _selectedFolderExplorer;
        private readonly ToolSet _tools;

        public HomePageContainer(IExtensionPoint folderExplorerExtensionPoint, IApplicationComponent preview)
            : base(Desktop.SplitOrientation.Vertical)
        {
            _folderExplorerComponents = new Dictionary<IFolderSystem, FolderExplorerComponent>();
            _folderContentComponent = new FolderContentsComponent();
            _previewComponent = preview;

            _tools = new ToolSet(folderExplorerExtensionPoint, new FolderExplorerToolContext(this));

            // Construct the explorer component and place each into a stack tab
            StackTabComponentContainer explorerComponents = new StackTabComponentContainer(StackStyle.ShowOneOnly);
            CollectionUtils.ForEach(_tools.Tools,
                delegate(ITool tool)
                {
                    FolderExplorerToolBase folderExplorerTool = (FolderExplorerToolBase) tool;

                    FolderExplorerComponent component = new FolderExplorerComponent(folderExplorerTool.FolderSystem);
                    _folderExplorerComponents.Add(folderExplorerTool.FolderSystem, component);
                    explorerComponents.Pages.Add(new TabPage(folderExplorerTool.FolderSystem.DisplayName, component));

                    component.SelectedFolderChanged += OnSelectedFolderChanged;

                    // TODO: what does this suppress??
                    //component.SuppressSelectionChanged += _folderContentComponent.OnSuppressSelectionChanged;
                });

            explorerComponents.CurrentPageChanged += 
                delegate { this.SelectedFolderExplorer = (FolderExplorerComponent)explorerComponents.CurrentPage.Component; };


            // Construct the home page
            SplitComponentContainer contentAndPreview = new SplitComponentContainer(
                new SplitPane("Folder Contents", _folderContentComponent, 0.4f),
                new SplitPane("Content Preview", _previewComponent, 0.6f),
                SplitOrientation.Vertical);

            this.Pane1 = new SplitPane("Folders", explorerComponents, 0.2f);
            this.Pane2 = new SplitPane("Contents", contentAndPreview, 0.8f);
        }

        public FolderExplorerComponent SelectedFolderExplorer
        {
            get { return _selectedFolderExplorer; }
            set
            {
                _selectedFolderExplorer = value;

                if (_selectedFolderExplorer != null)
                {
                    _folderContentComponent.FolderSystem = _selectedFolderExplorer.FolderSystem;

                    IFolder selectedFolder = ((IFolder)_selectedFolderExplorer.SelectedFolder.Item);
                    _folderContentComponent.FolderContentsTable = selectedFolder == null ? null : selectedFolder.ItemsTable;
                }
            }
        }

        void OnSelectedFolderChanged(object sender, EventArgs e)
        {
            IFolder selectedFolder = ((IFolder) _selectedFolderExplorer.SelectedFolder.Item);
            _folderContentComponent.FolderContentsTable = selectedFolder == null ? null : selectedFolder.ItemsTable;
        }

        public FolderContentsComponent ContentsComponent
        {
            get { return _folderContentComponent; }
        }
    }
}
