#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="VisitDetailsEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class VisitEditorDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// VisitEditorDetailsComponent class
	/// </summary>
	[AssociateView(typeof(VisitEditorDetailsComponentViewExtensionPoint))]
	public class VisitDetailsEditorComponent : ApplicationComponent
	{
		private VisitDetail _visit;

		private readonly List<EnumValueInfo> _visitNumberAssigningAuthorityChoices;
		private readonly List<EnumValueInfo> _patientClassChoices;
		private readonly List<EnumValueInfo> _patientTypeChoices;
		private readonly List<EnumValueInfo> _admissionTypeChoices;
		private readonly List<EnumValueInfo> _visitStatusChoices;
		private readonly List<FacilitySummary> _facilityChoices;
		private readonly List<LocationSummary> _locationChoices;

		private readonly List<EnumValueInfo> _ambulatoryStatusChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public VisitDetailsEditorComponent(
				List<EnumValueInfo> visitNumberAssigningAuthorityChoices,
				List<EnumValueInfo> patientClassChoices,
				List<EnumValueInfo> patientTypeChoices,
				List<EnumValueInfo> admissionTypeChoices,
				List<EnumValueInfo> ambulatoryStatusChoices,
				List<EnumValueInfo> visitStatusChoices,
				List<FacilitySummary> facilityChoices,
				List<LocationSummary> locationChoices)
		{
			_visitNumberAssigningAuthorityChoices = visitNumberAssigningAuthorityChoices;
			_patientClassChoices = patientClassChoices;
			_patientTypeChoices = patientTypeChoices;
			_admissionTypeChoices = admissionTypeChoices;
			_ambulatoryStatusChoices = ambulatoryStatusChoices;
			_visitStatusChoices = visitStatusChoices;
			_facilityChoices = facilityChoices;
			_locationChoices = locationChoices;
		}

		public VisitDetail Visit
		{
			get { return _visit; }
			set { _visit = value; }
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("DischargeDateTime",
				delegate
				{
					// only need to validate the if both AdmitTime and DischargeTime are specified
					if (!_visit.AdmitTime.HasValue || !_visit.DischargeTime.HasValue)
						return new ValidationResult(true, "");

					var ok = DateTime.Compare(_visit.DischargeTime.Value, _visit.AdmitTime.Value) >= 0;
					return new ValidationResult(ok, SR.MessageDischargeTimeMustBeLaterThanOrEqualToAdmitTime);
				}));

			if (_visit.VisitNumber == null)
			{
				_visit.VisitNumber = new CompositeIdentifierDetail
					{
						AssigningAuthority = _visitNumberAssigningAuthorityChoices[0]
					};
			}

			base.Start();
		}

		public override void Stop()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			base.Stop();
		}

		#region Presentation Model

		[ValidateNotNull]
		public string VisitNumber
		{
			get { return _visit.VisitNumber.Id; }
			set
			{
				_visit.VisitNumber.Id = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public EnumValueInfo VisitNumberAssigningAuthority
		{
			get { return _visit.VisitNumber.AssigningAuthority; }
			set
			{
				_visit.VisitNumber.AssigningAuthority = value;
				this.Modified = true;
			}
		}

		public IList VisitNumberAssigningAuthorityChoices
		{
			get { return this._visitNumberAssigningAuthorityChoices; }
		}

		public DateTime? AdmitDateTime
		{
			get { return _visit.AdmitTime; }
			set
			{
				_visit.AdmitTime = value;
				this.Modified = true;
			}
		}

		public DateTime? DischargeDateTime
		{
			get { return _visit.DischargeTime; }
			set
			{
				_visit.DischargeTime = value;
				this.Modified = true;
			}
		}

		public string DischargeDisposition
		{
			get { return _visit.DischargeDisposition; }
			set
			{
				_visit.DischargeDisposition = value;
				this.Modified = true;
			}
		}

		public string PreAdmitNumber
		{
			get { return _visit.PreadmitNumber; }
			set
			{
				_visit.PreadmitNumber = value;
				this.Modified = true;
			}
		}

		public bool Vip
		{
			get { return _visit.VipIndicator; }
			set
			{
				_visit.VipIndicator = value;
				this.Modified = true;
			}
		}

		public EnumValueInfo PatientClass
		{
			get { return _visit.PatientClass; }
			set
			{
				if (Equals(value, _visit.PatientClass))
					return;

				_visit.PatientClass = value;
				this.Modified = true;
			}
		}

		public IList PatientClassChoices
		{
			get { return _patientClassChoices; }
		}

		public EnumValueInfo PatientType
		{
			get { return _visit.PatientType; }
			set
			{
				if (Equals(value, _visit.PatientType))
					return;

				_visit.PatientType = value;
				this.Modified = true;
			}
		}

		public IList PatientTypeChoices
		{
			get { return _patientTypeChoices; }
		}

		public EnumValueInfo AdmissionType
		{
			get { return _visit.AdmissionType; }
			set
			{
				if (Equals(value, _visit.AdmissionType))
					return;

				_visit.AdmissionType = value;
				this.Modified = true;
			}
		}

		public IList AdmissionTypeChoices
		{
			get { return _admissionTypeChoices; }
		}

		public IList AmbulatoryStatusChoices
		{
			get { return _ambulatoryStatusChoices; }
		}

		public EnumValueInfo AmbulatoryStatus
		{
			get { return CollectionUtils.FirstElement(_visit.AmbulatoryStatuses); }
			set
			{
				if (_visit.AmbulatoryStatuses.Contains(value))
					return;

				_visit.AmbulatoryStatuses.Clear();
				_visit.AmbulatoryStatuses.Add(value);
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public EnumValueInfo VisitStatus
		{
			get { return _visit.Status; }
			set
			{
				if (Equals(value, _visit.Status))
					return;

				_visit.Status = value;
				this.Modified = true;
			}
		}

		public IList VisitStatusChoices
		{
			get { return _visitStatusChoices; }
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		[ValidateNotNull]
		public FacilitySummary Facility
		{
			get { return _visit.Facility; }
			set
			{
				if (Equals(value, _visit.Facility))
					return;

				_visit.Facility = value;
				this.Modified = true;
			}
		}

		public string FormatFacility(object item)
		{
			var f = (FacilitySummary) item;
			return f.Name;
		}

		public IList CurrentLocationChoices
		{
			get { return _locationChoices; }
		}

		public LocationSummary CurrentLocation
		{
			get { return _visit.CurrentLocation; }
			set
			{
				if (Equals(value, _visit.CurrentLocation))
					return;

				_visit.CurrentLocation = value;
				this.Modified = true;
			}
		}

		public string FormatCurrentLocation(object item)
		{
			var l = (LocationSummary) item;
			return l.Name;
		}

		#endregion
	}
}
