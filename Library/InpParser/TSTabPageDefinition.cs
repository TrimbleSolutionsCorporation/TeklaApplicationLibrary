namespace Tekla.Structures.InpParser
{
    using System.Collections.Generic;

    /// <summary>
    ///        Describes objects which should contain TabPage.
    /// </summary>
    public class TSTabPageDefinition
    {
        #region Fields

        /// <summary>
        /// The _objects.
        /// </summary>
        private List<TSTabPageObject> objects = new List<TSTabPageObject>();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TSTabPageDefinition"/> class. 
        ///         Initializes new instance of <see cref="TSTabPageDefinition"/> class.</summary>
        /// <param name="name">Unique name.</param>
        public TSTabPageDefinition(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the unique name.</summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the tabPage objects.
        ///        Indexes in the list define the order of "lines" in which objects will be placed.</summary>
        /// <value>The objects.</value>
        public List<TSTabPageObject> Objects
        {
            get
            {
                return this.objects;
            }

            set
            {
                this.objects = value;
            }
        }

        #endregion
    }
}