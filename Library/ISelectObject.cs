namespace Tekla.Structures
{
    /// <summary>
    /// Object selection interface.
    /// </summary>
    public interface ISelectObject
    {
        #region Public Methods and Operators

        /// <summary>
        /// Selects object by identifier.
        /// </summary>
        /// <param name="identifier">
        /// Object identifier.
        /// </param>
        /// <returns>
        /// Selected object or null if no matching object was found.
        /// </returns>
        object SelectObjectByIdentifier(Identifier identifier);

        #endregion
    }
}