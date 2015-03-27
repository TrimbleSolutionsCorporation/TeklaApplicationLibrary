namespace Tekla.Structures.ObjectPropertiesLibrary
{
    partial class PropertiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.shownPropertiesDialog1 = new Tekla.Structures.ObjectPropertiesLibrary.ShownPropertiesDialog();
            this.allPropertiesDialog1 = new Tekla.Structures.ObjectPropertiesLibrary.AllPropertiesDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.structuresExtender.SetAttributeName(this.button1, null);
            this.structuresExtender.SetAttributeTypeName(this.button1, null);
            this.structuresExtender.SetBindPropertyName(this.button1, null);
            this.button1.Location = new System.Drawing.Point(7, 98);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 25);
            this.button1.TabIndex = 3;
            this.button1.Text = "<-";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // button2
            // 
            this.structuresExtender.SetAttributeName(this.button2, null);
            this.structuresExtender.SetAttributeTypeName(this.button2, null);
            this.structuresExtender.SetBindPropertyName(this.button2, null);
            this.button2.Location = new System.Drawing.Point(7, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 25);
            this.button2.TabIndex = 4;
            this.button2.Text = "->";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.structuresExtender.SetAttributeName(this.OkButton, null);
            this.structuresExtender.SetAttributeTypeName(this.OkButton, null);
            this.structuresExtender.SetBindPropertyName(this.OkButton, null);
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(872, 382);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(108, 21);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "albl_OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.structuresExtender.SetAttributeName(this.CancelButton, null);
            this.structuresExtender.SetAttributeTypeName(this.CancelButton, null);
            this.structuresExtender.SetBindPropertyName(this.CancelButton, null);
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(763, 382);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(103, 21);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "albl_Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.structuresExtender.SetAttributeName(this.splitContainer1, null);
            this.structuresExtender.SetAttributeTypeName(this.splitContainer1, null);
            this.structuresExtender.SetBindPropertyName(this.splitContainer1, null);
            this.splitContainer1.Location = new System.Drawing.Point(1, -1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.structuresExtender.SetAttributeName(this.splitContainer1.Panel1, null);
            this.structuresExtender.SetAttributeTypeName(this.splitContainer1.Panel1, null);
            this.structuresExtender.SetBindPropertyName(this.splitContainer1.Panel1, null);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.shownPropertiesDialog1);
            // 
            // splitContainer1.Panel2
            // 
            this.structuresExtender.SetAttributeName(this.splitContainer1.Panel2, null);
            this.structuresExtender.SetAttributeTypeName(this.splitContainer1.Panel2, null);
            this.structuresExtender.SetBindPropertyName(this.splitContainer1.Panel2, null);
            this.splitContainer1.Panel2.Controls.Add(this.allPropertiesDialog1);
            this.splitContainer1.Size = new System.Drawing.Size(994, 377);
            this.splitContainer1.SplitterDistance = 321;
            this.splitContainer1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.structuresExtender.SetAttributeName(this.panel1, null);
            this.structuresExtender.SetAttributeTypeName(this.panel1, null);
            this.structuresExtender.SetBindPropertyName(this.panel1, null);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Location = new System.Drawing.Point(259, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(62, 377);
            this.panel1.TabIndex = 0;
            // 
            // shownPropertiesDialog1
            // 
            this.structuresExtender.SetAttributeName(this.shownPropertiesDialog1, null);
            this.structuresExtender.SetAttributeTypeName(this.shownPropertiesDialog1, null);
            this.structuresExtender.SetBindPropertyName(this.shownPropertiesDialog1, null);
            this.shownPropertiesDialog1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shownPropertiesDialog1.Location = new System.Drawing.Point(0, 0);
            this.shownPropertiesDialog1.Name = "shownPropertiesDialog1";
            this.shownPropertiesDialog1.Size = new System.Drawing.Size(321, 377);
            this.shownPropertiesDialog1.TabIndex = 2;
            // 
            // allPropertiesDialog1
            // 
            this.structuresExtender.SetAttributeName(this.allPropertiesDialog1, null);
            this.structuresExtender.SetAttributeTypeName(this.allPropertiesDialog1, null);
            this.structuresExtender.SetBindPropertyName(this.allPropertiesDialog1, null);
            this.allPropertiesDialog1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allPropertiesDialog1.Location = new System.Drawing.Point(0, 0);
            this.allPropertiesDialog1.Name = "allPropertiesDialog1";
            this.allPropertiesDialog1.Size = new System.Drawing.Size(669, 377);
            this.allPropertiesDialog1.TabIndex = 1;
            // 
            // PropertiesForm
            // 
            this.structuresExtender.SetAttributeName(this, null);
            this.structuresExtender.SetAttributeTypeName(this, null);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.structuresExtender.SetBindPropertyName(this, null);
            this.ClientSize = new System.Drawing.Size(996, 406);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropertiesForm";
            this.Text = "albl_Properties";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AllPropertiesDialog allPropertiesDialog1;
        private ShownPropertiesDialog shownPropertiesDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button OkButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
    }
}