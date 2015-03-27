namespace Tekla.Structures.ObjectPropertiesLibrary
{
    public partial class ShownPropertiesDialog
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ShownPropertiesDGW = new System.Windows.Forms.DataGridView();
            this.PropertyShown = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.SaveTSB = new System.Windows.Forms.ToolStripButton();
            this.LoadTSB = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ShownPropertiesDGW)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ShownPropertiesDGW
            // 
            this.ShownPropertiesDGW.AllowUserToAddRows = false;
            this.ShownPropertiesDGW.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ShownPropertiesDGW.BackgroundColor = System.Drawing.SystemColors.Window;
            this.ShownPropertiesDGW.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ShownPropertiesDGW.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ShownPropertiesDGW.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ShownPropertiesDGW.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ShownPropertiesDGW.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PropertyShown,
            this.Property});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ShownPropertiesDGW.DefaultCellStyle = dataGridViewCellStyle2;
            this.ShownPropertiesDGW.Location = new System.Drawing.Point(-1, 35);
            this.ShownPropertiesDGW.Name = "ShownPropertiesDGW";
            this.ShownPropertiesDGW.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ShownPropertiesDGW.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ShownPropertiesDGW.RowHeadersVisible = false;
            this.ShownPropertiesDGW.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ShownPropertiesDGW.Size = new System.Drawing.Size(240, 436);
            this.ShownPropertiesDGW.TabIndex = 0;
            // 
            // PropertyShown
            // 
            this.PropertyShown.DataPropertyName = "Visible";
            this.PropertyShown.FalseValue = "";
            this.PropertyShown.HeaderText = "albl_Visible";
            this.PropertyShown.Name = "PropertyShown";
            this.PropertyShown.ReadOnly = true;
            this.PropertyShown.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PropertyShown.TrueValue = "";
            this.PropertyShown.Width = 40;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveTSB,
            this.LoadTSB});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(239, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // SaveTSB
            // 
            this.SaveTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveTSB.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.save_as_big;
            this.SaveTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveTSB.Name = "SaveTSB";
            this.SaveTSB.Size = new System.Drawing.Size(23, 22);
            this.SaveTSB.ToolTipText = "albl_Save_view";
            this.SaveTSB.Click += new System.EventHandler(this.SaveTsbClick);
            // 
            // LoadTSB
            // 
            this.LoadTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.LoadTSB.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.open_big;
            this.LoadTSB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LoadTSB.Name = "LoadTSB";
            this.LoadTSB.Size = new System.Drawing.Size(23, 22);
            this.LoadTSB.ToolTipText = "albl_Load_view";
            this.LoadTSB.Click += new System.EventHandler(this.LoadTsbClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn1.HeaderText = "";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Property
            // 
            this.Property.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Property.DataPropertyName = "Name";
            this.Property.HeaderText = "albl_Name";
            this.Property.Name = "Property";
            this.Property.ReadOnly = true;
            this.Property.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ShownPropertiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.ShownPropertiesDGW);
            this.Name = "ShownPropertiesDialog";
            this.Size = new System.Drawing.Size(239, 472);
            ((System.ComponentModel.ISupportInitialize)(this.ShownPropertiesDGW)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ShownPropertiesDGW;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton SaveTSB;
        private System.Windows.Forms.ToolStripButton LoadTSB;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PropertyShown;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
    }
}