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
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Summary description for Example.
	/// </summary>
	public class StackTab : UserControl
	{        
        // Private field
        private Control _applicationComponentControl;
        private readonly EventHandler _titleClick;

		// Designer generated
        private TitleBar _titleBar;
        private Panel _panel;
        private TableLayoutPanel tableLayoutPanel1;

		private readonly StackTabPage _page;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StackTab()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		// New constructor
        public StackTab(StackTabPage page, ArrowButton arrow, EventHandler titleClick)
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        	_page = page;

            // Set initial values
			_titleBar.PreText = _page.PreText;
			_titleBar.Text = _page.Text;
			_titleBar.PostText = _page.PostText;
			if (_page.IconSet != null)
				_titleBar.Image = IconFactory.CreateIcon(_page.IconSet.SmallIcon, _page.ResourceResolver);

			_titleBar.ArrowButton = arrow;

			_page.TextChanged += _page_TextChanged;
			_page.IconChanged += _page_IconChanged;

			// Remember callback event handler
			_titleClick = titleClick;

			if ((arrow == ArrowButton.UpArrow) ||
				(arrow == ArrowButton.DownArrow) || 
				(arrow == ArrowButton.None))
				_titleBar.Dock = DockStyle.Top;
			
			if (arrow == ArrowButton.RightArrow)
				_titleBar.Dock = DockStyle.Right;

			if (arrow == ArrowButton.LeftArrow)
				_titleBar.Dock = DockStyle.Left;
		}

		private void _page_TextChanged(object sender, EventArgs e)
		{
			_titleBar.PreText = _page.PreText;
			_titleBar.Text = _page.Text;
			_titleBar.PostText = _page.PostText;
		}

		private void _page_IconChanged(object sender, EventArgs e)
		{
			if (_page.IconSet != null)
				_titleBar.Image = IconFactory.CreateIcon(_page.IconSet.SmallIcon, _page.ResourceResolver);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._panel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _titleBar
			// 
			this._titleBar.ArrowButton = Crownwood.DotNetMagic.Controls.ArrowButton.DownArrow;
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.ImageAlignment = Crownwood.DotNetMagic.Controls.ImageAlignment.Far;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.Margin = new System.Windows.Forms.Padding(0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(368, 24);
			this._titleBar.TabIndex = 0;
			this._titleBar.Text = "titleBar1";
			this._titleBar.ButtonClick += new System.EventHandler(this.OnArrowClick);
			// 
			// _panel
			// 
			this._panel.AutoScroll = true;
			this._panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panel.Location = new System.Drawing.Point(0, 24);
			this._panel.Margin = new System.Windows.Forms.Padding(0);
			this._panel.Name = "_panel";
			this._panel.Size = new System.Drawing.Size(368, 192);
			this._panel.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._panel, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(368, 216);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// StackTab
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StackTab";
			this.Size = new System.Drawing.Size(368, 216);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// Fire constructor provided event user clicks titlebar arrow
		private void OnArrowClick(object sender, EventArgs e)
		{
			if (_titleClick != null)
				_titleClick(sender, e);
		}
		
		// Allow direct access to the titlebar
		public TitleBar TitleBar
		{
			get { return _titleBar; }
		}

        public Control ApplicationComponentControl
        {
            get { return _applicationComponentControl; }
            set
            {
                _applicationComponentControl = value;
                this._panel.Controls.Clear();
                this._panel.Controls.Add(_applicationComponentControl);
            }
        }

		// Caller can discover minimum requested size
		public Size MinimumRequestedSize
		{
			get { return new Size(23, 23); }
		}
	}
}
