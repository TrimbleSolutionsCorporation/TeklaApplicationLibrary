namespace Tekla.Structures
{
    using System.IO;

    /// <summary>
    /// Representation of a model folder structure.
    /// </summary>
    /// <remarks>
    /// ModelFolder represents the folder layout of single model.
    /// </remarks>
    public class ModelFolder : VirtualFolder
    {
        #region Static Fields

        /// <summary>
        /// Assembly standard file extension.
        /// </summary>
        public static readonly string AssemblyFileExtension = ".ad";

        /// <summary>
        /// Attributes folder name.
        /// </summary>
        public static readonly string AttributesFolderName = "attributes";

        /// <summary>
        /// Cast unit standard file extension.
        /// </summary>
        public static readonly string CastUnitFileExtension = ".cud";

        /// <summary>
        /// Model database file extension.
        /// </summary>
        public static readonly string DatabaseFileExtension = ".db1";

        /// <summary>
        /// Drawings folder name.
        /// </summary>
        public static readonly string DrawingsFolderName = "drawings";

        /// <summary>
        /// Extended ruleset file extension.
        /// </summary>
        public static readonly string ExtendedRulesetFileExtension = ".xdproc";

        /// <summary>
        /// GA standard file extension.
        /// </summary>
        public static readonly string GeneralArrangementFileExtension = ".gd";

        /// <summary>
        /// Object definition file.
        /// </summary>
        public static readonly string ObjectDefinitionFile = "objects.inp";

        /// <summary>
        /// Object settings file extension.
        /// </summary>
        public static readonly string ObjectSettingsFileExtension = ".objectSettings";

        /// <summary>
        /// Ruleset file extension.
        /// </summary>
        public static readonly string RulesetFileExtension = ".dproc";

        /// <summary>
        /// Select filter file extension.
        /// </summary>
        public static readonly string SelectFilterFileExtension = ".SObjGrp";

        /// <summary>
        /// Single part standard file extension.
        /// </summary>
        public static readonly string SinglePartFileExtension = ".wd";

        /// <summary>
        /// View filter file extension.
        /// </summary>
        public static readonly string ViewFilterFileExtension = ".vf";

        #endregion

        #region Fields

        /// <summary>
        /// Attributes folder.
        /// </summary>
        private readonly VirtualFolder attributesFolder;

        /// <summary>
        /// Drawings folder.
        /// </summary>
        private readonly VirtualFolder drawingsFolder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFolder"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="folderPath">
        /// Model folder path.
        /// </param>
        /// <param name="searchPath">
        /// Model folder search path.
        /// </param>
        public ModelFolder(string folderPath, string searchPath)
            : base(folderPath, searchPath)
        {
            this.drawingsFolder = new VirtualFolder(Path.Combine(folderPath, DrawingsFolderName), searchPath);
            this.attributesFolder = new VirtualFolder(Path.Combine(folderPath, AttributesFolderName), searchPath);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the model attributes folder.
        /// </summary>
        /// <value> Model attributes folder.</value>
        public VirtualFolder AttributesFolder
        {
            get
            {
                return this.attributesFolder;
            }
        }

        /// <summary>
        /// Gets the model drawings folder.
        /// </summary>
        /// <value> Model drawings folder.</value>
        public VirtualFolder DrawingsFolder
        {
            get
            {
                return this.drawingsFolder;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the specified folder contains a model database file.
        /// </summary>
        /// <param name="modelFolder">
        /// Model folder path.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the folder contains a model database file.
        /// </returns>
        public static bool ContainsModelDatabase(string modelFolder)
        {
            var databaseFile = GetDatabaseFile(modelFolder);

            return !string.IsNullOrEmpty(databaseFile) && File.Exists(databaseFile);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the model database file path.
        /// </summary>
        /// <param name="modelFolder">
        /// Model folder path.
        /// </param>
        /// <returns>
        /// Model database file path.
        /// </returns>
        private static string GetDatabaseFile(string modelFolder)
        {
            var modelName = GetModelName(modelFolder);

            if (!string.IsNullOrEmpty(modelName))
            {
                return Path.Combine(modelFolder, modelName + DatabaseFileExtension);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the model name.
        /// </summary>
        /// <param name="modelFolder">
        /// Model folder path.
        /// </param>
        /// <returns>
        /// Model name.
        /// </returns>
        private static string GetModelName(string modelFolder)
        {
            if (!string.IsNullOrEmpty(modelFolder))
            {
                return Path.GetFileName(
                    modelFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}