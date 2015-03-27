namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    using TSDrawing = Tekla.Structures.Drawing.Drawing;

    /// <summary>
    /// Drawing connection interface.
    /// </summary>
    public interface IDrawing : IConnection
    {
        #region Public Events

        /// <summary>
        /// Announces that a drawing has been loaded.
        /// </summary>
        event EventHandler DrawingLoaded;

        /// <summary>
        /// Announces that the drawing editor has been closed.
        /// </summary>
        event EventHandler EditorClosed;

        /// <summary>
        /// Announces that the drawing editor has been opened.
        /// </summary>
        event EventHandler EditorOpened;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current drawing.
        /// </summary>
        /// <value>Current drawing.</value>
        TSDrawing Current { get; }

        /// <summary>
        /// Gets the drawings.
        /// </summary>
        /// <value>
        /// Enumerable collection of drawings.
        /// </value>
        ICollection<TSDrawing> Drawings { get; }

        /// <summary>
        /// Gets a value indicating whether the drawing editor is open.
        /// </summary>
        /// <value>
        /// Indicates whether the drawing editor is open.
        /// </value>
        bool IsEditorOpen { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Closes the drawing editor.
        /// </summary>
        /// <returns>A boolean value indicating whether the drawing editor was closed.</returns>
        bool Close();

        /// <summary>Closes the drawing editor.</summary>
        /// <param name="saveBeforeClosing">Indicates whether the drawing is saved before closing.</param>
        /// <returns>A boolean value indicating whether the drawing editor was closed.</returns>
        bool Close(bool saveBeforeClosing);

        /// <summary>Opens a drawing in the drawing editor.</summary>
        /// <param name="drawing">Drawing to open.</param>
        /// <returns>A boolean value indicating whether the drawing was opened.</returns>
        bool Open(TSDrawing drawing);

        /// <summary>
        /// Saves the current drawing.
        /// </summary>
        /// <returns>A boolean value indicating whether the drawing was saved.</returns>
        bool Save();

        #endregion
    }
}