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
using System.Text;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop.Configuration
{
	public sealed class ConfigurationDialog
	{
		private class NavigatorPagePathComparer : IComparer<NavigatorPage>
		{
			#region IComparer<NavigatorPage> Members

			/// <summary>
			/// Compares two <see cref="NavigatorPage"/>s.
			/// </summary>
			public int Compare(NavigatorPage x, NavigatorPage y)
			{
				if (x == null)
				{
					if (y == null)
						return 0;
					else
						return -1;
				}

				if (y == null)
					return 1;

				return x.Path.LocalizedPath.CompareTo(y.Path.LocalizedPath);
			}

			#endregion
		}

		private ConfigurationDialog()
		{
		}

		private class ConfigurationNavigatorComponentContainer : NavigatorComponentContainer
		{
			private int _initialPageIndex;
			private ConfigurationPageManager _configurationPageManager;

			public ConfigurationNavigatorComponentContainer(string initialPagePath)
				: base()
			{
				// We want to validate all configuration pages
				this.ValidationStrategy = new StartedComponentsValidationStrategy();

				_configurationPageManager = new ConfigurationPageManager();

				List<NavigatorPage> pages = new List<NavigatorPage>();

				foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
					pages.Add(new NavigatorPage(configurationPage.GetPath(), configurationPage.GetComponent()));

				pages.Sort(new NavigatorPagePathComparer());

				_initialPageIndex = 0;
				int i = 0;

				foreach (NavigatorPage page in pages)
				{
					//do the unresolved paths match?
					if (page.Path.ToString() == initialPagePath)
						_initialPageIndex = i;

					this.Pages.Add(page);
					++i;
				}

				if (Pages.Count == 0)
					throw new Exception(SR.MessageNoConfigurationPagesExist);
			}

			public IEnumerable<IConfigurationPage> ConfigurationPages
			{
				get { return _configurationPageManager.Pages; }
			}

			public override void Start()
			{
				base.Start();

				MoveTo(_initialPageIndex);
			}

			public override void Accept()
			{
				if (this.HasValidationErrors)
				{
					ShowValidation(true);
				}
				else
				{
					try
					{
						foreach (IConfigurationPage configurationPage in this.ConfigurationPages)
						{
							if (configurationPage.GetComponent().Modified)
								configurationPage.SaveConfiguration();
						}

                        this.Exit(ApplicationComponentExitCode.Accepted);
					}
					catch (Exception e)
					{
						ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
							delegate()
							{
                                this.Exit(ApplicationComponentExitCode.Error);
							});
					}
				}
			}
		}

		/// <summary>
		/// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="IConfigurationPageProvider"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow)
		{
			return ConfigurationDialog.Show(desktopWindow, null);
		}

		/// <summary>
		/// Shows all <see cref="IConfigurationPage"/>s returned by extensions of <see cref="IConfigurationPageProvider"/>
		/// in a dialog, with a navigable tree to select the pages.
		/// </summary>
		public static ApplicationComponentExitCode Show(IDesktopWindow desktopWindow, string initialPageIdentifier)
		{
			ConfigurationNavigatorComponentContainer container = new ConfigurationNavigatorComponentContainer(initialPageIdentifier);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(desktopWindow, container, SR.TitleMenuOptions);

			return exitCode;
		}
	}
}
