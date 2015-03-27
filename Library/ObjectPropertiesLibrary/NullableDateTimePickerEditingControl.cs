namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Forms;

    /// <summary>
    /// The nullable date time picker editing control.
    /// </summary>
    internal class NullableDateTimePickerEditingControl : NullableDateTimePicker, IDataGridViewEditingControl
    {
        #region Fields

        /// <summary>
        /// The value changed.
        /// </summary>
        private bool valueChanged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDateTimePickerEditingControl"/> class.
        /// </summary>
        public NullableDateTimePickerEditingControl()
        {
            this.Format = DateTimePickerFormat.Custom;
            this.CustomFormat = string.Format("{0}", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the editing control data grid view.
        /// </summary>
        public DataGridView EditingControlDataGridView { get; set; }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.

        /// <summary>
        /// Gets or sets the editing control formatted value.
        /// </summary>
        public object EditingControlFormattedValue
        {
            get
            {
                if (this.Value == null)
                {
                    return null;
                }
                else
                {
                    return this.Value;
                }
            }

            set
            {
                var stringValue = value as string;
                if (stringValue != null)
                {
                    if (stringValue.Length > 0)
                    {
                        this.Value = DateTime.Parse(stringValue);
                    }
                    else
                    {
                        this.Value = null;
                    }
                }
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method.

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property.

        /// <summary>
        /// Gets or sets the editing control row index.
        /// </summary>
        public int EditingControlRowIndex { get; set; }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method.

        // Implements the IDataGridViewEditingControl.EditingControlValueChanged property.

        /// <summary>
        /// Gets or sets a value indicating whether editing control value changed.
        /// </summary>
        public bool EditingControlValueChanged
        {
            get
            {
                return this.valueChanged;
            }

            set
            {
                this.valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingPanelCursor property.

        /// <summary>
        /// Gets the editing panel cursor.
        /// </summary>
        public Cursor EditingPanelCursor
        {
            [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists", Justification = "Reviewed. Suppression is OK here.")]
            get
            {
                return base.Cursor;
            }
        }

        /// <summary>
        /// Gets a value indicating whether reposition editing control on value change.
        /// </summary>
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The apply cell style to editing control.</summary>
        /// <param name="dataGridViewCellStyle">The data grid view cell style.</param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        /// <summary>The editing control wants input key.</summary>
        /// <param name="key">The key value.</param>
        /// <param name="dataGridViewWantsInputKey">The data grid view wants input key.</param>
        /// <returns>The System.Boolean.</returns>
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>The get editing control formatted value.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Object.</returns>
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.EditingControlFormattedValue;
        }

        /// <summary>The prepare editing control for edit.</summary>
        /// <param name="selectAll">The select all.</param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        #endregion

        #region Methods

        /// <summary>The on value changed.</summary>
        /// <param name="e">The e value.</param>
        protected override void OnValueChanged(EventArgs e)
        {
            // Notify the DataGridView that the contents of the cell has changed.
            this.valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(e);
        }

        #endregion
    }
}