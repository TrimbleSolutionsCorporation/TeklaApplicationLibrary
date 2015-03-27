namespace Tekla.Structures
{
    /// <summary>
    /// Connection interface.
    /// </summary>
    public interface IConnection : ITransaction, IRunMacro, IPicker, ISelection, ISelectObject
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the object is active.
        /// </summary>
        /// <value>
        /// Indicates whether the object is active.
        /// </value>
        bool IsActive { get; }

        #endregion
    }
}