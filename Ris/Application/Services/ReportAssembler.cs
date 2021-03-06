#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ReportAssembler
    {
        public ReportSummary CreateReportSummary(Procedure rp, Report report, IPersistenceContext context)
        {
            ReportSummary summary = new ReportSummary();

            ProcedureAssembler rpAssembler = new ProcedureAssembler();
            if (report != null)
            {
                summary.ReportRef = report.GetRef();
                summary.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);

                // use all procedures attached to report
                summary.Procedures = CollectionUtils.Map<Procedure, ProcedureSummary>(report.Procedures,
                    delegate(Procedure p) { return rpAssembler.CreateProcedureSummary(p, context); });
            }
            else
            {
                // use supplied procedure
                summary.Procedures = CollectionUtils.Map<Procedure, ProcedureSummary>(new Procedure[] { rp },
                    delegate(Procedure p) { return rpAssembler.CreateProcedureSummary(p, context); });
            }

            Order order = rp.Order;

            summary.VisitNumber = new VisitAssembler().CreateVisitNumberDetail(order.Visit.VisitNumber);
            summary.AccessionNumber = order.AccessionNumber;
            summary.DiagnosticServiceName = order.DiagnosticService.Name;

            return summary;
        }

        public ReportDetail CreateReportDetail(Report report, bool includeCancelledParts, IPersistenceContext context)
        {
            ReportDetail detail = new ReportDetail();
            detail.ReportRef = report.GetRef();
            detail.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);

            ProcedureAssembler rpAssembler = new ProcedureAssembler();
            detail.Procedures = CollectionUtils.Map<Procedure, ProcedureDetail>(report.Procedures,
                delegate(Procedure p)
                {
                    return rpAssembler.CreateProcedureDetail(
                        p,
                        delegate(ProcedureStep ps) { return ps.Is<ReportingProcedureStep>(); },	// only Reporting steps are relevant
                        false,	// exclude protocols
                        context);
                });

            List<ReportPartDetail> parts = CollectionUtils.Map<ReportPart, ReportPartDetail>(report.Parts,
                delegate(ReportPart part) { return CreateReportPartDetail(part, context); });

            detail.Parts = includeCancelledParts ? parts :
                CollectionUtils.Select(parts,
                    delegate(ReportPartDetail rpp)
                    {
                        return rpp.Status.Code.Equals(ReportPartStatus.X.ToString()) == false;
                    });

            return detail;
        }

        public ReportPartDetail CreateReportPartDetail(ReportPart reportPart, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();
            ReportPartDetail summary = new ReportPartDetail(
                reportPart.GetRef(),
                reportPart.Index,
                reportPart.Index > 0,
                EnumUtils.GetEnumValueInfo(reportPart.Status, context),
                reportPart.CreationTime,
                reportPart.PreliminaryTime,
                reportPart.CompletedTime,
                reportPart.CancelledTime,
                reportPart.Supervisor == null ? null : staffAssembler.CreateStaffSummary(reportPart.Supervisor, context),
                reportPart.Interpreter == null ? null : staffAssembler.CreateStaffSummary(reportPart.Interpreter, context),
                reportPart.Transcriber == null ? null : staffAssembler.CreateStaffSummary(reportPart.Transcriber, context),
                reportPart.TranscriptionSupervisor == null ? null : staffAssembler.CreateStaffSummary(reportPart.TranscriptionSupervisor, context),
                reportPart.Verifier == null ? null : staffAssembler.CreateStaffSummary(reportPart.Verifier, context),
                EnumUtils.GetEnumValueInfo(reportPart.TranscriptionRejectReason),
				ExtendedPropertyUtils.Copy(reportPart.ExtendedProperties));

            return summary;
        }
    }
}
