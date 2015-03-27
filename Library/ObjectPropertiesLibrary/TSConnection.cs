namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Tekla.Structures.Dialog;
    using Tekla.Structures.Model;

    using TSMUI = Tekla.Structures.Model.UI;

    /// <summary>
    /// The ts connection.
    /// </summary>
    public class TSConnection
    {
        #region Constants

        /// <summary>
        /// The privat e_ fil e_ o n_ th e_ side.
        /// </summary>
        private const string PrivateFileOnTheSide = @"ObjectBrowser.ail";

        /// <summary>
        /// The privat e_ localizatio n_ file.
        /// </summary>
        private const string PrivateLocalizationFile = @"messages\DotAppsStrings\ObjectBrowser.ail";

        /// <summary>
        /// The x s_ di r_ own.
        /// </summary>
        private const string XsDirOwn = "XS_DIR";

        #endregion

        #region Fields

        /// <summary>
        /// The _events.
        /// </summary>
        private Events events;

        /// <summary>
        /// The _localization common file.
        /// </summary>
        private string localizationCommonFile;

        /// <summary>
        /// The _localization private file.
        /// </summary>
        private string localizationPrivateFile;

        /// <summary>
        /// The _model.
        /// </summary>
        private Model model;

        /// <summary>
        /// The _model selector.
        /// </summary>
        private TSMUI.ModelObjectSelector modelSelector;

        /// <summary>
        /// The _tekla dir.
        /// </summary>
        private string teklaDir;

        /// <summary>
        /// The current language.
        /// </summary>
        private string currentLanguage;

        /// <summary>
        /// The localization form.
        /// </summary>
        private Localization localizationForm;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TSConnection"/> class.
        /// </summary>
        public TSConnection()
        {
            this.Connect();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the current language.</summary>
        /// <value>The current language.</value>
        public string CurrentLanguage
        {
            get
            {
                if (this.currentLanguage == null)
                {
                    this.SetLanguage();
                }

                return this.currentLanguage;
            }

            set
            {
                this.currentLanguage = value;
            }
        }

        /// <summary>Gets the TS localization form.</summary>
        /// <value>The localization form.</value>
        public Localization LocalizationForm
        {
            get
            {
                if (this.localizationForm == null)
                {
                    this.localizationForm = new Localization();
                    if (!string.IsNullOrEmpty(this.LocalizationCommonFile))
                    {
                        this.localizationForm.LoadFile(this.LocalizationCommonFile);
                    }

                    if (!string.IsNullOrEmpty(this.LocalizationPrivateFile))
                    {
                        this.localizationForm.LoadFile(this.LocalizationPrivateFile);
                    }

                    this.localizationForm.Language = this.CurrentLanguage;
                }

                return this.localizationForm;
            }
        }

        /// <summary>Gets the open model.</summary>
        /// <value>The open model.</value>
        public Model OpenModel
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>Gets the tekla events.</summary>
        /// <value>The tekla events.</value>
        public Events TeklaEvents
        {
            get
            {
                if (this.events == null && this.model != null && this.model.GetConnectionStatus())
                {
                    this.events = new Events();
                }

                return this.events;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets the model selector.</summary>
        /// <value>The model selector.</value>
        protected TSMUI.ModelObjectSelector ModelSelector
        {
            get
            {
                if (this.modelSelector == null && this.OpenModel.GetConnectionStatus())
                {
                    this.modelSelector = new TSMUI.ModelObjectSelector();
                }

                return this.modelSelector;
            }
        }

        /// <summary>Gets the Tekla Structures common localization file.</summary>
        private string LocalizationCommonFile
        {
            get
            {
                if (this.localizationCommonFile == null)
                {
                    if (this.TeklaDir != null)
                    {
                        this.localizationCommonFile = Localization.DefaultLocalizationFile;
                    }

                    if (!File.Exists(this.localizationCommonFile))
                    {
                        this.localizationCommonFile = string.Empty;
                    }
                }

                return this.localizationCommonFile;
            }
        }

        /// <summary>Gets the Localization file for this application only.</summary>
        private string LocalizationPrivateFile
        {
            get
            {
                if (this.localizationPrivateFile == null)
                {
                    if (this.TeklaDir != null)
                    {
                        this.localizationPrivateFile = Path.Combine(this.TeklaDir, PrivateLocalizationFile);
                    }

                    if (!File.Exists(this.localizationPrivateFile))
                    {
                        this.localizationPrivateFile = Application.ExecutablePath.Substring(
                            0, Application.ExecutablePath.LastIndexOf("\\") + 1) + PrivateFileOnTheSide;

                        if (!File.Exists(this.localizationPrivateFile))
                        {
                            this.localizationPrivateFile = string.Empty;
                        }
                    }
                }

                return this.localizationPrivateFile;
            }
        }

        /// <summary>
        /// Gets the tekla dir.
        /// </summary>
        private string TeklaDir
        {
            get
            {
                if (this.teklaDir == null && this.OpenModel != null)
                {
                    TeklaStructuresSettings.GetAdvancedOption(XsDirOwn, ref this.teklaDir);
                }

                return this.teklaDir;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// Throws an exception.
        /// </exception>
        public void Connect()
        {
            if (this.model == null)
            {
                // Create new model connection without creating channel. 
                var m = new Model();

                if (m.GetConnectionStatus())
                {
                    // check that model is opened
                    if (string.IsNullOrEmpty(m.GetInfo().ModelPath))
                    {
                        throw new ApplicationException("Model is not loaded into TeklaStructures.");
                    }

                    this.model = m;

                    this.TeklaEvents.TeklaStructuresExit += delegate { Application.Exit(); };

                    this.TeklaEvents.ModelLoad += delegate { Application.Restart(); };

                    this.TeklaEvents.Register();
                }
                else
                {
                    throw new ApplicationException("Cannot connect to TeklaStructures process.");
                }

                TeklaStructures.Connect();

                // Guess well have to have connections through both .Model and .Application.Library to be able to use methods in both...
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.events != null)
            {
                this.events.UnRegister();
            }

            TeklaStructures.Disconnect();
        }

        /// <summary>
        /// The get all objects.
        /// </summary>
        /// <returns>
        /// The System.Collections.Generic.Dictionary`2[TKey -&gt; System.String, TValue -&gt; Tekla.Structures.Model.ModelObject].
        /// </returns>
        public Dictionary<string, ModelObject> GetAllObjects()
        {
            var allObjects = new Dictionary<string, ModelObject>();
            var selector = this.model.GetModelObjectSelector();

            var modelObjectEnumerator = selector.GetAllObjects();
            modelObjectEnumerator.SelectInstances = false;

            try
            {
                while (modelObjectEnumerator.MoveNext())
                {
                    string guid;

                    // ModelObjectSelector.GetAllObjects() does not return ReferenceModelObjects. Not at least as ReferenceModelObjects.
                    if (modelObjectEnumerator.Current is ReferenceModel)
                    {
                        var childs = modelObjectEnumerator.Current.GetChildren();
                        childs.SelectInstances = false;

                        while (childs.MoveNext())
                        {
                            guid = this.OpenModel.GetGUIDByIdentifier(childs.Current.Identifier);

                            if (!string.IsNullOrEmpty(guid))
                            {
                                allObjects.Add(guid, childs.Current);
                            }
                        }
                    }

                    guid = this.OpenModel.GetGUIDByIdentifier(modelObjectEnumerator.Current.Identifier);

                    if (!string.IsNullOrEmpty(guid))
                    {
                        allObjects.Add(guid, modelObjectEnumerator.Current);
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.ToString());
            }

            return allObjects;
        }

        /// <summary>
        /// The get children of selected.
        /// </summary>
        [Conditional("DEBUG")]
        public void GetChildrenOfSelected()
        {
            var showString = string.Empty;

            var objects = TeklaStructures.Model.SelectedObjects.GetEnumerator();

            objects.MoveNext();

            var obj = (ModelObject)objects.Current;

            var childs = obj.GetChildren();
            childs.SelectInstances = false;

            while (childs.MoveNext())
            {
                showString += " ::: " + childs.Current.Identifier.ID;
            }

            MessageBox.Show(showString, "Childs", MessageBoxButtons.OK);
        }

        /// <summary>
        /// The get selected objects.
        /// </summary>
        /// <returns>
        /// The System.Collections.ArrayList.
        /// </returns>
        public ArrayList GetSelectedObjects()
        {
            var selectedObjects = new ArrayList();

            if (this.model != null)
            {
                var selector = this.model.GetModelObjectSelector();
                var modelObjectEnumerator = new TSMUI.ModelObjectSelector().GetSelectedObjects();
                modelObjectEnumerator.SelectInstances = false;

                try
                {
                    while (modelObjectEnumerator.MoveNext())
                    {
                        selectedObjects.Add(modelObjectEnumerator.Current);
                    }
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }

            return selectedObjects;
        }

        /// <summary>The select objects in model.</summary>
        /// <param name="selectedObjects">The selected objects.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SelectObjectsInModel(ArrayList selectedObjects)
        {
            var result = false;

            if (this.model != null)
            {
                try
                {
                    this.ModelSelector.Select(selectedObjects);
                    result = true;
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.ToString());
                }
            }

            return result;
        }

        // <summary>
        // language from environment
        // </summary>

        /// <summary>
        /// The set language.
        /// </summary>
        public void SetLanguage()
        {
            var language = string.Empty;

            TeklaStructuresSettings.GetAdvancedOption("XS_LANGUAGE", ref language);

            switch (language)
            {
                case "ENGLISH":
                    this.currentLanguage = "enu";
                    break;
                case "DUTCH":
                    this.currentLanguage = "nld";
                    break;
                case "FRENCH":
                    this.currentLanguage = "fra";
                    break;
                case "GERMAN":
                    this.currentLanguage = "deu";
                    break;
                case "ITALIAN":
                    this.currentLanguage = "ita";
                    break;
                case "SPANISH":
                    this.currentLanguage = "esp";
                    break;
                case "JAPANESE":
                    this.currentLanguage = "jpn";
                    break;
                case "CHINESE SIMPLIFIED":
                    this.currentLanguage = "chs";
                    break;
                case "CHINESE TRADITIONAL":
                    this.currentLanguage = "cht";
                    break;
                case "CZECH":
                    this.currentLanguage = "csy";
                    break;
                case "PORTUGUESE BRAZILIAN":
                    this.currentLanguage = "ptb";
                    break;
                case "HUNGARIAN":
                    this.currentLanguage = "hun";
                    break;
                case "POLISH":
                    this.currentLanguage = "plk";
                    break;
                case "RUSSIAN":
                    this.currentLanguage = "rus";
                    break;
                default:
                    this.currentLanguage = "enu";
                    break;
            }
        }

        /// <summary>
        /// The zoom to selected objects.
        /// </summary>
        public void ZoomToSelectedObjects()
        {
            // ExecuteMacro("acmdZoomToSelected", "", "main_frame");
            const string Script = @"akit.Callback(""acmdZoomToSelected"", """", ""main_frame"");";

            try
            {
                TeklaStructures.ExecuteScript(Script);
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.ToString());
            }
        }

        #endregion
    }
}