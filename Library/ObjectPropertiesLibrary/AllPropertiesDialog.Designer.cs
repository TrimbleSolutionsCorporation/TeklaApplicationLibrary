namespace Tekla.Structures.ObjectPropertiesLibrary
{
    public partial class AllPropertiesDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.AllPropertiesDGW = new System.Windows.Forms.DataGridView();
            this.PropertyShown = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReportProperty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UdaProperty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Decimals = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.alblIncludeselectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alblExcludeselectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.alblRemoveselectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllPropertiesToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.PasteNormalToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PasteExteralToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.LoadProperties = new System.Windows.Forms.ToolStripButton();
            this.SaveProperties = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.AllPropertiesDGW)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.AllPropertiesToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // AllPropertiesDGW
            // 
            this.AllPropertiesDGW.AllowUserToAddRows = false;
            this.AllPropertiesDGW.AllowUserToDeleteRows = false;
            this.AllPropertiesDGW.AllowUserToResizeRows = false;
            this.AllPropertiesDGW.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AllPropertiesDGW.BackgroundColor = System.Drawing.SystemColors.Window;
            this.AllPropertiesDGW.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AllPropertiesDGW.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.AllPropertiesDGW.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AllPropertiesDGW.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PropertyShown,
            this.Property,
            this.ReportProperty,
            this.UdaProperty,
            this.Type,
            this.DisplayType,
            this.Decimals,
            this.ColWidth});
            this.AllPropertiesDGW.Location = new System.Drawing.Point(2, 36);
            this.AllPropertiesDGW.Name = "AllPropertiesDGW";
            this.AllPropertiesDGW.ReadOnly = true;
            this.AllPropertiesDGW.RowHeadersVisible = false;
            this.AllPropertiesDGW.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.AllPropertiesDGW.Size = new System.Drawing.Size(665, 431);
            this.AllPropertiesDGW.TabIndex = 0;
            this.AllPropertiesDGW.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AllPropertiesDGWCellClick);
            this.AllPropertiesDGW.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AllPropertiesDGWCellDoubleClick);
            this.AllPropertiesDGW.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.AllPropertiesDGWCellValidating);
            this.AllPropertiesDGW.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.AllPropertiesDGWRowValidating);
            this.AllPropertiesDGW.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GridClicked);
            // 
            // PropertyShown
            // 
            this.PropertyShown.DataPropertyName = "Visible";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.NullValue = false;
            this.PropertyShown.DefaultCellStyle = dataGridViewCellStyle1;
            this.PropertyShown.FalseValue = "false";
            this.PropertyShown.HeaderText = "albl_Included";
            this.PropertyShown.Name = "PropertyShown";
            this.PropertyShown.ReadOnly = true;
            this.PropertyShown.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PropertyShown.TrueValue = "true";
            this.PropertyShown.Width = 66;
            // 
            // Property
            // 
            this.Property.DataPropertyName = "Name";
            this.Property.HeaderText = "albl_Name";
            this.Property.Name = "Property";
            this.Property.ReadOnly = true;
            this.Property.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Property.Width = 156;
            // 
            // ReportProperty
            // 
            this.ReportProperty.DataPropertyName = "ReportPropertyName";
            this.ReportProperty.HeaderText = "albl_Report_property";
            this.ReportProperty.Name = "ReportProperty";
            this.ReportProperty.ReadOnly = true;
            // 
            // UdaProperty
            // 
            this.UdaProperty.DataPropertyName = "UdaPropertyName";
            this.UdaProperty.HeaderText = "albl_UDA_name";
            this.UdaProperty.Name = "UdaProperty";
            this.UdaProperty.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "PropertyType";
            this.Type.HeaderText = "albl_Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // DisplayType
            // 
            this.DisplayType.HeaderText = "albl_Displayed_type";
            this.DisplayType.Name = "DisplayType";
            this.DisplayType.ReadOnly = true;
            this.DisplayType.Visible = false;
            // 
            // Decimals
            // 
            this.Decimals.DataPropertyName = "Decimals";
            this.Decimals.HeaderText = "albl_Decimals";
            this.Decimals.Name = "Decimals";
            this.Decimals.ReadOnly = true;
            this.Decimals.Width = 70;
            // 
            // ColWidth
            // 
            this.ColWidth.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColWidth.DataPropertyName = "Width";
            this.ColWidth.HeaderText = "albl_Width";
            this.ColWidth.Name = "ColWidth";
            this.ColWidth.ReadOnly = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alblIncludeselectedToolStripMenuItem,
            this.alblExcludeselectedToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.alblRemoveselectedToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(231, 126);
            // 
            // alblIncludeselectedToolStripMenuItem
            // 
            this.alblIncludeselectedToolStripMenuItem.Name = "alblIncludeselectedToolStripMenuItem";
            this.alblIncludeselectedToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.alblIncludeselectedToolStripMenuItem.Text = "albl_Include_selected";
            this.alblIncludeselectedToolStripMenuItem.Click += new System.EventHandler(this.AlblIncludeselectedToolStripMenuItemClick);
            // 
            // alblExcludeselectedToolStripMenuItem
            // 
            this.alblExcludeselectedToolStripMenuItem.Name = "alblExcludeselectedToolStripMenuItem";
            this.alblExcludeselectedToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.alblExcludeselectedToolStripMenuItem.Text = "albl_Exclude_selected";
            this.alblExcludeselectedToolStripMenuItem.Click += new System.EventHandler(this.AlblExcludeselectedToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem1.Text = "albl_Paste_external_properties";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem2.Text = "albl_Paste_model_properties";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // alblRemoveselectedToolStripMenuItem
            // 
            this.alblRemoveselectedToolStripMenuItem.Name = "alblRemoveselectedToolStripMenuItem";
            this.alblRemoveselectedToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.alblRemoveselectedToolStripMenuItem.Text = "albl_Remove_selected";
            this.alblRemoveselectedToolStripMenuItem.Click += new System.EventHandler(this.AlblRemoveselectedToolStripMenuItemClick);
            // 
            // AllPropertiesToolStrip
            // 
            this.AllPropertiesToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.PasteNormalToolStripButton,
            this.PasteExteralToolStripButton,
            this.LoadProperties,
            this.SaveProperties,
            this.toolStripButton2,
            this.toolStripButton3});
            this.AllPropertiesToolStrip.Location = new System.Drawing.Point(0, 0);
            this.AllPropertiesToolStrip.Name = "AllPropertiesToolStrip";
            this.AllPropertiesToolStrip.Size = new System.Drawing.Size(670, 25);
            this.AllPropertiesToolStrip.TabIndex = 1;
            this.AllPropertiesToolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.CheckOnClick = true;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.EditTableHS1;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.ToolTipText = "albl_Edit";
            this.toolStripButton1.Visible = false;
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1Click);
            // 
            // PasteNormalToolStripButton
            // 
            this.PasteNormalToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PasteNormalToolStripButton.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.PasteHS;
            this.PasteNormalToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PasteNormalToolStripButton.Name = "PasteNormalToolStripButton";
            this.PasteNormalToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.PasteNormalToolStripButton.ToolTipText = "albl_Paste_model_properties";
            this.PasteNormalToolStripButton.Visible = false;
            // 
            // PasteExteralToolStripButton
            // 
            this.PasteExteralToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PasteExteralToolStripButton.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.PasteHS;
            this.PasteExteralToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PasteExteralToolStripButton.Name = "PasteExteralToolStripButton";
            this.PasteExteralToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.PasteExteralToolStripButton.ToolTipText = "albl_Paste_external_properties";
            this.PasteExteralToolStripButton.Visible = false;
            // 
            // LoadProperties
            // 
            this.LoadProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.LoadProperties.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.open_big;
            this.LoadProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LoadProperties.Name = "LoadProperties";
            this.LoadProperties.Size = new System.Drawing.Size(23, 22);
            this.LoadProperties.ToolTipText = "albl_Load";
            this.LoadProperties.Click += new System.EventHandler(this.LoadPropertiesClick);
            // 
            // SaveProperties
            // 
            this.SaveProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveProperties.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.save_as_big;
            this.SaveProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveProperties.Name = "SaveProperties";
            this.SaveProperties.Size = new System.Drawing.Size(23, 22);
            this.SaveProperties.ToolTipText = "albl_Save";
            this.SaveProperties.Click += new System.EventHandler(this.SavePropertiesClick);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.AddTable;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "albl_Add";
            this.toolStripButton2.Click += new System.EventHandler(this.ToolStripButton2Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.delete_big;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "albl_Remove";
            this.toolStripButton3.Click += new System.EventHandler(this.ToolStripButton3Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn1.HeaderText = "";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "ReportPropertyName";
            this.dataGridViewTextBoxColumn2.HeaderText = "albl_Report_property";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "UdaPropertyName";
            this.dataGridViewTextBoxColumn3.HeaderText = "albl_UDA_name";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "PropertyType";
            this.dataGridViewTextBoxColumn4.HeaderText = "albl_Type";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "DisplayType";
            this.dataGridViewTextBoxColumn5.HeaderText = "albl_Displayed_type";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Visible = false;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "Decimals";
            this.dataGridViewTextBoxColumn6.HeaderText = "albl_Decimals";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 70;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn7.DataPropertyName = "Width";
            this.dataGridViewTextBoxColumn7.HeaderText = "albl_Width";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // AllPropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AllPropertiesToolStrip);
            this.Controls.Add(this.AllPropertiesDGW);
            this.Name = "AllPropertiesDialog";
            this.Size = new System.Drawing.Size(670, 470);
            ((System.ComponentModel.ISupportInitialize)(this.AllPropertiesDGW)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.AllPropertiesToolStrip.ResumeLayout(false);
            this.AllPropertiesToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView AllPropertiesDGW;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStrip AllPropertiesToolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.ToolStripButton PasteNormalToolStripButton;
        private System.Windows.Forms.ToolStripButton PasteExteralToolStripButton;
        private System.Windows.Forms.ToolStripButton LoadProperties;
        private System.Windows.Forms.ToolStripButton SaveProperties;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem alblIncludeselectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alblExcludeselectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem alblRemoveselectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PropertyShown;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportProperty;
        private System.Windows.Forms.DataGridViewTextBoxColumn UdaProperty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Decimals;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColWidth;
    }
}