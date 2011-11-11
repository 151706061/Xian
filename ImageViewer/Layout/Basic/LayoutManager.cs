#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    internal class KeyImageLayoutHook : HpLayoutHook
    {
        #region IHpLayoutHook Members

        public override bool HandleLayout(IHpLayoutHookContext context)
        {
            var primaryImageSet = context.ImageViewer.LogicalWorkspace.ImageSets[0];
            var firstKeyImageDisplaySet = primaryImageSet.DisplaySets.FirstOrDefault(IsKeyImageDisplaySet);
            if (firstKeyImageDisplaySet == null)
                return false;

            context.ImageViewer.PhysicalWorkspace.SetImageBoxGrid(1,1);
            var imageBox = context.ImageViewer.PhysicalWorkspace.ImageBoxes[0];
            imageBox.SetTileGrid(1,1);
            imageBox.DisplaySet = firstKeyImageDisplaySet.CreateFreshCopy();

            return true;
        }

        #endregion

        private bool IsKeyImageDisplaySet(IDisplaySet displaySet)
        {
            if (displaySet == null || displaySet.PresentationImages.Count == 0)
                return false;

            var descriptor = displaySet.Descriptor as IDicomDisplaySetDescriptor;
            if (descriptor == null)
                return false;

            const string koModality = "KO";

            if (descriptor.SourceSeries != null && descriptor.SourceSeries.Modality == koModality)
                return true;

            var allImagesDescriptor = descriptor as ModalityDisplaySetDescriptor;
            return allImagesDescriptor != null && allImagesDescriptor.Modality == koModality;
        }
    }

    internal class KeyImageDisplaySetCreationOptions : IModalityDisplaySetCreationOptions
    {
        private readonly IModalityDisplaySetCreationOptions _real;

        public KeyImageDisplaySetCreationOptions(IModalityDisplaySetCreationOptions real)
        {
            _real = real;
        }

        public bool ShowGrayscaleInverted
        {
            get { return _real.ShowGrayscaleInverted; }
        }

        public bool ShowOriginalMixedMultiframeSeries
        {
            get { return _real.ShowOriginalMixedMultiframeSeries; }
        }

        public bool SplitMixedMultiframes
        {
            get { return _real.SplitMixedMultiframes; }
        }

        public bool ShowOriginalMultiEchoSeries
        {
            get { return _real.ShowOriginalMultiEchoSeries; }
        }

        public bool SplitMultiEchoSeries
        {
            get { return _real.SplitMultiEchoSeries; }
        }

        public bool ShowOriginalSeries
        {
            get { return false; }
        }

        public bool CreateSingleImageDisplaySets
        {
            get { return false; }
        }

        public bool CreateAllImagesDisplaySet
        {
            get { return true; }
        }

        public string Modality
        {
            get { return _real.Modality; }
        }
    }

	[ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public partial class LayoutManager : ImageViewer.LayoutManager
	{
		#region LayoutHookContext

		class LayoutHookContext : IHpLayoutHookContext
		{
		    private readonly LayoutManager _layoutManager;

		    public LayoutHookContext(IImageViewer viewer, LayoutManager layoutManager)
			{
				this.ImageViewer = viewer;
		        this._layoutManager = layoutManager;
			}

			public IImageViewer ImageViewer { get; private set; }

		    public void PerformDefaultPhysicalWorkspaceLayout()
		    {
		        if (_layoutManager!=null)
		        {
		            _layoutManager.LayoutPhysicalWorkspace();
		        }
		    }
		}

		#endregion

	    private readonly IPatientReconciliationStrategy _reconciliationStrategy = new DefaultPatientReconciliationStrategy();
	    private ImageSetFiller _imageSetFiller;
	    private IHpLayoutHook _layoutHook;
	    private IDisplaySetCreationOptions _displaySetCreationOptions;

	    public LayoutManager()
		{
			AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer;

            //const string ko = "KO";
            //var realOptions = DisplaySetCreationOptions[ko];
            //this.DisplaySetCreationOptions[ko] = new KeyImageDisplaySetCreationOptions(realOptions);

	        //LayoutHook = new KeyImageLayoutHook();
		}

		public override void SetImageViewer(IImageViewer imageViewer)
		{
			base.SetImageViewer(imageViewer);

			StudyTree studyTree = null;
			if (imageViewer != null)
				studyTree = imageViewer.StudyTree;

			_reconciliationStrategy.SetStudyTree(studyTree);

            _imageSetFiller = new ImageSetFiller(studyTree, DisplaySetCreationOptions);
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || !(_layoutHook is IDisposable))
                return;
            
            ((IDisposable)_layoutHook).Dispose();
            _layoutHook = null;
        }

	    public IHpLayoutHook LayoutHook
	    {
	        get
	        {
	            if (_layoutHook == null)
	            {
	                try
	                {
	                    _layoutHook = (IHpLayoutHook)new HpLayoutHookExtensionPoint().CreateExtension();
	                }
	                catch (NotSupportedException)
	                {
	                    _layoutHook = HpLayoutHook.Default;
	                }
	            }

                return _layoutHook;
	        }
            set
            {
                _layoutHook = value;
            }
	    }

	    public IDisplaySetCreationOptions DisplaySetCreationOptions
	    {
	        get
	        {
                if (_displaySetCreationOptions == null)
                    _displaySetCreationOptions = new DisplaySetCreationOptions();

                return _displaySetCreationOptions;
	        }    
	    }

		#region Logical Workspace building 

		protected override IComparer<Series> GetSeriesComparer()
		{
			return new CompositeComparer<Series>(new DXSeriesPresentationIntentComparer(), base.GetSeriesComparer());
		}

        protected override IPatientData ReconcilePatient(IStudyRootData studyData)
		{
            var reconciled = _reconciliationStrategy.ReconcilePatientInformation(studyData);
			if (reconciled != null)
				return new StudyRootStudyIdentifier(reconciled, new StudyIdentifier());

            return base.ReconcilePatient(studyData);
		}

		protected override void FillImageSet(IImageSet imageSet, Study study)
		{
            _imageSetFiller.AddMultiSeriesDisplaySets(imageSet, study);

            base.FillImageSet(imageSet, study);
		}

	    protected override void UpdateImageSet(IImageSet imageSet, Series series)
		{
            _imageSetFiller.AddSeriesDisplaySets(imageSet, series);
		}

		#endregion

		protected override void LayoutAndFillPhysicalWorkspace()
		{
            if (LayoutHook.HandleLayout(new LayoutHookContext(ImageViewer, this)))
                    return;

		    // hooks did not handle it, so call base class
			base.LayoutAndFillPhysicalWorkspace();
		}

		protected override void LayoutPhysicalWorkspace()
		{
			StoredLayout layout = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities in the study).
			if (LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = LogicalWorkspace.ImageSets[0];
				foreach (IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (layout == null)
						layout = LayoutSettings.GetMinimumLayout();

					StoredLayout storedLayout = LayoutSettings.Default.GetLayout(displaySet.PresentationImages[0] as IImageSopProvider);
					layout.ImageBoxRows = Math.Max(layout.ImageBoxRows, storedLayout.ImageBoxRows);
					layout.ImageBoxColumns = Math.Max(layout.ImageBoxColumns, storedLayout.ImageBoxColumns);
					layout.TileRows = Math.Max(layout.TileRows, storedLayout.TileRows);
					layout.TileColumns = Math.Max(layout.TileColumns, storedLayout.TileColumns);
				}
			}

			if (layout == null)
				layout = LayoutSettings.Default.DefaultLayout;

			PhysicalWorkspace.SetImageBoxGrid(layout.ImageBoxRows, layout.ImageBoxColumns);
			for (int i = 0; i < PhysicalWorkspace.ImageBoxes.Count; ++i)
				PhysicalWorkspace.ImageBoxes[i].SetTileGrid(layout.TileRows, layout.TileColumns);
		}

		#region Comparers

		private class DXSeriesPresentationIntentComparer : DicomSeriesComparer
		{
			public override int Compare(Sop x, Sop y)
			{
				// this sorts FOR PRESENTATION series to the beginning.
				// FOR PROCESSING and unspecified series are considered equal for the purposes of sorting by intent.
				const string forPresentation = "FOR PRESENTATION";
				int presentationIntentX = GetPresentationIntent(x) == forPresentation ? 0 : 1;
				int presentationIntentY = GetPresentationIntent(y) == forPresentation ? 0 : 1;
				int result = presentationIntentX - presentationIntentY;
				if (Reverse)
					return -result;
				return result;
			}

			private static string GetPresentationIntent(Sop sop)
			{
				DicomAttribute attribute;
				if (sop.DataSource.TryGetAttribute(DicomTags.PresentationIntentType, out attribute))
					return (attribute.ToString() ?? string.Empty).ToUpperInvariant();
				return string.Empty;
			}
		}

	    /// TODO (CR Sep 2011): Move to ImageViewer.Comparers.
		private class CompositeComparer<T> : IComparer<T>
		{
			private readonly IList<IComparer<T>> _comparers;

			public CompositeComparer(params IComparer<T>[] comparers)
			{
				Platform.CheckForNullReference(comparers, "comparers");
				_comparers = new List<IComparer<T>>(comparers);
			}

			public int Compare(T x, T y)
			{
				foreach (IComparer<T> comparer in _comparers)
				{
					int result = comparer.Compare(x, y);
					if (result != 0)
						return result;
				}
				return 0;
			}
		}

		#endregion
	}
}
