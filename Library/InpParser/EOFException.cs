namespace Tekla.Structures.InpParser
{
    /// <summary>
    ///        The exception that is thrown when end of file is reached.
    /// </summary>
    public class EOFException : WrongFormatException
    {
        #region Constructors and Destructors

        /// <summary>
        ///        Initializes a new instance of the <see cref="EOFException"/> class.
        /// </summary>
        public EOFException()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EOFException"/> class. 
        ///         Initializes a new instance of the <see cref="WrongFormatException"/> class.</summary>
        /// <param name="expectedToken">Expected token.</param>
        public EOFException(Token expectedToken)
            : base(expectedToken)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EOFException"/> class.</summary>
        /// <param name="correctEnd">Initializes <see cref="IsCorrectEnd"/> property.</param>
        public EOFException(bool correctEnd)
            : base()
        {
            this.IsCorrectEnd = correctEnd;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is correct end.</summary>
        /// <value>The is correct end.</value>
        public bool IsCorrectEnd { get; set; }

        #endregion
    }
}