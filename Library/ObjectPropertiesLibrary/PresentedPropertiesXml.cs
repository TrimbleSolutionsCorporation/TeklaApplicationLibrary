namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;

    using Tekla.Structures.Model;

    /// <summary>
    /// Public class for additional model object properties read from .xml.
    /// </summary>
    public class PresentedPropertiesXml
    {
        #region Constants

        /// <summary>
        /// Cs ini default attributes.
        /// </summary>
        public const string CsIniDefaultAttributes =
            "NAME                            FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "OBJECT_DESCRIPTION              FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "OBJECT_TYPE                     FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "MATERIAL                        FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "PROFILE                         FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "MATERIAL_TYPE                   FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "PART_POS                        FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "ASSEMBLY_POS                    FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "MAINPART.PROFILE                FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "PHASE.NAME                      FLOAT       RIGHT    TRUE       8       2         string     string|"
            + "TOP_LEVEL                       FLOAT       RIGHT    TRUE       8       2         m          m|"
            + "AREA                            FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_GROSS                      FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NET                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PLAN                       FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GXY_NET         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GXZ_NET         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GYZ_NET         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GXY_GROSS       FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GXZ_GROSS       FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_GYZ_GROSS       FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_XY_NET          FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_XZ_NET          FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_YZ_NET          FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_XY_GROSS        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_XZ_GROSS        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PROJECTION_YZ_GROSS        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PGZ                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NGZ                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PGX                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NGX                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PGY                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NGY                        FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PZ                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NZ                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PX                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NX                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_PY                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_NY                         FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_FORM_TOP                   FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_FORM_BOTTOM                FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "AREA_FORM_SIDE                  FLOAT       RIGHT    TRUE       8       1         Area       m2|"
            + "ASSEMBLY_PLWEIGHT               FLOAT       RIGHT    TRUE      10       1         Weight     kg|"
            + "CAST_UNIT_REBAR_WEIGHT          FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "HEIGHT                          FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "LENGTH                          FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "LENGTH_GROSS                    FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "LENGTH_MAX                      FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "LENGTH_MIN                      FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "PROFILE.COVER_AREA              FLOAT       RIGHT    TRUE       7       2         Area       m2|"
            + "PROFILE.CROSS_SECTION_AREA      FLOAT       RIGHT    TRUE       7       2         Area       m2|"
            + "PROFILE.FLANGE_THICKNESS        FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.HEIGHT_1                FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.HEIGHT_2                FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.HEIGHT_3                FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.HEIGHT_4                FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.MAJOR_AXIS_LENGTH_1     FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.MAJOR_AXIS_LENGTH_2     FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.MINOR_AXIS_LENGTH_1     FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.MINOR_AXIS_LENGTH_2     FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.WEB_THICKNESS           FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.WIDTH_1                 FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "PROFILE.WIDTH_2                 FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "SUPPLEMENT_PART_WEIGHT          FLOAT       LEFT     TRUE       8       1         Weight     kg|"
            + "WEB_HEIGHT                      FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "WEB_LENGTH                      FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "WEB_WIDTH                       FLOAT       RIGHT    TRUE       7       2         Length     m|"
            + "WEIGHT                          FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "WEIGHT_GROSS                    FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "WEIGHT_MAX                      FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "WEIGHT_MIN                      FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "WEIGHT_NET                      FLOAT       RIGHT    TRUE       8       1         Weight     kg|"
            + "WIDTH                           FLOAT       RIGHT    TRUE       8       1         Length     m|"
            + "VOLUME                          FLOAT       RIGHT    TRUE       8       2         Volume     m3|"
            + "VOLUME_GROSS                    FLOAT       RIGHT    TRUE       8       2         Volume     m3|"
            + "VOLUME_NET                      FLOAT       RIGHT    TRUE       8       2         Volume     m3|"
            + "NUMBER                          INTEGER     LEFT     TRUE       8       2         Number     int";

        /// <summary>
        /// The comment.
        /// </summary>
        private const string Comment = @"Allowed property types:
    double    
    int 
    string
    date
    kg    (base type double)
    ton    (as of thousand of Kg, base type double)
    mm    (base type double)
    mm2    (square mm, base type double)
    mm3    (cubic mm, base type double)
    cm    (base type double)
    cm2    (square cm, base type double)
    cm3    (cubic cm, base type double)
    m    (base type double)
    m2    (base type double)
    m3    (base type double)
    in    (base type double)
    in2    (base type double)
    in3    (base type double)
    ft    (base type double)
    ft2    (base type double)
    ft3    (base type double)
    yard    (base type double)
    yard2    (base type double)
    yard3    (base type double)
    lbs (base type double)
    ton_short (base type double)
    ton_long (base type double)
    calc (indicates report type is calculated from formula)

    Display type can be any string.

    REPORT_property is either property name (for example ""WEIGHT"") or
    formula (for example ""WEIGHT / AREA""). For reference model objects, prefix ""EXTERNAL.""
    is needed (for example ""EXTERNAL.BaseQuantities.NetWeight"").

    For formula calculation report properties used have to be defined in this file. Defined property type is then
    used for calculation. Either defined display name or report property name can be used, first matching property is used.
";

        /// <summary>
        /// The Xs firm folder.
        /// </summary>
        private const string XsFirm = "XS_FIRM";

        /// <summary>
        /// The Xs project folder.
        /// </summary>
        private const string XsProject = "XS_PROJECT";

        /// <summary>
        /// The Xs system folder.
        /// </summary>
        private const string XsSystem = "XS_SYSTEM";

        #endregion

        #region Fields

        /// <summary>
        /// The current model.
        /// </summary>
        private Model currentModel;

        /// <summary>
        /// The last read time.
        /// </summary>
        private DateTime lastReadTime = DateTime.MinValue;

        /// <summary>
        /// The properties file name.
        /// </summary>
        private string propertiesFileName = @"\ObjectBrowserProperties.xml";

        /// <summary>
        /// The _ properties list.
        /// </summary>
        private SearchableSortableBindingList<PresentedProperties> propertiesList;

        /// <summary>
        /// The read only once.
        /// </summary>
        private int readOnlyOnce;

        /// <summary>
        /// The the full path.
        /// </summary>
        private string theFullPath;

        /// <summary>
        /// The _ visible properties list.
        /// </summary>
        private SearchableSortableBindingList<PresentedProperties> visiblePropertiesList;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PresentedPropertiesXml"/> class. 
        /// Instantiates a class for handling presented properties.</summary>
        /// <param name="fileName">And xml file for storing properties.</param>
        /// <param name="openModel">Tekla Structures model.</param>
        public PresentedPropertiesXml(string fileName, Model openModel)
        {
            this.Initialize(fileName, openModel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentedPropertiesXml"/> class. 
        /// Contractor for derived classes that dont use file storage.
        /// </summary>
        protected PresentedPropertiesXml()
        {
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The tick handler.
        /// </summary>
        /// <param name="m">The m value.</param>
        /// <param name="e">The e value.</param>
        public delegate void TickHandler(PresentedPropertiesXml m, EventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        /// The modified.
        /// </summary>
        public event TickHandler Modified;

        #endregion

        #region Public Properties

        /// <summary>Gets the load save directory.</summary>
        /// <value>The load save directory.</value>
        public virtual string LoadSaveDirectory
        {
            get
            {
                return string.IsNullOrEmpty(this.theFullPath)
                           ? string.Empty
                           : this.theFullPath.Substring(0, this.theFullPath.LastIndexOf("\\", StringComparison.Ordinal));
            }
        }

        /// <summary>Gets or sets additional object properties defined in ModelOrganizerPropertiex.xml.</summary>
        /// <value>The properties list.</value>
        public SearchableSortableBindingList<PresentedProperties> PropertiesList
        {
            get
            {
                // File is read only is its write time is later than last read time.
                if (this.readOnlyOnce == 0)
                {
                    var newProperties = this.ReadPropertiesList();
                    if (newProperties != null)
                    {
                        this.propertiesList = newProperties;
                    }

                    this.readOnlyOnce = 1;
                }

                return this.propertiesList;
            }

            set
            {
                this.propertiesList = value;
                this.visiblePropertiesList = null;
            }
        }

        /// <summary>Gets or sets the list of AdditionalProperties which Hidden=false.
        /// NOTE: This is needed to be nulled, if properties Hidden values are changed.</summary>
        /// <value>The visible properties list.</value>
        public SearchableSortableBindingList<PresentedProperties> VisiblePropertiesList
        {
            get
            {
                if (this.visiblePropertiesList == null)
                {
                    // File is read only is its write time is later than last read time.
                    if (this.readOnlyOnce == 0)
                    {
                        var newProperties = this.ReadPropertiesList();
                        if (newProperties != null)
                        {
                            this.propertiesList = newProperties;
                        }

                        this.readOnlyOnce = 1;
                    }

                    this.visiblePropertiesList = new SearchableSortableBindingList<PresentedProperties>();

                    foreach (var property in this.propertiesList.Where(property => !property.Hidden))
                    {
                        this.visiblePropertiesList.Add(property);
                    }
                }

                return this.visiblePropertiesList;
            }

            set
            {
                // TODO I think it is wrong that this setter is empty, but I am not sure. Please revise. basz 02/08/2012
                // it is only called in DOT_APPS\Model\Applications\LayoutManager\FormMainController.cs at line 328:
                // PresentedPropertiesXml local = new PresentedPropertiesXml(
                // TeklaStructuresService.shownPropertiesFile, TeklaStructuresService.Instance.Model)
                // {
                // VisiblePropertiesList = list
                // };
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Reads the properties list from file.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>List of properties.</returns>
        public static SearchableSortableBindingList<PresentedProperties> ReadPropertiesListFromFile(string fileName)
        {
            SearchableSortableBindingList<PresentedProperties> result = null;

            var xmlFileInfo = new FileInfo(fileName);

            if (xmlFileInfo.Length > 0)
            {
                try
                {
                    // To allow any root name.
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    if (xmlDocument.DocumentElement != null)
                    {
                        var defaultNameSpace = xmlDocument.DocumentElement.Name;

                        var serializer =
                            new XmlSerializer(
                                typeof(SearchableSortableBindingList<PresentedProperties>), 
                                new XmlRootAttribute(defaultNameSpace));
                        TextReader xmlReader = new StreamReader(fileName);
                        result = (SearchableSortableBindingList<PresentedProperties>)serializer.Deserialize(xmlReader);
                        xmlReader.Close();
                    }
                }
                catch (IOException ioe)
                {
                    Debug.WriteLine(ioe);
                }
                catch (XmlException xmle)
                {
                    Debug.WriteLine(xmle);
                }
            }

            return result;
        }

        /// <summary>Sort BindingList by property.</summary>
        /// <param name="list">The list value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="property">The property.</param>
        public static void SortBindingList(
            ref BindingList<object> list, ListSortDirection direction, PropertyDescriptor property)
        {
            var sortableList = new SearchableSortableBindingList<object>();

            foreach (var adapter in list)
            {
                sortableList.Add(adapter);
            }

            sortableList.Sort(property, direction);

            list.Clear();

            foreach (var adapter in sortableList)
            {
                list.Add(adapter);
            }
        }

        /// <summary>The xml write properties.</summary>
        /// <param name="listToSerialize">The list to serialize.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The System.Boolean.</returns>
        public static bool XmlWriteProperties(BindingList<PresentedProperties> listToSerialize, string fileName)
        {
            var result = false;
            try
            {
                var xmlRoot = new XmlRootAttribute("Properties") { IsNullable = true };
                var serializer = new XmlSerializer(typeof(BindingList<PresentedProperties>), xmlRoot);

                // TextWriter Writer = new StreamWriter(TheFullPath, false);
                var xmlSettings = new XmlWriterSettings
                    {
                       NewLineHandling = NewLineHandling.Entitize, Indent = true, NewLineOnAttributes = true 
                    };
                var writer = XmlWriter.Create(fileName, xmlSettings);
                writer.WriteComment(Comment);
                serializer.Serialize(writer, listToSerialize);
                writer.Close();
                result = true;
            }
            catch (IOException ioe)
            {
                Debug.WriteLine(ioe);
            }
            catch (XmlException xmle)
            {
                Debug.WriteLine(xmle);
            }

            return result;
        }

        /// <summary>Add a property to presented properties list. Sets property visible name same as ReportPropertyName
        /// and tries to deduct the type based on ValueString.</summary>
        /// <param name="reportPropertyName">Tekla Structures report property name.</param>
        /// <param name="valueString">Value of the report property.</param>
        /// <returns>If property with same visible name already exits, property is not added and <c>false.</c> is returned.
        /// Otherwise <c>true.</c>.</returns>
        public bool AddPropertyWithReportName(string reportPropertyName, string valueString)
        {
            var result = false;
            PresentedProperties newProperty = null;
            var addType = PresentedPropertiesManage.PropertyTypes.String;

            // Hack for inquiry-copy
            if (!string.IsNullOrEmpty(valueString))
            {
                addType = TryToGuessType(valueString);
            }

            if (!this.GetPropertyByName(reportPropertyName, ref newProperty))
            {
                newProperty = new PresentedProperties(
                    reportPropertyName, 
                    reportPropertyName, 
                    string.Empty, 
                    addType, 
                    2, 
                    PresentedProperties.DefaultWidth, 
                    true);
                this.PropertiesList.Add(newProperty);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Ensures that file is read when properties list is next used.
        /// </summary>
        public void ForceReadFile()
        {
            this.visiblePropertiesList = null;
            this.lastReadTime = DateTime.MinValue;
            this.readOnlyOnce = 0;
        }

        /// <summary>Gets the properties list hash code for quick comparsion.</summary>
        /// <param name="listToHash">The List To Hash.</param>
        /// <returns>The hash code representing the properties list.</returns>
        public int GetPropertiesHashCode(BindingList<PresentedProperties> listToHash)
        {
            var propertiesString = listToHash.Aggregate(
                string.Empty, (current, presentedProperties) => current + presentedProperties.ToString());

            return propertiesString.GetHashCode();
        }

        /// <summary>Gets property from property list by model property (report or UDA) name.</summary>
        /// <param name="modelPropertyName">Name of the model property.</param>
        /// <param name="property">The property.</param>
        /// <returns>True if found.</returns>
        public bool GetPropertyByModelPropertyName(string modelPropertyName, ref PresentedProperties property)
        {
            var result = false;

            if (!string.IsNullOrEmpty(modelPropertyName))
            {
                foreach (var propertyInList
                    in
                    this.PropertiesList.Where(
                        propertyInList =>
                        propertyInList.ReportPropertyName == modelPropertyName
                        || propertyInList.UdaPropertyName == modelPropertyName))
                {
                    property = propertyInList;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>Gets property from property list by name.</summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="property">The property.</param>
        /// <returns>True if found.</returns>
        public bool GetPropertyByName(string propertyName, ref PresentedProperties property)
        {
            var result = false;

            if (!string.IsNullOrEmpty(propertyName))
            {
                foreach (var propertyInList
                    in this.PropertiesList.Where(propertyInList => propertyInList.Name == propertyName))
                {
                    property = propertyInList;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>Gets property from property list by name, if not found, try to get by model property name.</summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="property">The property.</param>
        /// <returns>True if found.</returns>
        public bool GetPropertyByNameOrModelPropertyName(string propertyName, ref PresentedProperties property)
        {
            var result = false;

            if (this.GetPropertyByName(propertyName, ref property))
            {
                result = true;
            }
            else if (this.GetPropertyByModelPropertyName(propertyName, ref property))
            {
                result = true;
            }

            return result;
        }

        /// <summary>Initializes the specified file name.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="openModel">The open model.</param>
        /// <returns>True, unless there is an exception.</returns>
        public bool Initialize(string fileName, Model openModel)
        {
            this.propertiesFileName = fileName;

            if (!this.propertiesFileName.StartsWith("\\"))
            {
                this.propertiesFileName = "\\" + this.propertiesFileName;
            }

            this.currentModel = openModel;

            this.ForceReadFile();

            // If file not found, create a default one.
            if (!this.GetFullPathToExistingFile(this.propertiesFileName, ref this.theFullPath))
            {
                this.CreateDefaultFile();
            }

            // basz 03/08/2012: What is the point of always returning true?
            return true;
        }

        /// <summary>Merges all properties list from PropertiesToMerge to this instance properties list.</summary>
        /// <param name="propertiesToMerge">Properties to merge.</param>
        public void MergeProperties(PresentedPropertiesXml propertiesToMerge)
        {
            var propertiesExists =
                new SearchableSortableBindingList<PresentedProperties>();

            foreach (var property in propertiesToMerge.PropertiesList)
            {
                foreach (var oldProperty in this.PropertiesList)
                {
                    if (oldProperty.Equals(property))
                    {
                        oldProperty.Copy(property);
                        propertiesExists.Add(property);
                    }
                }
            }

            foreach (var property
                in propertiesToMerge.PropertiesList.Where(property => !propertiesExists.Contains(property)))
            {
                this.PropertiesList.Add(property);
            }
        }

        /// <summary>Moves property to different place in list.</summary>
        /// <param name="propertyToMove">The property to move.</param>
        /// <param name="propertyToMoveBy">The property to move by.</param>
        public void MoveProperty(PresentedProperties propertyToMove, PresentedProperties propertyToMoveBy)
        {
            var copyProperty = new PresentedProperties(propertyToMove);

            this.PropertiesList.Remove(propertyToMove);

            if (propertyToMoveBy != null)
            {
                this.PropertiesList.Insert(this.PropertiesList.IndexOf(propertyToMoveBy), copyProperty);
            }
            else
            {
                this.PropertiesList.Add(copyProperty);
            }
        }

        /// <summary>Removeve property from list.</summary>
        /// <param name="propertyToRemove">The property to remove.</param>
        public void RemoveProperty(PresentedProperties propertyToRemove)
        {
            if (this.PropertiesList.Contains(propertyToRemove))
            {
                this.PropertiesList.Remove(propertyToRemove);
            }
        }

        /// <summary>
        /// Write properties list to properties file.
        /// </summary>
        /// <returns> False if write fails.</returns>
        public bool XmlWriteProperties()
        {
            return this.XmlWriteProperties(this.PropertiesList);
        }

        /// <summary>Write given list to properties file.</summary>
        /// <param name="listToSerialize">The list to serialize.</param>
        /// <returns>false if write fails.</returns>
        public virtual bool XmlWriteProperties(BindingList<PresentedProperties> listToSerialize)
        {
            // primarily use file in execution folder
            if (this.theFullPath
                !=
                Application.ExecutablePath.Substring(
                    0, Application.ExecutablePath.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                + this.propertiesFileName)
            {
                // Write only to modelpath and after write, also read from modelpath.
                this.theFullPath = this.currentModel.GetInfo().ModelPath + this.propertiesFileName;
            }

            return XmlWriteProperties(listToSerialize, this.theFullPath);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The read properties list.
        /// </summary>
        /// <returns>List of properties.</returns>
        protected virtual SearchableSortableBindingList<PresentedProperties> ReadPropertiesList()
        {
            SearchableSortableBindingList<PresentedProperties> result = null;
            if (File.Exists(this.theFullPath))
            {
                var lastWriteTime = new FileInfo(this.theFullPath).LastWriteTime;

                if (this.lastReadTime < lastWriteTime)
                {
                    this.lastReadTime = lastWriteTime;
                    result = ReadPropertiesListFromFile(this.theFullPath);
                    if (this.Modified != null)
                    {
                        this.Modified(this, null);
                    }

                    if (this.theFullPath != this.currentModel.GetInfo().ModelPath + this.propertiesFileName)
                    {
                        this.XmlWriteProperties(this.propertiesList);
                    }
                }
            }

            return result;
        }

        /// <summary>Gets the full path.</summary>
        /// <param name="paths">The paths.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The full path.</returns>
        private static string GetFullPath(IEnumerable paths, string fileName)
        {
            foreach (var path
                in paths.Cast<string>().Where(path => !string.IsNullOrEmpty(path) && File.Exists(path + fileName)))
            {
                return path + fileName;
            }

            return string.Empty;
        }

        /// <summary>Occuranceses the of.</summary>
        /// <param name="searchIn">The search in.</param>
        /// <param name="searchThis">The search this.</param>
        /// <returns>Number of occurrences.</returns>
        private static int OccurencesOf(string searchIn, string searchThis)
        {
            var stillFound = true;
            var count = 0;
            var previousIndex = -1;

            if (!string.IsNullOrEmpty(searchIn))
            {
                while (stillFound)
                {
                    var foundIndex = previousIndex;

                    if (searchIn.Length > (previousIndex + 1))
                    {
                        foundIndex = searchIn.IndexOf(searchThis, previousIndex + 1, StringComparison.Ordinal);
                    }

                    if (foundIndex > previousIndex)
                    {
                        previousIndex = foundIndex;
                        count++;
                    }
                    else
                    {
                        stillFound = false;
                    }
                }
            }

            return count;
        }

        /// <summary>Try to guess the type of the value passed as a string.</summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns>A string describing the type of the input.</returns>
        private static string TryToGuessType(string stringValue)
        {
            var returnValue = PresentedPropertiesManage.PropertyTypes.String;

            var mightBeDouble = true;

            try
            {
                returnValue = PresentedPropertiesManage.PropertyTypes.Double;
            }
            catch (Exception)
            {
                try
                {
                    var tmpString = string.Empty;
                    if (stringValue.Contains("."))
                    {
                        tmpString = stringValue.Replace(".", ",");

                        // Special check for date. (12.12.2012 -> 12,12,2012 -> converts to double with en-us culture.
                        if (OccurencesOf(tmpString, ",") > 1)
                        {
                            mightBeDouble = false;
                        }
                    }
                    else if (stringValue.Contains(","))
                    {
                        tmpString = stringValue.Replace(",", ".");
                    }

                    if (!string.IsNullOrEmpty(tmpString) && mightBeDouble)
                    {
                        returnValue = PresentedPropertiesManage.PropertyTypes.Double;
                    }
                }
                catch (Exception)
                {
                    returnValue = PresentedPropertiesManage.PropertyTypes.String;
                }
            }

            return returnValue == PresentedPropertiesManage.PropertyTypes.String
                       ? PresentedPropertiesManage.PropertyTypes.Date
                       : returnValue;
        }

        /// <summary>
        /// Creates the default file.
        /// </summary>
        private void CreateDefaultFile()
        {
            var defaults = CsIniDefaultAttributes.Split('|');
            this.propertiesList = new SearchableSortableBindingList<PresentedProperties>();

            foreach (var property in defaults)
            {
                var splitChar = new char[1];
                splitChar[0] = ' ';
                var tmpProperty = property.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
                if (tmpProperty.Length <= 0)
                {
                    continue;
                }

                var defaultProp = new PresentedProperties(
                    tmpProperty[0], 
                    tmpProperty[0], 
                    string.Empty, 
                    tmpProperty[7], 
                    2, 
                    PresentedProperties.DefaultWidth, 
                    false);
                this.propertiesList.Add(defaultProp);
            }

            // Example of formula
            var formulaExample = new PresentedProperties(
                "Rebar Group Weight", 
                "WEIGHT * NUMBER", 
                string.Empty, 
                "calc", 
                2, 
                PresentedProperties.DefaultWidth, 
                false);

            this.propertiesList.Add(formulaExample);
            this.XmlWriteProperties(this.propertiesList);
        }

        /// <summary>The get full path to existing file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns>The System.Boolean.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "XS is OK here.")]
        private bool GetFullPathToExistingFile(string fileName, ref string fullPath)
        {
            var result = false;
            var xsSystemPath = string.Empty;
            var xsFirmPath = string.Empty;
            var xsProjectPath = string.Empty;

            if (!fileName.StartsWith("\\"))
            {
                fileName = "\\" + fileName;
            }

            TeklaStructuresSettings.GetAdvancedOption(XsProject, ref xsProjectPath);
            TeklaStructuresSettings.GetAdvancedOption(XsFirm, ref xsFirmPath);
            TeklaStructuresSettings.GetAdvancedOption(XsSystem, ref xsSystemPath);

            if (
                File.Exists(
                    Application.ExecutablePath.Substring(
                        0, Application.ExecutablePath.LastIndexOf("\\", StringComparison.Ordinal)) + fileName))
            {
                fullPath = Application.ExecutablePath.Substring(
                    0, Application.ExecutablePath.LastIndexOf("\\", StringComparison.Ordinal)) + fileName;
            }

            if (File.Exists(this.currentModel.GetInfo().ModelPath + fileName))
            {
                fullPath = this.currentModel.GetInfo().ModelPath + fileName;
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                fullPath = GetFullPath(xsProjectPath.Split(';'), fileName);
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                fullPath = GetFullPath(xsFirmPath.Split(';'), fileName);
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                fullPath = GetFullPath(xsSystemPath.Split(';'), fileName);
            }

            // No file found, default path modelpath
            if (string.IsNullOrEmpty(fullPath))
            {
                fullPath = this.currentModel.GetInfo().ModelPath + fileName;
            }
            else
            {
                result = true;
            }

            return result;
        }

        #endregion
    }
}