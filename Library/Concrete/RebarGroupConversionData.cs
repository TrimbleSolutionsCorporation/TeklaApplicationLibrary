namespace Tekla.Structures.Concrete
{
    using Tekla.Structures.Model;

    /// <summary>
    /// The reinforcement bar group conversion data.
    /// </summary>
    public class RebarGroupConversionData
    {
        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="RebarGroupConversionData"/> class.</summary>
        public RebarGroupConversionData()
        {
            this.DepthLocation = 0;
            this.FatherSlab = new ContourPlate();
            this.Spacing = 0.0;
            this.SpliceSection = 0;
        }

        #endregion

        #region Fields

        /// <summary>Gets or sets the depth location.</summary>
        /// <value>The depth location.</value>
        public int DepthLocation { get; set; }

        /// <summary>Gets or sets the father slab.</summary>
        /// <value>The father slab.</value>
        public Part FatherSlab { get; set; }

        /// <summary>Gets or sets the spacing.</summary>
        /// <value>The spacing.</value>
        public double Spacing { get; set; }

        /// <summary>Gets or sets the splice section.</summary>
        /// <value>The splice section.</value>
        public int SpliceSection { get; set; }

        #endregion
    }
}