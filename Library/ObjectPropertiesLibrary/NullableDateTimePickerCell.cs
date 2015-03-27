namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// The nullable date time picker cell.
    /// </summary>
    public class NullableDateTimePickerCell : DataGridViewTextBoxCell
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDateTimePickerCell"/> class.
        /// </summary>
        public NullableDateTimePickerCell()
            : base()
        {
            // Use the date/hour-minutes format.
            this.Style.Format = "d";

            // this.Style.Format = "g";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the default new row value.</summary>
        /// <value>The default new row value.</value>
        public override object DefaultNewRowValue
        {
            get
            {
                // Use the tomorrow date as the default value.
                return DBNull.Value;
            }
        }

        /// <summary>Gets the edit type.</summary>
        /// <value>The edit type.</value>
        public override Type EditType
        {
            get
            {
                // Return the isDayOfWeekSchedule of the editing contol that CalendarCell uses.
                return typeof(NullableDateTimePickerEditingControl);
            }
        }

        /// <summary>Gets the value type.</summary>
        /// <value>The value type.</value>
        public override Type ValueType
        {
            get
            {
                // Return the isDayOfWeekSchedule of the value that CalendarCell contains.
                return typeof(DBNull);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The initialize editing control.</summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="initialFormattedValue">The initial formatted value.</param>
        /// <param name="dataGridViewCellStyle">The data grid view cell style.</param>
        public override void InitializeEditingControl(
            int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var ctl =
                this.DataGridView.EditingControl as NullableDateTimePickerEditingControl;

            try
            {
                if (this.Value == null)
                {
                    ctl.Value = null;
                }
                else
                {
                    ctl.Value = this.Value is DateTime ? (DateTime)this.Value : DateTime.Now.Date;
                }
            }
            catch (ArgumentException)
            {
                ctl.Value = DateTime.Now.Date;
            }
        }

        /// <summary>The parse formatted value.</summary>
        /// <param name="formattedValue">The formatted value.</param>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="formattedValueTypeConverter">The formatted value type converter.</param>
        /// <param name="valueTypeConverter">The value type converter.</param>
        /// <returns>The System.Object.</returns>
        /// <exception cref="ArgumentException">Throws a argument exception.</exception>
        public override object ParseFormattedValue(
            object formattedValue, 
            DataGridViewCellStyle cellStyle, 
            TypeConverter formattedValueTypeConverter, 
            TypeConverter valueTypeConverter)
        {
            var stringValue = formattedValue as string;

            if (formattedValue != null)
            {
                if (formattedValue is DateTime)
                {
                    return formattedValue;
                }

                DateTime result;
                if (stringValue != null && DateTime.TryParse(stringValue, out result))
                {
                    return result;
                }
                else
                {
                    throw new ArgumentException("FormattedValue has wrong isDayOfWeekSchedule", "formattedValue");
                }
            }

            return null;
        }

        #endregion
    }
}