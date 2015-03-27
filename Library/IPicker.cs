namespace Tekla.Structures
{
    /// <summary>
    /// Picker interface.
    /// </summary>
    public interface IPicker
    {
        #region Public Methods and Operators

        /// <summary>Picks an object.</summary>
        /// <param name="prompt">Pick prompt.</param>
        /// <returns>Picked object or null if no object was picked.</returns>
        object PickObject(string prompt);

        #endregion
    }
}