namespace Tekla.UI.HyperToolTips
{
    partial class Popup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SetOwnerItem(null);
                if (components != null)
                {
                    components.Dispose();
                }
                if (_content != null)
                {
                    System.Windows.Forms.Control ctrl = _content;
                    _content = null;
                    ctrl.Dispose();
                }
                if (_fadingTimer != null)
                {
                    _fadingTimer.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
