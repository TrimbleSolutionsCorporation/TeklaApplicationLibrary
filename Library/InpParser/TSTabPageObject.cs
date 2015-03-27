namespace Tekla.Structures.InpParser
{
    /// <summary>
    ///        Basic class which joins objects that can be located on TabPage.
    /// </summary>
    public class TSTabPageObject
    {
        #region Constructors and Destructors

        /// <summary>
        ///        Initializes a new instance of the <see cref="TSTabPageObject"/> class.
        /// </summary>
        public TSTabPageObject()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TSTabPageObject"/> class.</summary>
        /// <param name="name">Unque name.</param>
        public TSTabPageObject(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the unique name. Used for inquiring value.</summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        #endregion
    }
}