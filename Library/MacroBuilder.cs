namespace Tekla.Structures
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Macro builder.
    /// </summary>
    public sealed class MacroBuilder
    {
        #region Static Fields

        /// <summary>
        /// File name format.
        /// </summary>
        private const string FileNameFormat = "macro_{0:00}.cs";

        /// <summary>
        /// Maximum number of temporary files to use.
        /// </summary>
        private const int MaxTempFiles = 32;

        /// <summary>
        /// Random number source.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Temporary file index.
        /// </summary>
        private static int tempFileIndex = -1;

        #endregion

        #region Fields

        /// <summary>
        /// Macro text.
        /// </summary>
        private readonly StringBuilder macro;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroBuilder"/> class. 
        /// Initializes a new instance of the class.
        /// </summary>
        public MacroBuilder()
        {
            this.macro = new StringBuilder();
        }

        /// <summary>Initializes a new instance of the <see cref="MacroBuilder"/> class. 
        /// Initializes a new instance of the class.</summary>
        /// <param name="script">Script text.</param>
        public MacroBuilder(string script)
        {
            this.macro = new StringBuilder(script);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Activates a field on a dialog.</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="field">Field name.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder Activate(string dialog, string field)
        {
            return this.AppendMethodCall("Activate", dialog, field);
        }

        /// <summary>Invokes a callback.</summary>
        /// <param name="callback">Callback name.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder Callback(string callback)
        {
            return this.Callback(callback, string.Empty);
        }

        /// <summary>Invokes a callback.</summary>
        /// <param name="callback">Callback name.</param>
        /// <param name="parameter">Callback parameter.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder Callback(string callback, string parameter)
        {
            return this.Callback(callback, parameter, "main_frame");
        }

        /// <summary>Invokes a callback.</summary>
        /// <param name="callback">Callback name.</param>
        /// <param name="parameter">Callback parameter.</param>
        /// <param name="frame">Target frame.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder Callback(string callback, string parameter, string frame)
        {
            return this.AppendMethodCall("Callback", callback, parameter, frame);
        }

        /// <summary>Checks or unchecks a field.</summary>
        /// <param name="name">Field name.</param>
        /// <param name="value">Check value.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder CheckValue(string name, int value)
        {
            return this.AppendMethodCall("CheckValue", name, value);
        }

        /// <summary>
        /// Ends a command.
        /// </summary>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder CommandEnd()
        {
            return this.AppendMethodCall("CommandEnd");
        }

        /// <summary>Starts a command.</summary>
        /// <param name="command">Command name.</param>
        /// <param name="parameter">Command parameter.</param>
        /// <param name="frame">Target frame.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder CommandStart(string command, string parameter, string frame)
        {
            return this.AppendMethodCall("CommandStart", command, parameter, frame);
        }

        /// <summary>Performs file selection.</summary>
        /// <param name="items">Items to select.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder FileSelection(params string[] items)
        {
            this.macro.Append("akit.FileSelection(");

            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                {
                    this.macro.Append(", ");
                }

                this.macro.Append('"').Append(items[i]).Append('"');
            }

            this.macro.AppendLine(");");
            return this;
        }

        /// <summary>Selects items from a list field.</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="field">Field name.</param>
        /// <param name="items">List items.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder ListSelect(string dialog, string field, params string[] items)
        {
            this.macro.AppendFormat(@"akit.ListSelect(""{0}"", ""{1}""", dialog, field);

            for (var i = 0; i < items.Length; i++)
            {
                this.macro.Append(", ").Append('"').Append(items[i]).Append('"');
            }

            this.macro.AppendLine(");");
            return this;
        }

        /// <summary>Invokes a modal dialog.</summary>
        /// <param name="value">Modal dialog value.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder ModalDialog(int value)
        {
            return this.AppendMethodCall("ModalDialog", value);
        }

        /// <summary>Simulates a mouse button down event.</summary>
        /// <param name="frame">Frame name.</param>
        /// <param name="subframe">Subframe name.</param>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="modifier">Mouse button modifiers.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder MouseDown(string frame, string subframe, int x, int y, int modifier)
        {
            return this.AppendMethodCall("MouseDown", frame, subframe, x, y, modifier);
        }

        /// <summary>Simulates a mouse button up event.</summary>
        /// <param name="frame">Frame name.</param>
        /// <param name="subframe">Subframe name.</param>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="modifier">Mouse button modifiers.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder MouseUp(string frame, string subframe, int x, int y, int modifier)
        {
            return this.AppendMethodCall("MouseUp", frame, subframe, x, y, modifier);
        }

        /// <summary>Pushes a button.</summary>
        /// <param name="button">Button name.</param>
        /// <param name="frame">Frame name.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder PushButton(string button, string frame)
        {
            return this.AppendMethodCall("PushButton", button, frame);
        }

        /// <summary>
        /// Runs the constructed macro.
        /// </summary>
        public void Run()
        {
            this.Run(TeklaStructures.Connection);
        }

        /// <summary>Runs the constructed macro.</summary>
        /// <param name="connection">Macro runner connection.</param>
        public void Run(IConnection connection)
        {
            try
            {
                var name = GetMacroFileName();

                File.WriteAllText(Path.Combine(TeklaStructures.Environment.MacrosFolder, name), "namespace Tekla.Technology.Akit.UserScript {" + "public class Script {" + "public static void Run(Tekla.Technology.Akit.IScript akit) {" + this.macro.ToString() + "}" + "}" + "}");

                connection.RunMacro("..\\" + name);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>Changes the active tab page.</summary>
        /// <param name="dialog">The Dialog name.</param>
        /// <param name="field">The Field name.</param>
        /// <param name="item">The Tab page.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder TabChange(string dialog, string field, string item)
        {
            return this.AppendMethodCall("TabChange", dialog, field, item);
        }

        /// <summary>Selects items on a table field.</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="field">Field name.</param>
        /// <param name="items">Table items.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder TableSelect(string dialog, string field, params int[] items)
        {
            this.macro.AppendFormat(@"akit.TableSelect(""{0}"", ""{1}""", dialog, field);

            for (var i = 0; i < items.Length; i++)
            {
                this.macro.Append(", ").Append(items[i]);
            }

            this.macro.AppendLine(");");
            return this;
        }

        /// <summary>Change values from table</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="table">Table name.</param>
        /// <param name="field">Field name.</param>
        /// <param name="value">Value.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder TableValueChange(string dialog, string table, string field, string value)
        {
            this.macro.AppendFormat(@"akit.TableValueChange(""{0}"",""{1}"",""{2}"",""{3}"")", 
                dialog, table, field, value);
            this.macro.AppendLine(";");
            return this;
        }

        /// <summary>
        /// Returns the constructed macro text.
        /// </summary>
        /// <returns>Macro text.</returns>
        public override string ToString()
        {
            return this.macro.ToString();
        }

        /// <summary>Selects items in a tree field.</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="field">Field name.</param>
        /// <param name="rowstring">Tree row string.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder TreeSelect(string dialog, string field, string rowstring)
        {
            return this.AppendMethodCall("TreeSelect", dialog, field, rowstring);
        }

        /// <summary>Changes a field value.</summary>
        /// <param name="dialog">Dialog name.</param>
        /// <param name="field">Field name.</param>
        /// <param name="data">Field value.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        public MacroBuilder ValueChange(string dialog, string field, string data)
        {
            return this.AppendMethodCall("ValueChange", dialog, field, data);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the generated macro file name.
        /// </summary>
        /// <returns>Generated macro file name.</returns>
        private static string GetMacroFileName()
        {
            lock (Random)
            {
                if (tempFileIndex < 0)
                {
                    tempFileIndex = Random.Next(0, MaxTempFiles);
                }
                else
                {
                    tempFileIndex = (tempFileIndex + 1) % MaxTempFiles;
                }

                return string.Format(FileNameFormat, tempFileIndex);
            }
        }

        /// <summary>Appends a method call to macro.</summary>
        /// <param name="method">Method to call.</param>
        /// <param name="arguments">Argument list.</param>
        /// <returns>Reference to self for fluent interface pattern.</returns>
        private MacroBuilder AppendMethodCall(string method, params object[] arguments)
        {
            this.macro.Append("akit.").Append(method).Append('(');

            for (var i = 0; i < arguments.Length; i++)
            {
                if (i > 0)
                {
                    this.macro.Append(", ");
                }

                if (arguments[i] is string)
                {
                    this.macro.Append('"').Append(arguments[i]).Append('"');
                }
                else
                {
                    this.macro.Append(arguments[i]);
                }
            }

            this.macro.AppendLine(");");
            return this;
        }

        #endregion
    }
}