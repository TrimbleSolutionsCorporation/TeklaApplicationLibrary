namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Selection interface.
    /// </summary>
    public interface ISelection
    {
        #region Public Events

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        event EventHandler SelectionChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets all objects.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        IEnumerable<object> AllObjects { get; }

        /// <summary>
        /// Gets the selected objects.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        IEnumerable<object> SelectedObjects { get; }

        #endregion
    }
}