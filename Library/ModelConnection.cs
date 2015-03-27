namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.Text;
    using System.Threading;

    using InpParser;

    using Tekla.Structures.Model;

    using TSConfiguration = Tekla.Structures.ModuleManager.ProgramConfigurationEnum;
    using TSLocalization = Tekla.Structures.Dialog.Localization;
    using TSModelConnection = Tekla.Structures.Model.Model;
    using TSModelEvents = Tekla.Structures.Model.Events;
    using TSModelInfo = Tekla.Structures.Model.ModelInfo;
    using TSModelOperation = Tekla.Structures.Model.Operations.Operation;
    using TSModelPicker = Tekla.Structures.Model.UI.Picker;
    using TSModuleManager = Tekla.Structures.ModuleManager;
    using TSMUI = Tekla.Structures.Model.UI;
    using TSView = Tekla.Structures.Model.UI.View;
    using TSViewHandler = Tekla.Structures.Model.UI.ViewHandler;

    /// <summary>
    /// Model connection.
    /// </summary>
    internal class ModelConnection : IModel, IEnvironment
    {
        #region Constants

        /// <summary>
        /// Timeout for long operations.
        /// </summary>
        private const int LongOperation = 60000;

        #endregion

        #region Static Fields

        /// <summary>
        /// Environment path separators.
        /// </summary>
        private static readonly char[] PathSeparators = new[] { Structures.SearchPath.Separator };

        #endregion

        #region Fields

        /// <summary>
        /// Model connection object.
        /// </summary>
        private TSModelConnection connection;

        /// <summary>
        /// Model event dispatcher.
        /// </summary>
        private Events events;

        /// <summary>
        /// Indicates whether event dispatcher has been registered.
        /// </summary>
        private bool eventsRegistered;

        /// <summary>
        /// Localization object.
        /// </summary>
        private TSLocalization localization;

        #endregion

        #region Public Events

        /// <summary>
        /// Announces that the application has been closed.
        /// </summary>
        public event EventHandler ApplicationClosed
        {
            add
            {
                this.UnregisterEvents();
                this.ApplicationClosed_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.ApplicationClosed_Event -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the model has changed.
        /// </summary>
        public event EventHandler Changed
        {
            add
            {
                this.UnregisterEvents();
                this.Changed_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.Changed_Event -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the changes have been committed.
        /// </summary>
        public event EventHandler ChangesCommitted;

        /// <summary>
        /// Announces that the changes have been discarded.
        /// </summary>
        public event EventHandler ChangesDiscarded;

        /// <summary>
        /// Announces that the model has been loaded.
        /// </summary>
        public event EventHandler Loaded
        {
            add
            {
                this.UnregisterEvents();
                this.Loaded_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.Loaded_Event -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the model is being numbered.
        /// </summary>
        public event EventHandler Numbering
        {
            add
            {
                this.UnregisterEvents();
                this.Numbering_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.Numbering_Event -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the model has been saved.
        /// </summary>
        public event EventHandler Saved
        {
            add
            {
                this.UnregisterEvents();
                this.Saved_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.Saved_Event -= value;
                this.RegisterEvents();
            }
        }

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                this.UnregisterEvents();
                this.SelectionChanged_Event += value;
                this.RegisterEvents();
            }

            remove
            {
                this.UnregisterEvents();
                this.SelectionChanged_Event -= value;
                this.RegisterEvents();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Announces that the application has been closed.
        /// </summary>
        private event EventHandler ApplicationClosed_Event;

        /// <summary>
        /// Announces that the model has chenged.
        /// </summary>
        private event EventHandler Changed_Event;

        /// <summary>
        /// Announces that the model has been loaded.
        /// </summary>
        private event EventHandler Loaded_Event;

        /// <summary>
        /// Announces that the model is being numbered.
        /// </summary>
        private event EventHandler Numbering_Event;

        /// <summary>
        /// Announces that the model has been saved.
        /// </summary>
        private event EventHandler Saved_Event;

        /// <summary>
        /// Announces that the selection has changed.
        /// </summary>
        private event EventHandler SelectionChanged_Event;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets all objects in the view.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        public IEnumerable<object> AllObjects
        {
            get
            {
                var snapshot = new List<object>();

                try
                {
                    if (this.IsActive)
                    {
                        SeparateThread.Execute(
                            LongOperation, 
                            delegate
                                {
                                    foreach (var item in this.connection.GetModelObjectSelector().GetAllObjects())
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }
                                });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    snapshot.Clear();
                }

                return snapshot;
            }
        }

        /// <summary>
        /// Gets the cloning template model folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        public IEnumerable<string> CloningTemplateModelFolders
        {
            get
            {
                foreach (var location in
                    this["XS_CLONING_TEMPLATE_DIRECTORY"].Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    foreach (var path in Directory.GetDirectories(location))
                    {
                        yield return path;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the company folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        public IEnumerable<string> CompanyFolders
        {
            get
            {
                return this["XS_FIRM"].Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// Application configuration.
        /// </value>
        public Configuration Configuration
        {
            get
            {
                if (this.IsActive)
                {
                    return
                        SeparateThread.Execute<Configuration>(
                            delegate { return (Configuration)ModuleManager.Configuration; });
                }

                return default(Configuration);
            }
        }

        /// <summary>
        /// Gets the application culture info.
        /// </summary>
        /// <value>
        /// Application culture info.
        /// </value>
        public CultureInfo CultureInfo
        {
            get
            {
                switch (this.Language)
                {
                    case "ENGLISH":
                        return new CultureInfo("en");

                    case "DUTCH":
                        return new CultureInfo("nl");

                    case "FRENCH":
                        return new CultureInfo("fr");

                    case "GERMAN":
                        return new CultureInfo("de");

                    case "ITALIAN":
                        return new CultureInfo("it");

                    case "SPANISH":
                        return new CultureInfo("es");

                    case "JAPANESE":
                        return new CultureInfo("ja");

                    case "CHINESE SIMPLIFIED":
                        return new CultureInfo("zh-CHS");

                    case "CHINESE TRADITIONAL":
                        return new CultureInfo("zh-CHT");

                    case "CZECH":
                        return new CultureInfo("cs");

                    case "PORTUGUESE BRAZILIAN":
                        return new CultureInfo("pt-BR");

                    case "HUNGARIAN":
                        return new CultureInfo("hu");

                    case "POLISH":
                        return new CultureInfo("pl");

                    case "RUSSIAN":
                        return new CultureInfo("ru");

                    default:
                        return new CultureInfo("en");
                }
            }
        }

        /// <summary>
        /// Gets the drawing macros.
        /// </summary>
        /// <value>
        /// Enumerable collection of drawing macro names.
        /// </value>
        public IEnumerable<string> DrawingMacros
        {
            get
            {
                var folder = Path.Combine(this.MacrosFolder, "drawings");

                if (Directory.Exists(folder))
                {
                    foreach (var path in Directory.GetFiles(folder, "*.cs"))
                    {
                        if (path.EndsWith(".cs"))
                        {
                            yield return Path.GetFileNameWithoutExtension(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the model folder.
        /// </summary>
        /// <value>
        /// Model folder.
        /// </value>
        public ModelFolder Folder
        {
            get
            {
                if (this.IsActive)
                {
                    var modelInfo = SeparateThread.Execute<ModelInfo>(this.connection.GetInfo);

                    if (modelInfo != null)
                    {
                        return new ModelFolder(modelInfo.ModelPath, this.SearchPath);
                    }
                }

                return new ModelFolder(string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is active.
        /// </summary>
        /// <value>
        /// Indicates whether the object is active.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.connection != null && this.connection.GetConnectionStatus();
            }
        }

        /// <summary>
        /// Gets the application language.
        /// </summary>
        /// <value>
        /// Application language.
        /// </value>
        public string Language
        {
            get
            {
                return this["XS_LANGUAGE"];
            }
        }

        /// <summary>
        /// Gets the application localization source.
        /// </summary>
        /// <value>
        /// Application localization source.
        /// </value>
        public TSLocalization Localization
        {
            get
            {
                if (this.IsActive && this.localization == null)
                {
                    this.localization = new TSLocalization();

                    try
                    {
                        switch (this.Language)
                        {
                            case "ENGLISH":
                                this.localization.Language = "enu";
                                break;

                            case "DUTCH":
                                this.localization.Language = "nld";
                                break;

                            case "FRENCH":
                                this.localization.Language = "fra";
                                break;

                            case "GERMAN":
                                this.localization.Language = "deu";
                                break;

                            case "ITALIAN":
                                this.localization.Language = "ita";
                                break;

                            case "SPANISH":
                                this.localization.Language = "esp";
                                break;

                            case "JAPANESE":
                                this.localization.Language = "jpn";
                                break;

                            case "CHINESE SIMPLIFIED":
                                this.localization.Language = "chs";
                                break;

                            case "CHINESE TRADITIONAL":
                                this.localization.Language = "cht";
                                break;

                            case "CZECH":
                                this.localization.Language = "csy";
                                break;

                            case "PORTUGUESE BRAZILIAN":
                                this.localization.Language = "ptb";
                                break;

                            case "HUNGARIAN":
                                this.localization.Language = "hun";
                                break;

                            case "POLISH":
                                this.localization.Language = "plk";
                                break;

                            case "RUSSIAN":
                                this.localization.Language = "rus";
                                break;

                            default:
                                this.localization.Language = "enu";
                                break;
                        }

                        this.localization.LoadFile(TSLocalization.DefaultLocalizationFile);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                return this.localization;
            }
        }

        /// <summary>
        /// Gets the macros folder. If there are several paths, the first will be returned.
        /// <seealso cref="MacrosFolders">The MacrosFolders returns enumerable collection of all macro paths.</seealso>
        /// </summary>
        /// <value>
        /// Macros folder path.
        /// </value>
        public string MacrosFolder
        {
            get
            {
                return this.MacrosFolders.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the macros folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        public IEnumerable<string> MacrosFolders
        {
            get
            {
                var macroDirectory = this["XS_MACRO_DIRECTORY"];
                var fixedMacroDirectory = macroDirectory.Replace(@"\\", @"\");
                return fixedMacroDirectory.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Gets the model macros.
        /// </summary>
        /// <value>
        /// Enumerable collection of model macro names.
        /// </value>
        public IEnumerable<string> ModelMacros
        {
            get
            {
                var folder = Path.Combine(this.MacrosFolder, "modeling");

                if (Directory.Exists(folder))
                {
                    foreach (var path in Directory.GetFiles(folder, "*.cs"))
                    {
                        if (path.EndsWith(".cs"))
                        {
                            yield return Path.GetFileNameWithoutExtension(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the model name.
        /// </summary>
        /// <value>
        /// Model name.
        /// </value>
        public string Name
        {
            get
            {
                if (this.IsActive)
                {
                    var modelInfo = SeparateThread.Execute<ModelInfo>(this.connection.GetInfo);

                    if (modelInfo != null)
                    {
                        return modelInfo.ModelName;
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the values and default switches of option type user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute name, values and default switches which UDAType is option.
        /// </value>
        public IEnumerable<Dictionary<string, string>> OptionTypeUDAIndexAndValue
        {
            get
            {
                var parser = new Parser();
                parser.ValidationOn = false;

                try
                {
                    parser.Parse(Path.Combine(this["XS_INP"], ModelFolder.ObjectDefinitionFile), false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                foreach (var item in this.CompanyFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.ProjectFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.SystemFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                try
                {
                    parser.Parse(Path.Combine(this.Folder.FolderPath, ModelFolder.ObjectDefinitionFile), true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return parser.FindUdas(UDATypes.Option).ConvertAll<Dictionary<string, string>>(
                    delegate(UDA item)
                        {
                            var result = new Dictionary<string, string>();
                            var index = 0;
                            result.Add("Name", item.Name);
                            foreach (var udaValue in item.Values)
                            {
                                // If the defaultSwitch of the value is "2", GetUserProperty() method of OpenAPI returns "-1".
                                if (udaValue.DefaultSwitch == 2)
                                {
                                    index = -1;
                                }

                                result.Add(index.ToString(), udaValue.Value);
                                index++;
                            }

                            return result;
                        });
            }
        }

        /// <summary>
        /// Gets the project folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        public IEnumerable<string> ProjectFolders
        {
            get
            {
                return this["XS_PROJECT"].Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Gets the search path.
        /// </summary>
        /// <value>
        /// Search path.
        /// </value>
        public string SearchPath
        {
            get
            {
                var result = new StringBuilder();

                foreach (var folder in this.ProjectFolders)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Structures.SearchPath.Separator);
                    }

                    result.Append(folder);
                }

                foreach (var folder in this.CompanyFolders)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Structures.SearchPath.Separator);
                    }

                    result.Append(folder);
                }

                foreach (var folder in this.SystemFolders)
                {
                    if (result.Length > 0)
                    {
                        result.Append(Structures.SearchPath.Separator);
                    }

                    result.Append(folder);
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Gets the selected objects in the view.
        /// </summary>
        /// <value>
        /// Enumerable collection of objects.
        /// </value>
        public IEnumerable<object> SelectedObjects
        {
            get
            {
                var snapshot = new List<object>();

                try
                {
                    if (this.IsActive)
                    {
                        SeparateThread.Execute(
                            LongOperation, 
                            delegate
                                {
                                    foreach (var item in new TSMUI.ModelObjectSelector().GetSelectedObjects())
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }

                                    foreach (var item in Enumerable.From(TSViewHandler.GetSelectedViews()))
                                    {
                                        if (item != null)
                                        {
                                            snapshot.Add(item);
                                        }
                                    }
                                });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    snapshot.Clear();
                }

                return snapshot;
            }
        }

        /// <summary>
        /// Gets the system folders.
        /// </summary>
        /// <value>
        /// Enumerable collection of paths.
        /// </value>
        public IEnumerable<string> SystemFolders
        {
            get
            {
                return this["XS_SYSTEM"].Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Gets a value indicating whether US imperial units are used in input fields.
        /// </summary>
        /// <value>
        /// Indicates whether US imperial units are used in input fields.
        /// </value>
        public bool UseUSImperialUnitsInInput
        {
            get
            {
                return this["XS_IMPERIAL_INPUT"] == "TRUE";
            }
        }

        /// <summary>
        /// Gets the user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute names which UDAType is string.
        /// </value>
        public IEnumerable<string> UserDefinedAttributes
        {
            get
            {
                var parser = new Parser();
                parser.ValidationOn = false;

                try
                {
                    parser.Parse(Path.Combine(this["XS_INP"], ModelFolder.ObjectDefinitionFile), false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                foreach (var item in this.CompanyFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.ProjectFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.SystemFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                try
                {
                    parser.Parse(Path.Combine(this.Folder.FolderPath, ModelFolder.ObjectDefinitionFile), true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return parser.FindUdas(UDATypes.String).ConvertAll<string>(delegate(UDA item) { return item.Name; });
            }
        }

        /// <summary>
        /// Gets the option type user defined attributes.
        /// </summary>
        /// <value>
        /// Enumerable collection of user defined attribute names which UDAType is option.
        /// </value>
        public IEnumerable<string> UserDefinedAttributesOptionType
        {
            get
            {
                var parser = new Parser();
                parser.ValidationOn = false;

                try
                {
                    parser.Parse(Path.Combine(this["XS_INP"], ModelFolder.ObjectDefinitionFile), false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                foreach (var item in this.CompanyFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.ProjectFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                foreach (var item in this.SystemFolders)
                {
                    try
                    {
                        parser.Parse(Path.Combine(item, ModelFolder.ObjectDefinitionFile), true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                try
                {
                    parser.Parse(Path.Combine(this.Folder.FolderPath, ModelFolder.ObjectDefinitionFile), true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return parser.FindUdas(UDATypes.Option).ConvertAll<string>(delegate(UDA item) { return item.Name; });
            }
        }

        /// <summary>
        /// Gets the application version string.
        /// </summary>
        /// <value>
        /// Application version string.
        /// </value>
        public string Version
        {
            get
            {
                if (this.IsActive)
                {
                    return SeparateThread.Execute<string>(TeklaStructuresInfo.GetCurrentProgramVersion);
                }

                return string.Empty;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>Gets an environment variable.</summary>
        /// <param name="variableName">Variable name.</param>
        /// <value>Environment variable value.</value>
        /// <returns>The System.String.</returns>
        public string this[string variableName]
        {
            get
            {
                if (this.IsActive)
                {
                    var value = string.Empty;

                    if (
                        SeparateThread.Execute<bool>(
                            delegate { return TeklaStructuresSettings.GetAdvancedOption(variableName, ref value); }))
                    {
                        return value;
                    }
                }

                return string.Empty;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Commits the changes made to the objects.</summary>
        /// <remarks>If Object.Modify() is not implemented this method does nothing.</remarks>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void CommitChanges(IEnumerable<object> objects)
        {
            if (this.IsActive)
            {
                try
                {
                    foreach (var item in objects)
                    {
                        try
                        {
                            var modelObject = item as ModelObject;

                            if (modelObject != null)
                            {
                                modelObject.Modify();
                            }

                            var modelView = item as TSView;

                            if (modelView != null)
                            {
                                modelView.Modify();
                            }
                        }
                        catch (NotImplementedException)
                        {
                            // no action (at the moment)
                            // TODO: Add overloaded CommitChanges(objects, failedObjects) method
                            // TODO: to get optionally to know any possibly failed objects 
                        }
                    }

                    if (SeparateThread.Execute<bool>(this.connection.CommitChanges))
                    {
                        if (this.ChangesCommitted != null)
                        {
                            this.ChangesCommitted(this, EventArgs.Empty);
                        }
                    }
                }
                catch (RemotingException e)
                {
                    Debug.WriteLine(e.ToString());
                    this.DiscardChanges(objects);
                }
            }
        }

        /// <summary>
        /// Connects the model interface.
        /// </summary>
        /// <returns>A boolean value indicating whether the interface is connected.</returns>
        public bool Connect()
        {
            try
            {
                if (this.connection == null)
                {
                    this.connection = new TSModelConnection();
                }

                if (this.events == null)
                {
                    this.events = new TSModelEvents();

                    this.RegisterEvents();
                }

                if (this.Localization == null)
                {
                    throw new NotSupportedException("Localization is not available.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                this.Disconnect();
            }

            return this.IsActive;
        }

        /// <summary>Discards the changed made to the objects.</summary>
        /// <param name="objects">Enumerable collection of modified objects.</param>
        public void DiscardChanges(IEnumerable<object> objects)
        {
            if (this.ChangesDiscarded != null)
            {
                this.ChangesDiscarded(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Disconnects the model interface.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                this.UnregisterEvents();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.events = null;
                this.connection = null;
                this.localization = null;
            }
        }

        /// <summary>Loads the specified localization file.</summary>
        /// <param name="fileName">File name.</param>
        public void LoadLocalizationFile(string fileName)
        {
            if (this.IsActive)
            {
                try
                {
                    this.Localization.LoadFile(Path.Combine(this["XS_DIR"], @"messages\DotAppsStrings\" + fileName));
                }
                catch (ConstraintException ex)
                {
                    throw new InvalidDataException(
                        "The localization file contains duplicate keys or keys that have already been loaded.", ex);
                }
            }
        }

        /// <summary>Picks an object.</summary>
        /// <param name="prompt">Pick prompt.</param>
        /// <returns>Picked object or null if no object was picked.</returns>
        public object PickObject(string prompt)
        {
            if (this.IsActive)
            {
                try
                {
                    // NOTE: Picking can not be placed into separate thread as the picking
                    // action may take arbitary long time if the user does not select anything.
                    return new TSModelPicker().PickObject(TSModelPicker.PickObjectEnum.PICK_ONE_OBJECT, prompt);
                }
                catch (RemotingException e)
                {
                    Debug.WriteLine(e.ToString());
                }
                catch (ApplicationException)
                {
                    // No action.
                }
            }

            return null;
        }

        /// <summary>Executes a macro in the view.</summary>
        /// <param name="macroName">Macro name.</param>
        public void RunMacro(string macroName)
        {
            if (this.IsActive)
            {
                if (!Path.HasExtension(macroName))
                {
                    macroName += ".cs";
                }

                SeparateThread.Execute(
                    delegate
                        {
                            while (TSModelOperation.IsMacroRunning())
                            {
                                Thread.Sleep(100);
                            }

                            TSModelOperation.RunMacro(macroName);
                        });
            }
        }

        /// <summary>Selects object by identifier.</summary>
        /// <param name="identifier">Object identifier.</param>
        /// <returns>Selected object or null if no matching object was found.</returns>
        public object SelectObjectByIdentifier(Identifier identifier)
        {
            if (this.IsActive)
            {
                return SeparateThread.Execute<ModelObject>(
                    delegate { return this.connection.SelectModelObject(identifier); });
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the ApplicationClosed event.
        /// </summary>
        private void OnApplicationClosed()
        {
            this.ApplicationClosed_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        private void OnChanged()
        {
            this.Changed_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the Loaded event.
        /// </summary>
        private void OnLoaded()
        {
            this.Loaded_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the Numbering event.
        /// </summary>
        private void OnNumbering()
        {
            this.Numbering_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the Saved event.
        /// </summary>
        private void OnSaved()
        {
            this.Saved_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        private void OnSelectionChanged()
        {
            this.SelectionChanged_Event.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        /// <summary>
        /// Registers the event handlers.
        /// </summary>
        private void RegisterEvents()
        {
            if (this.events != null && !this.eventsRegistered)
            {
                SeparateThread.Execute(
                    delegate
                        {
                            var registeredHandlers = 0;

                            if (this.ApplicationClosed_Event != null)
                            {
                                this.events.TeklaStructuresExit += this.OnApplicationClosed;
                                registeredHandlers++;
                            }

                            if (this.Numbering_Event != null)
                            {
                                this.events.Numbering += this.OnNumbering;
                                registeredHandlers++;
                            }

                            if (this.Loaded_Event != null)
                            {
                                this.events.ModelLoad += this.OnLoaded;
                                registeredHandlers++;
                            }

                            if (this.Saved_Event != null)
                            {
                                this.events.ModelSave += this.OnSaved;
                                registeredHandlers++;
                            }

                            if (this.SelectionChanged_Event != null)
                            {
                                this.events.SelectionChange += this.OnSelectionChanged;
                                registeredHandlers++;
                            }

                            if (this.Changed_Event != null)
                            {
                                this.events.ModelChanged += this.OnChanged;
                                registeredHandlers++;
                            }

                            if (registeredHandlers > 0)
                            {
                                this.events.Register();

                                this.eventsRegistered = true;
                            }
                        });
            }
        }

        /// <summary>
        /// Removes the event handler registration.
        /// </summary>
        private void UnregisterEvents()
        {
            if (this.events != null && this.eventsRegistered)
            {
                SeparateThread.Execute(
                    delegate
                        {
                            this.eventsRegistered = false;

                            this.events.UnRegister();

                            if (this.ApplicationClosed_Event != null)
                            {
                                this.events.TeklaStructuresExit -= this.OnApplicationClosed;
                            }

                            if (this.Numbering_Event != null)
                            {
                                this.events.Numbering -= this.OnNumbering;
                            }

                            if (this.Loaded_Event != null)
                            {
                                this.events.ModelLoad -= this.OnLoaded;
                            }

                            if (this.Saved_Event != null)
                            {
                                this.events.ModelSave -= this.OnSaved;
                            }

                            if (this.SelectionChanged_Event != null)
                            {
                                this.events.SelectionChange -= this.OnSelectionChanged;
                            }

                            if (this.Changed_Event != null)
                            {
                                this.events.ModelChanged -= this.OnChanged;
                            }
                        });
            }
        }

        #endregion
    }
}