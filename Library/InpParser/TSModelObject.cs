namespace Tekla.Structures.InpParser
{
    using System.Collections.Generic;

    /// <summary>
    ///        Model object for which user attributes are defined.
    /// </summary>
    public class TSModelObject
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TSModelObject"/> class. 
        ///         Initializes new instance of <see cref="TSModelObject"/> class.</summary>
        /// <param name="type">Type of model object.</param>
        public TSModelObject(TSModelObjectTypes type)
        {
            this.Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the list of attributes.</summary>
        /// <value>The attributes.</value>
        public List<UDA> Attributes { get; set; }

        /// <summary>Gets or sets the dummy parameter.</summary>
        /// <value>The dummy number.</value>
        public int DummyNumber { get; set; }

        /// <summary>Gets or sets a value indicating whether <b>True.</b> - object modify is possible, <b>False.</b> - modify is not possible.</summary>
        /// <value>The modify.</value>
        public bool Modify { get; set; }

        /// <summary>Gets or sets the object name in Tekla Structures.</summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the list of TabPages.</summary>
        /// <value>The tab pages.</value>
        public List<TSTabPageDeclaration> TabPages { get; set; }

        /// <summary>Gets or sets the type of model object.</summary>
        /// <value>The type value.</value>
        public TSModelObjectTypes Type { get; set; }

        #endregion
    }
}