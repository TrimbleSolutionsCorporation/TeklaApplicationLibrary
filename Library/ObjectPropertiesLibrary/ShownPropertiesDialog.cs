namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Tekla.Structures.Properties;

    /// <summary>
    /// The shown properties dialog.
    /// </summary>
    public partial class ShownPropertiesDialog : UserControl
    {
        #region Fields

        /// <summary>
        /// The prev selection.
        /// </summary>
        private readonly ArrayList prevSelection = new ArrayList();

        /// <summary>
        /// The all shown presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml allShownPresentedPropertiesXmlInstance;

        /// <summary>
        /// The new shown presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml newShownPresentedPropertiesXmlInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShownPropertiesDialog"/> class. 
        /// Instantiates a user control for handling all properties.
        /// </summary>
        public ShownPropertiesDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>Initializes a new instance of the <see cref="ShownPropertiesDialog"/> class. 
        /// Instantiates a user control for handling all properties.</summary>
        /// <param name="parentForm">A form in relation location of this form is set.</param>
        /// <param name="allPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains all properties.</param>
        /// <param name="shownPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains shown properties.</param>
        /// <param name="methodToLocalize">Delegate method to use for localization.</param>
        public ShownPropertiesDialog(
            Form parentForm, 
            ref PresentedPropertiesXml allPresentedPropertiesXmlInstance, 
            ref PresentedPropertiesXml shownPresentedPropertiesXmlInstance, 
            LocalisationDelegate methodToLocalize)
        {
            this.InitializeComponent();

            if (methodToLocalize != null)
            {
                methodToLocalize(this);
            }

            this.ShownPropertiesDGW.AutoGenerateColumns = false;
            this.ShownPropertiesDGW.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.ShownPropertiesDGW.CellMouseClick += this.ShownPropertiesDGWCellMouseClick;
            this.ShownPropertiesDGW.SelectionChanged += this.AllPropertiesDGWSelectionChanged;

            this.newShownPresentedPropertiesXmlInstance = shownPresentedPropertiesXmlInstance;
            this.allShownPresentedPropertiesXmlInstance = allPresentedPropertiesXmlInstance;
            this.ShownPropertiesDGW.DataSource = this.newShownPresentedPropertiesXmlInstance.PropertiesList;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Initialises a user control for handling all properties.</summary>
        /// <param name="newParentForm">A form in relation location of this form is set.</param>
        /// <param name="allPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains all properties.</param>
        /// <param name="shownPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains shown properties.</param>
        /// <param name="methodToLocalize">Delegate method to use for localization.</param>
        public void InitializeShownPropertiesDialog(
            Form newParentForm, 
            ref PresentedPropertiesXml allPresentedPropertiesXmlInstance, 
            ref PresentedPropertiesXml shownPresentedPropertiesXmlInstance, 
            LocalisationDelegate methodToLocalize)
        {
            if (methodToLocalize != null)
            {
                methodToLocalize(this);
            }

            this.ShownPropertiesDGW.AutoGenerateColumns = false;
            this.ShownPropertiesDGW.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.ShownPropertiesDGW.CellMouseClick += this.ShownPropertiesDGWCellMouseClick;
            this.ShownPropertiesDGW.SelectionChanged += this.AllPropertiesDGWSelectionChanged;

            this.newShownPresentedPropertiesXmlInstance = shownPresentedPropertiesXmlInstance;
            this.allShownPresentedPropertiesXmlInstance = allPresentedPropertiesXmlInstance;
            this.ShownPropertiesDGW.DataSource = this.newShownPresentedPropertiesXmlInstance.PropertiesList;

            // this.FormClosing += new FormClosingEventHandler(ShownPropertiesDialog_FormClosing);
        }

        /// <summary>The refresh dgw.</summary>
        /// <param name="reallyRefreshList">The really refresh list.</param>
        public void RefreshDGW(SearchableSortableBindingList<PresentedProperties> reallyRefreshList)
        {
            this.ShownPropertiesDGW.DataSource = this.newShownPresentedPropertiesXmlInstance.PropertiesList = reallyRefreshList;
            this.ShownPropertiesDGW.Refresh();
        }

        /// <summary>
        /// Removes selected properties from list.
        /// </summary>
        public void RemovePropertiesFromShown()
        {
            foreach (DataGridViewRow row in this.ShownPropertiesDGW.SelectedRows)
            {
                this.ShownPropertiesDGW.Rows.Remove(row);
            }
        }

        /// <summary>
        /// The save shown.
        /// </summary>
        public void SaveShown()
        {
            this.newShownPresentedPropertiesXmlInstance.XmlWriteProperties(this.newShownPresentedPropertiesXmlInstance.PropertiesList);
            this.newShownPresentedPropertiesXmlInstance.ForceReadFile();
        }

        /// <summary>
        /// Updates shown properties list according all properties hidden values.
        /// </summary>
        public void UpdateShownProperties()
        {
            PresentedPropertiesManage.MakeFileFromOtherFilesHiddenProperties(ref this.newShownPresentedPropertiesXmlInstance, ref this.allShownPresentedPropertiesXmlInstance);
            this.ShownPropertiesDGW.DataSource = this.newShownPresentedPropertiesXmlInstance.PropertiesList;
            this.Refresh();
        }

        #endregion

        #region Methods

        /// <summary>The all properties dg w_ selection changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void AllPropertiesDGWSelectionChanged(object sender, EventArgs e)
        {
            this.prevSelection.Clear();

            foreach (DataGridViewRow row in this.ShownPropertiesDGW.SelectedRows)
            {
                var property = row.DataBoundItem as PresentedProperties;
                if (property != null)
                {
                    this.prevSelection.Add(property);
                }
            }
        }

        /// <summary>The load ts b_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void LoadTsbClick(object sender, EventArgs e)
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
                    var loadedProperties = PresentedPropertiesXml.ReadPropertiesListFromFile(openShown.FileName);
                    if (loadedProperties.Count > 0)
                    {
                        this.newShownPresentedPropertiesXmlInstance.PropertiesList = loadedProperties;
                        Settings.Default.FileFolder = Path.GetDirectoryName(openShown.FileName);
                        Settings.Default.Save();
                    }

                    this.ShownPropertiesDGW.DataSource = this.newShownPresentedPropertiesXmlInstance.PropertiesList;
                    this.ShownPropertiesDGW.Refresh();

                    PresentedPropertiesManage.ChangeHiddenValuesByOtherFile(
                        ref this.allShownPresentedPropertiesXmlInstance, ref this.newShownPresentedPropertiesXmlInstance);

                    this.allShownPresentedPropertiesXmlInstance.ForceReadFile();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }
        }

        /// <summary>The save ts b_ click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void SaveTsbClick(object sender, EventArgs e)
        {
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
                        this.newShownPresentedPropertiesXmlInstance.PropertiesList, saveShown.FileName);
                    Settings.Default.FileFolder = Path.GetDirectoryName(saveShown.FileName);
                    Settings.Default.Save();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }
        }

        /// <summary>The shown properties dg w_ cell mouse click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void ShownPropertiesDGWCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.ShownPropertiesDGW.Columns[e.ColumnIndex].Name == "PropertyShown" && this.ShownPropertiesDGW.CurrentRow != null)
            {
                var newValue = !(bool)this.ShownPropertiesDGW.CurrentCell.Value;

                var property = this.ShownPropertiesDGW.CurrentRow.DataBoundItem as PresentedProperties;
                if (property != null)
                {
                    property.Hidden = !newValue;
                }

                foreach (PresentedProperties row in this.prevSelection)
                {
                    if (row != null)
                    {
                        row.Hidden = !newValue;
                    }
                }

                this.ShownPropertiesDGW.Refresh();
            }
        }

        #endregion
    }
}