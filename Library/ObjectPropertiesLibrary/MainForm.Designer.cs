using System;
using Tekla.Structures.Dialog;

namespace ObjectBrowserProto
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				FormBorders.StoreFormSizeAndLocation(this);

				if (ThisConnection != null)
					ThisConnection.Dispose();
			}
			catch (Exception ee)
			{
				System.Diagnostics.Debug.WriteLine("MainForm dispose: "+ee.ToString());
			}

			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.ObjectsGrid = new System.Windows.Forms.DataGridView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.alblZoomtoselectedobjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alblRemovefromlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabelCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonAuto = new System.Windows.Forms.ToolStripButton();
			this.AutoUpdateTSButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
			this.toolStripLabelGap = new System.Windows.Forms.ToolStripLabel();
			this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
			this.SumDGW = new System.Windows.Forms.DataGridView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.ObjectsGrid)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SumDGW)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ObjectsGrid
			// 
			this.ObjectsGrid.AllowUserToAddRows = false;
			this.ObjectsGrid.AllowUserToDeleteRows = false;
			this.ObjectsGrid.AllowUserToOrderColumns = true;
			this.ObjectsGrid.AllowUserToResizeRows = false;
			this.structuresExtender.SetAttributeName(this.ObjectsGrid, null);
			this.structuresExtender.SetAttributeTypeName(this.ObjectsGrid, null);
			this.ObjectsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.structuresExtender.SetBindPropertyName(this.ObjectsGrid, null);
			this.ObjectsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ObjectsGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.ObjectsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ObjectsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ObjectsGrid.Location = new System.Drawing.Point(0, 0);
			this.ObjectsGrid.Name = "ObjectsGrid";
			this.ObjectsGrid.ReadOnly = true;
			this.ObjectsGrid.RowHeadersVisible = false;
			this.ObjectsGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ObjectsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.ObjectsGrid.Size = new System.Drawing.Size(553, 211);
			this.ObjectsGrid.TabIndex = 0;
			// 
			// contextMenuStrip1
			// 
			this.structuresExtender.SetAttributeName(this.contextMenuStrip1, null);
			this.structuresExtender.SetAttributeTypeName(this.contextMenuStrip1, null);
			this.structuresExtender.SetBindPropertyName(this.contextMenuStrip1, null);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alblZoomtoselectedobjectsToolStripMenuItem,
            this.alblRemovefromlistToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(238, 48);
			// 
			// alblZoomtoselectedobjectsToolStripMenuItem
			// 
			this.alblZoomtoselectedobjectsToolStripMenuItem.Name = "alblZoomtoselectedobjectsToolStripMenuItem";
			this.alblZoomtoselectedobjectsToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.alblZoomtoselectedobjectsToolStripMenuItem.Text = "albl_Zoom_to_selected_objects";
			this.alblZoomtoselectedobjectsToolStripMenuItem.Click += new System.EventHandler(this.alblZoomtoselectedobjectsToolStripMenuItem_Click);
			// 
			// alblRemovefromlistToolStripMenuItem
			// 
			this.alblRemovefromlistToolStripMenuItem.Name = "alblRemovefromlistToolStripMenuItem";
			this.alblRemovefromlistToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.alblRemovefromlistToolStripMenuItem.Text = "albl_Remove_from_list";
			this.alblRemovefromlistToolStripMenuItem.Click += new System.EventHandler(this.alblRemovefromlistToolStripMenuItem_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "autoselect_objects_active_big.ico");
			this.imageList1.Images.SetKeyName(1, "autoselect_objects_passive_big.ico");
			this.imageList1.Images.SetKeyName(2, "Cancel.png");
			this.imageList1.Images.SetKeyName(3, "delete_big.ico");
			this.imageList1.Images.SetKeyName(4, "keep_selection_16.ico");
			this.imageList1.Images.SetKeyName(5, "move_down_24.ico");
			this.imageList1.Images.SetKeyName(6, "TeklaStructuresExtension.ico");
			// 
			// statusStrip1
			// 
			this.structuresExtender.SetAttributeName(this.statusStrip1, null);
			this.structuresExtender.SetAttributeTypeName(this.statusStrip1, null);
			this.structuresExtender.SetBindPropertyName(this.statusStrip1, null);
			this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatus,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1,
            this.toolStripStatusLabelCount});
			this.statusStrip1.Location = new System.Drawing.Point(0, 355);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(556, 22);
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelStatus
			// 
			this.toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
			this.toolStripStatusLabelStatus.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(439, 17);
			this.toolStripStatusLabel2.Spring = true;
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
			// 
			// toolStripStatusLabelCount
			// 
			this.toolStripStatusLabelCount.Name = "toolStripStatusLabelCount";
			this.toolStripStatusLabelCount.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStrip1
			// 
			this.structuresExtender.SetAttributeName(this.toolStrip1, null);
			this.structuresExtender.SetAttributeTypeName(this.toolStrip1, null);
			this.structuresExtender.SetBindPropertyName(this.toolStrip1, null);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAuto,
            this.AutoUpdateTSButton,
            this.toolStripButtonProperties,
            this.toolStripLabelGap,
            this.toolStripButtonGet});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(556, 25);
			this.toolStrip1.TabIndex = 9;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonAuto
			// 
			this.toolStripButtonAuto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAuto.Image = global::ObjectBrowserProto.Properties.Resources.autoselect_objects_active_big;
			this.toolStripButtonAuto.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAuto.Name = "toolStripButtonAuto";
			this.toolStripButtonAuto.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonAuto.Text = "toolStripButtonAuto";
			this.toolStripButtonAuto.ToolTipText = "albl_Auto_select_objects";
			this.toolStripButtonAuto.Visible = false;
			// 
			// AutoUpdateTSButton
			// 
			this.AutoUpdateTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.AutoUpdateTSButton.ForeColor = System.Drawing.Color.Coral;
			this.AutoUpdateTSButton.Image = global::ObjectBrowserProto.Properties.Resources.autoselect_objects_active_big;
			this.AutoUpdateTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.AutoUpdateTSButton.Name = "AutoUpdateTSButton";
			this.AutoUpdateTSButton.Size = new System.Drawing.Size(23, 22);
			this.AutoUpdateTSButton.ToolTipText = "albl_Update_list_automatically";
			this.AutoUpdateTSButton.Click += new System.EventHandler(this.AutoUpdateTSButton_Click);
			// 
			// toolStripButtonProperties
			// 
			this.toolStripButtonProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonProperties.Image = global::ObjectBrowserProto.Properties.Resources.keep_selection_16;
			this.toolStripButtonProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonProperties.Name = "toolStripButtonProperties";
			this.toolStripButtonProperties.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonProperties.Text = "toolStripButton1";
			this.toolStripButtonProperties.ToolTipText = "albl_Select_displayed_properties";
			// 
			// toolStripLabelGap
			// 
			this.toolStripLabelGap.Name = "toolStripLabelGap";
			this.toolStripLabelGap.Size = new System.Drawing.Size(0, 22);
			// 
			// toolStripButtonGet
			// 
			this.toolStripButtonGet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonGet.Image = global::ObjectBrowserProto.Properties.Resources.move_down_24;
			this.toolStripButtonGet.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonGet.Name = "toolStripButtonGet";
			this.toolStripButtonGet.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonGet.Text = "toolStripButton1";
			this.toolStripButtonGet.ToolTipText = "albl_Get_selected_objects_from_model";
			// 
			// SumDGW
			// 
			this.SumDGW.AllowUserToAddRows = false;
			this.SumDGW.AllowUserToDeleteRows = false;
			this.SumDGW.AllowUserToResizeColumns = false;
			this.SumDGW.AllowUserToResizeRows = false;
			this.structuresExtender.SetAttributeName(this.SumDGW, null);
			this.structuresExtender.SetAttributeTypeName(this.SumDGW, null);
			this.SumDGW.BackgroundColor = System.Drawing.SystemColors.Window;
			this.structuresExtender.SetBindPropertyName(this.SumDGW, null);
			this.SumDGW.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.SumDGW.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.SumDGW.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.SumDGW.ColumnHeadersVisible = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.SumDGW.DefaultCellStyle = dataGridViewCellStyle1;
			this.SumDGW.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SumDGW.Location = new System.Drawing.Point(0, 0);
			this.SumDGW.MultiSelect = false;
			this.SumDGW.Name = "SumDGW";
			this.SumDGW.ReadOnly = true;
			this.SumDGW.RowHeadersVisible = false;
			this.SumDGW.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.SumDGW.Size = new System.Drawing.Size(553, 111);
			this.SumDGW.TabIndex = 10;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.structuresExtender.SetAttributeName(this.splitContainer1, null);
			this.structuresExtender.SetAttributeTypeName(this.splitContainer1, null);
			this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
			this.structuresExtender.SetBindPropertyName(this.splitContainer1, null);
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(2, 29);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.structuresExtender.SetAttributeName(this.splitContainer1.Panel1, null);
			this.structuresExtender.SetAttributeTypeName(this.splitContainer1.Panel1, null);
			this.structuresExtender.SetBindPropertyName(this.splitContainer1.Panel1, null);
			this.splitContainer1.Panel1.Controls.Add(this.ObjectsGrid);
			// 
			// splitContainer1.Panel2
			// 
			this.structuresExtender.SetAttributeName(this.splitContainer1.Panel2, null);
			this.structuresExtender.SetAttributeTypeName(this.splitContainer1.Panel2, null);
			this.structuresExtender.SetBindPropertyName(this.splitContainer1.Panel2, null);
			this.splitContainer1.Panel2.Controls.Add(this.SumDGW);
			this.splitContainer1.Size = new System.Drawing.Size(553, 325);
			this.splitContainer1.SplitterDistance = 211;
			this.splitContainer1.SplitterWidth = 3;
			this.splitContainer1.TabIndex = 11;
			// 
			// MainForm
			// 
			this.structuresExtender.SetAttributeName(this, null);
			this.structuresExtender.SetAttributeTypeName(this, null);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.structuresExtender.SetBindPropertyName(this, null);
			this.ClientSize = new System.Drawing.Size(556, 377);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(320, 240);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "albl_Object_Browser";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.ObjectsGrid)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SumDGW)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView ObjectsGrid;
		private QuickSearchForDataTable QuickSearchUserCntrl;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem alblZoomtoselectedobjectsToolStripMenuItem;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStatus;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCount;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAuto;
		private System.Windows.Forms.ToolStripButton toolStripButtonProperties;
		private System.Windows.Forms.ToolStripButton toolStripButtonGet;
		private System.Windows.Forms.ToolStripLabel toolStripLabelGap;
		private System.Windows.Forms.DataGridView SumDGW;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripButton AutoUpdateTSButton;
		private System.Windows.Forms.ToolStripMenuItem alblRemovefromlistToolStripMenuItem;
	}
}

