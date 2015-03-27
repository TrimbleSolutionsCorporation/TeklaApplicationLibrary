namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The hover tracker tests.
    /// </summary>
    [TestFixture]
    public class HoverTrackerTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The auto stop on click test.
        /// </summary>
        [Test]
        public void AutoStopOnClickTest()
        {
            var tracker = new HoverTracker();

            var ctrl = new Button();
            var wnd = new Popup(new Button());

            tracker.Start(ctrl, wnd);
            Assert.IsTrue(tracker.Tracking);
            ctrl.PerformClick();
            Thread.Sleep(200);
            Application.DoEvents(); // force timer event processing
            Assert.IsFalse(tracker.Tracking);
            Assert.IsNull(tracker.Control);
            Assert.IsNull(tracker.Popup);
        }

        /// <summary>
        /// The auto stop on control disposed test.
        /// </summary>
        [Test]
        public void AutoStopOnControlDisposedTest()
        {
            var tracker = new HoverTracker();

            var ctrl = new Button();
            var wnd = new Popup(new Button());

            tracker.Start(ctrl, wnd);
            Assert.IsTrue(tracker.Tracking);
            ctrl.Dispose();
            Thread.Sleep(200);
            Application.DoEvents(); // force timer event processing
            Assert.IsFalse(tracker.Tracking);
            Assert.IsNull(tracker.Control);
            Assert.IsNull(tracker.Popup);
        }

        /// <summary>
        /// The common tracking test.
        /// </summary>
        [Test]
        public void CommonTrackingTest()
        {
            var tracker = new HoverTracker();

            var ctrl = new Button();
            var wnd = new Popup(new Button());

            Assert.IsFalse(tracker.Tracking);
            tracker.Start(ctrl, wnd);
            Assert.IsTrue(tracker.Tracking);
            Assert.AreEqual(ctrl, tracker.Control);
            Assert.AreEqual(wnd, tracker.Popup);
            tracker.Stop();
            Assert.IsFalse(tracker.Tracking);
            Assert.IsNull(tracker.Control);
            Assert.IsNull(tracker.Popup);
        }

        /// <summary>
        /// The constructor test.
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            var tracker = new HoverTracker();

            // test default values
            Assert.IsFalse(tracker.Tracking);
            Assert.AreEqual(100, tracker.CheckInterval);
            Assert.IsNull(tracker.Control);
            Assert.IsNull(tracker.Popup);
        }

        /// <summary>
        /// The dispose test.
        /// </summary>
        [Test]
        public void DisposeTest()
        {
            HoverTracker tracker;
            using (tracker = new HoverTracker())
            {
                var ctrl = new Button();
                var wnd = new Popup(new Button());

                tracker.Start(ctrl, wnd);
                Assert.IsTrue(tracker.Tracking);
                Assert.AreEqual(ctrl, tracker.Control);
                Assert.AreEqual(wnd, tracker.Popup);
            }

            Assert.IsFalse(tracker.Tracking);
            Assert.IsNull(tracker.Control);
            Assert.IsNull(tracker.Popup);
        }

        /// <summary>
        /// The fire tracking stopped on restart test.
        /// </summary>
        [Test]
        public void FireTrackingStoppedOnRestartTest()
        {
            var tracker = new HoverTracker();

            var ctrl = new Button();
            var wnd = new Popup(new Button());
            var eventCatcher = new EventCatcher();
            tracker.TrackingStopped += eventCatcher.OnTrackingStopped;

            tracker.Start(ctrl, wnd);
            Assert.AreEqual(0, eventCatcher.Fired);
            tracker.Start(ctrl, wnd);
            Assert.AreEqual(1, eventCatcher.Fired);
            Assert.IsTrue(tracker.Tracking);
        }

        /// <summary>
        /// The fire tracking stopped on stop test.
        /// </summary>
        [Test]
        public void FireTrackingStoppedOnStopTest()
        {
            var tracker = new HoverTracker();

            var ctrl = new Button();
            var wnd = new Popup(new Button());
            var eventCatcher = new EventCatcher();
            tracker.TrackingStopped += eventCatcher.OnTrackingStopped;

            tracker.Start(ctrl, wnd);
            Assert.AreEqual(0, eventCatcher.Fired);
            tracker.Stop();
            Assert.AreEqual(1, eventCatcher.Fired);
            Assert.IsNotNull(eventCatcher.Ev);
            Assert.AreEqual(ctrl, eventCatcher.Ev.Control);
            Assert.AreEqual(wnd, eventCatcher.Ev.Popup);
        }

        /// <summary>
        /// The start wrong arg 2 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartWrongArg2Test()
        {
            var tracker = new HoverTracker();

            tracker.Start(new Button(), null);
        }

        /// <summary>
        /// The start wrong arg test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartWrongArgTest()
        {
            var tracker = new HoverTracker();

            tracker.Start(null, null);
        }

        /// <summary>
        /// The stop wrong state test.
        /// </summary>
        [Test]
        public void StopWrongStateTest()
        {
            var tracker = new HoverTracker();

            Assert.IsFalse(tracker.Tracking);
            tracker.Stop();
            Assert.IsFalse(tracker.Tracking);
        }

        #endregion

        /// <summary>
        /// The event catcher.
        /// </summary>
        private class EventCatcher
        {
            #region Constructor

            /// <summary>Initializes a new instance of the <see cref="EventCatcher"/> class.</summary>
            public EventCatcher()
            {
                this.Fired = 0;
            }

            #endregion

            #region Fields

            /// <summary>
            /// Gets or sets the ev value.
            /// </summary>
            public HoverTracker.TrackerEventArgs Ev { get; set; }

            /// <summary>
            /// Gets or sets the fired value.
            /// </summary>
            public int Fired { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>The on tracking stopped.</summary>
            /// <param name="sender">The sender value.</param>
            /// <param name="e">The e value.</param>
            public void OnTrackingStopped(object sender, HoverTracker.TrackerEventArgs e)
            {
                this.Fired++;
                this.Ev = e;
            }

            #endregion
        }
    }
}