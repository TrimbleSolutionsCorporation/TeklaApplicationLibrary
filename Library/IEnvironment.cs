namespace Tekla.Structures
{
    using System.Collections.Generic;
    using System.Globalization;

    using Tekla.Structures.Dialog;

    /// <summary>
    /// Application environment interface.
    /// </summary>
    public interface IEnvironment
    {
        #region Public Properties

        /// <summary>
        /// Gets the cloning template model folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        IEnumerable<string> CloningTemplateModelFolders { get; }

        /// <summary>
        /// Gets the company folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        IEnumerable<string> CompanyFolders { get; }

        /// <summary>
        /// Gets the application culture info.
        /// </summary>
        /// <value>
        /// Application culture info.
        /// </value>
        CultureInfo CultureInfo { get; }

        /// <summary>
        /// Gets the drawing macros.
        /// </summary>
        /// <value>
        /// Enumerable collection of drawing macro names.
        /// </value>
        IEnumerable<string> DrawingMacros { get; }

        /// <summary>
        /// Gets the application language.
        /// </summary>
        /// <value>
        /// Application language.
        /// </value>
        string Language { get; }

        /// <summary>
        /// Gets the application localization source.
        /// </summary>
        /// <value>
        /// Application localization source.
        /// </value>
        Localization Localization { get; }

        /// <summary>
        /// Gets the macros folder.
        /// </summary>
        /// <value>
        /// Macros folder path.
        /// </value>
        string MacrosFolder { get; }

        /// <summary>
        /// Gets the model macros.
        /// </summary>
        /// <value>
        /// Enumerable collection of model macro names.
        /// </value>
        IEnumerable<string> ModelMacros { get; }

        /// <summary>
        /// Gets the values and default switches of option type user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute name, values and default switches which UDAType is option.
        /// </value>
        IEnumerable<Dictionary<string, string>> OptionTypeUDAIndexAndValue { get; }

        /// <summary>
        /// Gets the project folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        IEnumerable<string> ProjectFolders { get; }

        /// <summary>
        /// Gets the search path.
        /// </summary>
        /// <value>
        /// Search path.
        /// </value>
        string SearchPath { get; }

        /// <summary>
        /// Gets the system folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        IEnumerable<string> SystemFolders { get; }

        /// <summary>
        /// Gets a value indicating whether US imperial units are used in input fields.
        /// </summary>
        /// <value>
        /// Indicates whether US imperial units are used in input fields.
        /// </value>
        bool UseUSImperialUnitsInInput { get; }

        /// <summary>
        /// Gets the user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute names which UDAType is string.
        /// </value>
        IEnumerable<string> UserDefinedAttributes { get; }

        /// <summary>
        /// Gets the option type user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute names which UDAType is option.
        /// </value>
        IEnumerable<string> UserDefinedAttributesOptionType { get; }

        #endregion

        #region Public Indexers

        /// <summary>Gets an environment variable.</summary>
        /// <param name="variableName">Variable name.</param>
        /// <value>Environment variable value.</value>
        /// <returns>The System.String.</returns>
        string this[string variableName] { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Loads the specified localization file.</summary>
        /// <param name="fileName">File name.</param>
        void LoadLocalizationFile(string fileName);

        #endregion
    }
}