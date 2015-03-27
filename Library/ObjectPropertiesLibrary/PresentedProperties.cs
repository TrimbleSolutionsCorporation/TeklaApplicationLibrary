namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// A data class representing Tekla Structures property presented in UI.
    /// </summary>
    public class PresentedProperties : IComparable, INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// The default property name.
        /// </summary>
        public const string DefaultPropertyName = "New Property";

        /// <summary>
        /// The default width.
        /// </summary>
        public const int DefaultWidth = 100;

        #endregion

        #region Fields

        /// <summary>
        /// The _ decimals.
        /// </summary>
        private int decimals = 2;

        /// <summary>
        /// The _ display type.
        /// </summary>
        private string displayType;

        /// <summary>
        /// The _ hidden.
        /// </summary>
        private bool hidden;

        /// <summary>
        /// The _ name.
        /// </summary>
        private string name;

        /// <summary>
        /// The _ property type.
        /// </summary>
        private string propertyType;

        /// <summary>
        /// The _ report property name.
        /// </summary>
        private string reportPropertyName;

        /// <summary>
        /// The _ user defined attribute property name.
        /// </summary>
        private string udaPropertyName;

        /// <summary>
        /// The _ width.
        /// </summary>
        private int width = DefaultWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentedProperties"/> class. 
        /// Instantiates class representing Tekla Structures property presented in UI.
        /// </summary>
        public PresentedProperties()
        {
            this.name = DefaultPropertyName;
            this.reportPropertyName = string.Empty;
            this.udaPropertyName = string.Empty;
            this.propertyType = PresentedPropertiesManage.PropertyTypes.Double;

            // _DisplayType = PresentedPropertiesManage.PropertyTypes.Double;
            this.decimals = 2;
            this.width = DefaultWidth;
            this.hidden = true;
        }

        /// <summary>Initializes a new instance of the <see cref="PresentedProperties"/> class. 
        /// Instantiates class representing Tekla Structures property presented in UI.</summary>
        /// <param name="displayName">Name showed in UI.</param>
        /// <param name="reportName">Tekla Stuctures report property name.</param>
        /// <param name="udaName">Tekla Stuctures UDA property name.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="decimals">How many decimals are presented in UI for property.</param>
        /// <param name="width">Column width in UI that presents the property.</param>
        /// <param name="hidden">Visibility of the property column.</param>
        public PresentedProperties(
            string displayName, 
            string reportName, 
            string udaName, 
            string type, 
            /* string DisplayType,*/ int decimals, 
            int width, 
            bool hidden)
        {
            this.name = displayName;
            this.reportPropertyName = reportName;
            this.udaPropertyName = udaName;
            this.propertyType = type;

            // _DisplayType = DisplayType;
            this.decimals = decimals;
            this.width = width;
            this.hidden = hidden;
        }

        /// <summary>Initializes a new instance of the <see cref="PresentedProperties"/> class. 
        /// Instantiates class representing Tekla Structures property presented in UI. Copies values from other instance.</summary>
        /// <param name="propertyToCopy">Presented property from which attributes are copied from.</param>
        public PresentedProperties(PresentedProperties propertyToCopy)
        {
            this.name = propertyToCopy.Name;
            this.reportPropertyName = propertyToCopy.ReportPropertyName;
            this.udaPropertyName = propertyToCopy.UdaPropertyName;
            this.propertyType = propertyToCopy.PropertyType;
            this.displayType = propertyToCopy.DisplayType;
            this.decimals = propertyToCopy.Decimals;
            this.width = propertyToCopy.Width;
            this.hidden = propertyToCopy.Hidden;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the decimals.</summary>
        /// <value>The decimals.</value>
        [XmlAttribute("Decimals")]
        public int Decimals
        {
            get
            {
                return this.decimals;
            }

            set
            {
                if (value == this.decimals)
                {
                    return;
                }

                this.decimals = value;

                this.OnPropertyChanged("Decimals");
            }
        }

        /// <summary>Gets or sets the display type.</summary>
        /// <value>The display type.</value>
        [XmlAttribute("Display_type")]
        public string DisplayType
        {
            get
            {
                return this.displayType;
            }

            set
            {
                if (value == this.displayType)
                {
                    return;
                }

                this.displayType = value;

                this.OnPropertyChanged("DisplayType");
            }
        }

        /// <summary>Gets or sets a value indicating whether hidden.</summary>
        /// <value>The hidden.</value>
        [XmlAttribute("Hidden")]
        public bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                if (value == this.hidden)
                {
                    return;
                }

                this.hidden = value;

                this.OnPropertyChanged("Hidden");
            }
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name value.</value>
        [XmlAttribute("Display_name")]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>Gets or sets the property type.</summary>
        /// <value>The property type.</value>
        [XmlAttribute("Property_type")]
        public string PropertyType
        {
            get
            {
                return this.propertyType;
            }

            set
            {
                if (value == this.propertyType)
                {
                    return;
                }

                this.propertyType = value;

                this.OnPropertyChanged("PropertyType");
            }
        }

        /// <summary>Gets or sets the report property name.</summary>
        /// <value>The report property name.</value>
        [XmlAttribute("REPORT_property_name")]
        public string ReportPropertyName
        {
            get
            {
                return this.reportPropertyName;
            }

            set
            {
                if (value == this.reportPropertyName)
                {
                    return;
                }

                this.reportPropertyName = value;

                this.OnPropertyChanged("ReportPropertyName");
            }
        }

        /// <summary>Gets or sets the uda property name.</summary>
        /// <value>The uda property name.</value>
        [XmlAttribute("UDA_property_name")]
        public string UdaPropertyName
        {
            get
            {
                return this.udaPropertyName;
            }

            set
            {
                if (value == this.udaPropertyName)
                {
                    return;
                }

                this.udaPropertyName = value;

                this.OnPropertyChanged("UdaPropertyName");
            }
        }

        /// <summary>Gets or sets a value indicating whether visible.</summary>
        /// <value>The visible.</value>
        [XmlIgnore]
        public bool Visible
        {
            get
            {
                return !this.hidden;
            }

            set
            {
                if (value == !this.hidden)
                {
                    return;
                }

                this.hidden = !value;

                this.OnPropertyChanged("Visible");
            }
        }

        /// <summary>Gets or sets the width.</summary>
        /// <value>The width.</value>
        [XmlAttribute("Width")]
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (value == this.width)
                {
                    return;
                }

                this.width = value;

                this.OnPropertyChanged("Width");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Copy all attributes from other property.</summary>
        /// <param name="propertyToCopy">Presented property from which attributes are copied from.</param>
        public void Copy(PresentedProperties propertyToCopy)
        {
            this.name = propertyToCopy.Name;
            this.reportPropertyName = propertyToCopy.ReportPropertyName;
            this.udaPropertyName = propertyToCopy.UdaPropertyName;
            this.propertyType = propertyToCopy.PropertyType;
            this.displayType = propertyToCopy.DisplayType;
            this.decimals = propertyToCopy.Decimals;
            this.width = propertyToCopy.Width;
            this.hidden = propertyToCopy.Hidden;
        }

        /// <summary>Presented properties equal, if they Name property equals.</summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The System.Boolean.</returns>
        public override bool Equals(object obj)
        {
            // See the full list of guidelines at
            // http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            // http://go.microsoft.com/fwlink/?LinkId=85238
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.Name == ((PresentedProperties)obj).Name;
        }

        // override object.GetHashCode

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The System.String.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}_{1}_{2}_{3}_{4}_{5}", 
                this.name, 
                this.reportPropertyName, 
                this.udaPropertyName, 
                this.propertyType, 
                this.displayType, 
                this.decimals);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>The compare to.</summary>
        /// <param name="first">The first.</param>
        /// <returns>The System.Int32.</returns>
        int IComparable.CompareTo(object first)
        {
            var firstProperty = (PresentedProperties)first;

            // PresentedProperties SecondProperty = (PresentedProperties)Second;
            return string.CompareOrdinal(firstProperty.Name, this.Name);
        }

        #endregion

        #region Methods

        /// <summary>The on property changed.</summary>
        /// <param name="name">The name value.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            lock (this)
            {
                try
                {
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(name));
                    }
                }
                catch (InvalidOperationException ee)
                {
                    Debug.WriteLine(ee.Message);
                }
            }
        }

        #endregion
    }
}