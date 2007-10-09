using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PerformedProcedureComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PerformedProcedureComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PerformedProcedureComponent class
    /// </summary>
    [AssociateView(typeof(PerformedProcedureComponentViewExtensionPoint))]
    public class PerformedProcedureComponent : ApplicationComponent, IDocumentationPage
    {
        #region MPPS Details Component

        class MppsDetailsComponent : DHtmlComponent
        {
            private static readonly HtmlFormSelector _detailsFormSelector =
                new HtmlFormSelector(PerformedProcedureComponentSettings.Default.DetailsPageUrlSelectorScript, new string[] { "performedProcedureStep" });

            private readonly PerformedProcedureComponent _owner;

            public MppsDetailsComponent(PerformedProcedureComponent owner)
            {
                _owner = owner;
            }

            protected override string GetTagData(string tag)
            {
                string value;
                _owner._selectedMpps.ExtendedProperties.TryGetValue(tag, out value);
                return value;
            }

            protected override void SetTagData(string tag, string data)
            {
                _owner._selectedMpps.ExtendedProperties[tag] = data;
            }

            public void SelectedMppsChanged()
            {
                if (_owner._selectedMpps == null)
                {
                    SetUrl(null);
                }
                else
                {
                    SetUrl(_detailsFormSelector.SelectForm(_owner._selectedMpps));
                }
            }
        }

        #endregion

        private readonly TechnologistDocumentationMppsSummaryTable _mppsTable = new TechnologistDocumentationMppsSummaryTable();
        private ModalityPerformedProcedureStepSummary _selectedMpps;
        private EntityRef _orderRef;

        private SimpleActionModel _mppsActionHandler;
        private ClickAction _stopAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _mppsDetailsComponentHost;
        private MppsDetailsComponent _detailsComponent;
        private string _title;

        private event EventHandler<ProcedurePlanChangedEventArgs> _procedurePlanChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent(string title, EntityRef orderRef)
        {
            _title = title;
            _orderRef = orderRef;
        }

        internal void AddPerformedProcedureStep(ModalityPerformedProcedureStepSummary mpps)
        {
            _mppsTable.Items.Add(mpps);
            _mppsTable.Sort();
        }

        internal event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged
        {
            add { _procedurePlanChanged += value; }
            remove { _procedurePlanChanged -= value; }
        }

        internal void SaveData()
        {
            _detailsComponent.SaveData();
        }


        #region ApplicationComponent overrides

        public override void Start()
        {
            _mppsDetailsComponentHost = new ChildComponentHost(this.Host, _detailsComponent = new MppsDetailsComponent(this));
            //_mppsDetailsComponentHost.Title = SR.TitlePerformedProcedureComponent;
            _mppsDetailsComponentHost.StartComponent();

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _mppsActionHandler = new SimpleActionModel(resolver);

            _stopAction = _mppsActionHandler.AddAction("stop", SR.TitleStopMpps, "Icons.CompleteToolSmall.png", SR.TitleStopMpps, StopPerformedProcedureStep);
            _discontinueAction = _mppsActionHandler.AddAction("discontinue", SR.TitleDiscontinueMpps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMpps, DiscontinuePerformedProcedureStep);
            UpdateActionEnablement();

            if (_orderRef != null)
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                    {
                        ListPerformedProcedureStepsRequest mppsRequest = new ListPerformedProcedureStepsRequest(_orderRef);
                        ListPerformedProcedureStepsResponse mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

                        _mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
                    });
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region IDocumentationPage Members

        string IDocumentationPage.Title
        {
            get { return _title; }
        }

        IApplicationComponent IDocumentationPage.Component
        {
            get { return this; }   
        }

        #endregion

        #region Presentation Model

        public ITable MppsTable
        {
            get { return _mppsTable; }
        }

        public ISelection SelectedMpps
        {
            get { return new Selection(_selectedMpps); }
            set
            {
                _selectedMpps = (ModalityPerformedProcedureStepSummary)value.Item;
                UpdateActionEnablement();
                _detailsComponent.SelectedMppsChanged();
            }
        }

        public ActionModelNode MppsTableActionModel
        {
            get { return _mppsActionHandler; }
        }

        public ApplicationComponentHost DetailsComponentHost
        {
            get { return _mppsDetailsComponentHost; }
        }

        private void RefreshProcedurePlanTree(ProcedurePlanSummary procedurePlanSummary)
        {
            _orderRef = procedurePlanSummary.OrderRef;
            EventsHelper.Fire(_procedurePlanChanged, this, new ProcedurePlanChangedEventArgs(procedurePlanSummary));
        }
        
        #endregion

        #region Tool Click Handlers

        private void StopPerformedProcedureStep()
        {
            try
            {
                if (_selectedMpps != null)
                {
                    _detailsComponent.SaveData();

                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            StopModalityPerformedProcedureStepRequest request = new StopModalityPerformedProcedureStepRequest(
                                _selectedMpps.ModalityPerformendProcedureStepRef,
                                _selectedMpps.ExtendedProperties);
                            StopModalityPerformedProcedureStepResponse response = service.StopModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == _selectedMpps.ModalityPerformendProcedureStepRef;
                                },
                                response.StoppedMpps);

                            // Refresh selection
                            _selectedMpps = response.StoppedMpps;
                            UpdateActionEnablement();
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DiscontinuePerformedProcedureStep()
        {
            try
            {
                ModalityPerformedProcedureStepSummary selectedMpps = _selectedMpps;

                if (selectedMpps != null)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            //TODO should save details here too
                            DiscontinueModalityPerformedProcedureStepRequest request = new DiscontinueModalityPerformedProcedureStepRequest(selectedMpps.ModalityPerformendProcedureStepRef);
                            DiscontinueModalityPerformedProcedureStepResponse response = service.DiscontinueModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == selectedMpps.ModalityPerformendProcedureStepRef;
                                },
                                response.DiscontinuedMpps);

                            _selectedMpps = response.DiscontinuedMpps;
                            UpdateActionEnablement();
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateActionEnablement()
        {
            if(_selectedMpps != null)
            {
                // TOOD:  replace with server side logic
                _stopAction.Enabled = _discontinueAction.Enabled = _selectedMpps.State.Code == "IP";
            }
            else
            {
                _stopAction.Enabled = _discontinueAction.Enabled = false;
            }
        }

        #endregion
    }
}
