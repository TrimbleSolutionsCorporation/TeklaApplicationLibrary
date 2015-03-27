namespace Tekla.Structures.Concrete
{
    using Tekla.Structures.Model;

    /// <summary>
    /// The splice data.
    /// </summary>
    public class SpliceData
    {
        #region Fields

        /// <summary>Gets or sets the bar positions.</summary>
        /// <value>The bar positions.</value>
        public RebarSplice.RebarSpliceBarPositionsEnum BarPositions { get; set; }

        /// <summary>Gets or sets the lapping length.</summary>
        /// <value>The lapping length.</value>
        public double LappingLength { get; set; }

        /// <summary>Gets or sets the lapping length factor.</summary>
        /// <value>The lapping length factor.</value>
        public double LappingLengthFactor { get; set; }

        /// <summary>Gets or sets the splice type.</summary>
        /// <value>The splice type.</value>
        public int SpliceType { get; set; }

        #endregion
    }
}