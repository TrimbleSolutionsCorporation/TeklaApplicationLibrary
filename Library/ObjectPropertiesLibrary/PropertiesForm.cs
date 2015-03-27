namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Tekla.Structures.Dialog;

    /// <summary>
    /// Delegate method to localise this form and dialog forms.
    /// </summary>
    /// <param name="formToLocalise">Form to localize.</param>
    public delegate void LocalisationDelegate(Control formToLocalise);

    /// <summary>
    /// The properties form.
    /// </summary>
    public partial class PropertiesForm : ApplicationFormBase
    {
        #region Fields

        /// <summary>
        /// The own all presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml ownAllPresentedPropertiesXmlInstance;

        /// <summary>
        /// The own shown presented properties xml instance.
        /// </summary>
        private PresentedPropertiesXml ownShownPresentedPropertiesXmlInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesForm"/> class. 
        /// Initializes a form to select showable properties. 
        /// </summary>
        public PropertiesForm()
        {
            this.InitializeComponent();
        }

        /// <summary>Initializes a new instance of the <see cref="PropertiesForm"/> class.
        /// Initializes a form to select showable properties.</summary>
        /// <param name="parentForm">A form in relation location of this form is set.</param>
        /// <param name="allPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains all properties.</param>
        /// <param name="shownPresentedPropertiesXmlInstance">PresentedPropertiesXml instance which contains shown properties.</param>
        /// <param name="localizationMethod">Delegate method to use for localization.</param>
        /// <param name="showShownProperties">If true, shown properties are shown in separate panel as well as move properties buttons.</param>
        public PropertiesForm(
            Form parentForm, 
            ref PresentedPropertiesXml allPresentedPropertiesXmlInstance, 
            ref PresentedPropertiesXml shownPresentedPropertiesXmlInstance, 
            LocalisationDelegate localizationMethod, 
            bool showShownProperties)
        {
            this.InitializeComponent();

            this.StartPosition = FormStartPosition.Manual;

            if (parentForm != null)
            {
                this.TopMost = parentForm.TopMost;
                this.Location = this.GetLocation(parentForm.Location);
            }
            else
            {
                this.Location = this.GetLocation(Screen.PrimaryScreen.WorkingArea.Location);
            }

            this.splitContainer1.Panel1Collapsed = !showShownProperties;
            this.allPropertiesDialog1.ShowIncludedColumn(!showShownProperties);

            this.ownAllPresentedPropertiesXmlInstance = allPresentedPropertiesXmlInstance;
            this.ownShownPresentedPropertiesXmlInstance = shownPresentedPropertiesXmlInstance;

            this.allPropertiesDialog1.InitializeAllPropertiesDialog(
                parentForm, 
                ref allPresentedPropertiesXmlInstance, 
                ref shownPresentedPropertiesXmlInstance, 
                localizationMethod);
            this.shownPropertiesDialog1.InitializeShownPropertiesDialog(
                parentForm, 
                ref allPresentedPropertiesXmlInstance, 
                ref shownPresentedPropertiesXmlInstance, 
                localizationMethod);

            if (localizationMethod != null)
            {
                localizationMethod(this);
            }

            this.FormClosing += this.PropertiesFormFormClosing;
        }

        #endregion

        #region Methods

        /// <summary>Button1's click event handler
        /// Moves included properties to shown properties list.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Button1Click(object sender, EventArgs e)
        {
            this.allPropertiesDialog1.ShowSelected();
            this.shownPropertiesDialog1.UpdateShownProperties();
        }

        /// <summary>Handles the Click event of the button2 control.
        /// Set properties included parameter according the shown list.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Button2Click(object sender, EventArgs e)
        {
            this.shownPropertiesDialog1.RemovePropertiesFromShown();
            this.allPropertiesDialog1.UpdateAllProperties();
        }

        /// <summary>The get location.</summary>
        /// <param name="parentLocation">The parent location.</param>
        /// <returns>The System.Drawing.Point.</returns>
        private Point GetLocation(Point parentLocation)
        {
            var result = new Point(parentLocation.X, parentLocation.Y + 24);
            var leftEdge = parentLocation.X + this.Width;
            var thisBottom = parentLocation.Y + this.Height;

            var thisScreen = Screen.GetWorkingArea(parentLocation);

            if (leftEdge > thisScreen.Width)
            {
                result.X = result.X - (leftEdge - thisScreen.Width);
            }

            if (thisBottom > thisScreen.Height)
            {
                result.Y = result.Y - (thisBottom - thisScreen.Height);
            }

            return result;
        }

        /// <summary>Handles the Click event of the OkButton control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            this.ownAllPresentedPropertiesXmlInstance.XmlWriteProperties(
                this.ownAllPresentedPropertiesXmlInstance.PropertiesList);

            PresentedPropertiesManage.MakeFileFromOtherFilesHiddenProperties(
                ref this.ownShownPresentedPropertiesXmlInstance, ref this.ownAllPresentedPropertiesXmlInstance);
        }

        /// <summary>Propertieses the form form closing.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/>
        /// instance containing the event data.</param>
        private void PropertiesFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.ownAllPresentedPropertiesXmlInstance.ForceReadFile();
            this.ownShownPresentedPropertiesXmlInstance.ForceReadFile();
        }

        #endregion
    }
}