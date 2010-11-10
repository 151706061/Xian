﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alerts;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IExternalPractitionerAdminService))]
	public class ExternalPractitionerAdminService : ApplicationServiceBase, IExternalPractitionerAdminService
	{
		private delegate TOutput Converter<TInput1, TInput2, TOutput>(TInput1 input1, TInput2 input2);

		#region IExternalPractitionerAdminService Members

		[ReadOperation]
		// note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin
		// because it is used in non-admin situations - perhaps we need to create a separate operation???
		public ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request)
		{
			var assembler = new ExternalPractitionerAssembler();

			var criteria = new ExternalPractitionerSearchCriteria();

			if (request.SortByLastVerifiedTime)
			{
				if (request.SortAscending)
					criteria.LastVerifiedTime.SortAsc(0);
				else
					criteria.LastVerifiedTime.SortDesc(0);
			}
			else if (request.SortByLastEditedTime)
			{
				if (request.SortAscending)
					criteria.LastEditedTime.SortAsc(0);
				else
					criteria.LastEditedTime.SortDesc(0);
			}
			else
			{
				criteria.Name.FamilyName.SortAsc(0);
			}

			if (!string.IsNullOrEmpty(request.FirstName))
				criteria.Name.GivenName.StartsWith(request.FirstName);
			if (!string.IsNullOrEmpty(request.LastName))
				criteria.Name.FamilyName.StartsWith(request.LastName);

			switch (request.VerifiedState)
			{
				case VerifiedState.Verified:
					criteria.IsVerified.EqualTo(true);
					break;
				case VerifiedState.NotVerified:
					criteria.IsVerified.EqualTo(false);
					break;
			}

			if (request.LastVerifiedRangeFrom != null && request.LastVerifiedRangeUntil != null)
				criteria.LastVerifiedTime.Between(request.LastVerifiedRangeFrom, request.LastVerifiedRangeUntil);
			else if (request.LastVerifiedRangeFrom != null)
				criteria.LastVerifiedTime.MoreThanOrEqualTo(request.LastVerifiedRangeFrom);
			else if (request.LastVerifiedRangeUntil != null)
				criteria.LastVerifiedTime.LessThanOrEqualTo(request.LastVerifiedRangeUntil);

			if (!request.IncludeDeactivated)
				criteria.Deactivated.EqualTo(false);

			var results = new List<ExternalPractitionerSummary>();
			if (request.QueryItems)
				results = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
					PersistenceContext.GetBroker<IExternalPractitionerBroker>().Find(criteria, request.Page),
					s => assembler.CreateExternalPractitionerSummary(s, PersistenceContext));

			var itemCount = -1;
			if (request.QueryCount)
				itemCount = (int)PersistenceContext.GetBroker<IExternalPractitionerBroker>().Count(criteria);

			return new ListExternalPractitionersResponse(results, itemCount);
		}

		[ReadOperation]
		//[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
		public LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request)
		{
			// note that the version of the ExternalPractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
			var practitioner = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
			var assembler = new ExternalPractitionerAssembler();

			var response = new LoadExternalPractitionerForEditResponse { PractitionerDetail = assembler.CreateExternalPractitionerDetail(practitioner, this.PersistenceContext) };

			if (request.IncludeAlerts)
			{
				var alerts = new List<AlertNotification>();
				alerts.AddRange(AlertHelper.Instance.Test(practitioner, this.PersistenceContext));

				var alertAssembler = new AlertAssembler();
				response.Alerts = CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(alerts, alertAssembler.CreateAlertNotification);
			}

			return response;
		}

		[ReadOperation]
		public LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request)
		{
			return new LoadExternalPractitionerEditorFormDataResponse(
				EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
				(new SimplifiedPhoneTypeAssembler()).GetPractitionerPhoneTypeChoices(),
				EnumUtils.GetEnumValueList<ResultCommunicationModeEnum>(PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Create)]
		public AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request)
		{
			var prac = new ExternalPractitioner();

			var assembler = new ExternalPractitionerAssembler();
			assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

			prac.MarkEdited();
			var userCanVerify = Thread.CurrentPrincipal.IsInRole(Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification);
			if (request.MarkVerified && userCanVerify)
				prac.MarkVerified();

			PersistenceContext.Lock(prac, DirtyState.New);

			// ensure the new prac is assigned an OID before using it in the return value
			PersistenceContext.SynchState();

			return new AddExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Update)]
		public UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request)
		{
			var prac = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerDetail.PractitionerRef, EntityLoadFlags.CheckVersion);

			EnsureNoDeactivatedContactPointsWithActiveOrders(request.PractitionerDetail.ContactPoints);

			var assembler = new ExternalPractitionerAssembler();
			assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

			prac.MarkEdited();
			var userCanVerify = Thread.CurrentPrincipal.IsInRole(Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification);
			if (request.MarkVerified && userCanVerify)
				prac.MarkVerified();

			return new UpdateExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public DeleteExternalPractitionerResponse DeleteExternalPractitioner(DeleteExternalPractitionerRequest request)
		{
			try
			{
				var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
				var practitioner = broker.Load(request.PractitionerRef, EntityLoadFlags.Proxy);
				broker.Delete(practitioner);
				PersistenceContext.SynchState();
				return new DeleteExternalPractitionerResponse();
			}
			catch (PersistenceException)
			{
				throw new RequestValidationException(string.Format(SR.ExceptionFailedToDelete, TerminologyTranslator.Translate(typeof(ExternalPractitioner))));
			}
		}

		[ReadOperation]
		public TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request)
		{
			var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
			var assembler = new ExternalPractitionerAssembler();

			var helper = new TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary>(
					delegate
					{
						var rawQuery = request.TextQuery;

						var criteria = new List<ExternalPractitionerSearchCriteria>();

						// build criteria against names
						var names = TextQueryHelper.ParsePersonNames(rawQuery);
						criteria.AddRange(CollectionUtils.Map(names,
							delegate(PersonName n)
							{
								var sc = new ExternalPractitionerSearchCriteria();
								sc.Name.FamilyName.StartsWith(n.FamilyName);
								if (n.GivenName != null)
									sc.Name.GivenName.StartsWith(n.GivenName);
								return sc;
							}));

						// build criteria against identifiers
						var ids = TextQueryHelper.ParseIdentifiers(rawQuery);
						criteria.AddRange(CollectionUtils.Map(ids,
									 delegate(string word)
									 {
										 var c = new ExternalPractitionerSearchCriteria();
										 c.LicenseNumber.StartsWith(word);
										 return c;
									 }));

						return criteria.ToArray();
					},
					prac => assembler.CreateExternalPractitionerSummary(prac, PersistenceContext),
					(criteria, threshold) => broker.Count(criteria) <= threshold,
					broker.Find);

			return helper.Query(request);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Data.ExternalPractitioner)]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.ExternalPractitioner.Merge)]
		public MergeDuplicateContactPointResponse MergeDuplicateContactPoint(MergeDuplicateContactPointRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.RetainedContactPointRef, "RetainedContactPointRef");
			Platform.CheckMemberIsSet(request.ReplacedContactPointRef, "ReplacedContactPointRef");

			var dest = PersistenceContext.Load<ExternalPractitionerContactPoint>(request.RetainedContactPointRef, EntityLoadFlags.Proxy);
			var src = PersistenceContext.Load<ExternalPractitionerContactPoint>(request.ReplacedContactPointRef, EntityLoadFlags.Proxy);

			// if we are only doing a cost estimate, exit here without modifying any data
			if (request.EstimateCostOnly)
			{
				// compute cost estimate
				var cost = EstimateAffectedRecords(src);
				return new MergeDuplicateContactPointResponse(cost);
			}

			// merge contact points
			var result = ExternalPractitionerContactPoint.MergeContactPoints(
				dest,
				src,
				dest.Name,
				dest.Description,
				dest.PreferredResultCommunicationMode);

			// if user has verify permission, verify the practitioner
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.ExternalPractitionerVerification))
				result.Practitioner.MarkVerified();

			// queue work items to migrate orders
			foreach (var contactPoint in new[] { src, dest })
			{
				var queueItem = MergeWorkQueueItem.Create(contactPoint.GetRef());
				PersistenceContext.Lock(queueItem, DirtyState.New);
			}

			PersistenceContext.SynchState();

			var assembler = new ExternalPractitionerAssembler();
			return new MergeDuplicateContactPointResponse(assembler.CreateExternalPractitionerContactPointSummary(result));
		}

		[ReadOperation]
		public LoadMergeDuplicateContactPointFormDataResponse LoadMergeDuplicateContactPointFormData(LoadMergeDuplicateContactPointFormDataRequest request)
		{
			return new LoadMergeDuplicateContactPointFormDataResponse();
		}

		[UpdateOperation]
		public MergeExternalPractitionerResponse MergeExternalPractitioner(MergeExternalPractitionerRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.LeftPractitionerRef, "LeftPractitionerRef");
			Platform.CheckMemberIsSet(request.RightPractitionerRef, "RightPractitionerRef");

			// unpack the request, loading all required entities
			var left = PersistenceContext.Load<ExternalPractitioner>(request.LeftPractitionerRef, EntityLoadFlags.Proxy);
			var right = PersistenceContext.Load<ExternalPractitioner>(request.RightPractitionerRef, EntityLoadFlags.Proxy);

			var nameAssembler = new PersonNameAssembler();
			var name = new PersonName();
			nameAssembler.UpdatePersonName(request.Name, name);

			var defaultContactPoint = request.DefaultContactPointRef != null ?
				PersistenceContext.Load<ExternalPractitionerContactPoint>(request.DefaultContactPointRef) : null;

			var deactivatedContactPoints = request.DeactivatedContactPointRefs == null ? new List<ExternalPractitionerContactPoint>() :
				CollectionUtils.Map(request.DeactivatedContactPointRefs,
				(EntityRef cpRef) => PersistenceContext.Load<ExternalPractitionerContactPoint>(cpRef));

			var cpReplacements = CollectionUtils.Map(request.ContactPointReplacements ?? (new Dictionary<EntityRef, EntityRef>()),
				kvp => new KeyValuePair<ExternalPractitionerContactPoint, ExternalPractitionerContactPoint>(
							PersistenceContext.Load<ExternalPractitionerContactPoint>(kvp.Key, EntityLoadFlags.Proxy),
							PersistenceContext.Load<ExternalPractitionerContactPoint>(kvp.Value, EntityLoadFlags.Proxy)));

			// compute cost estimate
			var cost = EstimateAffectedRecords(right, left);

			// if we are only doing a cost estimate, exit here without modifying any data
			if (request.EstimateCostOnly)
				return new MergeExternalPractitionerResponse(cost);

			// merge the practitioners
			var result = ExternalPractitioner.MergePractitioners(left, right,
				name,
				request.LicenseNumber,
				request.BillingNumber,
				request.ExtendedProperties,
				defaultContactPoint,
				deactivatedContactPoints,
				cpReplacements
				);

			// if user has verify permission, verify the result
			if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.Data.ExternalPractitionerVerification))
				result.MarkVerified();

			// queue work items to migrate orders and visits
			foreach (var practitioner in new[] { right, left })
			{
				var queueItem = MergeWorkQueueItem.Create(practitioner.GetRef());
				PersistenceContext.Lock(queueItem, DirtyState.New);
			}

			PersistenceContext.SynchState();

			var assembler = new ExternalPractitionerAssembler();
			return new MergeExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(result, this.PersistenceContext));
		}

		[ReadOperation]
		public LoadMergeExternalPractitionerFormDataResponse LoadMergeExternalPractitionerFormData(LoadMergeExternalPractitionerFormDataRequest request)
		{
			var response = new LoadMergeExternalPractitionerFormDataResponse();

			if (request.PractitionerRef != null)
			{
				var broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
				var practitioner = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
				var duplicates = broker.GetMergeCandidates(practitioner);

				var assembler = new ExternalPractitionerAssembler();
				response.Duplicates = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary>(duplicates,
					item => assembler.CreateExternalPractitionerSummary(item, this.PersistenceContext));
			}

			return response;
		}

		#endregion

		private long EstimateAffectedRecords(ExternalPractitioner right, ExternalPractitioner left)
		{
			var rightOrderCount = QueryOrders<long>(right.ContactPoints, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient);
			var rightVisitCount = QueryVisits<long>(right, PersistenceContext.GetBroker<IVisitBroker>().CountByVisitPractitioner);
			var leftOrderCount = QueryOrders<long>(left.ContactPoints, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient);
			var leftVisitCount = QueryVisits<long>(left, PersistenceContext.GetBroker<IVisitBroker>().CountByVisitPractitioner);

			// total number of references
			var r = rightOrderCount + rightVisitCount;
			var l = leftOrderCount + leftVisitCount;

			// the cost is measured in terms of the total number of references that must be updated 
			return r + l;
		}

		private long EstimateAffectedRecords(ExternalPractitionerContactPoint target)
		{
			// number of contact point referneces that must be updated
			return QueryOrders<long>(new[] { target }, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient);
		}

		private static T QueryVisits<T>(ExternalPractitioner practitioner, Converter<VisitSearchCriteria, VisitPractitionerSearchCriteria, T> queryAction)
		{
			var visitsWhere = new VisitPractitionerSearchCriteria();
			visitsWhere.Practitioner.EqualTo(practitioner);
			return queryAction(new VisitSearchCriteria(), visitsWhere);
		}

		private static T QueryOrders<T>(ICollection<ExternalPractitionerContactPoint> contactPoints, Converter<OrderSearchCriteria, ResultRecipientSearchCriteria, T> queryAction)
		{
			return QueryOrders(contactPoints, false, queryAction);
		}

		private static T QueryOrders<T>(ICollection<ExternalPractitionerContactPoint> contactPoints, bool activeOnly, Converter<OrderSearchCriteria, ResultRecipientSearchCriteria, T> queryAction)
		{
			var recipientCriteria = new ResultRecipientSearchCriteria();
			recipientCriteria.PractitionerContactPoint.In(contactPoints);

			var orderCriteria = new OrderSearchCriteria();
			if (activeOnly)
			{
				// Active order search criteria
				orderCriteria.Status.In(new[] { OrderStatus.SC, OrderStatus.IP });
			}

			return queryAction(orderCriteria, recipientCriteria);
		}


		private void EnsureNoDeactivatedContactPointsWithActiveOrders(List<ExternalPractitionerContactPointDetail> details)
		{
			var broker = this.PersistenceContext.GetBroker<IExternalPractitionerContactPointBroker>();
			var contactPointsWithOrders = CollectionUtils.Select(
				details,
				detail =>
				{
					if (detail.ContactPointRef == null)
						return false;  // a new contact point will not have associated orders.q

					var contactPoint = broker.Load(detail.ContactPointRef);
					return contactPoint.Deactivated == false
						&& detail.Deactivated
						&& QueryOrders<long>(new[] { contactPoint }, true, PersistenceContext.GetBroker<IOrderBroker>().CountByResultRecipient) > 0;
				});

			if (contactPointsWithOrders.Count == 0)
				return;

			var contactPointNames = CollectionUtils.Map<ExternalPractitionerContactPointDetail, string>(
				contactPointsWithOrders,
				contactPoint => contactPoint.Name);
			var contactPointNameList = string.Join(", ", contactPointNames.ToArray());

			throw new RequestValidationException(string.Format(SR.ExceptionContactPointsHaveActiveOrders, contactPointNameList));
		}
	}
}
