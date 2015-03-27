namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// The drawing control.
    /// </summary>
    public class DrawingControl
    {
        #region Constants

        /// <summary>
        /// The w m_ setredraw.
        /// </summary>
        private const int WmSetRedraw = 11;

        #endregion

        #region Public Methods and Operators

        /// <summary>The resume drawing.</summary>
        /// <param name="parent">The parent.</param>
        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WmSetRedraw, true, 0);
            parent.Refresh();
        }

        /// <summary>The send message.</summary>
        /// <param name="windowHandle">The h wnd.</param>
        /// <param name="windowMessage">The w msg.</param>
        /// <param name="windowParameter">The w param.</param>
        /// <param name="parameterLong">The l param.</param>
        /// <returns>The System.Int32.</returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr windowHandle, int windowMessage, bool windowParameter, int parameterLong);

        /// <summary>The suspend drawing.</summary>
        /// <param name="parent">The parent.</param>
        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WmSetRedraw, false, 0);
        }

        #endregion
    }
}