#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergePropertiesComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergePropertiesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class ExtendedPropertyChoicesTableData
	{
		public ExtendedPropertyChoicesTableData()
		{
			this.ValueChoices = new List<object>();
		}

		public ExtendedPropertyChoicesTableData(string propertyName, List<object> valueChoices)
		{
			this.PropertyName = propertyName;
			this.ValueChoices = valueChoices;
		}

		public string PropertyName;

		public List<object> ValueChoices;
	}

	/// <summary>
	/// ExternalPractitionerMergePropertiesComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergePropertiesComponentViewExtensionPoint))]
	public class ExternalPractitionerMergePropertiesComponent : ApplicationComponent
	{
		class PersonNameComparer : IEqualityComparer<PersonNameDetail>
		{
			public bool Equals(PersonNameDetail x, PersonNameDetail y)
			{
				return Equals(x.FamilyName, y.FamilyName)
					&& Equals(x.GivenName, y.GivenName)
					&& Equals(x.MiddleName, y.MiddleName)
					&& Equals(x.Prefix, y.Prefix)
					&& Equals(x.Suffix, y.Suffix)
					&& Equals(x.Degree, y.Degree);
			}

			public int GetHashCode(PersonNameDetail name)
			{
				var result = name.FamilyName != null ? name.FamilyName.GetHashCode() : 0;
				result = 29 * result + (name.GivenName != null ? name.GivenName.GetHashCode() : 0);
				result = 29 * result + (name.MiddleName != null ? name.MiddleName.GetHashCode() : 0);
				result = 29 * result + (name.Prefix != null ? name.Prefix.GetHashCode() : 0);
				result = 29 * result + (name.Suffix != null ? name.Suffix.GetHashCode() : 0);
				result = 29 * result + (name.Degree != null ? name.Degree.GetHashCode() : 0);
				return result;
			}
		}

		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerDetail _duplicatePractitioner;
		private ExternalPractitionerDetail _mergedPractitioner;

		private List<ExtendedPropertyChoicesTableData> _extendedPropertyChoices;
		private event EventHandler _saveRequested;

		public ExternalPractitionerMergePropertiesComponent()
		{
			_extendedPropertyChoices = new List<ExtendedPropertyChoicesTableData>();
		}

		public ExternalPractitionerDetail OriginalPractitioner
		{
			get { return _originalPractitioner; }
			set
			{
				if (_originalPractitioner != null && _originalPractitioner.Equals(value))
					return;

				_originalPractitioner = value;
				_mergedPractitioner = new ExternalPractitionerDetail();
				UpdateExtendedPropertyChoices();
				NotifyAllPropertiesChanged();
			}
		}

		public ExternalPractitionerDetail DuplicatePractitioner
		{
			get { return _duplicatePractitioner; }
			set
			{
				if (_duplicatePractitioner != null && _duplicatePractitioner.Equals(value))
					return;

				_duplicatePractitioner = value;
				_mergedPractitioner = new ExternalPractitionerDetail();
				UpdateExtendedPropertyChoices();
				NotifyAllPropertiesChanged();
			}
		}

		public void Save(ExternalPractitionerDetail practitioner)
		{
			// Ask all responsible party to update the merged property
			EventsHelper.Fire(_saveRequested, this, EventArgs.Empty);

			// Clone all properties
			practitioner.Name = (PersonNameDetail)_mergedPractitioner.Name.Clone();
			practitioner.LicenseNumber = _mergedPractitioner.LicenseNumber;
			practitioner.BillingNumber = _mergedPractitioner.BillingNumber;
			practitioner.ExtendedProperties.Clear();
			foreach (var kvp in _mergedPractitioner.ExtendedProperties)
			{
				// Only keep properties that have value
				if (!string.IsNullOrEmpty(kvp.Value))
					practitioner.ExtendedProperties.Add(kvp.Key, kvp.Value);
			}
		}

		#region Presentation Models

		public string Instruction
		{
			get
			{
				var hasConflicts = this.NameChoices.Count > 1
					|| this.LicenseNumberChoices.Count > 1
					|| this.BillingNumberChoices.Count > 1
					|| CollectionUtils.Contains(_extendedPropertyChoices, data => data.ValueChoices.Count > 1);

				return hasConflicts ? SR.MessageInstructionMergeProperties : SR.MessageInstructionNoPropertyConflict;
			}
		}

		public PersonNameDetail Name
		{
			get { return _mergedPractitioner.Name; }
			set { _mergedPractitioner.Name = value; }
		}

		public bool NameEnabled
		{
			get { return this.NameChoices.Count > 1; }
		}

		public List<PersonNameDetail> NameChoices
		{
			get
			{
				if (_originalPractitioner == null || _duplicatePractitioner == null)
					return new List<PersonNameDetail>();

				var choices = new List<PersonNameDetail> {this.OriginalPractitioner.Name, this.DuplicatePractitioner.Name};
				return CollectionUtils.Unique(choices, new PersonNameComparer());
			}
		}

		public string FormatName(object item)
		{
			var name = (PersonNameDetail) item;
			return PersonNameFormat.Format(name);
		}

		public string LicenseNumber
		{
			get { return _mergedPractitioner.LicenseNumber; }
			set { _mergedPractitioner.LicenseNumber = value; }
		}

		public bool LicenseNumberEnabled
		{
			get { return this.LicenseNumberChoices.Count > 1; }
		}

		public List<string> LicenseNumberChoices
		{
			get
			{
				if (_originalPractitioner == null || _duplicatePractitioner == null)
					return new List<string>();

				var choices = new List<string> { this.OriginalPractitioner.LicenseNumber, this.DuplicatePractitioner.LicenseNumber };
				return CollectionUtils.Unique(choices);
			}
		}

		public string BillingNumber
		{
			get { return _mergedPractitioner.BillingNumber; }
			set { _mergedPractitioner.BillingNumber = value; }
		}

		public bool BillingNumberEnabled
		{
			get { return this.BillingNumberChoices.Count > 1; }
		}

		public List<string> BillingNumberChoices
		{
			get
			{
				if (_originalPractitioner == null || _duplicatePractitioner == null)
					return new List<string>();

				var choices = new List<string> { this.OriginalPractitioner.BillingNumber, this.DuplicatePractitioner.BillingNumber };
				return CollectionUtils.Unique(choices);
			}
		}

		public Dictionary<string, string> ExtendedProperties
		{
			get { return _mergedPractitioner.ExtendedProperties; }
			set { _mergedPractitioner.ExtendedProperties = value;}
		}

		public List<ExtendedPropertyChoicesTableData> ExtendedPropertyChoices
		{
			get { return _extendedPropertyChoices; }
		}

		public event EventHandler SaveRequested
		{
			add { _saveRequested += value; }
			remove { _saveRequested -= value; }
		}

		#endregion

		private void UpdateExtendedPropertyChoices()
		{
			// Clear existing data.
			_extendedPropertyChoices = new List<ExtendedPropertyChoicesTableData>();

			if (_originalPractitioner == null || _duplicatePractitioner == null)
				return;

			// Find the unique keys of the combined extended properties
			var combinedKeys = CollectionUtils.Concat<string>(_originalPractitioner.ExtendedProperties.Keys, _duplicatePractitioner.ExtendedProperties.Keys);
			var uniqueKeys = CollectionUtils.Unique(combinedKeys);

			// Construct a ExtendedPropertyChoicesTableData for each unique property
			CollectionUtils.ForEach(uniqueKeys,
				delegate(string key)
				{
					var choices = new List<object>();

					if (_originalPractitioner.ExtendedProperties.ContainsKey(key))
						choices.Add(_originalPractitioner.ExtendedProperties[key]);

					if (_duplicatePractitioner.ExtendedProperties.ContainsKey(key))
						choices.Add(_duplicatePractitioner.ExtendedProperties[key]);

					var data = new ExtendedPropertyChoicesTableData(key, CollectionUtils.Unique(choices));
					_extendedPropertyChoices.Add(data);
				});
		}
	}
}
