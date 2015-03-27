namespace Tekla.Structures
{
    /// <summary>
    /// Common tasks library interface.
    /// </summary>
    public interface ICommonTasks
    {
        #region Public Methods and Operators

        /// <summary>Creates a general arrangement drawing from template.</summary>
        /// <param name="name">Template name.</param>
        void CreateGeneralArrangementDrawingFromTemplate(string name);

        /// <summary>Opens assembly drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        void OpenAssemblyDrawingProperties(string name);

        /// <summary>Opens auto drawing script editor.</summary>
        /// <param name="name">Script name.</param>
        void OpenAutoDrawingScript(string name);

        /// <summary>Opens cast unit drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        void OpenCastUnitDrawingProperties(string name);

        /// <summary>
        /// Opens the drawing list.
        /// </summary>
        void OpenDrawingList();

        /// <summary>Opens general arrangement drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        void OpenGeneralArrangementDrawingProperties(string name);

        /// <summary>
        /// Opens the numbering settings dialog.
        /// </summary>
        void OpenNumberingSettings();

        /// <summary>Opens single part drawing properties dialog.</summary>
        /// <param name="name">Drawing name.</param>
        void OpenSinglePartDrawingProperties(string name);

        /// <summary>Performs numbering.</summary>
        /// <param name="fullNumbering">Indicates whether to perform full numbering for all parts.</param>
        void PerformNumbering(bool fullNumbering);

        #endregion
    }
}