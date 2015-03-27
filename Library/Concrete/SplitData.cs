namespace Tekla.Structures.Concrete
{
    /// <summary>
    /// The split data.
    /// </summary>
    public class SplitData
    {
        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="SplitData"/> class.</summary>
        public SplitData()
        {
            this.DepthLocation = 0;
            this.LappingLength = 0.0;
            this.MaxLength = 0.0;
            this.MinSplitDistance = 0.0;
            this.SpliceOffset = 0.0;
            this.SpliceSection = 0;
            this.SpliceSymmetry = 0;
            this.SpliceType = 0;
        }
        #endregion

        #region Fields

        /// <summary>Gets or sets the depth location.</summary>
        /// <value>The depth location.</value>
        public int DepthLocation { get; set; }

        /// <summary>Gets or sets the lapping length.</summary>
        /// <value>The lapping length.</value>
        public double LappingLength { get; set; }

        /// <summary>Gets or sets the max length.</summary>
        /// <value>The max length.</value>
        public double MaxLength { get; set; }

        /// <summary>Gets or sets the min split distance.</summary>
        /// <value>The min split distance.</value>
        public double MinSplitDistance { get; set; }

        /// <summary>Gets or sets the splice offset.</summary>
        /// <value>The splice offset.</value>
        public double SpliceOffset { get; set; }

        /// <summary>Gets or sets the splice section.</summary>
        /// <value>The splice section.</value>
        public int SpliceSection { get; set; }

        /// <summary>Gets or sets the splice symmetry.</summary>
        /// <value>The splice symmetry.</value>
        public int SpliceSymmetry { get; set; }

        /// <summary>Gets or sets the splice type.</summary>
        /// <value>The splice type.</value>
        public int SpliceType { get; set; }

        #endregion
    }
}