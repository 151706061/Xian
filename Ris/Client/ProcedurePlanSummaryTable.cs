using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class ProcedurePlanSummaryTableItem
    {
        private readonly RequestedProcedureDetail _rpDetail;
        private readonly ModalityProcedureStepDetail _mpsDetail;

        public ProcedurePlanSummaryTableItem(RequestedProcedureDetail rpDetail, ModalityProcedureStepDetail mpsDetail)
        {
            _rpDetail = rpDetail;
            _mpsDetail = mpsDetail;
        }

        #region Public Properties

        public RequestedProcedureDetail rpDetail
        {
            get { return _rpDetail; }
        }

        public ModalityProcedureStepDetail mpsDetail
        {
            get { return _mpsDetail; }
        }

        #endregion
    }

    public class ProcedurePlanSummaryTable : Table<Checkable<ProcedurePlanSummaryTableItem>>
    {
        private static readonly int NumRows = 2;
        private static readonly int ProcedureDescriptionRow = 1;

        private event EventHandler _checkedRowsChanged;

        public ProcedurePlanSummaryTable()
            : this(NumRows)
        {
        }

        private ProcedurePlanSummaryTable(int cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, bool>(
                                 "X",
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.IsChecked; },
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable, bool isChecked)
                                     {
                                         checkable.IsChecked = isChecked;
                                         EventsHelper.Fire(_checkedRowsChanged, this, EventArgs.Empty);
                                     },
                                 0.1f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 "Status",
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.mpsDetail.State.Value; },
                                 0.5f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 "Modality",
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.mpsDetail.ModalityName; },
                                 0.5f));

            ITableColumn sortColumn = new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>("Procedure Description",
                                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                                    {
                                        return string.Format("{0} - {1}", checkable.Item.rpDetail.Type.Name, checkable.Item.mpsDetail.ProcedureStepName);
                                    },
                                0.5f,
                                ProcedureDescriptionRow);
            this.Columns.Add(sortColumn);

            this.Sort(new TableSortParams(sortColumn, true));
        }

        public event EventHandler CheckedRowsChanged
        {
            add { _checkedRowsChanged += value; }
            remove { _checkedRowsChanged -= value; }
        }
    }
}