﻿namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms {
	partial class ThumbnailsConfigurationComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThumbnailsConfigurationComponentControl));
			this._chkAutoOpenThumbnails = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkAutoOpenThumbnails
			// 
			resources.ApplyResources(this._chkAutoOpenThumbnails, "_chkAutoOpenThumbnails");
			this._chkAutoOpenThumbnails.Name = "_chkAutoOpenThumbnails";
			this._chkAutoOpenThumbnails.UseVisualStyleBackColor = true;
			// 
			// ThumbnailsConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._chkAutoOpenThumbnails);
			this.Name = "ThumbnailsConfigurationComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkAutoOpenThumbnails;
	}
}
