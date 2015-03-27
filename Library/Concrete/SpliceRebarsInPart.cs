namespace Tekla.Structures.Concrete
{
    using System.Collections;

    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    /// <summary>
    /// The splice reinforcement bars in part.
    /// </summary>
    public class SpliceRebarsInPart
    {
        #region Constants

        /// <summary>
        /// The angl e_ epsilon.
        /// </summary>
        private const double AngleEpsilon = 0.01;

        /// <summary>
        /// The distanc e_ epsilon.
        /// </summary>
        private const double DistanceEpsilon = 0.1;

        #endregion

        #region Fields

        /// <summary>
        /// The m_ splice data.
        /// </summary>
        private SpliceData spliceData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpliceRebarsInPart"/> class.
        /// </summary>
        public SpliceRebarsInPart()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SpliceRebarsInPart"/> class.</summary>
        /// <param name="newSpliceData">The splice data.</param>
        public SpliceRebarsInPart(SpliceData newSpliceData)
        {
            this.spliceData = newSpliceData;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The splice reinforcement bars if needed.</summary>
        /// <param name="newSpliceData">The splice data.</param>
        /// <param name="rebarGroups">The reinforcement bar groups.</param>
        /// <param name="ungroupedRebars">The ungrouped reinforcement bars.</param>
        /// <param name="splices">The splices.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SpliceRebarsIfNeeded(SpliceData newSpliceData, ArrayList rebarGroups, ArrayList ungroupedRebars, out ArrayList splices)
        {
            var result = false;
            splices = new ArrayList();

            if (newSpliceData != null)
            {
                this.spliceData = newSpliceData;
            }

            if (this.spliceData != null)
            {
                result = this.SpliceRebarsIfNeeded(rebarGroups, ungroupedRebars, out splices);
            }

            return result;
        }

        /// <summary>The splice reinforcement bars if needed.</summary>
        /// <param name="rebarGroups">The reinforcement bar groups.</param>
        /// <param name="ungroupedRebars">The ungrouped reinforcement bars.</param>
        /// <param name="splices">The splices.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SpliceRebarsIfNeeded(ArrayList rebarGroups, ArrayList ungroupedRebars, out ArrayList splices)
        {
            var result = false;
            splices = new ArrayList();

            this.SpliceRebarGroups(rebarGroups, ref splices);
            this.SpliceUngroupedRebars(ungroupedRebars, ref splices);

            if (splices.Count > 0)
            {
                result = true;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>The create splices.</summary>
        /// <param name="primary">The primary.</param>
        /// <param name="secondary">The secondary.</param>
        /// <param name="size">The size value.</param>
        /// <param name="splices">The splices.</param>
        private void CreateSplices(Reinforcement primary, Reinforcement secondary, double size, ref ArrayList splices)
        {
            var splice = new RebarSplice { RebarGroup1 = primary, RebarGroup2 = secondary };

            switch (this.spliceData.SpliceType)
            {
                case 0:
                    splice.Type = RebarSplice.RebarSpliceTypeEnum.SPLICE_TYPE_LAP_BOTH;

                    if (!double.IsNaN(this.spliceData.LappingLength))
                    {
                        splice.LapLength = this.spliceData.LappingLength;
                    }
                    else
                    {
                        splice.LapLength = this.spliceData.LappingLengthFactor * size;
                    }

                    splice.BarPositions = this.spliceData.BarPositions;
                    splice.Clearance = 0.0;
                    break;
                case 1:
                    splice.Type = RebarSplice.RebarSpliceTypeEnum.SPLICE_TYPE_MUFF;
                    break;
                case 2:
                    splice.Type = RebarSplice.RebarSpliceTypeEnum.SPLICE_TYPE_WELD;
                    break;
                default:
                    splice.Type = RebarSplice.RebarSpliceTypeEnum.SPLICE_TYPE_LAP_BOTH;

                    if (!double.IsNaN(this.spliceData.LappingLength))
                    {
                        splice.LapLength = this.spliceData.LappingLength;
                    }
                    else
                    {
                        splice.LapLength = this.spliceData.LappingLengthFactor * size;
                    }

                    splice.BarPositions = this.spliceData.BarPositions;
                    splice.Clearance = 0.0;
                    break;
            }

            splice.Offset = 0.0;
            splices.Add(splice);
        }

        /// <summary>The get group lines.</summary>
        /// <param name="rebarGroup">The reinforcement bar group.</param>
        /// <param name="groupSegments">The group segments.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetGroupLines(RebarGroup rebarGroup, out ArrayList groupSegments)
        {
            var result = false;
            groupSegments = new ArrayList();

            var groupGeometries = rebarGroup.GetRebarGeometries(false);

            if (groupGeometries.Count > 1)
            {
                var firstRebarGeometry = groupGeometries[0] as RebarGeometry;
                var lastRebarGeometry = groupGeometries[groupGeometries.Count - 1] as RebarGeometry;

                if (firstRebarGeometry != null && lastRebarGeometry != null)
                {
                    var firstRebarPoints = firstRebarGeometry.Shape.Points;
                    var lastRebarPoints = lastRebarGeometry.Shape.Points;

                    groupSegments.Add(new LineSegment((Point)firstRebarPoints[0], (Point)lastRebarPoints[0]));
                    groupSegments.Add(new LineSegment((Point)firstRebarPoints[1], (Point)lastRebarPoints[1]));

                    result = true;
                }
            }

            return result;
        }

        /// <summary>The get nominal diameter.</summary>
        /// <param name="rebar">The rebar.</param>
        /// <returns>The System.Double.</returns>
        private double GetNominalDiameter(Reinforcement rebar)
        {
            var nominalDiameter = 0.0;
            var rebarGeometries = new ArrayList();

            var group = rebar as RebarGroup;
            if (group != null)
            {
                rebarGeometries = group.GetRebarGeometries(false);
            }

            var ungroupedRebar = rebar as SingleRebar;
            if (ungroupedRebar != null)
            {
                rebarGeometries = ungroupedRebar.GetRebarGeometries(false);
            }

            if (rebarGeometries.Count > 0)
            {
                var firstRebarGeometry = rebarGeometries[0] as RebarGeometry;

                if (firstRebarGeometry != null)
                {
                    nominalDiameter = firstRebarGeometry.Diameter;
                }
            }

            return nominalDiameter;
        }

        /// <summary>The get reinforcement bar vector for group.</summary>
        /// <param name="segmentIndex">The segment index.</param>
        /// <param name="groupSegments">The group segments.</param>
        /// <returns>The Tekla.Structures.Geometry3d.Vector.</returns>
        private Vector GetRebarVectorForGroup(int segmentIndex, ArrayList groupSegments)
        {
            var rebarVector = new Vector();
            var firstSegment = groupSegments[0] as LineSegment;
            var secondSegment = groupSegments[1] as LineSegment;

            if (firstSegment != null && secondSegment != null)
            {
                rebarVector = segmentIndex == 0 ? new Vector(secondSegment.Point1 - firstSegment.Point1) : new Vector(firstSegment.Point1 - secondSegment.Point1);
            }

            return rebarVector;
        }

        /// <summary>The get reinforcement bar vector for single rebar.</summary>
        /// <param name="pointIndex">The point index.</param>
        /// <param name="rebar">The rebar.</param>
        /// <returns>The Tekla.Structures.Geometry3d.Vector.</returns>
        private Vector GetRebarVectorForSingleRebar(int pointIndex, SingleRebar rebar)
        {
            return pointIndex == 0 ?
                new Vector((Point)rebar.Polygon.Points[1] - (Point)rebar.Polygon.Points[0]) :
                new Vector((Point)rebar.Polygon.Points[0] - (Point)rebar.Polygon.Points[1]);
        }

        /// <summary>The reinforcement bar groups can be spliced.</summary>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="secondaryRebarGroup">The secondary reinforcement bar group.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarGroupsCanBeSpliced(RebarGroup primaryRebarGroup, RebarGroup secondaryRebarGroup)
        {
            var result = false;
            ArrayList primaryGroupSegments, secondaryGroupSegments;

            this.GetGroupLines(primaryRebarGroup, out primaryGroupSegments);
            this.GetGroupLines(secondaryRebarGroup, out secondaryGroupSegments);

            for (var primaryIndex = 0; primaryIndex < primaryGroupSegments.Count && !result; primaryIndex++)
            {
                var primarySegment = primaryGroupSegments[primaryIndex] as LineSegment;

                for (var secondaryIndex = 0; secondaryIndex < secondaryGroupSegments.Count && !result; secondaryIndex++)
                {
                    var secondarySegment = secondaryGroupSegments[secondaryIndex] as LineSegment;

                    /*if (PrimarySegment != null && SecondarySegment != null &&
                        Parallel.LineSegmentToLineSegment(PrimarySegment, SecondarySegment, ANGLE_EPSILON) &&
                        ((Distance.PointToPoint(PrimarySegment.Point1, SecondarySegment.Point1) < DISTANCE_EPSILON &&
                        Distance.PointToPoint(PrimarySegment.Point2, SecondarySegment.Point2) < DISTANCE_EPSILON) ||
                        (Distance.PointToPoint(PrimarySegment.Point1, SecondarySegment.Point2) < DISTANCE_EPSILON &&
                        Distance.PointToPoint(PrimarySegment.Point2, SecondarySegment.Point1) < DISTANCE_EPSILON)))
                    {*/
                    if (primarySegment != null && secondarySegment != null
                        && Parallel.LineSegmentToLineSegment(primarySegment, secondarySegment, AngleEpsilon)
                        && Distance.PointToPoint(primarySegment.Point1, secondarySegment.Point1) < DistanceEpsilon
                        && Distance.PointToPoint(primarySegment.Point2, secondarySegment.Point2) < DistanceEpsilon)
                    {
                        var primaryVector = this.GetRebarVectorForGroup(primaryIndex, primaryGroupSegments);
                        var secondaryVector = this.GetRebarVectorForGroup(secondaryIndex, secondaryGroupSegments);
                        result = this.RebarsDoNotOverlap(primaryVector, secondaryVector);
                    }
                }
            }

            return result;
        }

        /// <summary>The reinforcement bars can be spliced.</summary>
        /// <param name="primaryRebar">The primary rebar.</param>
        /// <param name="secondaryRebar">The secondary rebar.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarsCanBeSpliced(SingleRebar primaryRebar, SingleRebar secondaryRebar)
        {
            var result = false;

            for (var primaryIndex = 0; primaryIndex < primaryRebar.Polygon.Points.Count && !result; primaryIndex++)
            {
                for (var secondaryIndex = 0; secondaryIndex < secondaryRebar.Polygon.Points.Count && !result; secondaryIndex++)
                {
                    if (Distance.PointToPoint(primaryRebar.Polygon.Points[primaryIndex] as Point, secondaryRebar.Polygon.Points[secondaryIndex] as Point) < DistanceEpsilon)
                    {
                        var primaryVector = this.GetRebarVectorForSingleRebar(primaryIndex, primaryRebar);
                        var secondaryVector = this.GetRebarVectorForSingleRebar(secondaryIndex, secondaryRebar);
                        result = this.RebarsDoNotOverlap(primaryVector, secondaryVector);
                    }
                }
            }

            return result;
        }

        /// <summary>The reinforcement bars do not overlap.</summary>
        /// <param name="primaryVector">The primary vector.</param>
        /// <param name="secondaryVector">The secondary vector.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarsDoNotOverlap(Vector primaryVector, Vector secondaryVector)
        {
            return primaryVector.Dot(secondaryVector) < 0.0;
        }

        /// <summary>The splice reinforcement bar groups.</summary>
        /// <param name="rebarGroups">The reinforcement bar groups.</param>
        /// <param name="splices">The splices.</param>
        private void SpliceRebarGroups(ArrayList rebarGroups, ref ArrayList splices)
        {
            if (rebarGroups.Count > 1)
            {
                for (var primaryGroupIndex = 0; primaryGroupIndex < rebarGroups.Count; primaryGroupIndex++)
                {
                    var primaryRebarGroup = rebarGroups[primaryGroupIndex] as RebarGroup;

                    for (var secondaryGroupIndex = primaryGroupIndex + 1; secondaryGroupIndex < rebarGroups.Count; secondaryGroupIndex++)
                    {
                        var secondaryRebarGroup = rebarGroups[secondaryGroupIndex] as RebarGroup;

                        if (this.RebarGroupsCanBeSpliced(primaryRebarGroup, secondaryRebarGroup))
                        {
                            if (primaryRebarGroup != null)
                            {
                                this.CreateSplices(primaryRebarGroup, secondaryRebarGroup, this.GetNominalDiameter(primaryRebarGroup), ref splices);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>The splice ungrouped reinforcement bars.</summary>
        /// <param name="ungroupedRebars">The ungrouped reinforcement bars.</param>
        /// <param name="splices">The splices.</param>
        private void SpliceUngroupedRebars(ArrayList ungroupedRebars, ref ArrayList splices)
        {
            if (ungroupedRebars.Count > 1)
            {
                for (var primaryRebarIndex = 0; primaryRebarIndex < ungroupedRebars.Count; primaryRebarIndex++)
                {
                    var primaryRebar = ungroupedRebars[primaryRebarIndex] as SingleRebar;

                    for (var secondaryGroupIndex = primaryRebarIndex + 1; secondaryGroupIndex < ungroupedRebars.Count; secondaryGroupIndex++)
                    {
                        var secondaryRebar = ungroupedRebars[secondaryGroupIndex] as SingleRebar;

                        if (this.RebarsCanBeSpliced(primaryRebar, secondaryRebar))
                        {
                            if (primaryRebar != null)
                            {
                                this.CreateSplices(
                                    primaryRebar, secondaryRebar, this.GetNominalDiameter(primaryRebar), ref splices);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}