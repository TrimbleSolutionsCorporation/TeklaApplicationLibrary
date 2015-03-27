namespace Tekla.Structures.InpParser
{
    using System.Collections.Generic;

    /// <summary>
    ///        User defined attribute.
    /// </summary>
    public class UDA : TSTabPageObject
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UDA"/> class.</summary>
        /// <param name="isUnique">True if this is unque attribute.</param>
        public UDA(bool isUnique)
            : base()
        {
            this.IsUnique = isUnique;
        }

        /// <summary>Initializes a new instance of the <see cref="UDA"/> class.</summary>
        /// <param name="name">Unque name.</param>
        public UDA(string name)
            : base(name)
        {
            this.IsUnique = false;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the maximum possible value for the attribute.</summary>
        /// <value>The attribute value max.</value>
        public string AttributeValueMax { get; set; }

        /// <summary>Gets or sets the minimum possible value for the attribute.</summary>
        /// <value>The attribute value min.</value>
        public string AttributeValueMin { get; set; }

        /// <summary>Gets or sets the possible values: none, check_max, check_min, check_maxmin.</summary>
        /// <value>The check switch.</value>
        public CheckSwitchValues CheckSwitch { get; set; }

        /// <summary>Gets or sets the C-like format definition for dialog field.</summary>
        /// <value>The field format.</value>
        public string FieldFormat { get; set; }

        /// <summary>Gets or sets a value indicating whether this attribute unique or not.</summary>
        /// <value>The is unique.</value>
        public bool IsUnique { get; set; }

        /// <summary>Gets or sets the label text shown in dialog.</summary>
        /// <value>The label text.</value>
        public string LabelText { get; set; }

        /// <summary>Gets or sets the position value 1.</summary>
        /// <value>The position value 1.</value>
        public int PositionValue1 { get; set; }

        /// <summary>Gets or sets the position value 2.</summary>
        /// <value>The position value 2.</value>
        public int PositionValue2 { get; set; }

        /// <summary>Gets or sets the position value 1.</summary>
        /// <value>The position value 3.</value>
        public int PositionValue3 { get; set; }

        /// <summary>Gets or sets a value indicating whether this is specially flagged.
        /// For parts means consider in numbering.
        /// For drawings - display the attribute's value in drawing list.
        /// For other elements - no effect.</summary>
        /// <value>The special flag.</value>
        public bool SpecialFlag { get; set; }

        /// <summary>Gets or sets the toggle_field string.</summary>
        /// <value>The toggle field.</value>
        public string ToggleField { get; set; }

        /// <summary>Gets or sets the type of the attribute.</summary>
        /// <value>The value type.</value>
        public UDATypes ValueType { get; set; }

        /// <summary>Gets or sets the values of attribute.</summary>
        /// <value>The values.</value>
        public List<UDAValue> Values { get; set; }

        #endregion
    }
}