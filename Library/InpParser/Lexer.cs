namespace Tekla.Structures.InpParser
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// The char type.
    /// </summary>
    public enum CharType
    {
        /// <summary>
        /// The not defined.
        /// </summary>
        NotDefined,

        /// <summary>
        /// The delimiter.
        /// </summary>
        Delimiter,

        /// <summary>
        /// The punctuation.
        /// </summary>
        Punctuation,

        /// <summary>
        /// The letter.
        /// </summary>
        Letter,

        /// <summary>
        /// The number.
        /// </summary>
        Number,
    }

    /// <summary>
    /// The token type.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// The punctuation.
        /// </summary>
        Punctuation,

        /// <summary>
        /// The identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// The number.
        /// </summary>
        Number,

        /// <summary>
        /// The string.
        /// </summary>
        String,
    }

    /// <summary>
    /// The lexer.
    /// </summary>
    public class Lexer
    {
        #region Public Methods and Operators

        /// <summary>The get lexeme.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <returns>The InpParser.Token.</returns>
        /// <exception cref="WrongFormatException">
        /// Throws an exception.
        /// </exception>
        public static Token GetLexeme(FileStream inputStream, ref int lineNumber)
        {
            var nextByte = inputStream.ReadByte();
            if (nextByte < 0)
            {
                // means EOF
                return null;
            }

            var nextSymbol = (char)nextByte;
            if (char.IsSeparator(nextSymbol) || nextSymbol == '\n' || nextSymbol == '\t' || nextSymbol == '\r')
            {
                if (nextSymbol == '\n')
                {
                    lineNumber++;
                }

                return GetLexeme(inputStream, ref lineNumber);
            }

            var charType = CharType.NotDefined;
            var state = 0;
            var result = new StringBuilder();

            while (1 == 1)
            {
                // determine type of symbol
                if (nextByte < 0 || char.IsSeparator(nextSymbol) || nextSymbol == '\n' || nextSymbol == '\t'
                    || nextSymbol == '\r')
                {
                    charType = CharType.Delimiter;
                }
                else if (char.IsNumber(nextSymbol))
                {
                    charType = CharType.Number;
                }
                else if (char.IsLetter(nextSymbol) || nextSymbol == '_')
                {
                    charType = CharType.Letter;
                }
                else if (char.IsPunctuation(nextSymbol))
                {
                    charType = CharType.Punctuation;
                }

                switch (state)
                {
                    case 0: // taking first char
                        if (charType == CharType.Delimiter)
                        {
                            return GetLexeme(inputStream, ref lineNumber);
                        }

                        if (charType != CharType.NotDefined)
                        {
                            result = result.Append(nextSymbol);
                        }

                        if (charType == CharType.Punctuation)
                        {
                            state = 1;
                        }

                        if (charType == CharType.Letter)
                        {
                            state = 2;
                        }

                        if (charType == CharType.Number)
                        {
                            state = 3;
                        }

                        break;
                    case 1: // first char is punctuation
                        if (charType == CharType.Punctuation)
                        {
                            if (result.ToString() == "/" && nextSymbol == '*')
                            {
                                char first;
                                var second = ' ';
                                do
                                {
                                    first = second;
                                    nextByte = inputStream.ReadByte();
                                    if (nextByte < 0)
                                    {
                                        throw new WrongFormatException();
                                    }

                                    second = (char)nextByte;
                                    if (second == '\n')
                                    {
                                        lineNumber++;
                                    }
                                }
                                while (!(first == '*' && second == '/'));
                                result = new StringBuilder();
                                state = 0;
                                break;
                            }
                        }

                        if (result.ToString() == "\"")
                        {
                            result = new StringBuilder();
                            while (nextSymbol != '\"')
                            {
                                result = result.Append(nextSymbol);
                                nextByte = inputStream.ReadByte();
                                if (nextByte < 0)
                                {
                                    throw new WrongFormatException();
                                }

                                nextSymbol = (char)nextByte;
                                if (nextSymbol == '\n')
                                {
                                    lineNumber++;
                                }
                            }

                            return new Token(result.ToString(), TokenType.String);
                        }

                        if (nextByte >= 0)
                        {
                            inputStream.Seek(-1, SeekOrigin.Current);
                        }

                        return new Token(result.ToString(), TokenType.Punctuation);
                    case 2: // first char is letter
                        if (charType == CharType.Letter || charType == CharType.Number)
                        {
                            result = result.Append(nextSymbol);
                            state = 2;
                        }

                        if (charType == CharType.Punctuation || charType == CharType.Delimiter)
                        {
                            if (nextByte >= 0)
                            {
                                inputStream.Seek(-1, SeekOrigin.Current);
                            }

                            return new Token(result.ToString(), TokenType.Identifier);
                        }

                        break;
                    case 3: // first char is number

                        // TODO: add logic for 12.3E4 and.45 numbers here
                        if (charType == CharType.Number || nextSymbol == '.')
                        {
                            result = result.Append(nextSymbol);
                            state = 3;
                        }
                        else if (charType == CharType.Punctuation || charType == CharType.Letter
                                 || charType == CharType.Delimiter)
                        {
                            if (nextByte >= 0)
                            {
                                inputStream.Seek(-1, SeekOrigin.Current);
                            }

                            return new Token(result.ToString(), TokenType.Number);
                        }

                        break;
                }

                nextByte = inputStream.ReadByte();
                nextSymbol = (char)nextByte;
            }
        }

        #endregion
    }
}