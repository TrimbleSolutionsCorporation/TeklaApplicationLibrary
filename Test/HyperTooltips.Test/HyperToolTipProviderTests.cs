namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The hyper tool tip provider tests.
    /// </summary>
    [TestFixture]
    public class HyperToolTipProviderTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The can extend test.
        /// </summary>
        [Test]
        public void CanExtendTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            Assert.IsFalse(hyperToolTipProvider.CanExtend(null));
            Assert.IsTrue(hyperToolTipProvider.CanExtend(new Control()));
            Assert.IsFalse(hyperToolTipProvider.CanExtend(new TabControl()));
            Assert.IsFalse(hyperToolTipProvider.CanExtend(new Form()));
            Assert.IsFalse(hyperToolTipProvider.CanExtend(new ToolStrip()));
            Assert.IsTrue(hyperToolTipProvider.CanExtend(new ToolStripMenuItem()));
            Assert.IsTrue(hyperToolTipProvider.CanExtend(new ToolStripButton()));
            Assert.IsTrue(hyperToolTipProvider.CanExtend(new ToolStripComboBox()));
            Assert.IsFalse(hyperToolTipProvider.CanExtend("xxx"));
        }

        /// <summary>
        /// The constructor test.
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            // test default values
            Assert.IsTrue(hyperToolTipProvider.Active);
            Assert.IsFalse(hyperToolTipProvider.Debug);
            Assert.AreEqual(250, hyperToolTipProvider.FadingInterval);
            Assert.AreEqual(500, hyperToolTipProvider.InitialDelay);
            Assert.AreEqual(hyperToolTipProvider.InitialDelay / 5, hyperToolTipProvider.ReshowDelay);
            Assert.AreEqual(0.95, hyperToolTipProvider.Opacity);
            Assert.AreEqual(new Size(320, 205), hyperToolTipProvider.PopupSizeLarge);
            Assert.AreEqual(new Size(300, 145), hyperToolTipProvider.PopupSizeNormal);
            Assert.IsFalse(hyperToolTipProvider.ShowAlways);
            Assert.AreEqual(SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled, hyperToolTipProvider.UseFading);
        }

        /// <summary>
        /// The dispose test.
        /// </summary>
        [Test]
        public void DisposeTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var ctrl = new Button();
            hyperToolTipProvider.SetHyperToolTipId(ctrl, id);

            var id2 = "id2";
            var ctrl2 = new Button();
            hyperToolTipProvider.SetHyperToolTipId(ctrl2, id2);

            hyperToolTipProvider.Dispose();

            Assert.AreEqual(string.Empty, hyperToolTipProvider.GetHyperToolTipId(ctrl));
            Assert.AreEqual(string.Empty, hyperToolTipProvider.GetHyperToolTipId(ctrl2));
        }

        /// <summary>
        /// The get hyper tool tip id for not registered control test.
        /// </summary>
        [Test]
        public void GetHyperToolTipIdForNotRegisteredControlTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            Assert.AreEqual(string.Empty, hyperToolTipProvider.GetHyperToolTipId(new Button()));
        }

        /// <summary>
        /// The get hyper tool tip id wrong arg 1 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetHyperToolTipIdWrongArg1Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            hyperToolTipProvider.GetHyperToolTipId(null);
        }

        /// <summary>
        /// The get hyper tool tip id wrong arg 2 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetHyperToolTipIdWrongArg2Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            hyperToolTipProvider.GetHyperToolTipId(new TabControl());
        }

        /// <summary>
        /// The get hyper tool tip id wrong arg 3 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetHyperToolTipIdWrongArg3Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            hyperToolTipProvider.GetHyperToolTipId("xxx");
        }

        /// <summary>
        /// The remove control on disposed control test.
        /// </summary>
        [Test]
        public void RemoveControlOnDisposedControlTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var ctrl = new Button();
            hyperToolTipProvider.SetHyperToolTipId(ctrl, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(ctrl));
            ctrl.Dispose();
            Assert.AreEqual(
                string.Empty, hyperToolTipProvider.GetHyperToolTipId(ctrl), "control is not deregistered automatically");
        }

        /// <summary>
        /// The remove control on disposed tab control test.
        /// </summary>
        [Test]
        public void RemoveControlOnDisposedTabControlTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var tabCtrl = new TabControl();
            var page = new TabPage();
            tabCtrl.Controls.Add(page);

            hyperToolTipProvider.SetHyperToolTipId(page, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(page));
            tabCtrl.Dispose();
            Assert.AreEqual(
                string.Empty, hyperToolTipProvider.GetHyperToolTipId(page), "control is not deregistered automatically");
        }

        /// <summary>
        /// The remove control on disposed tool strip test.
        /// </summary>
        [Test]
        public void RemoveControlOnDisposedToolStripTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var toolStripButton1 = new ToolStripButton();
            var toolStrip1 = new ToolStrip();
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });

            hyperToolTipProvider.SetHyperToolTipId(toolStripButton1, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(toolStripButton1));
            toolStrip1.Dispose();
            Assert.AreEqual(
                string.Empty, 
                hyperToolTipProvider.GetHyperToolTipId(toolStripButton1), 
                "control is not deregistered automatically");
        }

        /// <summary>
        /// The set get hyper tool tip id control test.
        /// </summary>
        [Test]
        public void SetGetHyperToolTipIdControlTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var ctrl = new Button();
            hyperToolTipProvider.SetHyperToolTipId(ctrl, id);

            var id2 = "id2";
            var ctrl2 = new Button();
            hyperToolTipProvider.SetHyperToolTipId(ctrl2, id2);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(ctrl));
            Assert.AreEqual(id2, hyperToolTipProvider.GetHyperToolTipId(ctrl2));

            // try to override id
            var id3 = "id3";
            hyperToolTipProvider.SetHyperToolTipId(ctrl2, id3);
            Assert.AreEqual(id3, hyperToolTipProvider.GetHyperToolTipId(ctrl2));
        }

        /// <summary>
        /// The set get hyper tool tip id tab control 2 test.
        /// </summary>
        [Test]
        public void SetGetHyperToolTipIdTabControl2Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var page = new TabPage();

            // try to set id before page is added to TabControl
            hyperToolTipProvider.SetHyperToolTipId(page, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(page));
        }

        /// <summary>
        /// The set get hyper tool tip id tab control test.
        /// </summary>
        [Test]
        public void SetGetHyperToolTipIdTabControlTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var tabCtrl = new TabControl();
            var page = new TabPage();
            tabCtrl.Controls.Add(page);

            hyperToolTipProvider.SetHyperToolTipId(page, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(page));
        }

        /// <summary>
        /// The set get hyper tool tip id tool strip 2 test.
        /// </summary>
        [Test]
        public void SetGetHyperToolTipIdToolStrip2Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var toolStripButton1 = new ToolStripButton();

            // try to set id before button is added to ToolStrip
            hyperToolTipProvider.SetHyperToolTipId(toolStripButton1, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(toolStripButton1));
        }

        /// <summary>
        /// The set get hyper tool tip id tool strip test.
        /// </summary>
        [Test]
        public void SetGetHyperToolTipIdToolStripTest()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            var id = "id";
            var toolStripButton1 = new ToolStripButton();
            var toolStrip1 = new ToolStrip();
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });

            hyperToolTipProvider.SetHyperToolTipId(toolStripButton1, id);

            Assert.AreEqual(id, hyperToolTipProvider.GetHyperToolTipId(toolStripButton1));
        }

        /// <summary>
        /// The set hyper tool tip id wrong arg 1 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetHyperToolTipIdWrongArg1Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            hyperToolTipProvider.SetHyperToolTipId(null, null);
        }

        /// <summary>
        /// The set hyper tool tip id wrong arg 2 test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetHyperToolTipIdWrongArg2Test()
        {
            var hyperToolTipProvider = new HyperToolTipProvider();

            hyperToolTipProvider.SetHyperToolTipId(new TabControl(), null);
        }

        /// <summary>
        /// The turn off normal tooltips on provider activation 1 test.
        /// </summary>
        [Test]
        public void TurnOffNormalTooltipsOnProviderActivation1Test()
        {
            this.TurnOffNormalTooltipsOnProviderActivationHelper(true);
        }

        /// <summary>
        /// The turn off normal tooltips on provider activation 2 test.
        /// </summary>
        [Test]
        [Ignore]
        public void TurnOffNormalTooltipsOnProviderActivation2Test()
        {
            this.TurnOffNormalTooltipsOnProviderActivationHelper(false);
        }

        /// <summary>
        /// The turn off normal tooltips on set hyper tool tip id 1 test.
        /// </summary>
        [Test]
        public void TurnOffNormalTooltipsOnSetHyperToolTipId1Test()
        {
            this.TurnOffNormalTooltipsOnSetHyperToolTipIdHelper(true);
        }

        /// <summary>
        /// The turn off normal tooltips on set hyper tool tip id 2 test.
        /// </summary>
        [Test]
        [Ignore]
        public void TurnOffNormalTooltipsOnSetHyperToolTipId2Test()
        {
            this.TurnOffNormalTooltipsOnSetHyperToolTipIdHelper(false);
        }

        #endregion

        #region Methods

        /// <summary>The turn off normal tooltips on provider activation helper.</summary>
        /// <param name="originalValueShowItemToolTips">The original value show item tool tips.</param>
        private void TurnOffNormalTooltipsOnProviderActivationHelper(bool originalValueShowItemToolTips)
        {
            var hyperToolTipProvider = new HyperToolTipProvider();
            hyperToolTipProvider.Active = false;

            var toolStripButton1 = new ToolStripButton();

            var toolStrip1 = new ToolStrip();
            toolStrip1.ShowItemToolTips = originalValueShowItemToolTips;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });

            hyperToolTipProvider.SetHyperToolTipId(toolStripButton1, "id");
            Assert.AreEqual(originalValueShowItemToolTips, toolStrip1.ShowItemToolTips);
            hyperToolTipProvider.Active = true;
            Assert.IsFalse(toolStrip1.ShowItemToolTips); // turn off on activation
            hyperToolTipProvider.Active = false;
            Assert.AreEqual(originalValueShowItemToolTips, toolStrip1.ShowItemToolTips);
        }

        /// <summary>The turn off normal tooltips on set hyper tool tip id helper.</summary>
        /// <param name="originalValueShowItemToolTips">The original value show item tool tips.</param>
        private void TurnOffNormalTooltipsOnSetHyperToolTipIdHelper(bool originalValueShowItemToolTips)
        {
            var hyperToolTipProvider = new HyperToolTipProvider();
            hyperToolTipProvider.Active = true;

            var toolStripButton1 = new ToolStripButton();

            var toolStrip1 = new ToolStrip();
            toolStrip1.ShowItemToolTips = originalValueShowItemToolTips;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1 });

            hyperToolTipProvider.SetHyperToolTipId(toolStripButton1, "id");
            Assert.IsFalse(toolStrip1.ShowItemToolTips); // since provider is active normal tips should be turned off
            hyperToolTipProvider.Active = false;
            Assert.AreEqual(originalValueShowItemToolTips, toolStrip1.ShowItemToolTips);
            hyperToolTipProvider.Active = true;
            Assert.IsFalse(toolStrip1.ShowItemToolTips);
        }

        #endregion

        // class EventCatcher
        // {
        // public int fired = 0;
        // public HoverTracker.TrackerEventArgs ev;
        // public void OnTipPopup(object sender, HyperToolTipProvider.TipEventArgs e)
        // {
        // fired++;
        // ev = e;
        // }
        // }
        // [Test]
        // public void FireTipPopupTest()
        // {
        // using (HyperToolTipProvider hyperToolTipProvider = new HyperToolTipProvider())
        // {
        // EventCatcher eventTracker = new EventCatcher();
        // hyperToolTipProvider.InitialDelay = 0;    //to turn off timer before showing popup
        // hyperToolTipProvider.Debug = true;
        // hyperToolTipProvider.TipPopup += new HyperToolTipProvider.TipEventHandler(eventTracker.OnTipPopup);

        // string id = "id";
        // Button ctrl = new Button();
        // hyperToolTipProvider.SetHyperToolTipId(ctrl, id);

        // Assert.AreEqual(0, eventTracker.fired);
        // hyperToolTipProvider.ShowTip(ctrl);
        // Assert.AreEqual(1, eventTracker.fired);
        // Assert.IsNotNull(eventTracker.ev);
        // Assert.AreEqual(ctrl, eventTracker.ev.Control);
        // Assert.IsNotNull(eventTracker.ev.Popup);
        // }

        // }
    }
}