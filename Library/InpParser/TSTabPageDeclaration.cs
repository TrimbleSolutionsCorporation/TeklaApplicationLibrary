namespace Tekla.Structures.InpParser
{
    /// <summary>
    ///        Joins TSTabPageDefinition with its usage.
    /// </summary>
    public class TSTabPageDeclaration
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TSTabPageDeclaration"/> class. 
        ///         Initializes new instance of <see cref="TSTabPageDeclaration"/> class.
        /// </summary>
        public TSTabPageDeclaration()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TSTabPageDeclaration"/> class. 
        ///         Initializes new instance of <see cref="TSTabPageDeclaration"/> class
        ///         and sets its <see cref="Definition"/> property.</summary>
        /// <param name="name">TabPage unique name.</param>
        public TSTabPageDeclaration(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the property which is used when TabPage declaration is used together with definition.</summary>
        /// <value>The definition.</value>
        public TSTabPageDefinition Definition { get; set; }

        /// <summary>Gets or sets the index in TabControl TabPages collection.</summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>Gets or sets the unique name.</summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the text Tekla Structures displays in the dialog box.</summary>
        /// <value>The prompt.</value>
        public string Prompt { get; set; }

        #endregion
    }
}