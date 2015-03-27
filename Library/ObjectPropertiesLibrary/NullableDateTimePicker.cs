namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The nullable date time picker.
    /// </summary>
    public class NullableDateTimePicker : DateTimePicker
    {
        // true, when no date shall be displayed (empty DateTimePicker)

        // The format of the DateTimePicker control
        #region Fields

        /// <summary>
        /// The _format.
        /// </summary>
        private DateTimePickerFormat format = DateTimePickerFormat.Long;

        // The custom format of the DateTimePicker control

        // The format of the DateTimePicker control as string

        /// <summary>
        /// The _format as string.
        /// </summary>
        private string formatAsString;

        /// <summary>
        /// The _is null.
        /// </summary>
        private bool isNull;

        // If _isNull = true, this value is shown in the DTP
 
        /// <summary>
        /// The _null value.
        /// </summary>
        private string nullValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NullableDateTimePicker"/> class. 
        /// Default Constructor.</summary>
        public NullableDateTimePicker()
            : base()
        {
            base.Format = DateTimePickerFormat.Custom;
            this.NullValue = " ";
            this.Format = DateTimePickerFormat.Long;
            this.DataBindings.CollectionChanged += this.DataBindingsCollectionChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom date/time format string.
        /// <value>A string that represents the custom date/time format. The default is a null
        /// reference (<b>Nothing.</b> in Visual Basic).</value>
        /// </summary>
        /// <value>The custom format.</value>
        public new string CustomFormat { get; set; }

        /// <summary>
        /// Gets or sets the format of the date and time displayed in the control.
        /// </summary>
        /// <value>One of the <see cref="DateTimePickerFormat"/> values. The default is 
        /// <see cref="DateTimePickerFormat.Long"/>.</value>
        [Browsable(true)]
        [DefaultValue(DateTimePickerFormat.Long)]
        [TypeConverter(typeof(Enum))]
        public new DateTimePickerFormat Format
        {
            get
            {
                return this.format;
            }

            set
            {
                this.format = value;
                if (!this.isNull)
                {
                    this.SetFormat();
                }

                this.OnFormatChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the string value that is assigned to the control as null value. 
        /// </summary>
        /// <value>The string value assigned to the control as null value.</value>
        /// <remarks>
        /// If the <see cref="Value"/> is <b>null.</b>, <b>NullValue.</b> is
        /// shown in the <b>DateTimePicker.</b> control.
        /// </remarks>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The string used to display null values in the control")]
        [DefaultValue(" ")]
        public string NullValue
        {
            get
            {
                return this.nullValue;
            }

            set
            {
                this.nullValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the date/time value assigned to the control.
        /// </summary>
        /// <value>The DateTime value assigned to the control.
        /// </value>
        /// <remarks>
        /// <p>If the <b>Value.</b> property has not been changed in code or by the user, it is set
        /// to the current date and time (<see cref="DateTime.Now"/>).</p>
        /// <p>If <b>Value.</b> is <b>null.</b>, the DateTimePicker shows 
        /// <see cref="NullValue"/>.</p>
        /// </remarks>
        [Bindable(true)]
        [Browsable(false)]
        public new object Value
        {
            get
            {
                if (this.isNull)
                {
                    return null;
                }
                else
                {
                    return base.Value;
                }
            }

            set
            {
                if (value == null || value == DBNull.Value)
                {
                    this.SetToNullValue();
                }
                else
                {
                    this.SetToDateTimeValue();
                    base.Value = (DateTime)value;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current format of the DateTimePicker as string. 
        /// </summary>
        private string FormatAsString
        {
            get
            {
                return this.formatAsString;
            }

            set
            {
                this.formatAsString = value;
                base.CustomFormat = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>This member overrides <see cref="Control.OnKeyDown"/>.</summary>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> that was raised.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                this.Value = null;
                this.OnValueChanged(EventArgs.Empty);
            }

            base.OnKeyDown(e);
        }

        /// <summary>This member overrides <see cref="Control.OnKeyPress"/>.</summary>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> that was raised.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
            {
                this.Value = null;
                this.OnValueChanged(EventArgs.Empty);
            }

            base.OnKeyPress(e);
        }

        /// <summary>This member overrides <see cref="Control.WndProc"/>.</summary>
        /// <param name="m">The m value.</param>
        protected override void WndProc(ref Message m)
        {
            if (this.isNull)
            {
                if (m.Msg == 0x4e)
                {
                    // WM_NOTIFY
                    var nm = (Nmhdr)m.GetLParam(typeof(Nmhdr));
                    if (nm.Code == -746 || nm.Code == -722)
                    {
                        // DTN_CLOSEUP || DTN_?
                        this.SetToDateTimeValue();
                    }
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>The data bindings_ collection changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void DataBindingsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                this.DataBindings[this.DataBindings.Count - 1].Parse += this.NullableDateTimePickerParse;
            }
        }

        /// <summary>The nullable date time picker_ parse.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e value.</param>
        private void NullableDateTimePickerParse(object sender, ConvertEventArgs e)
        {
            // saves null values to the object
            if (this.isNull)
            {
                e.Value = null;
            }
        }

        /// <summary>
        /// Sets the format according to the current DateTimePickerFormat.
        /// </summary>
        private void SetFormat()
        {
            var ci = Thread.CurrentThread.CurrentCulture;
            var dtf = ci.DateTimeFormat;
            switch (this.format)
            {
                case DateTimePickerFormat.Long:
                    this.FormatAsString = dtf.LongDatePattern;
                    break;
                case DateTimePickerFormat.Short:
                    this.FormatAsString = dtf.ShortDatePattern;
                    break;
                case DateTimePickerFormat.Time:
                    this.FormatAsString = dtf.ShortTimePattern;
                    break;
                case DateTimePickerFormat.Custom:
                    this.FormatAsString = this.CustomFormat;
                    break;
            }
        }

        /// <summary>
        /// Sets the <b>DateTimePicker.</b> back to a non null value.
        /// </summary>
        private void SetToDateTimeValue()
        {
            if (this.isNull)
            {
                this.SetFormat();
                this.isNull = false;

                // base.OnValueChanged is not enough, because then value changed event is not transmitted to father 
                this.OnValueChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Sets the <b>DateTimePicker.</b> to the value of the <see cref="NullValue"/> property.
        /// </summary>
        private void SetToNullValue()
        {
            this.isNull = true;
            base.CustomFormat = (this.nullValue == null || this.nullValue.Length == 0) ? " " : "'" + this.nullValue + "'";
        }

        #endregion

        /// <summary>
        /// The nmhdr.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Nmhdr
        {
            /// <summary>
            /// The windowHandle from.
            /// </summary>
            public readonly IntPtr WindowHandleFrom;

            /// <summary>
            /// The id from.
            /// </summary>
            public readonly IntPtr IdFrom;

            /// <summary>
            /// The code value.
            /// </summary>
            public readonly int Code;
        }
    }
}