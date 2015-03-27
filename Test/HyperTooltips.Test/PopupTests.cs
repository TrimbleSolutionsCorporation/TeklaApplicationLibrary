namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The popup tests.
    /// </summary>
    [TestFixture]
    public class PopupTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The constructor args test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorArgsTest()
        {
            var popup = new Popup(null);
        }

        /// <summary>
        /// The constructor test.
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            using (Control content = new Button())
            {
                var popup = new Popup(content);

                // test default values
                Assert.AreSame(content, popup.Content);
                Assert.IsFalse(popup.AutoSize);
                Assert.AreEqual(1, popup.Items.Count);
                Assert.IsInstanceOf(typeof(ToolStripControlHost), popup.Items[0]);
                Assert.AreEqual(Padding.Empty, popup.Padding);
                Assert.AreEqual(Padding.Empty, popup.Margin);
                Assert.AreEqual(
                    SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled, popup.UseFading);
            }
        }

        /// <summary>
        /// The content disposed on popup dispose test.
        /// </summary>
        [Test]
        public void ContentDisposedOnPopupDisposeTest()
        {
            Control content = new Button();
            using (var popup = new Popup(content))
            {
            }

            Assert.IsTrue(content.IsDisposed);
        }

        /// <summary>
        /// The popup disposed on content disposed test.
        /// </summary>
        [Test]
        public void PopupDisposedOnContentDisposedTest()
        {
            Popup popup = null;
            using (Control content = new Button())
            {
                popup = new Popup(content);
            }

            Assert.IsTrue(popup.IsDisposed);
        }

        /// <summary>
        /// The show 2 arg test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Show2ArgTest()
        {
            using (Control content = new Button())
            {
                var popup = new Popup(content);
                popup.Show(null, new Rectangle(0, 0, 1, 1));
            }
        }

        /// <summary>
        /// The show arg test.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShowArgTest()
        {
            using (Control content = new Button())
            {
                var popup = new Popup(content);
                popup.Show(null);
            }
        }

        /// <summary>
        /// The show test.
        /// </summary>
        [Test]
        public void ShowTest()
        {
            using (Control parent = new Label())
            {
                Control content = new Button();
                var popup = new Popup(content);

                popup.Show(parent);
            }
        }

        #endregion
    }
}