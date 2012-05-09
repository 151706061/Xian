﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public static class WorkItemRequestTypeProvider
    {
        private static List<Type> _knownTypes;
        private static readonly object SyncLock = new Object();

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
        {
            lock (SyncLock)
            {
                if (_knownTypes == null)
                {
                    // build the contract map by finding all types having a T attribute
                    _knownTypes = (from p in Platform.PluginManager.Plugins
                                from t in p.Assembly.GetTypes()
                                let a = AttributeUtils.GetAttribute<WorkItemKnownTypeAttribute>(t)
                                where (a != null)
                                select t).ToList();
                
                }

                return _knownTypes;
            }
        }
    }


    /// <summary>
    /// Base Request object for the creation of <see cref="WorkItem"/>s.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("b2d86945-96b7-4563-8281-02142e84ffc3")]
    [WorkItemKnownType]
    public abstract class WorkItemRequest : DataContractBase
    {
        [DataMember]
        public WorkItemPriorityEnum Priority { get; set; }

        [DataMember]
        public string WorkItemType { get; set; }

        [DataMember]
        public string UserName { get; set; }

        public abstract string ActivityDescription { get; }

        public abstract string ActivityTypeString { get; }

        public bool CancellationCanResultInPartialStudy { get; protected set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("00e165d6-44db-4bf4-b607-a8e82a395964")]
    [WorkItemKnownType]
    public class WorkItemPatient : PatientRootPatientIdentifier
    {
        public WorkItemPatient()
        { }

        public WorkItemPatient(IPatientData p) 
            : base(p)
        { }

        public WorkItemPatient(DicomAttributeCollection c)
            : base(c)
        { }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("3366be52-823c-484e-b0a7-7344fed16457")]
    [WorkItemKnownType]
    public class WorkItemStudy : StudyIdentifier
    {
        public WorkItemStudy()
        { }

        public WorkItemStudy(IStudyData s) 
            : base(s)
        {}

        public WorkItemStudy(DicomAttributeCollection c)
            : base(c)
        {            
            string modality = c[DicomTags.Modality].ToString();
            ModalitiesInStudy = new[] { modality };
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("E0BF69EF-1854-441c-9C1B-5D334094CB85")]
    [WorkItemKnownType]
    public abstract class WorkItemStudyRequest : WorkItemRequest
    {
        [DataMember(IsRequired = true)]
        public WorkItemStudy Study { get; set; }

        [DataMember(IsRequired = true)]
        public WorkItemPatient Patient { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("c6a4a14e-e877-45a3-871d-bb06054dd837")]
    [WorkItemKnownType]
    public abstract class DicomSendRequest : WorkItemStudyRequest
    {
        public static string WorkItemTypeString = "DicomSend";
        
        [DataMember]
        public string Destination { get; set; }

        [DataMember]
        public CompressionType CompressionType { get; set; }

        [DataMember]
        public int CompressionLevel { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("F0C1BA64-06BD-4E97-BE55-183915656811")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomSendStudyRequest : DicomSendRequest
    {
        public DicomSendStudyRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendStudyRequest_ActivityDescription, Destination); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomSendStudy; }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("EF7A33C7-6B8A-470D-98F4-796780D8E50E")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomSendSeriesRequest : DicomSendRequest
    {
        public DicomSendSeriesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSeriesRequest_ActivityDescription, Destination); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomSendSeries; }
        }
    }

    
        /// <summary>
    /// <see cref="WorkItemRequest"/> for sending series to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("75BD907A-45D3-471B-AD0A-DE13D422A794")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomSendSopRequest : DicomSendRequest
    {
        public DicomSendSopRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = true)]
        public string SeriesInstanceUid { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> SopInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSopRequest_ActivityDescription, Destination); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomSendSop; }
        }
    }


    /// <summary>
    /// <see cref="WorkItemRequest"/> for publishing files to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("46DCBBF6-A8B3-4F5E-8611-5712A2BBBEFC")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class PublishFilesRequest : DicomSendRequest
    {
        public PublishFilesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            DeletionBehaviour = DeletionBehaviour.None;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        [DataMember(IsRequired = false)]
        public List<string> FilePaths { get; set; }

        [DataMember(IsRequired = true)]
        public DeletionBehaviour DeletionBehaviour { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomSendSeriesRequest_ActivityDescription, Destination); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumPublishFiles; }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for sending a study to a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("1c63c863-aa4e-4672-bee5-8aa3db16edd5")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomAutoRouteRequest : DicomSendRequest
    {
        public DicomAutoRouteRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = false)]
        public int? TimeWindowStart { get; set; }

        [DataMember(IsRequired = false)]
        public int? TimeWindowEnd { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomAutoRouteRequest_ActivityDescription, Destination, Patient.PatientsName); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumAutoRoute; }
        }
        
        public DateTime GetScheduledTime(DateTime currentTime, int postponeSeconds)
        {
            if (!TimeWindowStart.HasValue || !TimeWindowEnd.HasValue || Priority == WorkItemPriorityEnum.Stat)
                return currentTime;

            if (TimeWindowStart.Value > TimeWindowEnd.Value)
            {
                if (currentTime.Hour >= TimeWindowStart.Value
                    || currentTime.Hour < TimeWindowEnd.Value)
                {
                    return currentTime.AddSeconds(postponeSeconds);
                }

                return currentTime.Date.AddHours(TimeWindowStart.Value);
            }

            if (currentTime.Hour >= TimeWindowStart.Value
                && currentTime.Hour < TimeWindowEnd.Value)
            {
                return currentTime.AddSeconds(postponeSeconds);
            }

            return currentTime.Hour < TimeWindowStart.Value
                       ? currentTime.Date.AddHours(TimeWindowStart.Value)
                       : currentTime.Date.Date.AddDays(1d).AddHours(TimeWindowStart.Value);
        }
    }

    [DataContract(Name = "DeletionBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
    public enum DeletionBehaviour
    {
        [EnumMember]
        DeleteOnSuccess = 0,
        [EnumMember]
        DeleteAlways,
        [EnumMember]
        None
    }


    [DataContract(Name = "BadFileBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
    public enum BadFileBehaviourEnum
    {
        [EnumMember]
        Ignore = 0,
        [EnumMember]
        Move,
        [EnumMember]
        Delete
    }

    [DataContract(Name = "FileImportBehaviour", Namespace = ImageViewerWorkItemNamespace.Value)]
    public enum FileImportBehaviourEnum
    {
        [EnumMember]
        Move = 0,
        [EnumMember]
        Copy,
        [EnumMember]
        Save
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for importing files/studies.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
	[WorkItemRequestDataContract("02b7d427-1107-4458-ade3-67ee6779a766")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class ImportFilesRequest : WorkItemRequest
    {
        public static string WorkItemTypeString = "Import";
      
        public ImportFilesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.High;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = true)]
        public bool Recursive { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> FileExtensions { get; set; }

        [DataMember(IsRequired = true)]
        public List<string> FilePaths { get; set; }

        [DataMember(IsRequired = true)]
        public BadFileBehaviourEnum BadFileBehaviour { get; set; }

        [DataMember(IsRequired = true)]
        public FileImportBehaviourEnum FileImportBehaviour { get; set; }

        public override string ActivityDescription
        {
            get
            {
                return string.Format(FilePaths.Count > 1
                                         ? SR.ImportFilesRequest_ActivityDescriptionPlural
                                         : SR.ImportFilesRequest_ActivityDescription, FilePaths.Count);
            }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumImportFiles; }
        }
    }

    /// <summary>
    /// DICOM Retrieve Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("0e04fa53-3f45-4ae2-9444-f3208047757c")]
    [WorkItemKnownType]
    public abstract class DicomRetrieveRequest : WorkItemStudyRequest
    {
        public static string WorkItemTypeString = "DicomRetrieve";
   
        [DataMember]
        public string Source { get; set; }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("61AB2801-5284-480B-B054-F0314865D84F")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomRetrieveStudyRequest : DicomRetrieveRequest
    {
        public DicomRetrieveStudyRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveRequest_ActivityDescription, Source); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomRetrieve; }
        }
    }

    
    /// <summary>
    /// <see cref="WorkItemRequest"/> for retrieving a study from a DICOM AE.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("09547DF4-E8B8-45E8-ABAF-33159E2C7098")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomRetrieveSeriesRequest : DicomRetrieveRequest
    {
        public DicomRetrieveSeriesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
            CancellationCanResultInPartialStudy = true;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomRetreiveSeriesRequest_ActivityDescription, Source); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomRetrieve; }
        }
    }


    /// <summary>
    /// Abstract Study Process Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("4d22984a-e750-467c-ab89-f680be38c6c1")]
    [WorkItemKnownType]
    public abstract class ProcessStudyRequest : WorkItemStudyRequest
    {
        public static string WorkItemTypeString = "ProcessStudy";

        protected ProcessStudyRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.High;
            CancellationCanResultInPartialStudy = true;
        }
    }


    /// <summary>
    /// DICOM Receive Study Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("146cc54f-7b98-468b-948a-415eeffd3d7f")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DicomReceiveRequest : ProcessStudyRequest
    {
        [DataMember(IsRequired = true)]
        public string FromAETitle { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DicomReceiveRequest_ActivityDescription, FromAETitle); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDicomReceive; }
        }
    }

    /// <summary>
    /// DICOM Import Study Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("2def790a-8039-4fc5-85d6-f4d3be3f2d8e")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class ImportStudyRequest : ProcessStudyRequest
    {
        public override string ActivityDescription
        {
            get { return string.Format(SR.ImportStudyRequest_AcitivityDescription); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumImportStudy; }
        }
    }

    /// <summary>
    /// ReindexRequest Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("875D13F2-621D-4277-8A32-34D9BF5AE40B")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class ReindexRequest : WorkItemRequest
    {
        public static string WorkItemTypeString = "ReIndex";

        public ReindexRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.High;
        }

        public override string ActivityDescription
        {
            get { return SR.ReindexRequest_ActivityDescription; }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumReIndex; }
        }
    }

    /// <summary>
    /// ReapplyRules Request
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("9361447F-C14F-498C-B0EA-40664F2BB396")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class ReapplyRulesRequest : WorkItemRequest
    {
        public static string WorkItemTypeString = "ReapplyRules";
        
        public ReapplyRulesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.Normal;
        }

        [DataMember(IsRequired = true)]
        public string RuleId { get; set; }

        [DataMember(IsRequired = true)]
        public string RuleName { get; set; }

        [DataMember(IsRequired = true)]
        public RulesEngineContext RulesEngineContext { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.ReapplyRulesRequest_ActivityDescription, RuleName); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumReapplyRules; }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for deleting a study.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("0A7BE406-E5BE-4E10-997F-BCA37D53FED7")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DeleteStudyRequest : WorkItemStudyRequest
    {
        public static string WorkItemTypeString = "DeleteStudy";
        
        public DeleteStudyRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.High;
        }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DeleteStudyRequest_ActivityDescription); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDeleteStudy; }
        }
    }

    /// <summary>
    /// <see cref="WorkItemRequest"/> for deleting series.
    /// </summary>
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemRequestDataContract("64BCF5FF-1AFD-409B-B0EB-1E576124D61E")]
    [WorkItemKnownType]
    [WorkItemRequest]
    public class DeleteSeriesRequest : WorkItemStudyRequest
    {
        public static string WorkItemTypeString = "DeleteSeries";
        
        public DeleteSeriesRequest()
        {
            WorkItemType = WorkItemTypeString;
            Priority = WorkItemPriorityEnum.High;
        }

        [DataMember(IsRequired = false)]
        public List<string> SeriesInstanceUids { get; set; }

        public override string ActivityDescription
        {
            get { return string.Format(SR.DeleteSeriesRequest_ActivityDescription); }
        }

        public override string ActivityTypeString
        {
            get { return SR.ActivityTypeEnumDeleteSeries; }
        }
    }
}
