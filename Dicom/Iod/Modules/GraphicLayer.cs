#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GraphicLayer Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.7 (Table C.10-7)</remarks>
	public class GraphicLayerModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicLayerModuleIod"/> class.
		/// </summary>	
		public GraphicLayerModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicLayerModuleIod"/> class.
		/// </summary>
		public GraphicLayerModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets or sets the value of GraphicLayerSequence in the underlying collection. Type 1.
		/// </summary>
		public GraphicLayerSequenceItem[] GraphicLayerSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.GraphicLayerSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				GraphicLayerSequenceItem[] result = new GraphicLayerSequenceItem[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new GraphicLayerSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "GraphicLayerSequence is Type 1 Required.");

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.GraphicLayerSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.GraphicLayerSequence;
			}
		}
	}

	/// <summary>
	/// GraphicLayer Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.7 (Table C.10-7)</remarks>
	public class GraphicLayerSequenceItem : SequenceIodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicLayerSequenceItem"/> class.
		/// </summary>
		public GraphicLayerSequenceItem() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicLayerSequenceItem"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public GraphicLayerSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		/// <summary>
		/// Gets or sets the value of GraphicLayer in the underlying collection. Type 1.
		/// </summary>
		public string GraphicLayer
		{
			get { return base.DicomAttributeProvider[DicomTags.GraphicLayer].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "GraphicLayer is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.GraphicLayer].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GraphicLayerOrder in the underlying collection. Type 1.
		/// </summary>
		public int GraphicLayerOrder
		{
			get { return base.DicomAttributeProvider[DicomTags.GraphicLayerOrder].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.GraphicLayerOrder].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of GraphicLayerRecommendedDisplayGrayscaleValue in the underlying collection. Type 3.
		/// </summary>
		public int? GraphicLayerRecommendedDisplayGrayscaleValue
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayGrayscaleValue].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayGrayscaleValue] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayGrayscaleValue].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of GraphicLayerRecommendedDisplayCielabValue in the underlying collection. Type 3.
		/// </summary>
		public int[] GraphicLayerRecommendedDisplayCielabValue
		{
			get
			{
				int[] result = new int[3];
				if (base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].TryGetInt32(0, out result[0]))
					if (base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].TryGetInt32(1, out result[1]))
						if (base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].TryGetInt32(2, out result[2]))
							return result;
				return null;
			}
			set
			{
				if (value == null || value.Length != 3)
				{
					base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].SetInt32(0, value[0]);
				base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].SetInt32(1, value[1]);
				base.DicomAttributeProvider[DicomTags.GraphicLayerRecommendedDisplayCielabValue].SetInt32(2, value[2]);
			}
		}

		/// <summary>
		/// Gets or sets the value of GraphicLayerDescription in the underlying collection. Type 3.
		/// </summary>
		public string GraphicLayerDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.GraphicLayerDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.GraphicLayerDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.GraphicLayerDescription].SetString(0, value);
			}
		}
	}
}