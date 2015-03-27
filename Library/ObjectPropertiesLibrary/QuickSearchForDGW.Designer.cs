namespace Tekla.Structures.ObjectPropertiesLibrary
{
    public partial class QuickSearchForDGW
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.QuickSearchTB = new System.Windows.Forms.TextBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // QuickSearchTB
            // 
            this.QuickSearchTB.Location = new System.Drawing.Point(3, 3);
            this.QuickSearchTB.Name = "QuickSearchTB";
            this.QuickSearchTB.Size = new System.Drawing.Size(190, 20);
            this.QuickSearchTB.TabIndex = 0;
            this.QuickSearchTB.TextChanged += new System.EventHandler(this.QuickSearchTbTextChanged);
            // 
            // ClearButton
            // 
            this.ClearButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.ClearButton.FlatAppearance.BorderSize = 0;
            this.ClearButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.ClearButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.ClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClearButton.Image = global::Tekla.Structures.ObjectPropertiesLibrary.Properties.Resources.Cancel;
            this.ClearButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ClearButton.Location = new System.Drawing.Point(195, 2);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(20, 20);
            this.ClearButton.TabIndex = 2;
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButtonClick);
            // 
            // QuickSearchForDGW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.QuickSearchTB);
            this.Name = "QuickSearchForDGW";
            this.Size = new System.Drawing.Size(218, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox QuickSearchTB;
        private System.Windows.Forms.Button ClearButton;
    }
}
