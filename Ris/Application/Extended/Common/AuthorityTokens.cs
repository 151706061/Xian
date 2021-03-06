#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Extended.Common
{
	/// <summary>
	/// Defines constants for all core RIS authority tokens.
	/// </summary>
	public static class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Admin
		{
			public static class Data
			{
				[AuthorityToken(Description = "Allow administration of Protocol Groups and Codes.")]
				public const string ProtocolGroups = "RIS/Admin/Data/Protocol Groups";
			}
		}

		/// <summary>
		/// Tokens that permit workflow actions.
		/// </summary>
		public static class Workflow
		{
			public static class Patient
			{
				[AuthorityToken(Description = "Allow reconciliation of existing Patient records.")]
				public const string Reconcile = "RIS/Workflow/Patient/Reconcile";
			}

			public static class Protocol
			{
				[AuthorityToken(Description = "Allow access to the Protocol Editor and creation of procedure protocols.")]
				public const string Create = "RIS/Workflow/Protocol/Create";

				[AuthorityToken(Description = "Allow verification of procedure protocols.")]
				public const string Accept = "RIS/Workflow/Protocol/Verify";

				[AuthorityToken(Description = "Allow orders that were rejected by the radiologist to be re-submitted for protocoling.")]
				public const string Resubmit = "RIS/Workflow/Protocol/Resubmit";

				[AuthorityToken(Description = "Allow procedure protocols to be submitted for review by another radiologist.")]
				public const string SubmitForReview = "RIS/Workflow/Protocol/Submit for Review";

				[AuthorityToken(Description = "Allow creation of procedure protocols without specifying a supervisor.")]
				public const string OmitSupervisor = "RIS/Workflow/Protocol/Omit Supervisor";

				[AuthorityToken(Description = "Allow re-assignment of a procedure protocol that is owned by one radiologist to another radiologist.")]
				public const string Reassign = "RIS/Workflow/Protocol/Reassign";

				[AuthorityToken(Description = "Allow cancellation of a procedure protocol that is currently owned by another radiologist.")]
				public const string Cancel = "RIS/Workflow/Protocol/Cancel";
			}

			public static class PreliminaryDiagnosis
			{
				[AuthorityToken(Description = "Allow creation of Preliminary Diagnosis conversations.")]
				public const string Create = "RIS/Workflow/Preliminary Diagnosis/Create";
			}

			public static class Worklist
			{
				[AuthorityToken(Description = "Allow printing of a worklist.")]
				public const string Print = "RIS/Workflow/Worklist/Print";
			}
		}

		/// <summary>
		/// Tokens that control access to Folder Systems.
		/// </summary>
		public static class FolderSystems
		{
			[AuthorityToken(Description = "Allow access to the Booking folder system.")]
			public const string Booking = "RIS/Folder Systems/Booking";

			[AuthorityToken(Description = "Allow access to the Protocolling folder system.")]
			public const string Protocolling = "RIS/Folder Systems/Protocolling";

			[AuthorityToken(Description = "Allow access to the Emergency folder system.")]
			public const string Emergency = "RIS/Folder Systems/Emergency";

			[AuthorityToken(Description = "Allow access to the Order Notes folder system.")]
			public const string OrderNotes = "RIS/Folder Systems/Order Notes";

			[AuthorityToken(Description = "Allow access to the External Practitioner folder system.")]
			public const string ExternalPractitioner = "RIS/Folder Systems/External Practitioner";
		}
	}
}
