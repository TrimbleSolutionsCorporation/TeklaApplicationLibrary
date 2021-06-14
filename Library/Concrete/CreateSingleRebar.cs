namespace Tekla.Structures.Concrete
{
    using System.Collections;

    using Tekla.Structures.Model;

    /// <summary>
    /// The create single rebar.
    /// </summary>
    public class CreateSingleRebar
    {
        #region Constants

        /// <summary>
        /// The overhan g_ epsilon.
        /// </summary>
        private const double OverhangEpsilon = 0.01;

        #endregion

        #region Fields

        /// <summary>
        /// The m_ original rebar.
        /// </summary>
        private readonly Reinforcement originalRebar;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CreateSingleRebar"/> class.</summary>
        /// <param name="newOriginalRebar">The original rebar.</param>
        public CreateSingleRebar(Reinforcement newOriginalRebar)
        {
            this.originalRebar = newOriginalRebar;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get single rebar.</summary>
        /// <param name="rebarPoints">The reinforcement bar points.</param>
        /// <returns>The Tekla.Structures.Model.SingleRebar.</returns>
        public SingleRebar GetSingleRebar(ArrayList rebarPoints)
        {
            SingleRebar newRebar = new SingleRebar();
            newRebar.Polygon.Points = new ArrayList(rebarPoints);

            SingleRebar originalSingleRebar = this.originalRebar as SingleRebar;

            if (originalSingleRebar != null)
            {
                this.SetNewRebarPropertiesFromSingleRebar(newRebar, originalSingleRebar);
            }
            else
            {
                RebarGroup originalRebarGroup = this.originalRebar as RebarGroup;

                if (originalRebarGroup != null)
                {
                    this.SetNewRebarPropertiesFromRebarGroup(newRebar, originalRebarGroup);
                }
            }

            // Needed to ensure that the reinforcement bar ends will not overcome the cover thinkness.
            if (newRebar.EndPointOffsetType == Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS &&
                newRebar.EndPointOffsetValue < OverhangEpsilon)
            {
                newRebar.EndPointOffsetValue = OverhangEpsilon;
            }

            if (newRebar.StartPointOffsetType == Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS &&
                newRebar.StartPointOffsetValue < OverhangEpsilon)
            {
                newRebar.StartPointOffsetValue = OverhangEpsilon;
            }

            return newRebar;
        }

        #endregion

        #region Methods

        /// <summary>The set new reinforcement bar properties from reinforcement bar group.</summary>
        /// <param name="newRebar">The new rebar.</param>
        /// <param name="originalRebarGroup">The original reinforcement bar group.</param>
        private void SetNewRebarPropertiesFromRebarGroup(SingleRebar newRebar, RebarGroup originalRebarGroup)
        {
            newRebar.Father = originalRebarGroup.Father;
            newRebar.Name = originalRebarGroup.Name;
            newRebar.Size = originalRebarGroup.Size;
            newRebar.Grade = originalRebarGroup.Grade;
            newRebar.Class = originalRebarGroup.Class;

            newRebar.StartHook.Shape = originalRebarGroup.StartHook.Shape;
            newRebar.StartHook.Angle = originalRebarGroup.StartHook.Angle;
            newRebar.StartHook.Radius = originalRebarGroup.StartHook.Radius;
            newRebar.StartHook.Length = originalRebarGroup.StartHook.Length;

            newRebar.EndHook.Shape = originalRebarGroup.EndHook.Shape;
            newRebar.EndHook.Angle = originalRebarGroup.EndHook.Angle;
            newRebar.EndHook.Radius = originalRebarGroup.EndHook.Radius;
            newRebar.EndHook.Length = originalRebarGroup.EndHook.Length;

            newRebar.OnPlaneOffsets = new ArrayList();
            newRebar.FromPlaneOffset = 0.0;
            newRebar.EndPointOffsetValue = 0.0;
            newRebar.EndPointOffsetType = originalRebarGroup.EndPointOffsetType;
            newRebar.StartPointOffsetValue = 0.0;
            newRebar.StartPointOffsetType = originalRebarGroup.StartPointOffsetType;

            newRebar.NumberingSeries.Prefix = originalRebarGroup.NumberingSeries.Prefix;
            newRebar.NumberingSeries.StartNumber = originalRebarGroup.NumberingSeries.StartNumber;
            newRebar.RadiusValues = originalRebarGroup.RadiusValues;
        }

        /// <summary>The set new reinforcement bar properties from single rebar.</summary>
        /// <param name="newRebar">The new rebar.</param>
        /// <param name="originalSingleRebar">The original single rebar.</param>
        private void SetNewRebarPropertiesFromSingleRebar(SingleRebar newRebar, SingleRebar originalSingleRebar)
        {
            newRebar.Father = originalSingleRebar.Father;
            newRebar.Name = originalSingleRebar.Name;
            newRebar.Size = originalSingleRebar.Size;
            newRebar.Grade = originalSingleRebar.Grade;
            newRebar.Class = originalSingleRebar.Class;

            newRebar.StartHook.Shape = originalSingleRebar.StartHook.Shape;
            newRebar.StartHook.Angle = originalSingleRebar.StartHook.Angle;
            newRebar.StartHook.Radius = originalSingleRebar.StartHook.Radius;
            newRebar.StartHook.Length = originalSingleRebar.StartHook.Length;

            newRebar.EndHook.Shape = originalSingleRebar.EndHook.Shape;
            newRebar.EndHook.Angle = originalSingleRebar.EndHook.Angle;
            newRebar.EndHook.Radius = originalSingleRebar.EndHook.Radius;
            newRebar.EndHook.Length = originalSingleRebar.EndHook.Length;

            newRebar.FromPlaneOffset = originalSingleRebar.FromPlaneOffset;
            newRebar.OnPlaneOffsets = originalSingleRebar.OnPlaneOffsets;
            newRebar.NumberingSeries.Prefix = originalSingleRebar.NumberingSeries.Prefix;
            newRebar.NumberingSeries.StartNumber = originalSingleRebar.NumberingSeries.StartNumber;
            newRebar.RadiusValues = originalSingleRebar.RadiusValues;

            newRebar.EndPointOffsetValue = originalSingleRebar.EndPointOffsetValue;
            newRebar.EndPointOffsetType = originalSingleRebar.EndPointOffsetType;
            newRebar.StartPointOffsetValue = originalSingleRebar.StartPointOffsetValue;
            newRebar.StartPointOffsetType = originalSingleRebar.StartPointOffsetType;
        }

        #endregion
    }
}