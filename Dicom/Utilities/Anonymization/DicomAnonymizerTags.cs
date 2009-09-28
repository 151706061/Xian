﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	public partial class DicomAnonymizer
	{
		private static readonly List<uint> TagsToNull = new List<uint>(GetTagsToNull());
		private static readonly List<uint> TagsToRemove = new List<uint>(GetTagsToRemove());
		private static readonly List<uint> UidsToRemap = new List<uint>(GetUidsToRemap());
		private static readonly List<uint> DateTimeTagsToAdjust = new List<uint>(GetDateTimeTagsToAdjust());

		private static IEnumerable<uint> GetTagsToRemove()
		{
			yield return DicomTags.InstanceCreatorUid;
			yield return DicomTags.StorageMediaFileSetUid;
			yield return DicomTags.RequestAttributesSequence;

			//A bunch of tags from Patient Identification and Patient Demographic Modules
			// which seem like they should be anonymized, but aren't explicitly covered by PS 3.15 E.1.
			// None of these attributes are crucial to maintain the DICOM model, so we'll just remove them.
			// See Also: http://groups.google.com/group/comp.protocols.dicom/browse_thread/thread/d3be2bf5dfdac19f#
			yield return DicomTags.OtherPatientIdsSequence;
			yield return DicomTags.PatientsBirthName;
			yield return DicomTags.PatientsMothersBirthName;
			yield return DicomTags.PatientsInsurancePlanCodeSequence;
			yield return DicomTags.PatientsPrimaryLanguageCodeSequence;
			yield return DicomTags.PatientsAddress;
			yield return DicomTags.MilitaryRank;
			yield return DicomTags.BranchOfService;
			yield return DicomTags.PatientsTelephoneNumbers;
			yield return DicomTags.ResponsiblePerson;
			yield return DicomTags.ResponsiblePersonRole;
			yield return DicomTags.ResponsibleOrganization;
		}

		private static IEnumerable<uint> GetUidsToRemap()
		{
			//Type 3, Sop Common
			//removetag: DicomTags.InstanceCreatorUid;

			//Type 1, Sop Common
			yield return DicomTags.SopInstanceUid;

			//Type 1 and 1C, many different modules and sequences
			yield return DicomTags.ReferencedSopInstanceUid;

			//Type 1, Series Module, but also Type 1 Hierarchical Sop Instance Reference Macro
			yield return DicomTags.StudyInstanceUid;
			//Type 1, Study Module, but also Type 1 Hierarchical Series Reference Macro
			yield return DicomTags.SeriesInstanceUid;

			//Type 1, Frame Of Reference
			yield return DicomTags.FrameOfReferenceUid;
			//Type 1, Synchronization Module
			yield return DicomTags.SynchronizationFrameOfReferenceUid;

			//Type 1C, Content Item Macro, Document Content Macro
			yield return DicomTags.Uid;

			//Type 3, Sop Instance Reference Macro, Instance Availability, Storage Commitment, Media Creation Management Module
			//removetag: DicomTags.StorageMediaFileSetUid;
			
			//Type 1C, STRUCTURE SET MODULE
			yield return DicomTags.ReferencedFrameOfReferenceUid;
			//Type 1C, STRUCTURE SET MODULE
			yield return DicomTags.RelatedFrameOfReferenceUid;
		}

		private static IEnumerable<uint> GetTagsToNull()
		{
			//Type 2, General Study
			yield return DicomTags.AccessionNumber;
			//Type 3, General Equipment
			yield return DicomTags.InstitutionName;
			//Type 3, General Equipment
			yield return DicomTags.InstitutionAddress;
			//Type 2, General Study
			yield return DicomTags.ReferringPhysiciansName;
			//Type 2, General Study
			yield return DicomTags.ReferringPhysiciansAddress;
			//VISIT ADMISSION MODULE
			yield return DicomTags.ReferringPhysiciansTelephoneNumbers;
			//Type 3, General Equipment
			yield return DicomTags.StationName;
			//Type 2, General Study
			yield return DicomTags.StudyDescription;
			//Type 3, General Series
			yield return DicomTags.SeriesDescription;
			//Type 3, General Equipment
			yield return DicomTags.InstitutionalDepartmentName;
			//Type 3, General Study
			yield return DicomTags.PhysiciansOfRecord;
			//Type 3, General Series
			yield return DicomTags.PerformingPhysiciansName;
			//Type 3, General Study
			yield return DicomTags.NameOfPhysiciansReadingStudy;
			//Type 3, General Series
			yield return DicomTags.OperatorsName;
			//Type 3, Patient Study
			yield return DicomTags.AdmittingDiagnosesDescription;
			//Type 3, General Image
			yield return DicomTags.DerivationDescription;
			//Type 2, Patient
			yield return DicomTags.PatientsName;
			//Type 1, Patient
			yield return DicomTags.PatientId;
			//Type 2, Patient
			yield return DicomTags.PatientsBirthDate;
			//Type 2, Patient
			yield return DicomTags.PatientsBirthTime;
			//Type 2, Patient
			yield return DicomTags.PatientsSex;
			//Type 3, Patient
			yield return DicomTags.OtherPatientIds;
			//Type 3, Patient
			yield return DicomTags.OtherPatientNames;
			//Type 3, Patient Study
			yield return DicomTags.PatientsAge;
			//Type 3, Patient Study
			yield return DicomTags.PatientsSize;
			//Type 3, Patient Study
			yield return DicomTags.PatientsWeight;
			//PATIENT IDENTIFICATION
			yield return DicomTags.MedicalRecordLocator;
			//Type 3, Patient
			yield return DicomTags.EthnicGroup;
			//Type 3, Patient Study
			yield return DicomTags.Occupation;
			//Type 3, Patient Study
			yield return DicomTags.AdditionalPatientHistory;
			//Type 3, Patient
			yield return DicomTags.PatientComments;
			//Type 3, General Equipment
			yield return DicomTags.DeviceSerialNumber;
			//Type 3, General Series
			yield return DicomTags.ProtocolName;
			//Type 2, General Study
			yield return DicomTags.StudyId;
			//Type 3, General Image
			yield return DicomTags.ImageComments;

			//NOTE: In RemoveTags
			//Type 3, General Series, RT Series, Mammo Series, Encapsulted Document
			//yield return DicomTags.RequestAttributesSequence;
			
			//NOTE: Specific to SR Documents; way too complicated to try and do this one at the moment.
			//Type 1C, Document Relationship Macro, SR Document Keys
			//yield return DicomTags.ContentSequence;
		}

		private static IEnumerable<uint> GetDateTimeTagsToAdjust()
		{
			// This isn't every single date and time tag, only ones that
			// might be affected by a change in study date.
			yield return DicomTags.InstanceCreationDate;
			yield return DicomTags.InstanceCreationTime;

			yield return DicomTags.SeriesDate;
			yield return DicomTags.SeriesTime;

			yield return DicomTags.AcquisitionDate;
			yield return DicomTags.AcquisitionTime;

			yield return DicomTags.ContentDate;
			yield return DicomTags.ContentTime;

			yield return DicomTags.AcquisitionDatetime;

			yield return DicomTags.DateOfSecondaryCapture;
			yield return DicomTags.TimeOfSecondaryCapture;

			yield return DicomTags.RadiopharmaceuticalStartDatetime;
			yield return DicomTags.RadiopharmaceuticalStopDatetime;
			
			yield return DicomTags.FrameAcquisitionDatetime;
			yield return DicomTags.FrameReferenceDatetime;
			
			yield return DicomTags.StartAcquisitionDatetime;
			yield return DicomTags.EndAcquisitionDatetime;
			
			yield return DicomTags.SubstanceAdministrationDatetime;
			
			yield return DicomTags.CreationDate;
			
			//yield return DicomTags.RtPlanDate;
			
			//yield return DicomTags.SourceStrengthReferenceDate;
		}
	}
}