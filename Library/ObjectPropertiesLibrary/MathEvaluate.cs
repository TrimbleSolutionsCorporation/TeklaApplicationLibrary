namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Collections;
    using System.Globalization;

    /// <summary>
    /// The math evaluate.
    /// </summary>
    public class MathEvaluate
    {
        #region Constants

        /// <summary>
        /// The dividebyzero.
        /// </summary>
        public const string DivideByZero = "Divide by Zero";

        #endregion

        #region Fields

        /// <summary>
        /// The equation.
        /// </summary>
        private readonly ArrayList equation = new ArrayList();

        /// <summary>
        /// The post fix.
        /// </summary>
        private readonly ArrayList postFix = new ArrayList();

        /// <summary>
        /// The default function evaluation.
        /// </summary>
        private EvaluateFunctionDelegate defaultFunctionEvaluation;

        /// <summary>
        /// The error description.
        /// </summary>
        private string errorDescription = "None";

        /// <summary>
        /// The evaluation error.
        /// </summary>
        private bool evaluationError;

        /// <summary>
        /// The evaluation result.
        /// </summary>
        private double evaluationResult;

        #endregion

        #region Delegates

        /// <summary>
        /// The evaluate function delegate.
        /// </summary>
        /// <param name="name">
        /// The name value.
        /// </param>
        /// <param name="args">
        /// The args value.
        /// </param>
        /// <returns>Returns a symbol object.
        /// </returns>
        public delegate Symbol EvaluateFunctionDelegate(string name, params object[] args);

        #endregion

        #region Enums

        /// <summary>
        /// The type value.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// The variable.
            /// </summary>
            Variable, 

            /// <summary>
            /// The value.
            /// </summary>
            Value, 

            /// <summary>
            /// The operator.
            /// </summary>
            Operator, 

            /// <summary>
            /// The function.
            /// </summary>
            Function, 

            /// <summary>
            /// The result.
            /// </summary>
            Result, 

            /// <summary>
            /// The bracket.
            /// </summary>
            Bracket, 

            /// <summary>
            /// The comma.
            /// </summary>
            Comma, 

            /// <summary>
            /// The error.
            /// </summary>
            Error
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the default function evaluation.</summary>
        /// <value>The default function evaluation.</value>
        public EvaluateFunctionDelegate DefaultFunctionEvaluation
        {
            get
            {
                return this.defaultFunctionEvaluation;
            }

            set
            {
                this.defaultFunctionEvaluation = value;
            }
        }

        /// <summary>Gets the equation.</summary>
        /// <value>The equation.</value>
        public ArrayList Equation
        {
            get
            {
                return (ArrayList)this.equation.Clone();
            }
        }

        /// <summary>Gets a value indicating whether error.</summary>
        /// <value>The error.</value>
        public bool Error
        {
            get
            {
                return this.evaluationError;
            }
        }

        /// <summary>Gets the error description.</summary>
        /// <value>The error description.</value>
        public string ErrorDescription
        {
            get
            {
                return this.errorDescription;
            }
        }

        /// <summary>Gets the postfix.</summary>
        /// <value>The postfix.</value>
        public ArrayList Postfix
        {
            get
            {
                return (ArrayList)this.postFix.Clone();
            }
        }

        /// <summary>Gets the result.</summary>
        /// <value>The result.</value>
        public double Result
        {
            get
            {
                return this.evaluationResult;
            }
        }

        /// <summary>Gets or sets the variables.</summary>
        /// <value>The variables.</value>
        public ArrayList Variables
        {
            get
            {
                var var = new ArrayList();
                foreach (Symbol sym in this.equation)
                {
                    if ((sym.SymbolType == Type.Variable) && (!var.Contains(sym)))
                    {
                        var.Add(sym);
                    }
                }

                return var;
            }

            set
            {
                int count;
                foreach (Symbol sym in value)
                {
                    for (count = 0; count < this.postFix.Count; count++)
                    {
                        if ((sym.SymbolName == ((Symbol)this.postFix[count]).SymbolName)
                            && (((Symbol)this.postFix[count]).SymbolType == Type.Variable))
                        {
                            var sym1 = (Symbol)this.postFix[count];
                            sym1.SymbolValue = sym.SymbolValue;
                            this.postFix[count] = sym1;
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The evaluate postfix.
        /// </summary>
        public void EvaluatePostfix()
        {
            Symbol sym1, sym2, evaluateFunction;
            var stack = new Stack();
            var param = new ArrayList();
            this.evaluationError = false;
            foreach (Symbol sym in this.postFix)
            {
                if ((sym.SymbolType == Type.Value) || (sym.SymbolType == Type.Variable)
                    || (sym.SymbolType == Type.Result))
                {
                    stack.Push(sym);
                }
                else if (sym.SymbolType == Type.Operator)
                {
                    if (stack.Count < 2)
                    {
                        this.evaluationError = true;
                        break;
                    }
                    else
                    {
                        sym1 = (Symbol)stack.Pop();
                        sym2 = (Symbol)stack.Pop();
                        evaluateFunction = this.Evaluate(sym2, sym, sym1);
                        if (evaluateFunction.SymbolType == Type.Error)
                        {
                            this.evaluationError = true;
                            this.errorDescription = evaluateFunction.SymbolName;
                        }

                        if (this.evaluationError == false)
                        {
                            stack.Push(evaluateFunction);
                        }
                    }
                }
                else if (sym.SymbolType == Type.Function)
                {
                    param.Clear();
                    sym1 = (Symbol)stack.Pop();
                    if ((sym1.SymbolType == Type.Value) || (sym1.SymbolType == Type.Variable)
                        || (sym1.SymbolType == Type.Result))
                    {
                        evaluateFunction = this.EvaluateFunction(sym.SymbolName, sym1);
                        if (evaluateFunction.SymbolType == Type.Error)
                        {
                            this.evaluationError = true;
                            this.errorDescription = evaluateFunction.SymbolName;
                        }

                        if (this.evaluationError == false)
                        {
                            stack.Push(evaluateFunction);
                        }
                    }
                    else if (sym1.SymbolType == Type.Comma)
                    {
                        while (sym1.SymbolType == Type.Comma)
                        {
                            sym1 = (Symbol)stack.Pop();
                            param.Add(sym1);
                            sym1 = (Symbol)stack.Pop();
                        }

                        param.Add(sym1);
                        evaluateFunction = this.EvaluateFunction(sym.SymbolName, param.ToArray());
                        if (evaluateFunction.SymbolType == Type.Error)
                        {
                            this.evaluationError = true;
                            this.errorDescription = evaluateFunction.SymbolName;
                        }

                        if (this.evaluationError == false)
                        {
                            stack.Push(evaluateFunction);
                        }
                    }
                    else
                    {
                        stack.Push(sym1);
                        evaluateFunction = this.EvaluateFunction(sym.SymbolName);
                        if (evaluateFunction.SymbolType == Type.Error)
                        {
                            this.evaluationError = true;
                            this.errorDescription = evaluateFunction.SymbolName;
                        }

                        if (this.evaluationError == false)
                        {
                            stack.Push(evaluateFunction);
                        }
                    }
                }
            }

            if (this.evaluationError == false && stack.Count == 1)
            {
                evaluateFunction = (Symbol)stack.Pop();
                this.evaluationResult = evaluateFunction.SymbolValue;
            }
        }

        /// <summary>
        /// The infix 2 postfix.
        /// </summary>
        public void Infix2Postfix()
        {
            Symbol symbol;
            var stack = new Stack();
            foreach (Symbol sym in this.equation)
            {
                if ((sym.SymbolType == Type.Value) || (sym.SymbolType == Type.Variable))
                {
                    this.postFix.Add(sym);
                }
                else if ((sym.SymbolName == "(") || (sym.SymbolName == "[") || (sym.SymbolName == "{"))
                {
                    stack.Push(sym);
                }
                else if ((sym.SymbolName == ")") || (sym.SymbolName == "]") || (sym.SymbolName == "}"))
                {
                    if (stack.Count > 0)
                    {
                        symbol = (Symbol)stack.Pop();
                        while ((symbol.SymbolName != "(") && (symbol.SymbolName != "[") && (symbol.SymbolName != "{"))
                        {
                            this.postFix.Add(symbol);
                            symbol = (Symbol)stack.Pop();
                        }
                    }
                }
                else
                {
                    if (stack.Count > 0)
                    {
                        symbol = (Symbol)stack.Pop();
                        while ((stack.Count != 0)
                               &&
                               ((symbol.SymbolType == Type.Operator) || (symbol.SymbolType == Type.Function)
                                || (symbol.SymbolType == Type.Comma)) && (this.Precedence(symbol) >= this.Precedence(sym)))
                        {
                            this.postFix.Add(symbol);
                            symbol = (Symbol)stack.Pop();
                        }

                        if (((symbol.SymbolType == Type.Operator) || (symbol.SymbolType == Type.Function)
                             || (symbol.SymbolType == Type.Comma)) && (this.Precedence(symbol) >= this.Precedence(sym)))
                        {
                            this.postFix.Add(symbol);
                        }
                        else
                        {
                            stack.Push(symbol);
                        }
                    }

                    stack.Push(sym);
                }
            }

            while (stack.Count > 0)
            {
                symbol = (Symbol)stack.Pop();
                this.postFix.Add(symbol);
            }
        }

        /// <summary>The parse.</summary>
        /// <param name="newEquation">The equation.</param>
        /// <returns>The System.Boolean.</returns>
        public bool Parse(string newEquation)
        {
            var result = true;
            int state = 1, count;
            var tempEquation = string.Empty;
            Symbol symbol;
            var currentCulture = CultureInfo.CurrentCulture;
            var cultureInfoUs = new CultureInfo("en-us");

            this.evaluationError = false;
            this.errorDescription = "None";

            this.equation.Clear();
            this.postFix.Clear();

            int pos;

            // -- Remove all white spaces from the Equation string --
            newEquation = newEquation.Trim();
            while ((pos = newEquation.IndexOf(' ')) != -1)
            {
                newEquation = newEquation.Remove(pos, 1);
            }

            for (count = 0; count < newEquation.Length; count++)
            {
                switch (state)
                {
                    case 1:
                        if (char.IsNumber(newEquation[count]))
                        {
                            state = 2;
                            tempEquation += newEquation[count];
                        }
                        else if (char.IsLetter(newEquation[count]))
                        {
                            state = 3;
                            tempEquation += newEquation[count];
                        }
                        else
                        {
                            symbol.SymbolName = newEquation[count].ToString();
                            symbol.SymbolValue = 0;
                            switch (symbol.SymbolName)
                            {
                                case ",":
                                    symbol.SymbolType = Type.Comma;
                                    break;
                                case "(":
                                case ")":
                                case "[":
                                case "]":
                                case "{":
                                case "}":
                                    symbol.SymbolType = Type.Bracket;
                                    break;
                                default:
                                    symbol.SymbolType = Type.Operator;
                                    break;
                            }

                            this.equation.Add(symbol);
                        }

                        break;
                    case 2:
                        if (char.IsNumber(newEquation[count]) || (newEquation[count] == '.') ||
                            (currentCulture.NumberFormat.NumberDecimalSeparator.Length == 1 &&
                            newEquation[count] == currentCulture.NumberFormat.NumberDecimalSeparator[0]))
                        {
                            tempEquation += newEquation[count];
                        }
                        else if (!char.IsLetter(newEquation[count]))
                        {
                            state = 1;
                            symbol.SymbolName = tempEquation;

                            result = double.TryParse(tempEquation, NumberStyles.Any, currentCulture, out symbol.SymbolValue);

                            if (result == false)
                            {
                                result = double.TryParse(tempEquation, NumberStyles.Any, cultureInfoUs, out symbol.SymbolValue);
                            }

                            symbol.SymbolType = Type.Value;
                            this.equation.Add(symbol);
                            symbol.SymbolName = newEquation[count].ToString();
                            symbol.SymbolValue = 0;
                            switch (symbol.SymbolName)
                            {
                                case ",":
                                    symbol.SymbolType = Type.Comma;
                                    break;
                                case "(":
                                case ")":
                                case "[":
                                case "]":
                                case "{":
                                case "}":
                                    symbol.SymbolType = Type.Bracket;
                                    break;
                                default:
                                    symbol.SymbolType = Type.Operator;
                                    break;
                            }

                            this.equation.Add(symbol);
                            tempEquation = string.Empty;
                        }

                        break;
                    case 3:
                        if (char.IsLetterOrDigit(newEquation[count]))
                        {
                            tempEquation += newEquation[count];
                        }
                        else
                        {
                            state = 1;
                            symbol.SymbolName = tempEquation;
                            symbol.SymbolValue = 0;
                            if (newEquation[count] == '(')
                            {
                                symbol.SymbolType = Type.Function;
                            }
                            else
                            {
                                if (symbol.SymbolName == "pi")
                                {
                                    symbol.SymbolValue = Math.PI;
                                }
                                else if (symbol.SymbolName == "e")
                                {
                                    symbol.SymbolValue = Math.E;
                                }

                                symbol.SymbolType = Type.Variable;
                            }

                            this.equation.Add(symbol);
                            symbol.SymbolName = newEquation[count].ToString();
                            symbol.SymbolValue = 0;
                            switch (symbol.SymbolName)
                            {
                                case ",":
                                    symbol.SymbolType = Type.Comma;
                                    break;
                                case "(":
                                case ")":
                                case "[":
                                case "]":
                                case "{":
                                case "}":
                                    symbol.SymbolType = Type.Bracket;
                                    break;
                                default:
                                    symbol.SymbolType = Type.Operator;
                                    break;
                            }

                            this.equation.Add(symbol);
                            tempEquation = string.Empty;
                        }

                        break;
                }
            }

            if (tempEquation != string.Empty)
            {
                symbol.SymbolName = tempEquation;

                if (state == 2)
                {
                    result = double.TryParse(tempEquation, NumberStyles.Any, currentCulture, out symbol.SymbolValue);

                    if (result == false)
                    {
                        result = double.TryParse(tempEquation, NumberStyles.Any, cultureInfoUs, out symbol.SymbolValue);
                    }

                    if (result == false)
                    {
                        this.evaluationError = true;
                    }

                    symbol.SymbolType = Type.Value;
                }
                else
                {
                    if (symbol.SymbolName == "pi")
                    {
                        symbol.SymbolValue = Math.PI;
                    }
                    else if (symbol.SymbolName == "e")
                    {
                        symbol.SymbolValue = Math.E;
                    }
                    else
                    {
                        symbol.SymbolValue = 0;
                    }

                    symbol.SymbolType = Type.Variable;
                }

                this.equation.Add(symbol);
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>The evaluate.</summary>
        /// <param name="sym1">The sym 1 value.</param>
        /// <param name="opr">The opr value.</param>
        /// <param name="sym2">The sym 2 value.</param>
        /// <returns>The Tekla.Structures.ObjectPropertiesLibrary.MathEvaluate+Symbol.</returns>
        protected Symbol Evaluate(Symbol sym1, Symbol opr, Symbol sym2)
        {
            Symbol result;
            result.SymbolName = sym1.SymbolName + opr.SymbolName + sym2.SymbolName;
            result.SymbolType = Type.Result;
            result.SymbolValue = 0;
            switch (opr.SymbolName)
            {
                case "^":
                    result.SymbolValue = Math.Pow(sym1.SymbolValue, sym2.SymbolValue);
                    break;
                case "/":
                    {
                        if (sym2.SymbolValue > 0)
                        {
                            result.SymbolValue = sym1.SymbolValue / sym2.SymbolValue;
                        }
                        else
                        {
                            result.SymbolName = DivideByZero;
                            result.SymbolType = Type.Error;
                        }

                        break;
                    }

                case "*":
                    result.SymbolValue = sym1.SymbolValue * sym2.SymbolValue;
                    break;
                case "%":
                    result.SymbolValue = sym1.SymbolValue % sym2.SymbolValue;
                    break;
                case "+":
                    result.SymbolValue = sym1.SymbolValue + sym2.SymbolValue;
                    break;
                case "-":
                    result.SymbolValue = sym1.SymbolValue - sym2.SymbolValue;
                    break;
                default:
                    result.SymbolType = Type.Error;
                    result.SymbolName = "Undefine operator: " + opr.SymbolName + ".";
                    break;
            }

            return result;
        }

        /// <summary>The evaluate function.</summary>
        /// <param name="name">The name value.</param>
        /// <param name="args">The args value.</param>
        /// <returns>The Tekla.Structures.ObjectPropertiesLibrary.MathEvaluate+Symbol.</returns>
        protected Symbol EvaluateFunction(string name, params object[] args)
        {
            Symbol result;
            result.SymbolName = string.Empty;
            result.SymbolType = Type.Result;
            result.SymbolValue = 0;
            switch (name)
            {
                case "cos":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Cos(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "sin":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Sin(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "tan":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Tan(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "cosh":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Cosh(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "sinh":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Sinh(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "tanh":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Tanh(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "log":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Log10(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "ln":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Log(((Symbol)args[0]).SymbolValue, 2);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "logn":
                    if (args.Length == 2)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + "'"
                                             + ((Symbol)args[1]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Log(((Symbol)args[0]).SymbolValue, ((Symbol)args[1]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "sqrt":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Sqrt(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "abs":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Abs(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "acos":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Acos(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "asin":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Asin(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "atan":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Atan(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                case "exp":
                    if (args.Length == 1)
                    {
                        result.SymbolName = name + "(" + ((Symbol)args[0]).SymbolValue.ToString() + ")";
                        result.SymbolValue = Math.Exp(((Symbol)args[0]).SymbolValue);
                    }
                    else
                    {
                        result.SymbolName = "Invalid number of parameters in: " + name + ".";
                        result.SymbolType = Type.Error;
                    }

                    break;
                default:
                    if (this.DefaultFunctionEvaluation != null)
                    {
                        result = this.DefaultFunctionEvaluation(name, args);
                    }
                    else
                    {
                        result.SymbolName = "Function: " + name + ", not found.";
                        result.SymbolType = Type.Error;
                    }

                    break;
            }

            return result;
        }

        /// <summary>The precedence.</summary>
        /// <param name="sym">The sym value.</param>
        /// <returns>The System.Int32.</returns>
        protected int Precedence(Symbol sym)
        {
            var result = -1;

            switch (sym.SymbolType)
            {
                case Type.Bracket:
                    result = 5;
                    break;
                case Type.Function:
                    result = 4;
                    break;
                case Type.Comma:
                    result = 0;
                    break;
            }

            switch (sym.SymbolName)
            {
                case "^":
                    result = 3;
                    break;
                case "/":
                case "*":
                case "%":
                    result = 2;
                    break;
                case "+":
                case "-":
                    result = 1;
                    break;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// The symbol.
        /// </summary>
        public struct Symbol
        {
            #region Fields

            /// <summary>
            /// The _ symbol name.
            /// </summary>
            public string SymbolName;

            /// <summary>
            /// The _ symbol type.
            /// </summary>
            public Type SymbolType;

            /// <summary>
            /// The _ symbol value.
            /// </summary>
            public double SymbolValue;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The System.String.
            /// </returns>
            public override string ToString()
            {
                return this.SymbolName;
            }

            #endregion
        }
    }
}