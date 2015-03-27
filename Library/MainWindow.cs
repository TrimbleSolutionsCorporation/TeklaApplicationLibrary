namespace Tekla.Structures
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Windows.Forms;

    using TSMainWindow = Tekla.Structures.Dialog.MainWindow;

    /// <summary>
    ///     Tekla Structures main window interface.
    /// </summary>
    internal class MainWindow : IMainWindow
    {
        #region Public Properties

        /// <summary>
        ///     Gets the main window handle.
        /// </summary>
        /// <value> Main window handle.</value>
        /// <summary>
        ///     Gets the main window handle.
        /// </summary>
        /// <value> Main window handle.</value>
        public IntPtr Handle
        {
            get
            {
                if (TSMainWindow.Frame != null)
                {
                    return TSMainWindow.Frame.Handle;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the main window is active.
        /// </summary>
        /// <value> Indicates whether the main window is active.</value>
        public bool IsActive
        {
            get
            {
                return TSMainWindow.Frame.Handle == GetForegroundWindow();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the main window is minimized.
        /// </summary>
        /// <value> Indicates whether the main window is minimized.</value>
        public bool IsMinimized
        {
            get
            {
                return IsIconic(TSMainWindow.Frame.Handle);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Activates the main window.
        /// </summary>
        public void Activate()
        {
            SetForegroundWindow(TSMainWindow.Frame.Handle);
        }

        /// <summary>
        /// Attaches a child form.
        /// </summary>
        /// <param name="form">
        /// Child form. 
        /// </param>
        /// <remarks>
        /// Each child form must have unique name.
        /// </remarks>
        public void AttachChildForm(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            try
            {
                TSMainWindow.Frame.AddExternalWindow(form.Name, form.Handle);
            }
            catch (RemotingException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Detaches a child form.
        /// </summary>
        /// <param name="form">
        /// Child form. 
        /// </param>
        /// <remarks>
        /// Each child form must have unique name.
        /// </remarks>
        public void DetachChildForm(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            try
            {
                TSMainWindow.Frame.RemoveExternalWindow(form.Name, form.Handle);
            }
            catch (RemotingException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the foreground window handle.
        /// </summary>
        /// <returns> Foreground window handle.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Determinws whether the specified window is minimized.
        /// </summary>
        /// <param name="windowHandle">
        /// Window handle. 
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the window is minimized. 
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr windowHandle);

        /// <summary>
        /// Gets the foreground window handle.
        /// </summary>
        /// <param name="windowHandle">
        /// Foreground window handle. 
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the operation was successful. 
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr windowHandle);

        #endregion
    }
}