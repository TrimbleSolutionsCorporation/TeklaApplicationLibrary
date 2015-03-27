namespace Tekla.Structures.InpParser
{
    /// <summary>
    /// The token.
    /// </summary>
    public class Token
    {
        #region Fields

        /// <summary>
        /// The _type.
        /// </summary>
        private readonly TokenType tokenType;

        /// <summary>
        /// The _value.
        /// </summary>
        private readonly string value;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Token"/> class.</summary>
        /// <param name="stringValue">The string value.</param>
        /// <param name="type">The type value.</param>
        public Token(string stringValue, TokenType type)
        {
            this.value = stringValue;
            this.tokenType = type;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the token type.</summary>
        /// <value>The token type value.</value>
        public TokenType TokenType
        {
            get
            {
                return this.tokenType;
            }
        }

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return this.value;
            }
        }

        #endregion
    }
}