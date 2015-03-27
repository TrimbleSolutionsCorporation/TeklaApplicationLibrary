namespace Tekla.Structures
{
    using System.Drawing;

    using Microsoft.Win32;

    /// <summary>
    /// Provides access to application registry keys and values.
    /// </summary>
    public interface IRegistry
    {
        #region Public Properties

        /// <summary>
        /// Gets the registry key for current application version.
        /// </summary>
        /// <value>
        /// Registry key.
        /// </value>
        RegistryKey CurrentVersion { get; }

        /// <summary>
        /// Gets the application root registry key.
        /// </summary>
        /// <value>
        /// Registry key.
        /// </value>
        RegistryKey Root { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets the registry key for the specified version.</summary>
        /// <param name="version">Version string.</param>
        /// <returns>Registry key.</returns>
        RegistryKey GetVersion(string version);

        /// <summary>Loads dialog bounds rectangle from registry.</summary>
        /// <param name="dialogName">Dialog name.</param>
        /// <returns>Loaded dialog bounds or <see cref="Rectangle.Empty"/> if not available.</returns>
        Rectangle LoadDialogBounds(string dialogName);

        /// <summary>Loads dialog bounds rectangle from registry.</summary>
        /// <param name="dialogName">Dialog name.</param>
        /// <param name="version">Version string.</param>
        /// <returns>Loaded dialog bounds or <see cref="Rectangle.Empty"/> if not available.</returns>
        Rectangle LoadDialogBounds(string dialogName, string version);

        /// <summary>Saves dialog bounds rectangle to registry.</summary>
        /// <param name="dialogName">Dialog name.</param>
        /// <param name="bounds">Bounds rectangle.</param>
        void SaveDialogBounds(string dialogName, Rectangle bounds);

        /// <summary>Saves dialog bounds rectangle to registry.</summary>
        /// <param name="dialogName">Dialog name.</param>
        /// <param name="version">Version string.</param>
        /// <param name="bounds">Bounds rectangle.</param>
        void SaveDialogBounds(string dialogName, string version, Rectangle bounds);

        #endregion
    }
}