namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Transaction interface.
    /// </summary>
    public interface ITransaction
    {
        #region Public Events

        /// <summary>
        /// Announces that the changes have been committed.
        /// </summary>
        event EventHandler ChangesCommitted;

        /// <summary>
        /// Announces that the changes have been discarded.
        /// </summary>
        event EventHandler ChangesDiscarded;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Commits the changes made to the objects.
        /// </summary>
        /// <param name="objects">
        /// Enumerable collection of modified objects.
        /// </param>
        void CommitChanges(IEnumerable<object> objects);

        /// <summary>
        /// Discards the changed made to the objects.
        /// </summary>
        /// <param name="objects">
        /// Enumerable collection of modified objects.
        /// </param>
        void DiscardChanges(IEnumerable<object> objects);

        #endregion
    }
}