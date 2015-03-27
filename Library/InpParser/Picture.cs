namespace Tekla.Structures.InpParser
{
    /// <summary>
    ///        Describes picture object on TabPage.
    /// </summary>
    public class Picture : TSTabPageObject
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Picture"/> class.</summary>
        /// <param name="name">Unque name.</param>
        public Picture(string name)
            : base(name)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the picture height.</summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>Gets or sets the picture width.</summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>Gets or sets the picture upper left corner X coordinate.</summary>
        /// <value>The x value.</value>
        public int X { get; set; }

        /// <summary>Gets or sets the picture upper left corner Y coordinate.</summary>
        /// <value>The y value.</value>
        public int Y { get; set; }

        #endregion
    }
}