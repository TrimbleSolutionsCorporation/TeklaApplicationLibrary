namespace Tekla.UI.HyperToolTips
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWithTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWithoutTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2WithTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subitemWithTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWithoutTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withoutTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.tipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tipToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.noTipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.hypertooltipsProvider1 = new Tekla.UI.HyperToolTips.HyperToolTipProvider(this.components);
            this.tip.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.button1, "OutlookBar.ChangeSelection");
            this.button1.Location = new System.Drawing.Point(31, 235);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 53);
            this.button1.TabIndex = 0;
            this.button1.Tag = "";
            this.button1.Text = "Open subform (Tip)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(331, 63);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = "No tip";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.button3, "tipchm/save1");
            this.button3.Location = new System.Drawing.Point(207, 79);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(106, 53);
            this.button3.TabIndex = 2;
            this.button3.Tag = "";
            this.button3.Text = "Bad Tip";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.listBox1, "tipchm/open");
            this.listBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.listBox1.Location = new System.Drawing.Point(12, 79);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(124, 30);
            this.listBox1.TabIndex = 3;
            this.listBox1.Tag = "";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.hypertooltipsProvider1.SetHyperToolTipId(this.checkBox1, "ToolBarTextLabels.ShowHide");
            this.checkBox1.Location = new System.Drawing.Point(18, 23);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Tag = "";
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.hypertooltipsProvider1.SetHyperToolTipId(this.label1, "Discussion.New");
            this.label1.Location = new System.Drawing.Point(113, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "label with tip!";
            // 
            // textBox1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.textBox1, "User.New");
            this.textBox1.Location = new System.Drawing.Point(18, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(171, 20);
            this.textBox1.TabIndex = 7;
            // 
            // tip
            // 
            this.tip.Controls.Add(this.tabPage1);
            this.tip.Controls.Add(this.tabPage2);
            this.tip.Controls.Add(this.tabPage3);
            this.tip.HotTrack = true;
            this.tip.Location = new System.Drawing.Point(185, 174);
            this.tip.Name = "tip";
            this.tip.SelectedIndex = 0;
            this.tip.Size = new System.Drawing.Size(220, 100);
            this.tip.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBox1);
            this.hypertooltipsProvider1.SetHyperToolTipId(this.tabPage1, "tipchm/save");
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(212, 74);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tip";
            this.tabPage1.ToolTipText = "ddsfsfsdf sdf ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.hypertooltipsProvider1.SetHyperToolTipId(this.tabPage2, "OutlookBar.ChangeSelection");
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(212, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tip2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(212, 74);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "no tip";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.menuWithoutTipToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(509, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemWithTipToolStripMenuItem,
            this.toolStripMenuItem1,
            this.menuItemWithoutTipToolStripMenuItem,
            this.menuItem2WithTipToolStripMenuItem});
            this.hypertooltipsProvider1.SetHyperToolTipId(this.menuToolStripMenuItem, "tipchm/open");
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.menuToolStripMenuItem.Text = "Menu with tip";
            // 
            // menuItemWithTipToolStripMenuItem
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.menuItemWithTipToolStripMenuItem, "OutlookBar.ChangeSelection");
            this.menuItemWithTipToolStripMenuItem.Name = "menuItemWithTipToolStripMenuItem";
            this.menuItemWithTipToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.menuItemWithTipToolStripMenuItem.Text = "Menu Item 1 with Tip";
            // 
            // toolStripMenuItem1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripMenuItem1, "tipchm/properties");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 22);
            this.toolStripMenuItem1.Text = "Menu Item with bad Tip";
            // 
            // menuItemWithoutTipToolStripMenuItem
            // 
            this.menuItemWithoutTipToolStripMenuItem.Name = "menuItemWithoutTipToolStripMenuItem";
            this.menuItemWithoutTipToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.menuItemWithoutTipToolStripMenuItem.Text = "Menu Item without Tip";
            // 
            // menuItem2WithTipToolStripMenuItem
            // 
            this.menuItem2WithTipToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subitemWithTipToolStripMenuItem,
            this.notTipToolStripMenuItem});
            this.hypertooltipsProvider1.SetHyperToolTipId(this.menuItem2WithTipToolStripMenuItem, "OutlookBar.ChangeSelection");
            this.menuItem2WithTipToolStripMenuItem.Name = "menuItem2WithTipToolStripMenuItem";
            this.menuItem2WithTipToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.menuItem2WithTipToolStripMenuItem.Text = "Menu Item 2 with Tip";
            // 
            // subitemWithTipToolStripMenuItem
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.subitemWithTipToolStripMenuItem, "User.New");
            this.subitemWithTipToolStripMenuItem.Name = "subitemWithTipToolStripMenuItem";
            this.subitemWithTipToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.subitemWithTipToolStripMenuItem.Text = "Subitem with tip";
            // 
            // notTipToolStripMenuItem
            // 
            this.notTipToolStripMenuItem.Name = "notTipToolStripMenuItem";
            this.notTipToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.notTipToolStripMenuItem.Text = "not tip";
            // 
            // menuWithoutTipToolStripMenuItem
            // 
            this.menuWithoutTipToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.withoutTipToolStripMenuItem});
            this.menuWithoutTipToolStripMenuItem.Name = "menuWithoutTipToolStripMenuItem";
            this.menuWithoutTipToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.menuWithoutTipToolStripMenuItem.Text = "Menu without tip";
            // 
            // withoutTipToolStripMenuItem
            // 
            this.withoutTipToolStripMenuItem.Name = "withoutTipToolStripMenuItem";
            this.withoutTipToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.withoutTipToolStripMenuItem.Text = "without tip";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSplitButton1,
            this.toolStripDropDownButton1,
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(509, 36);
            this.toolStrip1.TabIndex = 10;
            // 
            // toolStripButton1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripButton1, "User.New");
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 33);
            this.toolStripButton1.Text = "tip";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.ToolTipText = "This is a normal tooltip";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(38, 33);
            this.toolStripButton2.Text = "no tip";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoToolTip = false;
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripButton3, "tipchm/open1");
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(44, 33);
            this.toolStripButton3.Text = "bad tip";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoToolTip = false;
            this.toolStripButton4.Enabled = false;
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripButton4, "tipchm/save");
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 33);
            this.toolStripButton4.Text = "tip";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.AutoToolTip = false;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tipToolStripMenuItem,
            this.noToolStripMenuItem});
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripSplitButton1, "tipchm/save");
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(35, 33);
            this.toolStripSplitButton1.Text = "tip";
            this.toolStripSplitButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripSplitButton1.ToolTipText = "This is a tooltip";
            // 
            // tipToolStripMenuItem
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.tipToolStripMenuItem, "tipchm/open");
            this.tipToolStripMenuItem.Name = "tipToolStripMenuItem";
            this.tipToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tipToolStripMenuItem.Text = "tip";
            this.tipToolStripMenuItem.ToolTipText = "This is a tooltip";
            // 
            // noToolStripMenuItem
            // 
            this.noToolStripMenuItem.Name = "noToolStripMenuItem";
            this.noToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.noToolStripMenuItem.Text = "no tip";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.AutoToolTip = false;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tipToolStripMenuItem1,
            this.noTipToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(67, 33);
            this.toolStripDropDownButton1.Text = "normal tip";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripDropDownButton1.ToolTipText = "This is a tooltip";
            // 
            // tipToolStripMenuItem1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.tipToolStripMenuItem1, "RFI.New");
            this.tipToolStripMenuItem1.Name = "tipToolStripMenuItem1";
            this.tipToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.tipToolStripMenuItem1.Text = "tip";
            this.tipToolStripMenuItem1.ToolTipText = "This is a tooltip";
            // 
            // noTipToolStripMenuItem
            // 
            this.noTipToolStripMenuItem.Name = "noTipToolStripMenuItem";
            this.noTipToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.noTipToolStripMenuItem.Text = "no tip";
            // 
            // toolStripComboBox1
            // 
            this.hypertooltipsProvider1.SetHyperToolTipId(this.toolStripComboBox1, "aaa");
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 36);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.hypertooltipsProvider1.SetHyperToolTipId(this.button4, "Project.New");
            this.button4.Location = new System.Drawing.Point(31, 294);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(106, 53);
            this.button4.TabIndex = 0;
            this.button4.Tag = "";
            this.button4.Text = "Disabled button with Tip";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(249, 294);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(124, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "HyperTooltips Active";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(249, 313);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(126, 17);
            this.checkBox3.TabIndex = 11;
            this.checkBox3.Text = "HyperTooltips Debug";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // hypertooltipsProvider1
            // 
            this.hypertooltipsProvider1.Debug = true;
            this.hypertooltipsProvider1.InitialDelay = 800;
            this.hypertooltipsProvider1.ReshowDelay = 200;
            this.hypertooltipsProvider1.TipsNamespace = "./tooltips.chm";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 359);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Tag = "";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tip.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private HyperToolTipProvider hypertooltipsProvider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tip;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemWithTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemWithoutTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem2WithTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subitemWithTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuWithoutTipToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem tipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tipToolStripMenuItem1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolStripMenuItem withoutTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem notTipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noTipToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

