﻿using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class StoreStudyRootQuery : IStudyRootQuery, IDisposable
    {
        private IStudyStoreQuery _real;

        public StoreStudyRootQuery()
        {
        }

        public StoreStudyRootQuery(IStudyStoreQuery real)
        {
            _real = real;
        }

        private IStudyStoreQuery Real
        {
            get { return _real ?? (_real = Platform.GetService<IStudyStoreQuery>()); }
        }

        #region IStudyRootQuery Members

        public System.Collections.Generic.IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            var criteria = new StudyEntry {Study = queryCriteria};
            var result = Real.GetStudyEntries(new GetStudyEntriesRequest {Criteria = criteria});
            return result.StudyEntries.Select(e => e.Study).ToList();
        }

        public System.Collections.Generic.IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            var criteria = new SeriesEntry {Series = queryCriteria};
            var result = Real.GetSeriesEntries(new GetSeriesEntriesRequest { Criteria = criteria });
            return result.SeriesEntries.Select(e => e.Series).ToList();
        }

        public System.Collections.Generic.IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            var criteria = new ImageEntry { Image = queryCriteria };
            var result = Real.GetImageEntries(new GetImageEntriesRequest { Criteria = criteria });
            return result.ImageEntries.Select(e => e.Image).ToList();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_real == null)return;

            var disposable = _real as IDisposable;
            if (disposable == null)
                return;

            disposable.Dispose();
            _real = null;
        }

        #endregion
    }
}
