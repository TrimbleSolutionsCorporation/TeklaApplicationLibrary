namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Security.Permissions;
    using System.Windows.Forms;

    ///<summary>
    /// Reason of pop-up closing.
    ///</summary>
    public enum PopupCloseReason
    {
        /// <summary>
        /// 
        /// </summary>
        AppFocusChange = 0, 

        /// <summary>
        /// 
        /// </summary>
        AppClicked = 1, 

        /// <summary>
        /// 
        /// </summary>
        ItemClicked = 2, 

        /// <summary>
        /// 
        /// </summary>
        Keyboard = 3, 

        /// <summary>
        /// 
        /// </summary>
        CloseCalled = 4
    }

    /// <summary>
    /// Represents a pop-up window.
    /// </summary>
    public partial class Popup : ToolStripDropDown
    {
        #region Constants

        /// <summary>
        /// The _fading interval.
        /// </summary>
        private const int _fadingInterval = 25; // maximum update interval in ms that visually still looks nice

        #endregion

        #region Fields

        /// <summary>
        /// The _fading timer.
        /// </summary>
        private readonly Timer _fadingTimer;

        /// <summary>
        /// The _host.
        /// </summary>
        private readonly ToolStripControlHost _host;

        /// <summary>
        /// The _accept alt.
        /// </summary>
        private bool _acceptAlt = true;

        /// <summary>
        /// The _child popup.
        /// </summary>
        private Popup _childPopup;

        /// <summary>
        /// The _content.
        /// </summary>
        private Control _content;

        /// <summary>
        /// The _fading direction.
        /// </summary>
        private int _fadingDirection;

        /// <summary>
        /// The _fading duration.
        /// </summary>
        private int _fadingDuration = 250;

        /// <summary>
        /// The _max opacity.
        /// </summary>
        private double _maxOpacity = 0.95;

        // Parent (or owner) of the tooltip, not nesesary a control that we show tip for.
        // Parent is used to track hierarhy of popups
        /// <summary>
        /// The _owner popup.
        /// </summary>
        private Popup _ownerPopup;

        /// <summary>
        /// The _resizable.
        /// </summary>
        private bool _resizable = true;

        /// <summary>
        /// The _use fading.
        /// </summary>
        private bool _useFading;

        /// <summary>
        /// The focus on open.
        /// </summary>
        private bool focusOnOpen = false;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Popup"/> class.</summary>
        /// <param name="content">The content of the pop-up.</param>
        /// <remarks>Pop-up will be disposed immediately after disposion of the content control.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is 
        /// <code>
        /// null</code>
        /// </exception>
        public Popup(Control content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this._content = content;

            this._useFading = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;
            this._fadingDirection = 1;
            this._fadingTimer = new Timer();
            this._fadingTimer.Interval = _fadingInterval;
            this._fadingTimer.Tick += this.OnFadingTimer;

            this.InitializeComponent();
            this.AutoSize = false;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.AutoClose = false;

            // this property is extremely important for arhiving correct focus behaviour: popup should not change current keyboard focus
            this._host = new ToolStripControlHost(this._content);
            this._host.Dock = DockStyle.Fill;
            this.Margin = this.Padding = this._host.Margin = this._host.Padding = Padding.Empty;
            this.MinimumSize = this._content.MinimumSize;
            this.MaximumSize = this._content.MaximumSize;
            this.Size = this._content.Size;
            this._content.Location = Point.Empty;

            this.Items.Add(this._host);

            this._content.Disposed += delegate(object sender, EventArgs e) { this.Dispose(true); };
            this._content.RegionChanged += delegate(object sender, EventArgs e) { this.UpdateRegion(); };
            this.UpdateRegion();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether presing the alt key should close the pop-up.
        /// </summary>
        /// <value><c>true</c> if presing the alt key does not close the pop-up; otherwise, <c>false</c>.</value>
        public bool AcceptAlt
        {
            get
            {
                return this._acceptAlt;
            }

            set
            {
                this._acceptAlt = value;
            }
        }

        /// <summary>
        /// Gets the content of the pop-up.
        /// </summary>
        public Control Content
        {
            get
            {
                return this._content;
            }
        }

        /// <summary>
        /// Get/set fading timer interval in milleseconds. The bigger number the slower fading is.
        /// <seealso cref="UseFading"/> should be set to <code>true</code> to turn fading effect on.
        /// </summary>
        /// <value>Fading timer interval in milleseconds.</value>
        public int FadingInterval
        {
            get
            {
                return this._fadingDuration;
            }

            set
            {
                this._fadingDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to focus the content after the pop-up has been opened.
        /// </summary>
        /// <value><c>true</c> if the content should be focused after the pop-up has been opened; otherwise, <c>false</c>.</value>
        /// <remarks>If the FocusOnOpen property is set to <c>false</c>, then pop-up cannot use the fade effect.</remarks>
        public bool FocusOnOpen
        {
            get
            {
                return this.focusOnOpen;
            }

            set
            {
                this.focusOnOpen = value;
            }
        }

        ///<summary>
        ///</summary>
        public bool Resizable
        {
            get
            {
                return this._resizable;
            }

            set
            {
                this._resizable = value;
            }
        }

        /// <summary>
        /// Get/set whether to use fadding while show tooltip window.
        /// <seealso cref="FadingInterval"/> is used to define the speed of fading.
        /// </summary>
        /// <value><b>true</b> indicates fadding will be used.</value>
        public bool UseFading
        {
            get
            {
                return this._useFading;
            }

            set
            {
                this._useFading = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets parameters of a new window.
        /// </summary>
        /// <returns>An object of type <see cref="CreateParams" /> used when creating a new window.</returns>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Shows pop-up window below the specified control.</summary>
        /// <param name="control">The control below which the pop-up will be shown.</param>
        /// <remarks>When there is no space below the specified control, the pop-up control is shown above it.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="control"/> is 
        /// <code>
        /// null</code>
        /// </exception>
        public virtual void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            Show(control, control.ClientRectangle);
        }

        /// <summary>Shows pop-up window below the specified area of specified control.</summary>
        /// <param name="control">The control used to compute screen location of specified area.</param>
        /// <param name="area">The area of control below which the pop-up will be shown.</param>
        /// <remarks>When there is no space below specified area, the pop-up control is shown above it.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="control"/> is 
        /// <code>
        /// null</code>
        /// </exception>
        public virtual void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            // hide first, before changing any properties for new window
            if (this._fadingTimer.Enabled)
            {
                this._fadingTimer.Enabled = false;
                this.Close();
            }

            this.SetOwnerItem(control);

            // find location and direction to show popup
            Point location;
            if (control is TabPage || control is ToolStrip)
            {
                // special calculation of the location for TabPages
                var mousePos = Cursor.Position;
                var curs = Cursor.Current;
                if (null != curs)
                {
                    location = new Point(mousePos.X, mousePos.Y + curs.Size.Height / 2);
                }
                else
                {
                    location = new Point(mousePos.X, mousePos.Y + 10);
                }
            }
            else
            {
                location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
            }

            // correct location and direction nicelly (try to keep control visible) if don't fit to screen work area
            var screen = Screen.FromControl(control);
            if (null != screen)
            {
                if ((location.Y + this.Size.Height) > (screen.WorkingArea.Top + screen.WorkingArea.Height))
                {
                    location.Y -= this.Size.Height + area.Height;
                }
            }

            if (this._useFading)
            {
                this.FadeIn();
            }

            this.Show(location, ToolStripDropDownDirection.BelowRight);
        }

        #endregion

        #region Methods

        /// <summary>Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Closed"/> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripDropDownClosedEventArgs"/> that contains the event data.</param>
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            if (this._ownerPopup != null)
            {
                this._ownerPopup._resizable = true;
            }

            base.OnClosed(e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opened"/> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (this._ownerPopup != null)
            {
                this._ownerPopup.Resizable = false;
            }

            if (this.focusOnOpen)
            {
                this._content.Focus();
            }

            base.OnOpened(e);
        }

        /// <summary>Raises the <see cref="ToolStripDropDown.Opening"/> event.</summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected override void OnOpening(CancelEventArgs e)
        {
            if (this._content == null || this._content.IsDisposed || this._content.Disposing)
            {
                e.Cancel = true;
                return;
            }

            this.UpdateRegion();
            base.OnOpening(e);
        }

        /// <summary>Raises the <see cref="System.Windows.Forms.Control.SizeChanged"/> event.</summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            this._content.MinimumSize = this.Size;
            this._content.MaximumSize = this.Size;
            this._content.Size = this.Size;
            this._content.Location = Point.Empty;
            base.OnSizeChanged(e);
        }

        /// <summary>Processes a dialog box key.</summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>true if the key was processed by the control; otherwise, false.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // TODO: it doesn't close popup in case AutoClose property set to false
            if (this._acceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Updates the pop-up region.
        /// </summary>
        protected void UpdateRegion()
        {
            if (this.Region != null)
            {
                this.Region.Dispose();
                this.Region = null;
            }

            if (this._content.Region != null)
            {
                this.Region = this._content.Region.Clone();
            }
        }

        /// <summary>Processes Windows messages.</summary>
        /// <param name="m">The Windows <see cref="Message"/> to process.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (this.InternalProcessResizing(ref m))
            {
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// The fade in.
        /// </summary>
        private void FadeIn()
        {
            this._maxOpacity = this.Opacity; // remember original opacity
            this.Opacity = 0;
            this._fadingDirection = 1;
            this._fadingTimer.Enabled = true;
        }

        /// <summary>
        /// The fade out.
        /// </summary>
        private void FadeOut()
        {
            this._maxOpacity = this.Opacity; // remember original opacity
            this._fadingDirection = -1;
            this._fadingTimer.Enabled = true;
        }

        /// <summary>The internal process resizing.</summary>
        /// <param name="m">The m.</param>
        /// <returns>The System.Boolean.</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCACTIVATE && m.WParam != IntPtr.Zero && this._childPopup != null
                && this._childPopup.Visible)
            {
                this._childPopup.Hide();
            }

            return false;
        }

        /// <summary>The on fading timer.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnFadingTimer(object sender, EventArgs e)
        {
            var fadingStep = (this._maxOpacity * _fadingInterval) / this._fadingDuration;

            // use temporay variable to make sure opacity is inside defined boundaries
            var tmp = this.Opacity + fadingStep * this._fadingDirection;
            if (tmp < 0)
            {
                tmp = 0;
            }

            if (tmp > this._maxOpacity)
            {
                tmp = this._maxOpacity;
            }

            this.Opacity = tmp;

            if (this.Opacity >= this._maxOpacity && this._fadingDirection == 1)
            {
                this._fadingTimer.Enabled = false;
            }
            else if (this.Opacity <= 0 && this._fadingDirection == -1)
            {
                this.Close();
                this._fadingTimer.Enabled = false;
                this.Opacity = this._maxOpacity;
            }
        }

        /// <summary>The set owner item.</summary>
        /// <param name="control">The control.</param>
        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control is Popup)
            {
                var popupControl = control as Popup;
                this._ownerPopup = popupControl;
                this._ownerPopup._childPopup = this;
                this.OwnerItem = popupControl.Items[0];
            }
            else if (control.Parent != null)
            {
                // reqursion to nearest ToolStripDropDown
                this.SetOwnerItem(control.Parent);
            }
        }

        #endregion
    }
}