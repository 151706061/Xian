using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class FloatingPointDicomTagColumn : MultiValuedDicomTagColumn<double>, INumericSortableColumn
	{
		public FloatingPointDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<double> GetTypedValue(DicomAttribute attribute)
		{
			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<double>();

			double?[] result;
			try
			{
				result = new double?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					double value;
					if (attribute.TryGetFloat64(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<double>(result);
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareNumerically(x, y);
		}

		public int CompareNumerically(StudyItem x, StudyItem y)
		{
			return DicomArray<double>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}
	}
}