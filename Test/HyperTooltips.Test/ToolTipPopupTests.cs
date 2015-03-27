namespace Tekla.UI.HyperToolTips
{
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The tool tip popup tests.
    /// </summary>
    [TestFixture]
    public class ToolTipPopupTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The constructor test.
        /// </summary>
        [Test]
        public void ConstructorTest()
        {
            var text = "tip text";
            var popup = new ToolTipPopup(text);

            // test default values
            Assert.IsInstanceOf(typeof(Label), popup.Content);
            Assert.AreEqual(text, popup.Content.Text);
            Assert.IsFalse(popup.UseFading);
            Assert.AreEqual(1.0, popup.Opacity);
            Assert.IsFalse(popup.DropShadowEnabled);
        }

        /// <summary>
        /// The show test.
        /// </summary>
        [Test]
        public void ShowTest()
        {
            Control parent = new Label();

            var text = "tip text";
            var popup = new ToolTipPopup(text);

            popup.Show(parent);
        }

        #endregion
    }
}