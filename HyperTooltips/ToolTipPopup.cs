namespace Tekla.UI.HyperToolTips
{
    using System.Windows.Forms;

    /// <summary>
    /// Class to show popups similar to standard tooltips
    /// </summary>
    public class ToolTipPopup : Popup
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ToolTipPopup"/> class. 
        /// Constracts a popup that shows the text specified.</summary>
        /// <param name="text">Text to show in popup</param>
        public ToolTipPopup(string text)
            : base(CreateControlInstance(text))
        {
            this.UseFading = false;
            this.Opacity = 1.0;
            this.DropShadowEnabled = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Create a strongly typed property called Label - handy to prevent casting everywhere.
        /// </summary>
        public Label Label
        {
            get
            {
                return this.Content as Label;
            }
        }

        #endregion

        #region Methods

        /// <summary>Create the actual control, note this is static so it can be called from the
        /// constructor.</summary>
        /// <param name="text">The text.</param>
        /// <returns>The System.Windows.Forms.Control.</returns>
        private static Control CreateControlInstance(string text)
        {
            var ctrl = new Label();

            ctrl.Dock = DockStyle.Fill;
            ctrl.Padding = Padding.Empty;
            ctrl.Text = text;
            ctrl.Width = ctrl.PreferredSize.Width + 16;
            ctrl.Height = ctrl.PreferredSize.Height + 4;
            ctrl.BackColor = new ToolTip().BackColor;
            ctrl.BorderStyle = BorderStyle.FixedSingle;

            return ctrl;
        }

        #endregion
    }
}