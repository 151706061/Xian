#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alerts;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Application.Services.BrowsePatientData
{
	[ServiceImplementsContract(typeof(IBrowsePatientDataService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class BrowsePatientDataService : ApplicationServiceBase, IBrowsePatientDataService
	{
		#region IBrowsePatientDataService Members

		[ReadOperation]
		public GetDataResponse GetData(GetDataRequest request)
		{
			var response = new GetDataResponse();

			if (request.ListPatientProfilesRequest != null)
				response.ListPatientProfilesResponse = ListPatientProfiles(request.ListPatientProfilesRequest);

			if (request.GetPatientProfileDetailRequest != null)
				response.GetPatientProfileDetailResponse = GetPatientProfileDetail(request.GetPatientProfileDetailRequest);

			if (request.ListVisitsRequest != null)
				response.ListVisitsResponse = ListVisits(request.ListVisitsRequest);

			if (request.GetVisitDetailRequest != null)
				response.GetVisitDetailResponse = GetVisitDetail(request.GetVisitDetailRequest);

			if (request.ListOrdersRequest != null)
				response.ListOrdersResponse = ListOrders(request.ListOrdersRequest);

			if (request.GetOrderDetailRequest != null)
				response.GetOrderDetailResponse = GetOrderDetail(request.GetOrderDetailRequest);

			if (request.ListReportsRequest != null)
				response.ListReportsResponse = ListReports(request.ListReportsRequest);

			if (request.GetReportDetailRequest != null)
				response.GetReportDetailResponse = GetReportDetail(request.GetReportDetailRequest);

			return response;
		}

		#endregion


		private ListPatientProfilesResponse ListPatientProfiles(ListPatientProfilesRequest request)
		{
			var patient = this.PersistenceContext.Load<Patient>(request.PatientRef);

			var patientProfileAssembler = new PatientProfileAssembler();
			return new ListPatientProfilesResponse(
				CollectionUtils.Map<PatientProfile, PatientProfileSummary>(
					patient.Profiles,
					profile => patientProfileAssembler.CreatePatientProfileSummary(profile, this.PersistenceContext)));
		}

		private GetPatientProfileDetailResponse GetPatientProfileDetail(GetPatientProfileDetailRequest request)
		{
			var profile = this.PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);

			var patientProfileAssembler = new PatientProfileAssembler();
			var response = new GetPatientProfileDetailResponse();

			response.PatientProfile = patientProfileAssembler.CreatePatientProfileDetail(
				profile,
				this.PersistenceContext,
				request.IncludeAddresses,
				request.IncludeContactPersons,
				request.IncludeEmailAddresses,
				request.IncludeTelephoneNumbers,
				request.IncludeNotes,
				request.IncludeAttachments,
				request.IncludeAllergies);

			if (request.IncludeAlerts)
			{
				var alerts = new List<AlertNotification>();
				alerts.AddRange(AlertHelper.Instance.Test(profile.Patient, this.PersistenceContext));
				alerts.AddRange(AlertHelper.Instance.Test(profile, this.PersistenceContext));

				var alertAssembler = new AlertAssembler();
				response.PatientAlerts = CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(alerts, alertAssembler.CreateAlertNotification);
			}

			return response;
		}

		private ListVisitsResponse ListVisits(ListVisitsRequest request)
		{
			var browsePatientDataAssembler = new BrowsePatientDataAssembler();

			var patient = this.PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);

			var where = new VisitSearchCriteria();
			where.Patient.EqualTo(patient);

			var visits = this.PersistenceContext.GetBroker<IVisitBroker>().Find(where);
			return new ListVisitsResponse(
				CollectionUtils.Map<Visit, VisitListItem>(
					visits,
					v => browsePatientDataAssembler.CreateVisitListItem(v, this.PersistenceContext)));
		}

		private GetVisitDetailResponse GetVisitDetail(GetVisitDetailRequest request)
		{
			var visit = this.PersistenceContext.Load<Visit>(request.VisitRef, EntityLoadFlags.Proxy);

			var assembler = new VisitAssembler();
			var detail = assembler.CreateVisitDetail(visit, this.PersistenceContext);

			return new GetVisitDetailResponse(detail);
		}

		private ListOrdersResponse ListOrders(ListOrdersRequest request)
		{
			var assembler = new BrowsePatientDataAssembler();
			var patient = this.PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);

			switch (request.QueryDetailLevel)
			{
				case PatientOrdersQueryDetailLevel.Order:
					return new ListOrdersResponse(
						CollectionUtils.Map<Order, OrderListItem>(
							this.PersistenceContext.GetBroker<IPatientHistoryBroker>().GetOrderHistory(patient),
							order => assembler.CreateOrderListItem(order, this.PersistenceContext)));
				case PatientOrdersQueryDetailLevel.Procedure:
					return new ListOrdersResponse(
						CollectionUtils.Map<Procedure, OrderListItem>(
							this.PersistenceContext.GetBroker<IPatientHistoryBroker>().GetProcedureHistory(patient),
							rp => assembler.CreateOrderListItem(rp, this.PersistenceContext)));
			}

			return new ListOrdersResponse(new List<OrderListItem>());
		}

		private GetOrderDetailResponse GetOrderDetail(GetOrderDetailRequest request)
		{
			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);

			var response = new GetOrderDetailResponse();
			var orderAssembler = new OrderAssembler();
			var createOrderDetailOptions = new OrderAssembler.CreateOrderDetailOptions(
				request.IncludeVisit,
				request.IncludeProcedures,
				request.IncludeNotes,
				request.NoteCategoriesFilter,
				request.IncludeAttachments,
				request.IncludeResultRecipients,
				request.IncludeExtendedProperties);
			response.Order = orderAssembler.CreateOrderDetail(order, createOrderDetailOptions, this.PersistenceContext);

			if (request.IncludeAlerts)
			{
				var alertAssembler = new AlertAssembler();
				response.OrderAlerts =
					CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(
						AlertHelper.Instance.Test(order, this.PersistenceContext),
						alertAssembler.CreateAlertNotification);
			}

			return response;
		}

		private ListReportsResponse ListReports(ListReportsRequest request)
		{
			IList<Report> reports = new List<Report>();
			if (request.OrderRef != null)
			{
				// list only reports for this order
				var order = this.PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.Proxy);
				reports = this.PersistenceContext.GetBroker<IPatientHistoryBroker>().GetReportsForOrder(order);
			}
			else if (request.PatientRef != null)
			{
				var patient = this.PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);
				reports = this.PersistenceContext.GetBroker<IPatientHistoryBroker>().GetReportHistory(patient);
			}

			var assembler = new BrowsePatientDataAssembler();
			var reportListItems = new List<ReportListItem>();

			foreach (var report in reports)
			{
				foreach (var procedure in report.Procedures)
				{
					reportListItems.Add(assembler.CreateReportListItem(report, procedure, this.PersistenceContext));
				}
			}

			return new ListReportsResponse(reportListItems);
		}

		private GetReportDetailResponse GetReportDetail(GetReportDetailRequest request)
		{
			var report = request.ReportRef != null
				? this.PersistenceContext.Load<Report>(request.ReportRef)
				: this.PersistenceContext.Load<Procedure>(request.ProcedureRef).ActiveReport;

			return new GetReportDetailResponse(report == null ? null :
				new ReportAssembler().CreateReportDetail(report, request.IncludeCancelledParts, this.PersistenceContext));
		}
	}
}
