namespace Tekla.Structures.ObjectPropertiesLibrary
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The data grid view nullable date time picker column.
    /// </summary>
    public class DataGridViewNullableDateTimePickerColumn : DataGridViewColumn
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewNullableDateTimePickerColumn"/> class.
        /// </summary>
        public DataGridViewNullableDateTimePickerColumn()
            : base(new NullableDateTimePickerCell())
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the cell template.</summary>
        /// <exception cref="InvalidCastException">Throws an exception.</exception>
        /// <value>The cell template.</value>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }

            set
            {
                // Ensure that the cell used for the template is a NullableDateTimePickerCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(NullableDateTimePickerCell)))
                {
                    throw new InvalidCastException("Must be a NullableDateTimePickerCell");
                }

                base.CellTemplate = value;
            }
        }

        #endregion
    }
}