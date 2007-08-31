using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PresentationLutTool : ImageViewerTool
	{
		private class PresentationLutActionContainer
		{
			private readonly PresentationLutTool _ownerTool;
			private readonly MenuAction _action;
			private readonly PresentationLutDescriptor _descriptor;

			public PresentationLutActionContainer(PresentationLutTool ownerTool, PresentationLutDescriptor descriptor, int index)
			{
				_ownerTool = ownerTool;
				_descriptor = descriptor;

				string actionId = String.Format("apply{0}", index);
				ActionPath actionPath = new ActionPath(String.Format("imageviewer-contextmenu/ColourMaps/colourMap{0}", index), _ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, _ownerTool._resolver);
				_action.GroupHint = new GroupHint("Tools.Image.Manipulation.Lut.ColourMaps");
				_action.Label = _descriptor.Description;
				_action.SetClickHandler(this.Apply);
			}
			
			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				PresentationLutOperationApplicator applicator = new PresentationLutOperationApplicator(_ownerTool.SelectedPresentationImage);
				UndoableCommand command = new UndoableCommand(applicator);
				command.BeginState = applicator.CreateMemento();

				ImageOperationApplicator.Apply del =
					delegate(IPresentationImage image)
						{
							if (image is IPresentationLutProvider)
								((IPresentationLutProvider)image).PresentationLutManager.InstallLut(_descriptor);
						};

				applicator.ApplyToAllImages(del);

				command.EndState = applicator.CreateMemento();
				if (!command.EndState.Equals(command.BeginState))
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private readonly ActionResourceResolver _resolver;

		public PresentationLutTool()
		{
			_resolver = new ActionResourceResolver(this);
		}

		public override IActionSet Actions
		{
			get
			{
				return new ActionSet(GetActions());
			}
		}

		private IEnumerable<IAction> GetActions()
		{
			if (this.SelectedPresentationImage is IPresentationLutProvider)
			{
				int i = 0;
				foreach (PresentationLutDescriptor descriptor in ((IPresentationLutProvider)this.SelectedPresentationImage).PresentationLutManager.AvailablePresentationLuts)
				{
					PresentationLutActionContainer container = new PresentationLutActionContainer(this, descriptor, ++i);
					yield return container.Action;
				}
			}
			else
			{
				yield break;
			}
		}
	}
}
