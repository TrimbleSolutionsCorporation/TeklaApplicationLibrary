namespace Tekla.UI.HyperToolTips
{
    using System.Threading;
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The hyper tool tip popup tests.
    /// </summary>
    [TestFixture]
    public class HyperToolTipPopupTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The constructor test.
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            var thread = new Thread(this.ConstructorTestI);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        /// <summary>
        /// The show test.
        /// </summary>
        [Test]
        public void ShowTest()
        {
            var thread = new Thread(this.ShowTestI);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The constructor test i.
        /// </summary>
        private void ConstructorTestI()
        {
            var popup = new HyperToolTipPopup();

            // test default values
            Assert.IsInstanceOf(typeof(WebBrowser), popup.Content);
            Assert.AreEqual(DockStyle.Fill, popup.Content.Dock);
            Assert.AreEqual(0.95, popup.Opacity);
            Assert.IsFalse((popup.Content as WebBrowser).ScrollBarsEnabled);
            Assert.IsFalse((popup.Content as WebBrowser).IsWebBrowserContextMenuEnabled);
        }

        /// <summary>
        /// The show test i.
        /// </summary>
        private void ShowTestI()
        {
            Control parent = new Label();

            var popup = new HyperToolTipPopup();

            popup.Show(parent);
        }

        #endregion
    }
}