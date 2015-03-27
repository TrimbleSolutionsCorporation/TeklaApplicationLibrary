namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Helper class to track mouse movements and to hide a tip window when mouse goes out of baundaries of the control.
    /// Instance of this class is not responsible for hiding hyperTooltip window it just tracks mouse and fire "Hoverout" event
    /// when mouse pointer goes put of boundaries.
    /// Instance of this class typically reused for all tip popups on the form because tip popups are shown one after another (not semultenously)
    /// that is why control and popup to track is not passed in the constructor, <see cref="Start"/> method should be called to start actual tracking.
    /// </summary>
    internal class HoverTracker : IDisposable
    {
        // Track whether Dispose has been called.
        #region Fields

        /// <summary>
        /// The _check interval.
        /// </summary>
        private int _checkInterval = 100; // times interval in ms to check hoverout

        /// <summary>
        /// The _control.
        /// </summary>
        private object _control;

        /// <summary>
        /// The _timer track hoverout.
        /// </summary>
        private Timer _timerTrackHoverout;

        /// <summary>
        /// The _wnd.
        /// </summary>
        private Popup _wnd;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HoverTracker"/> class. 
        /// Contructs tracker instance.
        /// Instance of this class typically reused for all tip popups on the form because tip popups are shown one after another (not semultenously)
        /// that is why control and popup to track is not passed in the constructor, <see cref="Start"/> method should be called to start actual tracking.
        /// </summary>
        public HoverTracker()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HoverTracker"/> class. 
        /// </summary>
        ~HoverTracker()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Represents the method that will handle the <see cref="TrackingStopped"/> events of a <see cref="HoverTracker"/>.
        /// </summary>
        public delegate void TrackerEventHandler(object sender, TrackerEventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs right after tracking is stoped, because e.g. mouse goes out of boundaries of the tracked region.
        /// This event is fired also if tracking is cancelled or stopped by any other reason.
        /// Typically this event is used to hide tooltip popup window.
        /// </summary>
        public event TrackerEventHandler TrackingStopped;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets time interval in ms used to check mouse movements. Default is 100ms.
        /// </summary>
        /// <value>Time interval in ms used to check mouse movements</value>
        public int CheckInterval
        {
            get
            {
                return this._checkInterval;
            }

            set
            {
                this._checkInterval = value;
            }
        }

        /// <summary>
        /// Control (UI element) beeing tracked.
        /// </summary>
        /// <value>Control (UI element) beeing tracked. <code>null</code> if nothing tracked currently.</value>
        public object Control
        {
            get
            {
                return this._control;
            }
        }

        /// <summary>
        /// <seealso cref="Popup"/> window beeing tracked.
        /// </summary>
        /// <value><seealso cref="Popup"/> window beeing tracked. <code>null</code> if nothing tracked currently.</value>
        public Popup Popup
        {
            get
            {
                return this._wnd;
            }
        }

        /// <summary>
        /// Whether the instance currently tracking mouse movements or not?
        /// </summary>
        /// <value><b>true</b> if mouse movements are tracking currently</value>
        public bool Tracking
        {
            get
            {
                return this._timerTrackHoverout != null && this._timerTrackHoverout.Enabled;
            }
        }

        #endregion

        #region Public Methods and Operators

        ///<summary>
        /// Implements IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        ///</summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>Starts tracking for mouse movements.
        /// When mouse pointer goes out of both control and tip window boundaries then <see cref="TrackingStopped"/> event is fired.
        /// If this instance of tracker is currently tracking something else then current tracker is stoped (<seealso cref="TrackingStopped"/>
        /// fired) and new one is started.</summary>
        /// <param name="control">UI element/control for which tooltip is displayed</param>
        /// <param name="hyperWnd">HyperTooltip window</param>
        /// <exception cref="ArgumentNullException">control or hyperWnd arguments is <b>null</b></exception>
        public virtual void Start(object control, Popup hyperWnd)
        {
            if (null == control)
            {
                throw new ArgumentNullException("control");
            }

            if (null == hyperWnd)
            {
                throw new ArgumentNullException("hyperWnd");
            }

            if (this.Tracking)
            {
                this.Stop();
            }

            this._control = control;
            this._wnd = hyperWnd;

            if (this._timerTrackHoverout == null)
            {
                // create on demand
                this._timerTrackHoverout = new Timer();
                this._timerTrackHoverout.Tick += new EventHandler(this.TimerHandler);
                this._timerTrackHoverout.Interval = this.CheckInterval;
            }

            this._timerTrackHoverout.Start();
        }

        /// <summary>
        /// Stops tracking (don't destroy/hide tracked popup), do nothing if nothing is tracked currently.
        /// Fire <seealso cref="TrackingStopped"/> event.
        /// </summary>
        public virtual void Stop()
        {
            if (!this.Tracking)
            {
                return;
            }

            if (this._timerTrackHoverout != null)
            {
                this._timerTrackHoverout.Stop();
            }

            var e = new TrackerEventArgs(this._wnd, this._control);

            // have to clear internal state BEFORE firing event, because event handler can and will reuse this instance of tracker
            this._control = null;
            this._wnd = null;
            this.OnTrackingStopped(e);

            // don't touch internal state AFTER event firing, because event handler might alter it already!
        }

        #endregion

        #region Methods

        /// <summary>Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.</summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            // this class's instance may be reused after disposing,
            // so don't check whether Dispose ha already been called
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.Stop();
                }

                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.
            }

            this.disposed = true;
        }

        /// <summary>Raises the <see cref="TrackingStopped"/> event.</summary>
        /// <param name="e">The data for the event.</param>
        protected virtual void OnTrackingStopped(TrackerEventArgs e)
        {
            if (this.TrackingStopped != null)
            {
                this.TrackingStopped(this, e);
            }
        }

        /// <summary>
        /// The check for hoverout.
        /// </summary>
        private void CheckForHoverout()
        {
            var mousePos = Cursor.Position;

            Rectangle ctrlRectScreen;
            if (this._control is Control)
            {
                if (this._control is TabPage)
                {
                    var tabCtrl = (this._control as TabPage).Parent as TabControl;
                    var rect = new Rectangle();
                    for (var i = 0; i < tabCtrl.TabCount; i++)
                    {
                        if (tabCtrl.TabPages[i] == this._control)
                        {
                            rect = tabCtrl.GetTabRect(i);
                        }
                    }

                    ctrlRectScreen = tabCtrl.RectangleToScreen(rect);
                }
                else
                {
                    var control = this._control as Control;
                    ctrlRectScreen = control.Parent.RectangleToScreen(control.Bounds);
                }
            }
            else
            {
                Debug.Assert(this._control is ToolStripItem);
                var control = this._control as ToolStripItem;
                ctrlRectScreen = control.Owner.RectangleToScreen(control.Bounds);
            }

            // Find a rectangle that fill the hole between contol and the tip window
            // These calculations depend on ToolStripDropDownDirection.BelowRight flag used in HyperToolTipPopup.Show() method
            var hole = new Rectangle(
                this._wnd.Bounds.Left, 
                ctrlRectScreen.Bottom, 
                ctrlRectScreen.Right - this._wnd.Bounds.Left, 
                this._wnd.Bounds.Top - ctrlRectScreen.Bottom);
            if (!(ctrlRectScreen.Contains(mousePos) || this._wnd.Bounds.Contains(mousePos) || hole.Contains(mousePos)))
            {
                this.Stop();
            }
        }

        /// <summary>The timer handler.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void TimerHandler(object sender, EventArgs args)
        {
            var ctrl = this._control as Control;
            if (ctrl == null)
            {
                Debug.Assert(
                    this._control is ToolStripItem, 
                    "unknown UI element type, should be consistent with HyperToolTipProvider.CanExtend()");
                ctrl = (this._control as ToolStripItem).Owner;
            }

            if (ctrl is TabPage)
            {
                ctrl = (this._control as TabPage).Parent;
            }

            // the handler can be potentially called when UI element already disposed, so have to check it
            // note that TabPages may be not initialized yet, so have to check their parent TabControl
            // TODO: how to check it properly, do we have a leak of timers?
            if (null != ctrl && ctrl.Created && ctrl.IsHandleCreated && !ctrl.IsDisposed)
            {
                if (ctrl.InvokeRequired)
                {
                    ctrl.Invoke(new MethodInvoker(this.CheckForHoverout));
                }
                else
                {
                    // we are on the right thread, we can call directly
                    this.CheckForHoverout();
                }
            }
            else
            {
                // cleanup since control doesn't exist any more
                this.Stop();
            }
        }

        #endregion

        /// <summary>
        /// Provides data for the <see cref="TrackingStopped"/> event. 
        /// </summary>
        public class TrackerEventArgs : EventArgs
        {
            #region Fields

            /// <summary>
            /// The _control.
            /// </summary>
            private readonly object _control;

            /// <summary>
            /// The _wnd.
            /// </summary>
            private readonly Popup _wnd;

            #endregion

            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="TrackerEventArgs"/> class. 
            /// Creates a new instance of a <see cref="TrackerEventArgs"/> object.</summary>
            /// <param name="wnd">The <see cref="Popup"/> to be shown.</param>
            /// <param name="control">The UI element for which the HyperToolTip was displayed.</param>
            public TrackerEventArgs(Popup wnd, object control)
            {
                this._wnd = wnd;
                this._control = control;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the UI element/control for which the tip is displayed.
            /// </summary>
            /// <value>The Object for which the tip is displayed. For example, on a toolbar control, this would be an instance of a toolbar button object.</value>
            public object Control
            {
                get
                {
                    return this._control;
                }
            }

            /// <summary>
            /// Gets the <see cref="Popup"/> was tracked.
            /// </summary>
            /// <value>The <see cref="Popup"/> to be shown.</value>
            /// <remarks>You can modify properties of the window to alter the tip before it is shown.</remarks>
            public Popup Popup
            {
                get
                {
                    return this._wnd;
                }
            }

            #endregion
        }
    }
}