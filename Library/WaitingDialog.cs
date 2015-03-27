namespace Tekla.Structures
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Waiting dialog.
    /// </summary>
    public partial class WaitingDialog : Form
    {
        #region Fields

        /// <summary>
        /// Predicate that determines whether the operation has been completed.
        /// </summary>
        private readonly Predicate<WaitingDialog> completed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingDialog"/> class. 
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="completed">
        /// Predicate that determines whether the operation has been completed.
        /// </param>
        public WaitingDialog(Predicate<WaitingDialog> completed)
        {
            this.completed = completed;
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the Load event.
        /// </summary>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.DesignMode && TeklaStructures.Connection.IsActive)
            {
                TeklaStructures.Environment.Localization.Localize(this);
            }
            else
            {
                this.cancelButton.Text = "Cancel";
            }
        }

        /// <summary>
        /// Handles the Tick event.
        /// </summary>
        /// <param name="sender">
        /// The Sender.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void OnTick(object sender, EventArgs e)
        {
            if (this.completed(this))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion
    }
}