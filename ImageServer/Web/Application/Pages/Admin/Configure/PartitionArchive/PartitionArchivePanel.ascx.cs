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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel.js")]
    /// <summary>
    /// Server parition panel  used in <seealso cref="Default"/> web page.
    /// </summary>
    public partial class PartitionArchivePanel : AJAXScriptControl
    {
        #region Private Members

        // list of partitions displayed in the list
        private IList<Model.PartitionArchive> _partitionArchives = new List<Model.PartitionArchive>();
        // used for database interaction
        private PartitionArchiveConfigController _theController;
		private ServerPartition _serverPartition;

        #endregion Private Members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeletePartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string RestoreButtonClientID
        {
            get { return EditPartitionButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("PartitionArchiveListClientID")]
        public string PartitionArchiveListClientID
        {
            get { return PartitionArchiveGridPanel.TheGrid.ClientID; }
        }

        // Sets/Gets the list of partitions displayed in the panel
        public IList<Model.PartitionArchive> PartitionArchives
        {
            get { return _partitionArchives; }
            set
            {
                _partitionArchives = value;
                PartitionArchiveGridPanel.Partitions = _partitionArchives;
            }
        }

        // Sets/Gets the controller used to retrieve load partitions.
        public PartitionArchiveConfigController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

		/// <summary>
		/// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
			set { _serverPartition = value; }
		}
        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            int archiveSelectedIndex = ArchiveTypeFilter.SelectedIndex;

            ArchiveTypeFilter.Items.Clear();
            ArchiveTypeFilter.Items.Add(new ListItem(App_GlobalResources.SR.All));
            foreach (ArchiveTypeEnum archiveTypeEnum in ArchiveTypeEnum.GetAll())
            {
                ArchiveTypeFilter.Items.Add(
                    new ListItem(archiveTypeEnum.Description, archiveTypeEnum.Lookup));
            }
            ArchiveTypeFilter.SelectedIndex = archiveSelectedIndex;

            int statusSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.All, App_GlobalResources.SR.All));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Enabled, App_GlobalResources.SR.Enabled));
            StatusFilter.Items.Add(new ListItem(App_GlobalResources.SR.Disabled, App_GlobalResources.SR.Disabled));
            StatusFilter.SelectedIndex = statusSelectedIndex;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new PartitionArchiveConfigController();

            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerPartitionSingleItem, App_GlobalResources.SR.GridPagerPartitionMultipleItems, PartitionArchiveGridPanel.TheGrid, delegate { return PartitionArchives.Count; }, ImageServerConstants.GridViewPagerPosition.Top);
            PartitionArchiveGridPanel.Pager = GridPagerTop;

        }

        public override void DataBind()
        {
            LoadData();
            base.DataBind();
        }

        protected void LoadData()
        {
            PartitionArchiveSelectCriteria criteria = new PartitionArchiveSelectCriteria();

            if (String.IsNullOrEmpty(DescriptionFilter.Text) == false)
            {
                string key = SearchHelper.TrailingWildCard(DescriptionFilter.Text);
                key = key.Replace("*", "%");
                criteria.Description.Like(key + "%");
            }

            if (StatusFilter.SelectedIndex > 0)
            {
                if (StatusFilter.SelectedIndex == 1)
                    criteria.Enabled.EqualTo(true);
                else
                    criteria.Enabled.EqualTo(false);
            }

        	criteria.ServerPartitionKey.EqualTo(ServerPartition.Key);

            PartitionArchives =
                _theController.GetPartitions(criteria);
            PartitionArchiveGridPanel.RefreshCurrentPage();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            DataBind();
        }

        protected void AddPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
           ((Default)Page).AddPartition(ServerPartition);
        }

        protected void EditPartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).EditPartition(selectedPartition);
            }
        }

        protected void DeletePartitionButton_Click(object sender, ImageClickEventArgs e)
        {
            Model.PartitionArchive selectedPartition =
                PartitionArchiveGridPanel.SelectedPartition;

            if (selectedPartition != null)
            {
                ((Default)Page).DeletePartition(selectedPartition);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            LoadData();
            PartitionArchiveGridPanel.UpdateUI();

            SearchUpdatePanel.Update();
        }

        #endregion Public methods       
    }
}