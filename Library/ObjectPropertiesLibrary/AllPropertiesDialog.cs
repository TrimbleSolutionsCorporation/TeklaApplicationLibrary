namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using Tekla.Structures.Properties;

    /// <summary>
    /// The all properties dialog.
    /// </summary>
    public partial class AllPropertiesDialog : UserControl
    {
        #region Fields

        /// <summary>
        /// The prev selection.
        /// </summary>
        private readonly ArrayList prevSelection = new ArrayList();

        /// <summary>
        /// The validation error label.
        /// </summary>
        private readonly Label validationErrorLabel = new Label();

        /// <summary>
        /// The all shown presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml allShownPresentedPropertiesXmlInstance;

        /// <summary>
        /// The new shown presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml newShownPresentedPropertiesXmlInstance;

        /// <summary>
        /// The quick search user cntrl.
        /// </summary>
        private QuickSearchForDGW quickSearchUserCntrl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AllPropertiesDialog"/> class. 
        /// Instantiates a user control for handling all properties.
        /// </summary>
        public AllPropertiesDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>Initializes a new instance of the <see cref="AllPropertiesDialog"/> class. 
        /// Instantiates a user control for handling all properties.</summary>
        /// <param name="parentForm">A form in relation location of this form is set.</param>
        /// <param name="allPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains all properties.</param>
        /// <param name="shownPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains shown properties.</param>
        /// <param name="methodToLocalize">Delegate method to use for localization.</param>
        public AllPropertiesDialog(
            Form parentForm, 
            ref PresentedPropertiesXml allPresentedPropertiesXmlInstance, 
            ref PresentedPropertiesXml shownPresentedPropertiesXmlInstance, 
            LocalisationDelegate methodToLocalize)
        {
            this.InitializeComponent();
            this.InitializeAllPropertiesDialog(parentForm, ref allPresentedPropertiesXmlInstance, ref shownPresentedPropertiesXmlInstance, methodToLocalize);
        }

        /// <summary>Gets or sets the all shown presented properties xml instance.</summary>
        /// <value>The all shown presented properties xml instance.</value>
        public PresentedPropertiesXml AllShownPresentedPropertiesXmlInstance
        {
            get
            {
                return this.allShownPresentedPropertiesXmlInstance;
            }

            set
            {
                this.allShownPresentedPropertiesXmlInstance = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Initializes a user control for handling all properties.</summary>
        /// <param name="newParentForm">A form in relation location of this form is set.</param>
        /// <param name="allPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains all properties.</param>
        /// <param name="shownPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains shown properties.</param>
        /// <param name="methodToLocalize">Delegate method to use for localization.</param>
        public void InitializeAllPropertiesDialog(
            Form newParentForm, 
            ref PresentedPropertiesXml allPresentedPropertiesXmlInstance, 
            ref PresentedPropertiesXml shownPresentedPropertiesXmlInstance, 
            LocalisationDelegate methodToLocalize)
        {
            // Add quick search
            this.quickSearchUserCntrl = new QuickSearchForDGW();
            this.quickSearchUserCntrl.SetReferenceProperties(ref this.AllPropertiesDGW);
            var quickSearchUserCntrlControlHost = new ToolStripControlHost(this.quickSearchUserCntrl) { Alignment = ToolStripItemAlignment.Right };
            this.quickSearchUserCntrl.Width = 140;
            this.AllPropertiesToolStrip.SuspendLayout();
            this.AllPropertiesToolStrip.Items.Add(quickSearchUserCntrlControlHost);
            this.AllPropertiesToolStrip.ResumeLayout();

            if (methodToLocalize != null)
            {
                methodToLocalize(this);
                methodToLocalize(this.contextMenuStrip1);
                this.validationErrorLabel.Text = "albl_Change_default_property_name";
                methodToLocalize(this.validationErrorLabel);
            }

            this.AllPropertiesDGW.AutoGenerateColumns = false;

            this.AllPropertiesDGW.SelectionChanged += this.ShownPropertiesDGWSelectionChanged;
            this.AllPropertiesDGW.KeyDown += this.AllPropertiesDGWKeyDown;
            this.AllPropertiesDGW.Sorted += this.AllPropertiesDGWSorted;

            PresentedPropertiesManage.ChangeHiddenValuesByOtherFile(
                ref allPresentedPropertiesXmlInstance, ref shownPresentedPropertiesXmlInstance);
            allPresentedPropertiesXmlInstance.ForceReadFile();
            this.newShownPresentedPropertiesXmlInstance = shownPresentedPropertiesXmlInstance;
            this.allShownPresentedPropertiesXmlInstance = allPresentedPropertiesXmlInstance;
            this.AllPropertiesDGW.DataSource = this.allShownPresentedPropertiesXmlInstance.PropertiesList;

            this.AllPropertiesDGW.ReadOnly = false;
            this.AllPropertiesDGW.AllowUserToAddRows = true;
            this.AllPropertiesDGW.AllowUserToDeleteRows = true;
            this.AllPropertiesDGW.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.AllPropertiesDGW.Columns[0].ReadOnly = false;
        }

        /// <summary>Sets visibility of Included check box column.</summary>
        /// <param name="show">The show value.</param>
        public void ShowIncludedColumn(bool show)
        {
            foreach (DataGridViewColumn column in this.AllPropertiesDGW.Columns)
            {
                if (column.Name == "PropertyShown")
                {
                    column.Visible = show;
                }
            }
        }

        /// <summary>
        /// Sets selected rows properties visibility to true.
        /// </summary>
        public void ShowSelected()
        {
            foreach (PresentedProperties property in this.prevSelection)
            {
                if (property != null)
                {
                    property.Visible = true;
                }
            }
        }

        /// <summary>
        /// Updates all properties hidden values according shown properties.
        /// </summary>
        public void UpdateAllProperties()
        {
            PresentedPropertiesManage.ChangeHiddenValuesByOtherFile(
                ref this.allShownPresentedPropertiesXmlInstance, ref this.newShownPresentedPropertiesXmlInstance);
            this.AllPropertiesDGW.DataSource = this.allShownPresentedPropertiesXmlInstance.PropertiesList;
            this.Refresh();
        }

        #endregion

        #region Methods

        /// <summary>The add properties.</summary>
        /// <param name="external">The external.</param>
        private void AddProperties(bool external)
        {
            string copyTextBulk = null;
            char[] lineFeeds = { '\n' };

            try
            {
                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                {
                    copyTextBulk = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
                }

                if (this.AllPropertiesDGW.Rows.Count != 0)
                {
                    this.allShownPresentedPropertiesXmlInstance.PropertiesList.CancelNew(this.AllPropertiesDGW.Rows.Count - 1);
                }

                if (copyTextBulk != null)
                {
                    this.AllPropertiesDGW.SuspendLayout();

                    var copyLines = copyTextBulk.Split(lineFeeds);

                    foreach (var copyLine in copyLines)
                    {
                        var toIndex = copyLine.IndexOf(':');
                        var thisProperty = copyLine.Substring(0, toIndex);
                        string valueString = null;

                        if (copyLine.Length > toIndex)
                        {
                            valueString = copyLine.Substring(toIndex + 1);
                        }

                        thisProperty = thisProperty.Trim();

                        if (external)
                        {
                            thisProperty = "EXTERNAL." + thisProperty;
                        }

                        this.allShownPresentedPropertiesXmlInstance.AddPropertyWithReportName(thisProperty, valueString);
                    }

                    this.AllPropertiesDGW.ResumeLayout();
                    this.AllPropertiesDGW.Refresh();
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.ToString());

                this.AllPropertiesDGW.ResumeLayout();
                this.AllPropertiesDGW.Refresh();
            }
        }

        /// <summary>The all properties dgw cell click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.AllPropertiesDGW.Columns[e.ColumnIndex].Name == "PropertyShown")
            {
                this.CancelEditIfPropertyIsDefault(this.AllPropertiesDGW.Rows.Count - 1, null);
            }
        }

        /// <summary>The all properties dgw cell double click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.AllPropertiesDGW.Columns[e.ColumnIndex].Name == "PropertyShown")
            {
                this.CancelEditIfPropertyIsDefault(this.AllPropertiesDGW.Rows.Count - 1, null);
            }
        }

        /// <summary>The all properties dgw row validating.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWRowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.CancelEditIfPropertyIsDefault(e.RowIndex, e);
        }

        /// <summary>The all properties dg w_ cell validating.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // FIX To validate the contents of the property name.
            // It cannot be unmodified (= default name), because at that point the UI component's data source
            // will crash occasionally
            if (e.ColumnIndex == 1)
            {
                // Check that we are validating the property name column
                if (e.FormattedValue.ToString() == PresentedProperties.DefaultPropertyName)
                {
                    if (this.ParentForm != null)
                    {
                        MessageBox.Show(
                            this.validationErrorLabel.Text, 
                            this.ParentForm.Text, 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Exclamation, 
                            MessageBoxDefaultButton.Button1);
                    }

                    e.Cancel = true;
                }
            }
        }

        /// <summary>The all properties dg w_ key down.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.V:
                    if (e.Control)
                    {
                        var copyText = Clipboard.GetText();
                        foreach (DataGridViewCell cellToCopy in this.AllPropertiesDGW.SelectedCells)
                        {
                            if (this.AllPropertiesDGW.Rows[this.AllPropertiesDGW.Rows.Count - 1].Cells[1] == cellToCopy)
                            {
                                if (this.AllPropertiesDGW.Rows.Count != 0)
                                {
                                    this.allShownPresentedPropertiesXmlInstance.PropertiesList.CancelNew(
                                        this.AllPropertiesDGW.Rows.Count - 1);
                                }

                                this.allShownPresentedPropertiesXmlInstance.PropertiesList.AddNew();
                            }

                            cellToCopy.Value = copyText;
                        }
                    }

                    break;
            }
        }

        /// <summary>The all properties dg w_ mouse click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show();
            }
        }

        /// <summary>The all properties dg w_ sorted.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWSorted(object sender, EventArgs e)
        {
            this.quickSearchUserCntrl.QuickSearchExecute();
        }

        /// <summary>Cancel the edit mode if the Property Name is equal to the defalult name. The DataGridView control
        /// has some bugs that occurs in databinding. This logic is done in order to prevent the DataGriedView internal crash.</summary>
        /// <param name="rowNumber">The number of the row to check.</param>
        /// <param name="e">The e value.</param>
        private void CancelEditIfPropertyIsDefault(int rowNumber, DataGridViewCellCancelEventArgs e)
        {
            if (this.AllPropertiesDGW.Rows.Count > 0)
            {
                var tmp = this.AllPropertiesDGW.Rows[rowNumber].Cells["Property"].Value;
                if (this.AllPropertiesDGW.Rows[rowNumber].Cells["Property"].Value != null)
                {
                    if (this.AllPropertiesDGW.Rows[rowNumber].Cells["Property"].Value.ToString()
                        == PresentedProperties.DefaultPropertyName)
                    {
                        if (e == null)
                        {
                            this.AllPropertiesDGW.CancelEdit();
                        }
                    }
                }
            }
        }

        /// <summary>The grid clicked.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void GridClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Location.Y < this.AllPropertiesDGW.ColumnHeadersHeight)
                {
                    this.ShowHideColumns(e.Location);
                }
                else
                {
                    this.contextMenuStrip1.Show(this, e.Location);
                }
            }
        }

        /// <summary>The load properties_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void LoadPropertiesClick(object sender, EventArgs e)
        {
            var openShown = new OpenFileDialog
                {
                    InitialDirectory = string.IsNullOrEmpty(Settings.Default.FileFolder) ?
                    this.allShownPresentedPropertiesXmlInstance.LoadSaveDirectory :
                    Settings.Default.FileFolder
                };

            if (openShown.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var loadedProperties =
                        PresentedPropertiesXml.ReadPropertiesListFromFile(openShown.FileName);
                    if (loadedProperties.Count > 0)
                    {
                        this.allShownPresentedPropertiesXmlInstance.PropertiesList = loadedProperties;
                        Settings.Default.FileFolder = Path.GetDirectoryName(openShown.FileName);
                        Settings.Default.Save();
                     }

                    this.AllPropertiesDGW.DataSource = this.allShownPresentedPropertiesXmlInstance.PropertiesList;
                    this.AllPropertiesDGW.Refresh();

                    PresentedPropertiesManage.MakeFileFromOtherFilesHiddenProperties(
                        ref this.newShownPresentedPropertiesXmlInstance, ref this.allShownPresentedPropertiesXmlInstance);

                    this.newShownPresentedPropertiesXmlInstance.ForceReadFile();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }
        }

        /// <summary>
        /// The remove selected rows.
        /// </summary>
        private void RemoveSelectedRows()
        {
            var copyList = new ArrayList(this.prevSelection);

            foreach (PresentedProperties selectedProperty in copyList)
            {
                this.allShownPresentedPropertiesXmlInstance.RemoveProperty(selectedProperty);
            }

            this.AllPropertiesDGW.Refresh();
        }

        /// <summary>The save properties_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void SavePropertiesClick(object sender, EventArgs e)
        {
            this.AllPropertiesDGW.EndEdit();
            this.allShownPresentedPropertiesXmlInstance.XmlWriteProperties(
                this.allShownPresentedPropertiesXmlInstance.PropertiesList);

            var saveShown = new SaveFileDialog
                {
                    InitialDirectory = string.IsNullOrEmpty(Settings.Default.FileFolder) ?
                    this.allShownPresentedPropertiesXmlInstance.LoadSaveDirectory :
                    Settings.Default.FileFolder
                };

            if (saveShown.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PresentedPropertiesXml.XmlWriteProperties(
                        this.allShownPresentedPropertiesXmlInstance.PropertiesList, saveShown.FileName);
                    Settings.Default.FileFolder = Path.GetDirectoryName(saveShown.FileName);
                    Settings.Default.Save();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }
        }

        /// <summary>The show hide columns.</summary>
        /// <param name="showLocation">The show location.</param>
        private void ShowHideColumns(Point showLocation)
        {
            if (this.AllPropertiesDGW != null)
            {
                var items = new MenuItem[this.AllPropertiesDGW.Columns.Count];
                var count = 0;

                foreach (DataGridViewColumn column in this.AllPropertiesDGW.Columns)
                {
                    var item = new MenuItem(column.HeaderText, this.ShowHideOnClick);

                    if (column.Visible)
                    {
                        item.Checked = true;
                    }

                    items[count] = item;
                    count++;
                }

                var showHide = new ContextMenu(items);

                showHide.Show(this, showLocation);
            }
        }

        /// <summary>The show hide on click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ShowHideOnClick(object sender, EventArgs e)
        {
            var thisItem = sender as MenuItem;

            if (thisItem != null)
            {
                foreach (DataGridViewColumn column in this.AllPropertiesDGW.Columns)
                {
                    if (column.HeaderText == thisItem.Text)
                    {
                        column.Visible = !thisItem.Checked;
                        thisItem.Checked = !thisItem.Checked;
                    }
                }
            }
        }

        /// <summary>The shown properties dg w_ selection changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ShownPropertiesDGWSelectionChanged(object sender, EventArgs e)
        {
            this.prevSelection.Clear();

            foreach (DataGridViewCell cell in this.AllPropertiesDGW.SelectedCells)
            {
                var presentedProperties = this.AllPropertiesDGW.Rows[cell.RowIndex].DataBoundItem as PresentedProperties;

                if (presentedProperties != null && !this.prevSelection.Contains(presentedProperties))
                {
                    this.prevSelection.Add(presentedProperties);
                }
            }
        }

        /// <summary>The albl excludeselected tool strip menu item_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AlblExcludeselectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (PresentedProperties selectedProperty in this.prevSelection)
            {
                selectedProperty.Visible = false;
            }

            this.AllPropertiesDGW.Refresh();
        }

        /// <summary>The albl includeselected tool strip menu item_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AlblIncludeselectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            foreach (PresentedProperties selectedProperty in this.prevSelection)
            {
                selectedProperty.Visible = true;
            }

            this.AllPropertiesDGW.Refresh();
        }

        /// <summary>The albl removeselected tool strip menu item_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AlblRemoveselectedToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.RemoveSelectedRows();
        }

        /// <summary>The tool strip button 1_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ToolStripButton1Click(object sender, EventArgs e)
        {
            if (!this.toolStripButton1.Checked)
            {
                this.AllPropertiesDGW.ReadOnly = true;
                this.AllPropertiesDGW.AllowUserToAddRows = false;
                this.AllPropertiesDGW.AllowUserToDeleteRows = true;
                this.AllPropertiesDGW.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            else
            {
                this.AllPropertiesDGW.ReadOnly = false;
                this.AllPropertiesDGW.AllowUserToAddRows = true;
                this.AllPropertiesDGW.AllowUserToDeleteRows = false;
                this.AllPropertiesDGW.SelectionMode = DataGridViewSelectionMode.CellSelect;

                this.AllPropertiesDGW.Columns[0].ReadOnly = true;
            }
        }

        /// <summary>The tool strip button 2_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ToolStripButton2Click(object sender, EventArgs e)
        {
            if (this.AllPropertiesDGW.CurrentCell == null || !this.AllPropertiesDGW.CurrentCell.IsInEditMode)
            {
                this.AllPropertiesDGW.CurrentCell = this.AllPropertiesDGW.Rows[this.AllPropertiesDGW.Rows.Count - 1].Cells[1];
            }
        }

        /// <summary>The tool strip button 3_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ToolStripButton3Click(object sender, EventArgs e)
        {
            if (this.AllPropertiesDGW.CurrentCell == null || !this.AllPropertiesDGW.CurrentCell.IsInEditMode)
            {
                this.RemoveSelectedRows();
            }
        }

        /// <summary>The tool strip menu item 1_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ToolStripMenuItem1Click(object sender, EventArgs e)
        {
            this.AddProperties(true);
        }

        /// <summary>The tool strip menu item 2_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            this.AddProperties(false);
        }

        #endregion
    }
}