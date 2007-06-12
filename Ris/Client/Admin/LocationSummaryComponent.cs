using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Locations")]
    [ClickHandler("launch", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.LocationAdmin)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class LocationSummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    LocationSummaryComponent component = new LocationSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleLocations,
                        delegate(IApplicationComponent c) { _workspace = null; });

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="LocationSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class LocationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// LocationSummaryComponent class
    /// </summary>
    [AssociateView(typeof(LocationSummaryComponentViewExtensionPoint))]
    public class LocationSummaryComponent : ApplicationComponent
    {
        private LocationSummary _selectedLocation;
        private LocationTable _locationTable;
        private CrudActionModel _locationActionHandler;

        private PagingController<LocationSummary> _pagingController;
        private PagingActionModel<LocationSummary> _pagingActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationSummaryComponent()
        {
        }

        public override void Start()
        {
            _locationTable = new LocationTable();
            _locationActionHandler = new CrudActionModel();
            _locationActionHandler.Add.SetClickHandler(AddLocation);
            _locationActionHandler.Edit.SetClickHandler(UpdateSelectedLocation);
            _locationActionHandler.Add.Enabled = true;
            _locationActionHandler.Delete.Enabled = false;

            InitialisePaging();
            _locationActionHandler.Merge(_pagingActionHandler);

            LoadLocationTable();

            base.Start();
        }

        private void InitialisePaging()
        {
            _pagingController = new PagingController<LocationSummary>(
                delegate(int firstRow, int maxRows)
                {
                    ListAllLocationsResponse listResponse = null;

                    Platform.GetService<ILocationAdminService>(
                        delegate(ILocationAdminService service)
                        {
                            ListAllLocationsRequest listRequest = new ListAllLocationsRequest(false);
                            listRequest.PageRequest.FirstRow = firstRow;
                            listRequest.PageRequest.MaxRows = maxRows;

                            listResponse = service.ListAllLocations(listRequest);
                        });

                    return listResponse.Locations;
                }
            );

            _pagingActionHandler = new PagingActionModel<LocationSummary>(_pagingController, _locationTable, Host.DesktopWindow);
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable Locations
        {
            get { return _locationTable; }
        }

        public ActionModelNode LocationListActionModel
        {
            get { return _locationActionHandler; }
        }

        public ISelection SelectedLocation
        {
            get { return _selectedLocation == null ? Selection.Empty : new Selection(_selectedLocation); }
            set
            {
                _selectedLocation = (LocationSummary)value.Item;
                LocationSelectionChanged();
            }
        }

        public void AddLocation()
        {
            try
            {
                LocationEditorComponent editor = new LocationEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddLocation);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _locationTable.Items.Add(_selectedLocation = editor.LocationSummary);
                    LocationSelectionChanged();
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedLocation()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedLocation == null) return;

                LocationEditorComponent editor = new LocationEditorComponent(_selectedLocation.LocationRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleUpdateLocation);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _locationTable.Items.Replace(delegate(LocationSummary l) { return l.LocationRef.Equals(editor.LocationSummary.LocationRef); }, editor.LocationSummary);
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }


        #endregion

        private void LoadLocationTable()
        {
            _locationTable.Items.Clear();
            _locationTable.Items.AddRange(_pagingController.GetFirst());
        }

        private void LocationSelectionChanged()
        {
            if (_selectedLocation != null)
                _locationActionHandler.Edit.Enabled = true;
            else
                _locationActionHandler.Edit.Enabled = false;

            this.NotifyPropertyChanged("SelectedLocation");
        }

    }
}
