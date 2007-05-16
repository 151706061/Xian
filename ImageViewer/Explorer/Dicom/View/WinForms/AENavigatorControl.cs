using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class AENavigatorControl : UserControl
	{
        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _component = component;

			ClearCanvasStyle.SetTitleBarStyle(_titleBar);
			
			ServerTreeUpdated += new EventHandler(OnServerTreeUpdated);
            this._aeTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this._aeTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            this._aeTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this._aeTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);

			this._titleBar.Visible = _component.ShowTitlebar;
			this._serverTools.Visible = _component.ShowTools;

			if (this._component.ShowTools)
			{
				this.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
				this.ToolbarModel = _component.ToolbarModel;
				this.MenuModel = _component.ContextMenuModel;
			}

			_aeTreeView.MouseDown += new MouseEventHandler(AETreeView_Click);
			_aeTreeView.AfterSelect += new TreeViewEventHandler(AETreeView_AfterSelect);

			_component.ServerTree.RootNode.LocalDataStoreNode.Updated += new EventHandler(OnLocalDataStoreNodeUpdated);
			BuildServerTreeView(_aeTreeView, _component.ServerTree);
        }

		void OnLocalDataStoreNodeUpdated(object sender, EventArgs e)
		{
			_aeTreeView.Nodes[0].ToolTipText = _component.ServerTree.RootNode.LocalDataStoreNode.ToString();
		}

        void OnServerTreeUpdated(object sender, EventArgs e)
        {
            if (_lastClickedNode == null)
                return;

			if (_component.UpdateType == 0)
				return;

			if (_component.UpdateType == (int)ServerUpdateType.Add)
            {
                IServerTreeNode dataChild = _component.ServerTree.CurrentNode;
                AddTreeNode(_lastClickedNode, dataChild);
                _lastClickedNode.Expand();
            }
            else if (_component.UpdateType == (int)ServerUpdateType.Delete)
            {
                _aeTreeView.SelectedNode = _lastClickedNode.Parent;
                _lastClickedNode.Remove();
                _lastClickedNode = _aeTreeView.SelectedNode;
            }
            else if (_component.UpdateType == (int)ServerUpdateType.Edit)
            {
                IServerTreeNode dataNode = _component.ServerTree.CurrentNode;
                _lastClickedNode.Text = dataNode.Name;
                _lastClickedNode.Tag = dataNode;
                _lastClickedNode.ToolTipText = dataNode.ToString();
            }
            _component.SetSelection(_lastClickedNode.Tag as IServerTreeNode);
        }

		void AETreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (_lastClickedNode != e.Node)
			{
				_lastClickedNode = _aeTreeView.SelectedNode;
				_component.SetSelection(e.Node.Tag as IServerTreeNode);
			}
		}

		void AETreeView_Click(object sender, EventArgs e)
		{
			TreeNode treeNode = _aeTreeView.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
			if (treeNode == null || treeNode == _lastClickedNode)
				return;

			_aeTreeView.SelectedNode = _lastClickedNode = treeNode;
			_component.SetSelection(_lastClickedNode.Tag as IServerTreeNode);
		}

        /// <summary>
        /// Builds the root and first-level of the tree
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="dataRoot"></param>
        private void BuildServerTreeView(TreeView treeView, ServerTree dicomServerTree)
        {
            treeView.Nodes.Clear();
            treeView.ShowNodeToolTips = true;

            // build the localdatastorenode
            TreeNode localDataStoreTreeNode = new TreeNode(dicomServerTree.RootNode.LocalDataStoreNode.Name);
            localDataStoreTreeNode.Tag = dicomServerTree.RootNode.LocalDataStoreNode;
            localDataStoreTreeNode.ToolTipText = dicomServerTree.RootNode.LocalDataStoreNode.ToString();
            SetIcon(dicomServerTree.RootNode.LocalDataStoreNode, localDataStoreTreeNode);
            treeView.Nodes.Add(localDataStoreTreeNode);
            treeView.SelectedNode = localDataStoreTreeNode;
            _lastClickedNode = localDataStoreTreeNode;

            // build the default server group
            TreeNode firstServerGroup = new TreeNode(dicomServerTree.RootNode.ServerGroupNode.Name);
            firstServerGroup.Tag = dicomServerTree.RootNode.ServerGroupNode;
            firstServerGroup.ToolTipText = dicomServerTree.RootNode.ServerGroupNode.ToString();
            SetIcon(dicomServerTree.RootNode.ServerGroupNode, firstServerGroup);
            treeView.Nodes.Add(firstServerGroup);

            BuildNextTreeLevel(firstServerGroup);
        }

        private void BuildNextTreeLevel(TreeNode serverGroupUITreeNode)
        {
            ServerGroup serverGroupNode = serverGroupUITreeNode.Tag as ServerGroup;
            if (null == serverGroupNode)
                return;

            foreach (ServerGroup childServerGroup in serverGroupNode.ChildGroups)
            {
                TreeNode childServerGroupUINode = AddTreeNode(serverGroupUITreeNode, childServerGroup);
                BuildNextTreeLevel(childServerGroupUINode);
            }

            foreach (Server server in serverGroupNode.ChildServers)
            {
                AddTreeNode(serverGroupUITreeNode, server);
            }
        }

        private TreeNode AddTreeNode(TreeNode treeNode, IServerTreeNode dataChild)
        {
            TreeNode treeChild = new TreeNode(dataChild.Name);
            SetIcon(dataChild, treeChild);
            treeChild.Tag = dataChild;
            treeChild.ToolTipText = dataChild.ToString();
            treeNode.Nodes.Add(treeChild);
            return treeChild;
        }

        private void SetIcon(IServerTreeNode browserNode, TreeNode treeNode)
		{
            if (browserNode == null)
				return;

            if (browserNode.IsServer || browserNode.IsLocalDataStore)
			{
                if (browserNode.Name == AENavigatorComponent.MyDatastoreTitle)
				{
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 0;
				}
				else
				{
					treeNode.ImageIndex = 1;
					treeNode.SelectedImageIndex = 1;
				}
			}
			else
			{
				treeNode.ImageIndex = 2;
				treeNode.SelectedImageIndex = 2;
			}
		}

        private AENavigatorComponent _component;
        private TreeNode _lastClickedNode;
        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
        private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
        private TreeNode _lastMouseOverNode = null;

        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                ToolStripBuilder.Clear(_serverTools.Items);
                if (_toolbarModel != null)
                {
                    ToolStripBuilder.BuildToolbar(_serverTools.Items, _toolbarModel.ChildNodes, _toolStripItemDisplayStyle);

                    foreach (ToolStripItem item in _serverTools.Items)
                        item.DisplayStyle = _toolStripItemDisplayStyle;
                }
            }
        }

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                ToolStripBuilder.Clear(_contextMenu.Items);
                if (_menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
                }
            }
        }

        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return _toolStripItemDisplayStyle; }
            set { _toolStripItemDisplayStyle = value; }
        }

        public event EventHandler ServerTreeUpdated
        {
            add { _component.ServerTree.ServerTreeUpdated += value; }
            remove { _component.ServerTree.ServerTreeUpdated -= value; }
        }

        private void treeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            // precondition
            TreeNode underPointerNode = _aeTreeView.GetNodeAt(_aeTreeView.PointToClient(new Point(e.X, e.Y)));
            if (null == underPointerNode)
                return;

            IServerTreeNode underPointerDataNode = underPointerNode.Tag as IServerTreeNode;
            IServerTreeNode lastClickedDataNode = _lastClickedNode.Tag as IServerTreeNode;

            // _lastMouseOverNode was the node that I highlighted last 
            // but its highlight must be turned off
            if ((_lastMouseOverNode != null) && (_lastMouseOverNode != underPointerNode))
            {
                _lastMouseOverNode.BackColor = Color.White;
                _lastMouseOverNode.ForeColor = Color.Black;
            }

            // highlight node only if the target node is a potential place
            // for us to drop a node for moving
            if (underPointerDataNode.CanAddAsChild(lastClickedDataNode))
            {
                underPointerNode.BackColor = Color.DarkBlue;
                underPointerNode.ForeColor = Color.White;
		
				_lastMouseOverNode = underPointerNode;
			}
        }

        private void treeView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
           if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode destinationNode = ((TreeView)sender).GetNodeAt(pt);
                TreeNode draggingNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                IServerTreeNode draggingDataNode = draggingNode.Tag as IServerTreeNode;
                IServerTreeNode destinationDataNode = destinationNode.Tag as IServerTreeNode;

                // turn off the white-on-blue highlight of a destination node
                destinationNode.BackColor = Color.White;
                destinationNode.ForeColor = Color.Black;
                _lastMouseOverNode.BackColor = Color.White;
                _lastMouseOverNode.ForeColor = Color.Black;
 
                // detecting if a node is being moved to the same/current parent, then do nothing
                // or if we're not dragging into a server group, do nothing
                // or if you're dragging over a child group
                //if (destinationDataNode.Path == draggingDataNode.Path ||
                //    !destinationDataNode.IsServerGroup ||
                //    draggingDataNode.Path.IndexOf(destinationDataNode.Path) == -1)  // don't allow dropping a node into one of its own children

                if (!destinationDataNode.CanAddAsChild(draggingDataNode))
                {
                    return;
                }
                
                if (!_component.NodeMoved(destinationNode.Tag as IServerTreeNode, draggingNode.Tag as IServerTreeNode))
                    return;

                draggingNode.Remove();
                _lastClickedNode = destinationNode;

                // build up the destination
                destinationNode.Nodes.Clear();
                BuildNextTreeLevel(_lastClickedNode);
                destinationNode.Expand(); 
                _aeTreeView.SelectedNode = _lastClickedNode;
            }
        }

        private void _aeTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _lastClickedNode = e.Node;
            _aeTreeView.SelectedNode = _lastClickedNode;
            IServerTreeNode dataNode = _lastClickedNode.Tag as IServerTreeNode;
            _component.SetSelection(dataNode);
            _component.NodeDoubleClick();
        }

    }
}
