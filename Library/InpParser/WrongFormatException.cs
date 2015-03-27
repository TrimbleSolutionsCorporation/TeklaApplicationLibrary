namespace Tekla.Structures.InpParser
{
    using System;

    /// <summary>
    ///        The exception that is thrown when input data doesn't satisfy predefined format.
    /// </summary>
    public class WrongFormatException : ApplicationException
    {
        #region Constructors and Destructors

        /// <summary>
        ///        Initializes a new instance of the <see cref="WrongFormatException"/> class.
        /// </summary>
        public WrongFormatException()
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WrongFormatException"/> class.</summary>
        /// <param name="expectedToken">Expected token.</param>
        public WrongFormatException(Token expectedToken)
        {
            this.ExpectedToken = expectedToken;
        }

        /// <summary>Initializes a new instance of the <see cref="WrongFormatException"/> class.</summary>
        /// <param name="receivedToken">Received token.</param>
        /// <param name="expectedToken">Expected token.</param>
        /// <param name="lineNumber">Line number.</param>
        public WrongFormatException(Token receivedToken, Token expectedToken, int lineNumber)
        {
            this.ReceivedToken = receivedToken;
            this.ExpectedToken = expectedToken;
            this.LineNumber = lineNumber;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the expected token.</summary>
        /// <value>The expected token.</value>
        public Token ExpectedToken { get; set; }

        /// <summary>Gets or sets the line number.</summary>
        /// <value>The line number.</value>
        public int LineNumber { get; set; }

        /// <summary>Gets or sets the received token.</summary>
        /// <value>The received token.</value>
        public Token ReceivedToken { get; set; }

        #endregion
    }
}