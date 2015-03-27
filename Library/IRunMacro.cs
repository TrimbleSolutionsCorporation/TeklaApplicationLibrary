namespace Tekla.Structures
{
    /// <summary>
    /// Macro execution interface.
    /// </summary>
    public interface IRunMacro
    {
        #region Public Methods and Operators

        /// <summary>Executes a macro.</summary>
        /// <param name="macroName">Macro name.</param>
        void RunMacro(string macroName);

        #endregion
    }
}