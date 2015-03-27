namespace Tekla.Structures.InpParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// The parser.
    /// </summary>
    public class Parser
    {
        #region Fields

        /// <summary>
        /// The _defined model objects.
        /// </summary>
        private readonly Dictionary<TSModelObjectTypes, TSModelObject> definedModelObjects;

        /// <summary>
        /// The _defined tab pages.
        /// </summary>
        private Dictionary<string, TSTabPageDefinition> definedTabPages;

        /// <summary>
        /// The _line number.
        /// </summary>
        private int lineNumber = 1;

        /// <summary>
        /// The _override existing.
        /// </summary>
        private bool overrideExisting;

        /// <summary>
        /// The _validation on.
        /// </summary>
        private bool validationOn = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        public Parser()
        {
            this.definedModelObjects = new Dictionary<TSModelObjectTypes, TSModelObject>();
            this.definedTabPages = new Dictionary<string, TSTabPageDefinition>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the list of defined ModelObjects.</summary>
        /// <value>The defined model objects.</value>
        public Dictionary<TSModelObjectTypes, TSModelObject> DefinedModelObjects
        {
            get
            {
                return this.definedModelObjects;
            }
        }

        /// <summary>Gets the list of found TabPage definitions.</summary>
        /// <value>The defined tab pages.</value>
        public Dictionary<string, TSTabPageDefinition> DefinedTabPages
        {
            get
            {
                return this.definedTabPages;
            }
        }

        /// <summary>Gets or sets a value indicating whether to validation on/off.</summary>
        /// <value>The validation on.</value>
        public bool ValidationOn
        {
            get
            {
                return this.validationOn;
            }

            set
            {
                this.validationOn = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Parses specified file and gathers all known UDAsDictionary (without repeats) with specified type.</summary>
        /// <param name="udaType">Needed type.</param>
        /// <returns>List of UDAsDictionary.</returns>
        public List<UDA> FindUdas(UDATypes udaType)
        {
            var udaNames = new List<string>();
            var udas = new List<UDA>();

            foreach (var tabPageDef in this.DefinedTabPages.Values)
            {
                foreach (var obj in tabPageDef.Objects)
                {
                    if (!(obj is UDA))
                    {
                        continue;
                    }

                    var nextUDA = obj as UDA;
                    if (nextUDA.ValueType == udaType)
                    {
                        if (udaNames.Contains(nextUDA.Name))
                        {
                            continue;
                        }

                        udaNames.Add(nextUDA.Name);
                        udas.Add(nextUDA);
                    }
                }
            }

            foreach (var modelObject in this.DefinedModelObjects.Values)
            {
                if (modelObject.TabPages != null && modelObject.TabPages.Count == 1 && modelObject.TabPages[0].Definition != null)
                {
                    foreach (var obj in modelObject.TabPages[0].Definition.Objects)
                    {
                        if (!(obj is UDA))
                        {
                            continue;
                        }

                        var nextUDA = obj as UDA;
                        if (nextUDA.ValueType == udaType)
                        {
                            if (udaNames.Contains(nextUDA.Name))
                            {
                                continue;
                            }

                            udaNames.Add(nextUDA.Name);
                            udas.Add(nextUDA);
                        }
                    }
                }
            }

            return udas;
        }

        /// <summary>Parses spacified file and fills <see cref="DefinedTabPages"/> and <see cref="DefinedModelObjects"/> properties.</summary>
        /// <param name="filePath">File to parse.</param>
        /// <param name="overrideExisting">Indicates wheather existing objects should be overridden with new or not.</param>
        /// <exception cref="FileNotFoundException">Thrown if specified file doesn/t exist.</exception>
        /// <exception cref="EOFException">Thrown when not correct end of file detected.</exception>
        /// <exception cref="WrongFormatException">Thrown when parsing can't be done because input data format is not correct.</exception>
        public void Parse(string filePath, bool overrideExisting)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            this.overrideExisting = overrideExisting;
            this.lineNumber = 1;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // , System.Security.AccessControl.FileSystemRights.Read, FileShare.ReadWrite, 8, FileOptions.None))
                TSModelObject nextObject;
                try
                {
                    while (1 == 1)
                    {
                        nextObject = this.TryGetModelObject(fs);
                        if (nextObject != null)
                        {
                            if (!this.definedModelObjects.ContainsKey(nextObject.Type))
                            {
                                this.definedModelObjects.Add(nextObject.Type, nextObject);
                            }
                            else if (this.overrideExisting)
                            {
                                this.definedModelObjects.Remove(nextObject.Type);
                                this.definedModelObjects.Add(nextObject.Type, nextObject);
                            }
                        }
                    }
                }
                catch (EOFException ex)
                {
                    if (!ex.IsCorrectEnd)
                    {
                        throw ex;
                    }
                }
                catch (WrongFormatException ex)
                {
                    Trace.WriteLine("Line " + ex.LineNumber.ToString() + ": " + ex.ToString());
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>The try get attribute value.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.UDAValue.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private UDAValue TryGetAttributeValue(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;

            // key-word "value"
            if (!this.TryGetKeWordY(inputStream, KeyWords.value.ToString()))
            {
                inputStream.Seek(startPosition, SeekOrigin.Begin);
                this.lineNumber = startLineNumber;

                // if there is no such key-word then we suppose that
                // there is no attribute value definition
                return null;
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Name"
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Attribute value string parameter", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token(string.Empty, TokenType.String), this.lineNumber);
            }

            var resultValue = new UDAValue(nextToken.Value);

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "DefaultSwitch"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("0-2", TokenType.Number));
            }

            if (!(nextToken.TokenType == TokenType.Number))
            {
                throw new WrongFormatException(nextToken, new Token("0-2", TokenType.Number), this.lineNumber);
            }

            int number;
            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(nextToken, new Token("0-2", TokenType.Number), this.lineNumber);
            }

            resultValue.DefaultSwitch = number;

            // symbol ')'
            this.TryGetPunctuation(inputStream, ")");

            return resultValue;
        }

        /// <summary>The try get key word.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="keyWord">The key word.</param>
        /// <returns>The System.Boolean.</returns>
        private bool TryGetKeWordY(FileStream inputStream, string keyWord)
        {
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null || !(nextToken.TokenType == TokenType.Identifier && nextToken.Value == keyWord))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>The try get model object.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.TSModelObject.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private TSModelObject TryGetModelObject(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;
            TSModelObject resultModelObject;

            // TS model object key-word
            Tekla.Structures.InpParser.Token nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                // only this is correct EOF!!!
                throw new EOFException(true);
            }

            if (nextToken.TokenType != Tekla.Structures.InpParser.TokenType.Identifier)
            {
                throw new WrongFormatException(nextToken, new Token("Model object identifier", TokenType.Identifier), this.lineNumber);
            }

            try
            {
                resultModelObject =
                    new TSModelObject((TSModelObjectTypes)Enum.Parse(typeof(TSModelObjectTypes), nextToken.Value, true));
            }
            catch (ArgumentException)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Correct model object identifier", TokenType.Identifier), this.lineNumber);
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "DummyNumber"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Dummy number", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(nextToken, new Token("Dummy number", TokenType.Number), this.lineNumber);
            }

            int number;
            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Dummy number as integer", TokenType.Number), this.lineNumber);
            }

            resultModelObject.DummyNumber = number;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Name"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Model object name", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("Model object name", TokenType.String), this.lineNumber);
            }

            resultModelObject.Name = nextToken.Value;

            // symbol ')'
            this.TryGetPunctuation(inputStream, ")");

            // symbol '{'
            this.TryGetPunctuation(inputStream, "{");

            // gathering attributes and TabPage definitions and declarations
            var endOfCycle = false;
            UDA uda;
            while (endOfCycle == false)
            {
                var tabPageDef = this.TryGetTabPageDefinition(inputStream);
                if (tabPageDef == null)
                {
                    var tabPageDec = this.TryGetTabPageDeclaration(inputStream);
                    if (tabPageDec == null)
                    {
                        try
                        {
                            uda = this.TryGetUDA(inputStream);
                        }
                        catch (WrongFormatException ex)
                        {
                            Trace.WriteLine("Line " + ex.LineNumber.ToString() + ": " + ex.ToString());
                            while (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == "}"))
                            {
                                nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                            }

                            continue;
                        }

                        if (uda == null)
                        {
                            endOfCycle = true;
                        }
                        else
                        {
                            if (resultModelObject.Attributes == null)
                            {
                                resultModelObject.Attributes = new List<UDA>();
                            }

                            resultModelObject.Attributes.Add(uda);
                        }
                    }
                    else
                    {
                        if (resultModelObject.TabPages == null)
                        {
                            resultModelObject.TabPages = new List<TSTabPageDeclaration>();
                        }

                        // TODO: check - maybe it is not needed
                        if (tabPageDec.Definition == null && !this.definedTabPages.ContainsKey(tabPageDec.Name))
                        {
                            throw new WrongFormatException(new Token("Not defined yet TabPage name", TokenType.String));
                        }

                        resultModelObject.TabPages.Add(tabPageDec);
                    }
                }
                else
                {
                    if (this.definedTabPages == null)
                    {
                        this.definedTabPages = new Dictionary<string, TSTabPageDefinition>();
                    }

                    if (!this.definedTabPages.ContainsKey(tabPageDef.Name))
                    {
                        this.definedTabPages.Add(tabPageDef.Name, tabPageDef);
                    }
                    else if (this.overrideExisting)
                    {
                        this.definedTabPages.Remove(tabPageDef.Name);
                        this.definedTabPages.Add(tabPageDef.Name, tabPageDef);
                    }
                }
            }

            // key-word "modify"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token(KeyWords.modify.ToString(), TokenType.Identifier));
            }

            if (!(nextToken.TokenType == TokenType.Identifier && nextToken.Value == KeyWords.modify.ToString()))
            {
                if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == "}"))
                {
                    throw new WrongFormatException(null, new Token("modify", TokenType.Identifier), this.lineNumber);
                }

                return resultModelObject;
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Modify"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("0-1", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException();
            }

            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(nextToken, new Token("0-1", TokenType.Number), this.lineNumber);
            }

            if (number == 0)
            {
                resultModelObject.Modify = false;
            }
            else if (number == 1)
            {
                resultModelObject.Modify = true;
            }
            else
            {
                throw new WrongFormatException(nextToken, new Token("0-1", TokenType.Number), this.lineNumber);
            }

            // symbol ')'
            this.TryGetPunctuation(inputStream, ")");

            // symbol '}'
            this.TryGetPunctuation(inputStream, "}");

            // success end
            return resultModelObject;
        }

        /// <summary>The try get picture.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.Picture.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private Picture TryGetPicture(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;

            // key-word "picture"
            if (!this.TryGetKeWordY(inputStream, KeyWords.picture.ToString()))
            {
                inputStream.Seek(startPosition, SeekOrigin.Begin);
                this.lineNumber = startLineNumber;

                // if there is no such key-word then we suppose that
                // there is no picture definition
                return null;
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Name"
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Picture name", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("Picture name", TokenType.String), this.lineNumber);
            }

            var resultPicture = new Picture(nextToken.Value);

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Width"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Picture width", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(nextToken, new Token("Picture width", TokenType.Number), this.lineNumber);
            }

            int number;
            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture width as integer number", TokenType.Number), this.lineNumber);
            }

            resultPicture.Width = number;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Height"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Picture height", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(nextToken, new Token("Picture height", TokenType.Number), this.lineNumber);
            }

            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture height as integer number", TokenType.Number), this.lineNumber);
            }

            resultPicture.Height = number;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "X"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Picture X coordinate", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture X coordinate", TokenType.Number), this.lineNumber);
            }

            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture X coordinate as integer number", TokenType.Number), this.lineNumber);
            }

            resultPicture.X = number;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Y"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Picture Y coordinate", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture Y coordinate", TokenType.Number), this.lineNumber);
            }

            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Picture Y coordinate as integer number", TokenType.Number), this.lineNumber);
            }

            resultPicture.Y = number;

            // symbol ')'
            this.TryGetPunctuation(inputStream, ")");

            // success end
            return resultPicture;
        }

        /// <summary>The try get punctuation.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private void TryGetPunctuation(FileStream inputStream, string symbol)
        {
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token(symbol, TokenType.Punctuation));
            }

            if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == symbol))
            {
                throw new WrongFormatException(nextToken, new Token(symbol, TokenType.Punctuation), this.lineNumber);
            }
        }

        /// <summary>The try get tab page declaration.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.TSTabPageDeclaration.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private TSTabPageDeclaration TryGetTabPageDeclaration(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;

            // key-word "tab_page"
            if (!this.TryGetKeWordY(inputStream, KeyWords.tab_page.ToString()))
            {
                inputStream.Seek(startPosition, SeekOrigin.Begin);
                this.lineNumber = startLineNumber;

                // if there is no such key-word then we suppose that
                // there is no TabPage declaration
                return null;
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Name"
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("TabPage name", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("TabPage name", TokenType.String), this.lineNumber);
            }

            var resultTabPageDeclaration = new TSTabPageDeclaration(nextToken.Value);

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Prompt"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("TabPage title", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("TabPage title", TokenType.String), this.lineNumber);
            }

            resultTabPageDeclaration.Prompt = nextToken.Value;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "Index"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("TabPage index", TokenType.Number));
            }

            if (nextToken.TokenType != TokenType.Number)
            {
                throw new WrongFormatException(nextToken, new Token("TabPage index", TokenType.Number), this.lineNumber);
            }

            int number;
            if (!int.TryParse(nextToken.Value, out number))
            {
                throw new WrongFormatException(
                    nextToken, new Token("TabPage index as integer number", TokenType.Number), this.lineNumber);
            }

            resultTabPageDeclaration.Index = number;

            // symbol ')'
            this.TryGetPunctuation(inputStream, ")");

            // if this is only declaration without definition then it is the end
            if (resultTabPageDeclaration.Name != string.Empty)
            {
                return resultTabPageDeclaration;
            }

            var definition = new TSTabPageDefinition(string.Empty);
            this.TryGetTabPageDefinitionBody(inputStream, ref definition);
            resultTabPageDeclaration.Definition = definition;

            // success end
            return resultTabPageDeclaration;
        }

        /// <summary>The try get tab page definition.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.TSTabPageDefinition.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private TSTabPageDefinition TryGetTabPageDefinition(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;

            // key-word "tab_page"
            if (!this.TryGetKeWordY(inputStream, KeyWords.tab_page.ToString()))
            {
                inputStream.Seek(startPosition, SeekOrigin.Begin);
                this.lineNumber = startLineNumber;

                // if there is no such key-word then we suppose that
                // there is no TabPage definition
                return null;
            }

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Name"
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("TabPage name", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("TabPage name", TokenType.String), this.lineNumber);
            }

            var resultTabPageDefinition = new TSTabPageDefinition(nextToken.Value);

            // symbol ')'
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            const string Symbol = ")";

            if (nextToken == null)
            {
                throw new EOFException(new Token(Symbol, TokenType.Punctuation));
            }

            if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == Symbol))
            {
                // maybe it is TabPage declaration?
                if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ","))
                {
                    throw new WrongFormatException(nextToken, new Token(Symbol, TokenType.Punctuation), this.lineNumber);
                }
                else
                {
                    // this is TabPage declaration!!!
                    inputStream.Seek(startPosition, SeekOrigin.Begin);
                    this.lineNumber = startLineNumber;
                    return null;
                }
            }

            this.TryGetTabPageDefinitionBody(inputStream, ref resultTabPageDefinition);

            // success end
            return resultTabPageDefinition;
        }

        /// <summary>The try get tab page definition body.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="definition">The definition.</param>
        private void TryGetTabPageDefinitionBody(FileStream inputStream, ref TSTabPageDefinition definition)
        {
            // symbol '{'
            this.TryGetPunctuation(inputStream, "{");

            // gethering TabPage objects
            var endOfDefinition = false;
            UDA uda;
            while (endOfDefinition == false)
            {
                try
                {
                    uda = this.TryGetUDA(inputStream);
                }
                catch (WrongFormatException ex)
                {
                    Trace.WriteLine("Line " + ex.LineNumber.ToString() + ": " + ex.ToString());
                    var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                    while (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == "}"))
                    {
                        nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                    }

                    continue;
                }

                if (uda == null)
                {
                    var pic = this.TryGetPicture(inputStream);
                    if (pic == null)
                    {
                        endOfDefinition = true;
                    }
                    else
                    {
                        if (definition.Objects == null)
                        {
                            definition.Objects = new List<TSTabPageObject>();
                        }

                        definition.Objects.Add(pic);
                    }
                }
                else
                {
                    if (definition.Objects == null)
                    {
                        definition.Objects = new List<TSTabPageObject>();
                    }

                    definition.Objects.Add(uda);
                }
            }

            // symbol '}'
            this.TryGetPunctuation(inputStream, "}");
        }

        /// <summary>The try get uda.</summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The InpParser.UDA.</returns>
        /// <exception cref="EOFException">Throws an exception.</exception>
        /// <exception cref="WrongFormatException">Throws an exception.</exception>
        private UDA TryGetUDA(FileStream inputStream)
        {
            var startPosition = inputStream.Position;
            var startLineNumber = this.lineNumber;

            // key-word "attribute" or "unique_attribute"
            var nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null
                ||
                !(nextToken.TokenType == TokenType.Identifier
                  &&
                  (nextToken.Value == KeyWords.attribute.ToString()
                   || nextToken.Value == KeyWords.unique_attribute.ToString())))
            {
                inputStream.Seek(startPosition, SeekOrigin.Begin);
                this.lineNumber = startLineNumber;

                // if there is no such key-word then we suppose that
                // there is no attribute definition
                return null;
            }

            var resultUDA = nextToken.Value == KeyWords.unique_attribute.ToString() ? new UDA(true) : new UDA(false);

            // symbol '('
            this.TryGetPunctuation(inputStream, "(");

            // parameter "Name"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Attribute name", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("Attribute name", TokenType.String), this.lineNumber);
            }

            resultUDA.Name = nextToken.Value;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "LabelText"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Label text", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("Label text", TokenType.String), this.lineNumber);
            }

            resultUDA.LabelText = nextToken.Value;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "ValueType"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Type of attribute value", TokenType.Identifier));
            }

            if (nextToken.TokenType != TokenType.Identifier)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Type of attribute value", TokenType.Identifier), this.lineNumber);
            }

            try
            {
                resultUDA.ValueType = (UDATypes)Enum.Parse(typeof(UDATypes), nextToken.Value, true);
            }
            catch (ArgumentException)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Correct type identifier", TokenType.Identifier), this.lineNumber);
            }

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "FieldFormat"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Format string", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(nextToken, new Token("Format string", TokenType.String), this.lineNumber);
            }

            if (!nextToken.Value.StartsWith("%"))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Correct format string", TokenType.String), this.lineNumber);
            }

            resultUDA.FieldFormat = nextToken.Value;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "SpecialFlag"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("yes|no", TokenType.Identifier));
            }

            if (this.validationOn
                && !(nextToken.TokenType == TokenType.Identifier && (nextToken.Value == "yes" || nextToken.Value == "no")))
            {
                throw new WrongFormatException(nextToken, new Token("yes|no", TokenType.Identifier), this.lineNumber);
            }

            resultUDA.SpecialFlag = nextToken.Value == "yes";

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "CheckSwitch"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Check switch identifier", TokenType.Identifier));
            }

            if (nextToken.TokenType != TokenType.Identifier)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Check switch identifier", TokenType.Identifier), this.lineNumber);
            }

            if (!Enum.IsDefined(typeof(CheckSwitchValues), nextToken.Value))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Correct check switch identifier", TokenType.Identifier), this.lineNumber);
            }

            resultUDA.CheckSwitch = (CheckSwitchValues)Enum.Parse(typeof(CheckSwitchValues), nextToken.Value);

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "AttributeValueMax"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Attribute value max", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Attribute value max", TokenType.String), this.lineNumber);
            }

            if (this.validationOn && !char.IsNumber(nextToken.Value, 0))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Attribute value max number as string", TokenType.String), this.lineNumber);
            }

            resultUDA.AttributeValueMax = nextToken.Value;

            // symbol ','
            this.TryGetPunctuation(inputStream, ",");

            // parameter "AttributeValueMin"
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token("Attribute value min", TokenType.String));
            }

            if (nextToken.TokenType != TokenType.String)
            {
                throw new WrongFormatException(
                    nextToken, new Token("Attribute value max", TokenType.String), this.lineNumber);
            }

            if (this.validationOn && !char.IsNumber(nextToken.Value, 0))
            {
                throw new WrongFormatException(
                    nextToken, new Token("Attribute value max number as string", TokenType.String), this.lineNumber);
            }

            resultUDA.AttributeValueMin = nextToken.Value;

            // parameter "PositionValue1", "toggle_field" or symbol ')'
            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
            if (nextToken == null)
            {
                throw new EOFException(new Token(")", TokenType.Punctuation));
            }

            if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ")"))
            {
                // if it is parameter "PositionValue1" or "toggle_field" then symbol ',' should be here
                if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ","))
                {
                    throw new WrongFormatException(nextToken, new Token(")|,", TokenType.Punctuation), this.lineNumber);
                }

                int number;

                // parameter "PositionValue1" or "toggle_field"
                nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                if (nextToken == null)
                {
                    throw new EOFException(new Token("Integer number", TokenType.Number));
                }

                if (nextToken.TokenType == TokenType.String)
                {
                    // parameter "toggle_field"
                    resultUDA.ToggleField = nextToken.Value;

                    nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                    if (nextToken == null)
                    {
                        throw new EOFException(new Token(")", TokenType.Punctuation));
                    }
                }
                else
                {
                    // parameter "PositionValue1"
                    if (nextToken.TokenType != TokenType.Number)
                    {
                        throw new WrongFormatException(
                            nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                    }

                    if (!int.TryParse(nextToken.Value, out number))
                    {
                        throw new WrongFormatException(
                            nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                    }

                    resultUDA.PositionValue1 = number;

                    // parameter "PositionValue2", "toggle_field" or symbol ')'
                    nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                    if (nextToken == null)
                    {
                        throw new EOFException(new Token(")|,", TokenType.Punctuation));
                    }

                    if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ")"))
                    {
                        // if it is parameter "PositionValue2" or "toggle_field" then symbol ',' should be here
                        if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ","))
                        {
                            throw new WrongFormatException(
                                nextToken, new Token(")|,", TokenType.Punctuation), this.lineNumber);
                        }

                        // parameter "PositionValue2" or "toggle_field"
                        nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                        if (nextToken == null)
                        {
                            throw new EOFException(new Token("Integer number", TokenType.Number));
                        }

                        if (nextToken.TokenType == TokenType.String)
                        {
                            // parameter "toggle_field"
                            resultUDA.ToggleField = nextToken.Value;

                            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                            if (nextToken == null)
                            {
                                throw new EOFException(new Token(")", TokenType.Punctuation));
                            }
                        }
                        else
                        {
                            // parameter "PositionValue2"
                            if (nextToken.TokenType != TokenType.Number)
                            {
                                throw new WrongFormatException(
                                    nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                            }

                            if (!int.TryParse(nextToken.Value, out number))
                            {
                                throw new WrongFormatException(
                                    nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                            }

                            resultUDA.PositionValue2 = number;

                            // parameter "PositionValue3", "toggle_field" or symbol ')'
                            nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                            if (nextToken == null)
                            {
                                throw new EOFException(new Token(")|,", TokenType.Punctuation));
                            }

                            if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ")"))
                            {
                                // if it is parameter "PositionValue3" or "toggle_field" then symbol ',' should be here
                                if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ","))
                                {
                                    throw new WrongFormatException(new Token(")|,", TokenType.Punctuation));
                                }

                                // parameter "PositionValue3" or "toggle_field"
                                nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                                if (nextToken == null)
                                {
                                    throw new EOFException(new Token("Integer number", TokenType.Number));
                                }

                                if (nextToken.TokenType == TokenType.String)
                                {
                                    // parameter "toggle_field"
                                    resultUDA.ToggleField = nextToken.Value;

                                    nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                                    if (nextToken == null)
                                    {
                                        throw new EOFException(new Token(")", TokenType.Punctuation));
                                    }
                                }
                                else
                                {
                                    // parameter "PositionValue3"
                                    if (nextToken.TokenType != TokenType.Number)
                                    {
                                        throw new WrongFormatException(
                                            nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                                    }

                                    if (!int.TryParse(nextToken.Value, out number))
                                    {
                                        throw new WrongFormatException(
                                            nextToken, new Token("Integer number", TokenType.Number), this.lineNumber);
                                    }

                                    resultUDA.PositionValue3 = number;

                                    nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                                    if (nextToken == null)
                                    {
                                        throw new EOFException(new Token(")", TokenType.Punctuation));
                                    }

                                    // parameter "toggle_field" or symbol ')'
                                    if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ")"))
                                    {
                                        // if it is parameter "toggle_field" then symbol ',' should be here
                                        if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ","))
                                        {
                                            throw new WrongFormatException(
                                                nextToken, new Token(")|,", TokenType.Punctuation), this.lineNumber);
                                        }

                                        // parameter "toggle_field"
                                        nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                                        if (nextToken == null)
                                        {
                                            throw new EOFException(new Token("Toggle field", TokenType.String));
                                        }

                                        if (nextToken.TokenType != TokenType.String)
                                        {
                                            throw new WrongFormatException(
                                                nextToken, new Token("Toggle field", TokenType.String), this.lineNumber);
                                        }

                                        resultUDA.ToggleField = nextToken.Value;

                                        nextToken = Lexer.GetLexeme(inputStream, ref this.lineNumber);
                                        if (nextToken == null)
                                        {
                                            throw new EOFException(new Token(")", TokenType.Punctuation));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // symbol ')'
            // nextToken = Lexer.GetLexeme(inputStream, ref _lineNumber);
            if (!(nextToken.TokenType == TokenType.Punctuation && nextToken.Value == ")"))
            {
                throw new WrongFormatException(nextToken, new Token(")", TokenType.Punctuation), this.lineNumber);
            }

            // if this UDA is label then it is ready
            if (resultUDA.ValueType == UDATypes.Label || resultUDA.ValueType == UDATypes.label2
                || resultUDA.ValueType == UDATypes.label3)
            {
                return resultUDA;
            }

            // Dynamic value types do not need fixed values, real options pulled from core
            if (resultUDA.ValueType == UDATypes.stud_length || resultUDA.ValueType == UDATypes.stud_size
                || resultUDA.ValueType == UDATypes.stud_standard || resultUDA.ValueType == UDATypes.bolt_type
                || resultUDA.ValueType == UDATypes.Bolt_standard || resultUDA.ValueType == UDATypes.Bolt_standard)
            {
                return resultUDA;
            }

            // symbol '{'
            this.TryGetPunctuation(inputStream, "{");

            // attribute values
            // minimum 1 value should be presented
            var val = this.TryGetAttributeValue(inputStream);
            if (val == null)
            {
                throw new WrongFormatException(new Token("At least one attribute definition", TokenType.Identifier));
            }

            resultUDA.Values = new List<UDAValue>();
            do
            {
                resultUDA.Values.Add(val);
                val = this.TryGetAttributeValue(inputStream);
            }
            while (val != null);

            // symbol '}'
            this.TryGetPunctuation(inputStream, "}");

            // success end
            return resultUDA;
        }

        #endregion
    }
}