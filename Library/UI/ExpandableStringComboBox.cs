namespace Tekla.Structures.UI
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// ExpandableStringComboBox extends standard ComboBox. 
    /// It provides, e.g., 
    /// 1) functionality to add strings to the items list by editing a new string in the text field and
    /// 2) possibility to add a delegate for retrieving new string from implementation specific source.
    /// 3) possibility to add a delegate for saving a new string to, e.g., implementation specific container.
    /// </summary>
    /// <example>
    /// The following example creates an ExpandableStringComboBox with non-default ListTerminator and adds 
    /// implementation specific delegates to the instance.
    /// <code>
    /// using System;
    /// using System.Diagnostics;
    /// using System.Windows.Forms;
    /// using Tekla.Structures.UI;
    /// 
    /// namespace Sample
    /// {
    ///     public partial class Sample : Form
    ///     {
    ///         private readonly ExpandableStringComboBox _ExScb;
    /// 
    ///         public Sample()
    ///         {
    ///             InitializeComponent();
    ///             _ExScb = new ExpandableStringComboBox()
    ///                          {
    ///                              ListTerminator = "- new -",
    ///                              Location = new System.Drawing.Point(10, 10),
    ///                              Size = new System.Drawing.Size(120, 20)
    ///                             
    ///                          };
    /// 
    ///             _ExScb.GetNewStringEvent += GetNewStringHandler;
    ///             _ExScb.AddNewStringEvent += AddNewStringHandler;
    /// 
    ///             string[] SampleData = { "TT600*2400-100-50-1200-0.04-200-0.25", "P27(265X1200)", "P32(320X1200)" };
    ///             _ExScb.Initialize( SampleData, SampleData[ 0 ]);
    /// 
    ///             Controls.Add(_ExScb);
    ///         }
    /// 
    ///         public void GetNewStringHandler(object Sender, ExpandableStringComboBox.ExpandableStringComboBoxEvent Event)
    ///         {
    ///             Event.Result = DateTime.Now.ToLongTimeString();
    ///         }
    /// 
    ///         public void AddNewStringHandler(object Sender, ExpandableStringComboBox.ExpandableStringComboBoxEvent Event)
    ///         {
    ///             Trace.WriteLine(string.Format("AddNewStringHandler {0}", Event.Input));
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class ExpandableStringComboBox : ComboBox
    {
        #region Fields

        /// <summary>
        /// The _ previous index.
        /// </summary>
        private int previousIndex;

        /// <summary>
        /// The _ state.
        /// </summary>
        private Status state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableStringComboBox"/> class.
        /// </summary>
        public ExpandableStringComboBox()
        {
            this.DropDownStyle = ComboBoxStyle.DropDown;

            this.SelectionChangeCommitted += this.SelectionChangeCommittedHandler;
            this.TextUpdate += this.TextUpdateHandler;
            this.Leave += this.LeaveHandler;

            this.state = Status.Unknown;
            this.AddNewStringEvent += AddToListEvent;

            this.ListTerminator = "..."; // default terminator
            this.previousIndex = -1;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Client implementation can preserve the string that does not exist on the list.
        /// </summary>
        /// <param name="sender">Instance of the control triggering the event.</param>
        /// <param name="Event">Event.Input contains the new string.</param>
        public delegate void AddNewStringHandler(object sender, ExpandableStringComboBoxEvent Event);

        /// <summary>
        /// Client implementation uses delegate to provide a new string to be added to the list.
        /// </summary>
        /// <param name="sender">Instance of the control triggering the event.</param>
        /// <param name="Event">Event.Input contains the current selection. Client should add new string into <b>Event.Result.</b>.</param>
        public delegate void GetNewStringHandler(object sender, ExpandableStringComboBoxEvent Event);

        #endregion

        #region Public Events

        /// <summary>
        /// The add new string event.
        /// </summary>
        public event AddNewStringHandler AddNewStringEvent;

        /// <summary>
        /// The get new string event.
        /// </summary>
        public event GetNewStringHandler GetNewStringEvent;

        #endregion

        #region Enums

        /// <summary>
        /// The status.
        /// </summary>
        private enum Status
        {
            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown, 

            /// <summary>
            /// The old value.
            /// </summary>
            Old, 

            /// <summary>
            /// The new value.
            /// </summary>
            New, 

            /// <summary>
            /// The invalid.
            /// </summary>
            Invalid, 
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the property to be used to mark the list item that triggers GetNewStringDelegates.</summary>
        /// <value>The list terminator.</value>
        public string ListTerminator { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Initializes control with provided data and sets text part according to current value. The ListTerminator is added to the end of the list.</summary>
        /// <param name="data">Strings to initialize the list.</param>
        /// <param name="currentValue">Value that is used as selected value. CurrentValue is added to the list if it does not exist in Data.</param>
        public void Initialize(string[] data, string currentValue = null)
        {
            this.Items.Clear();
            this.Items.AddRange(data);

            if (this.ListTerminator != null)
            {
                this.Items.Add(this.ListTerminator);
            }

            if (!string.IsNullOrEmpty(currentValue))
            {
                if (!this.Items.Contains(currentValue))
                {
                    this.AddNewString(currentValue);
                }

                this.previousIndex = this.Items.IndexOf(currentValue);
                this.Text = currentValue;
                this.state = Status.Old;
            }
            else
            {
                this.previousIndex = -1;
                this.state = Status.Unknown;
            }
        }

        /// <summary>Adds, if necessary, the given value to items and sets the current selection to the given value. 
        /// Method automatically adds ListTerminator to items if the ListTerminator is set (not null) and it is not already in items.</summary>
        /// <param name="currentValue">Value to be selected.</param>
        public void Select(string currentValue)
        {
            if (this.ListTerminator != null && !this.Items.Contains(this.ListTerminator))
            {
                this.Items.Add(this.ListTerminator);
            }

            if (!this.Items.Contains(currentValue))
            {
                this.AddNewString(currentValue);
            }

            this.previousIndex = this.Items.IndexOf(currentValue);
            this.Text = currentValue;
            this.state = Status.Old;
        }

        #endregion

        #region Methods

        /// <summary>Internal delegate to handle adding new string to items.</summary>
        /// <param name="sender">The sender value.</param>
        /// <param name="expandableStringComboBoxEvent">The expandableStringComboBoxEvent value.</param>
        private static void AddToListEvent(object sender, ExpandableStringComboBoxEvent expandableStringComboBoxEvent)
        {
            var ctrl = (ExpandableStringComboBox)sender;
            if (ctrl.ListTerminator != null)
            {
                ctrl.Items.Insert(ctrl.Items.Count - 1, expandableStringComboBoxEvent.Input); // combo will sort automatically, if Sort=true
            }
            else
            {
                ctrl.Items.Add(expandableStringComboBoxEvent.Input);
            }
        }

        /// <summary>Method that triggers combo (and client) to add new string.</summary>
        /// <param name="newString">Value of the new item.</param>
        private void AddNewString(string newString)
        {
            this.AddNewStringEvent(this, new ExpandableStringComboBoxEvent(newString));
        }

        /// <summary>Method to handle Leave-event. The implementation calls AddNewString -method if necessary to add user modified new value to items.</summary>
        /// <param name="sender">The sender value.</param>
        /// <param name="e">The e value.</param>
        private void LeaveHandler(object sender, EventArgs e)
        {
            if (this.state == Status.New)
            {
                this.AddNewString(this.Text);
                this.SelectedIndex = this.previousIndex = this.Items.IndexOf(this.Text);
            }
            else if (this.state == Status.Invalid)
            {
                this.SelectedItem = (-1 != this.previousIndex) ? this.Items[this.previousIndex] : null;
            }

            this.state = Status.Old;
        }

        /// <summary>Internal handler to handle user selection. The method triggers GetNewStringEvent if user selects the ListTerminator-option.</summary>
        /// <param name="sender">The sender value.</param>
        /// <param name="eventArgs">The eventArgs value.</param>
        private void SelectionChangeCommittedHandler(object sender, EventArgs eventArgs)
        {
            if (this.ListTerminator != null && this.ListTerminator.Equals(this.SelectedItem))
            {
                var previousValue = (-1 != this.previousIndex) ? this.Items[this.previousIndex] as string : string.Empty;
                string result = null;

                if (this.GetNewStringEvent != null)
                {
                    var newEvent = new ExpandableStringComboBoxEvent(previousValue);
                    this.GetNewStringEvent(this, newEvent);
                    result = newEvent.Result;
                }

                if (result != null)
                {
                    if (!this.Items.Contains(result))
                    {
                        // add only if value not in the list
                        this.AddNewString(result);
                    }

                    this.SelectedIndex = this.previousIndex = this.Items.IndexOf(result);

                    this.Text = result;
                }
                else
                {
                    // restore previous
                    this.SelectedIndex = this.previousIndex;
                }
            }
            else
            {
                this.previousIndex = this.SelectedIndex;
            }
        }

        /// <summary>Method to handle user made modifications to the value in the text field.</summary>
        /// <param name="sender">The sender value.</param>
        /// <param name="e">The e value.</param>
        private void TextUpdateHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.state = this.Items.Contains(this.Text) ? Status.Old : Status.New;
            }
            else
            {
                this.state = Status.Invalid;
            }
        }

        #endregion

        /// <summary>
        /// Event used by ExpandableStringComboBox to retrieve and save strings.
        /// </summary>
        public class ExpandableStringComboBoxEvent : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="ExpandableStringComboBoxEvent"/> class. 
            /// Constucts a new event.</summary>
            /// <param name="strInput">String value for input.</param>
            public ExpandableStringComboBoxEvent(string strInput)
            {
                this.Input = strInput;
                this.Result = null;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the currently selected value or new string.</summary>
            /// <value>The input.</value>
            public string Input { get; set; }

            /// <summary>Gets or sets the return value from a delegate.</summary>
            /// <value>The result.</value>
            public string Result { get; set; }

            #endregion
        }
    }
}