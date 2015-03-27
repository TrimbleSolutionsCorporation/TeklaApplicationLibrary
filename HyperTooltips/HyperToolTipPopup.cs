namespace Tekla.UI.HyperToolTips
{
    using System.Windows.Forms;

    /// <summary>
    /// Specialized popup for Office-2007-like tooltips.
    /// Typically created automatically by <seealso cref="HyperToolTipProvider"/> to show HyperToolTip. 
    /// </summary>
    public class HyperToolTipPopup : Popup
    {
        #region Fields

        /// <summary>
        /// The _url.
        /// </summary>
        private string _url;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperToolTipPopup"/> class. 
        /// Creates HyperToolTipPopup.
        /// </summary>
        public HyperToolTipPopup()
            : base(CreateControlInstance())
        {
            this.Opacity = 0.95;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/set url to tip content to show when <see cref="Popup.Show(Control)"/> method called.
        /// </summary>
        /// <value>url to tip content.</value>
        public string TipUrl
        {
            get
            {
                return this._url;
            }

            set
            {
                this._url = value;
                this.WebBrowser.Navigate(this._url);
            }
        }

        /// <summary>
        /// Create a strongly typed property called WebBrowser - handy to prevent casting everywhere.
        /// </summary>
        public WebBrowser WebBrowser
        {
            get
            {
                return this.Content as WebBrowser;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create the actual control, note this is static so it can be called from the
        /// constructor.
        /// </summary>
        /// <returns>
        /// The System.Windows.Forms.Control.
        /// </returns>
        private static Control CreateControlInstance()
        {
            var ctrl = new WebBrowser();

            ctrl.Name = "webBrowser";
            ctrl.Dock = DockStyle.Fill;
            ctrl.ScrollBarsEnabled = false;
            ctrl.IsWebBrowserContextMenuEnabled = false;
            ctrl.WebBrowserShortcutsEnabled = false;
            ctrl.AllowWebBrowserDrop = false;
            ctrl.ScriptErrorsSuppressed = true;
            return ctrl;
        }

        #endregion
    }
}