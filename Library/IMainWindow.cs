namespace Tekla.Structures
{
    using System.Windows.Forms;

    /// <summary>
    /// Tekla Structures main window interface.
    /// </summary>
    public interface IMainWindow : IWin32Window
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the main window is active.
        /// </summary>
        /// <value>
        /// Indicates whether the main window is active.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets a value indicating whether the main window is minimized.
        /// </summary>
        /// <value>
        /// Indicates whether the main window is minimized.
        /// </value>
        bool IsMinimized { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Activates the main window.
        /// </summary>
        void Activate();

        /// <summary>Attaches a child form.</summary>
        /// <param name="form">Child form.</param>
        /// <remarks>Each child form must have unique name.</remarks>
        void AttachChildForm(Form form);

        /// <summary>Detaches a child form.</summary>
        /// <param name="form">Child form.</param>
        /// <remarks>Each child form must have unique name.</remarks>
        void DetachChildForm(Form form);

        #endregion
    }
}