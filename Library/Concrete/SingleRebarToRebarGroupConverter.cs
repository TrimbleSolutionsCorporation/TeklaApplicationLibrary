namespace Tekla.Structures.Concrete
{
    using System;
    using System.Collections;

    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    /// <summary>
    /// The single reinforcement bar to reinforcement bar group converter.
    /// </summary>
    public class SingleRebarToRebarGroupConverter
    {
        #region Constants

        /// <summary>
        /// The angl e_ epsilon.
        /// </summary>
        private const double AngleEpsilon = 0.01;

        /// <summary>
        /// The degres s_90.
        /// </summary>
        private const double Degress90 = Math.PI / 2;

        /// <summary>
        /// The distanc e_ epsilon.
        /// </summary>
        private const double DistanceEpsilon = 0.1;

        /// <summary>
        /// The do t_ epsilon.
        /// </summary>
        private const double DotEpsilon = 0.5;

        /// <summary>
        /// The maximu m_ iterations.
        /// </summary>
        private const double MaximumIterations = 10;

        #endregion

        #region Fields

        /// <summary>
        /// The m_ coordinate z.
        /// </summary>
        private readonly Vector coordinateZ;

        /// <summary>
        /// The m_ reinforcement bar group conversion data.
        /// </summary>
        private readonly RebarGroupConversionData rebarGroupConversionData;

        /// <summary>
        /// The m_ single rebars.
        /// </summary>
        private readonly ArrayList singleRebars;

        /// <summary>
        /// The m_ considered reinforcement bars index.
        /// </summary>
        private ArrayList consideredRebarsIndex;

        /// <summary>
        /// The m_ reinforcement bar groups.
        /// </summary>
        private ArrayList rebarGroups;

        /// <summary>
        /// The m_ ungrouped rebars.
        /// </summary>
        private ArrayList ungroupedRebars;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SingleRebarToRebarGroupConverter"/> class.</summary>
        /// <param name="newRebarGroupConversionData">The reinforcement bar group conversion data.</param>
        /// <param name="newSingleRebars">The single reinforcement bars.</param>
        public SingleRebarToRebarGroupConverter(RebarGroupConversionData newRebarGroupConversionData, ArrayList newSingleRebars)
        {
            this.rebarGroupConversionData = newRebarGroupConversionData;
            this.singleRebars = newSingleRebars;
            this.rebarGroups = new ArrayList();
            this.ungroupedRebars = new ArrayList();

            var fatherCoordinateSystem = this.rebarGroupConversionData.FatherSlab.GetCoordinateSystem();
            this.coordinateZ = fatherCoordinateSystem.AxisY.Cross(fatherCoordinateSystem.AxisX);
        }

        #endregion

        #region Enums

        /// <summary>
        /// The group type.
        /// </summary>
        private enum GroupType
        {
            /// <summary>
            /// The undefined.
            /// </summary>
            Undefined, 

            /// <summary>
            /// The normal.
            /// </summary>
            Normal, 

            /// <summary>
            /// The tapered.
            /// </summary>
            Tapered, 
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get hook direction.</summary>
        /// <param name="rebar">The rebar.</param>
        /// <returns>The Tekla.Structures.Geometry3d.Vector.</returns>
        public static Vector GetHookDirection(Reinforcement rebar)
        {
            Vector hookDirection = null;

            rebar.Insert();
            var rebarGeometriesWithHooks = rebar.GetRebarGeometries(true);
            var rebarGeometriesWithoutHooks = rebar.GetRebarGeometries(false);
            rebar.Delete();

            if (rebarGeometriesWithHooks.Count > 0)
            {
                var geometryWithHooks = rebarGeometriesWithHooks[0] as RebarGeometry;
                var geometryWithoutHooks = rebarGeometriesWithoutHooks[0] as RebarGeometry;

                var rebarPointsWithHooks = geometryWithHooks.Shape.Points;
                var rebarPointsWithoutHooks = geometryWithoutHooks.Shape.Points;

                if (rebarPointsWithHooks.Count > 2)
                {
                    var hookPoint = new Point(rebarPointsWithHooks[0] as Point);
                    var rebarPoint = new Point(rebarPointsWithoutHooks[0] as Point);

                    if (Distance.PointToPoint(hookPoint, rebarPoint) > DistanceEpsilon)
                    {
                        hookDirection = new Vector(hookPoint - rebarPoint);
                    }
                    else
                    {
                        hookPoint = rebarPointsWithHooks[rebarPointsWithHooks.Count - 1] as Point;
                        rebarPoint = rebarPointsWithoutHooks[rebarPointsWithoutHooks.Count - 1] as Point;

                        if (Distance.PointToPoint(hookPoint, rebarPoint) > DistanceEpsilon)
                        {
                            hookDirection = new Vector(hookPoint - rebarPoint);
                        }
                    }
                }
            }

            return hookDirection;
        }

        /// <summary>The swap start hook with end hook.</summary>
        /// <param name="newRebar">The new rebar.</param>
        public static void SwapStartHookWithEndHook(ref SingleRebar newRebar)
        {
            var auxiliaryHook = new RebarHookData
                {
                    Shape = newRebar.StartHook.Shape, 
                    Angle = newRebar.StartHook.Angle,
                    Radius = newRebar.StartHook.Radius,
                    Length = newRebar.StartHook.Length
                };

            newRebar.StartHook.Shape = newRebar.EndHook.Shape;
            newRebar.StartHook.Angle = newRebar.EndHook.Angle;
            newRebar.StartHook.Radius = newRebar.EndHook.Radius;
            newRebar.StartHook.Length = newRebar.EndHook.Length;

            newRebar.EndHook.Shape = auxiliaryHook.Shape;
            newRebar.EndHook.Angle = auxiliaryHook.Angle;
            newRebar.EndHook.Radius = auxiliaryHook.Radius;
            newRebar.EndHook.Length = auxiliaryHook.Length;
        }

        /// <summary>The convert single reinforcement bar to reinforcement bar group.</summary>
        /// <param name="splitForSplicing">The split for splicing.</param>
        /// <param name="newRebarGroups">The reinforcement bar groups.</param>
        /// <param name="newUngroupedRebars">The ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        public bool ConvertSingleRebarToRebarGroup(bool splitForSplicing, out ArrayList newRebarGroups, out ArrayList newUngroupedRebars)
        {
            var result = false;
            newRebarGroups = new ArrayList();
            newUngroupedRebars = new ArrayList();
            this.consideredRebarsIndex = new ArrayList();

            if (this.CheckIfRebarsAreInSamePlane())
            {
                this.GetRebarGroups();
                this.GetUngroupedRebars();

                if (splitForSplicing)
                {
                    this.SplitRebarGroupsForSplicing();
                }

                newRebarGroups = new ArrayList(this.rebarGroups);
                newUngroupedRebars = new ArrayList(this.ungroupedRebars);

                result = true;
            }

            return result;
        }

        #endregion

        // Needed until the splicing between groups with different number of reinforcement bars can be done in the core. 
        #region Methods

        /// <summary>The add parallel reinforcement bars to group.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <param name="type">The type value.</param>
        /// <param name="newRebarGroups">The reinforcement bar groups.</param>
        /// <param name="newUngroupedRebars">The ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool AddParallelRebarsToGroup(ArrayList parallelRebars, GroupType type, ref ArrayList newRebarGroups, ref ArrayList newUngroupedRebars)
        {
            var result = false;
            ArrayList parallelRebarGroups, parallelUngroupedRebars;

            var converter = new SingleRebarToRebarGroupConverter(this.rebarGroupConversionData, parallelRebars);

            if (converter.ConvertSingleRebarToRebarGroup(false, out parallelRebarGroups, out parallelUngroupedRebars))
            {
                if (parallelRebarGroups.Count > 0)
                {
                    foreach (RebarGroup group in parallelRebarGroups)
                    {
                        if (!this.RebarGroupArrayListContainsRebarGroup(group, newRebarGroups))
                        {
                            newRebarGroups.Add(group);
                        }
                    }
                }

                if (parallelUngroupedRebars.Count > 0)
                {
                    foreach (SingleRebar rebar in parallelUngroupedRebars)
                    {
                        if (!this.RebarArrayListContainsSingleRebar(rebar, newUngroupedRebars))
                        {
                            newUngroupedRebars.Add(rebar);
                        }
                    }
                }

                result = true;
            }

            return result;
        }

        /// <summary>The add reinforcement bars if do not overlap.</summary>
        /// <param name="newRebar">The new rebar.</param>
        /// <param name="primaryRebarVector">The primary reinforcement bar vector.</param>
        /// <param name="secondaryRebarVector">The secondary reinforcement bar vector.</param>
        /// <param name="newParallelRebars">The new parallel reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool AddRebarsIfDoNotOverlap(SingleRebar newRebar, Vector primaryRebarVector, Vector secondaryRebarVector, ArrayList newParallelRebars)
        {
            var result = false;

            if (this.RebarsDoNotOverlap(primaryRebarVector, secondaryRebarVector))
            {
                newParallelRebars.Add(newRebar);
                result = true;
            }

            return result;
        }

        /// <summary>The add reinforcement bars to considered reinforcement bars index.</summary>
        /// <param name="newGroupRebars">The new group reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool AddRebarsToConsideredRebarsIndex(ArrayList newGroupRebars)
        {
            var result = false;

            foreach (SingleRebar addedRebar in newGroupRebars)
            {
                var firstPoint = new Point((Point)addedRebar.Polygon.Points[0]);
                var secondPoint = new Point((Point)addedRebar.Polygon.Points[1]);

                for (var rebarIndex = 0; rebarIndex < this.singleRebars.Count; rebarIndex++)
                {
                    var rebar = this.singleRebars[rebarIndex] as SingleRebar;

                    if (rebar != null
                        && Distance.PointToPoint(firstPoint, (Point)rebar.Polygon.Points[0]) < DistanceEpsilon
                        && Distance.PointToPoint(secondPoint, (Point)rebar.Polygon.Points[1]) < DistanceEpsilon)
                    {
                        this.consideredRebarsIndex.Add(rebarIndex);
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The check if reinforcement bars are in same plane.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception.
        /// </exception>
        private bool CheckIfRebarsAreInSamePlane()
        {
            var result = false;

            if (this.singleRebars.Count > 1)
            {
                var firstRebar = this.singleRebars[0] as SingleRebar;
                var secondRebar = this.singleRebars[1] as SingleRebar;

                if (firstRebar != null && secondRebar != null)
                {
                    var rebarGroupPlane = new GeometricPlane(
                        (Point)firstRebar.Polygon.Points[0], 
                        new Vector((Point)firstRebar.Polygon.Points[1] - (Point)firstRebar.Polygon.Points[0]), 
                        new Vector((Point)secondRebar.Polygon.Points[0] - (Point)firstRebar.Polygon.Points[0]));

                    foreach (SingleRebar rebar in this.singleRebars)
                    {
                        if (Distance.PointToPlane((Point)rebar.Polygon.Points[0], rebarGroupPlane) > DistanceEpsilon ||
                            Distance.PointToPlane((Point)rebar.Polygon.Points[1], rebarGroupPlane) > DistanceEpsilon)
                        {
                            throw new Exception("The original reinforcement bars are not located in the same plane.");
                        }
                    }

                    result = true;
                }
            }
            else if (this.singleRebars.Count == 1)
            {
                result = true;
            }

            return result;
        }

        /// <summary>The create normal or tapered reinforcement bar group.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <param name="type">The type value.</param>
        /// <param name="newRebarGroup">The new reinforcement bar group.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateNormalOrTaperedRebarGroup(ArrayList parallelRebars, GroupType type, out RebarGroup newRebarGroup)
        {
            var result = false;
            Polygon polygon1, polygon2;
            newRebarGroup = new RebarGroup();

            if (this.GetPolygonForRebarGroup(parallelRebars, out polygon1, out polygon2))
            {
                newRebarGroup = this.GetRebarGroup(parallelRebars, type, polygon1, polygon2);
                result = true;
            }

            return result;
        }

        /// <summary>The create reinforcement bar group by single rebar.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="groupLines">The group lines.</param>
        /// <param name="type">The type value.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateRebarGroupBySingleRebar(int rebarIndex, ArrayList groupLines, GroupType type)
        {
            var result = false;
            ArrayList parallelRebars;

            if (this.FindRebarsInSameGroup(rebarIndex, groupLines, out parallelRebars))
            {
                var newGroupRebars = this.SplitInDifferentGroupsIfNeeded(parallelRebars);

                if (newGroupRebars != null)
                {
                    foreach (ArrayList groupRebars in newGroupRebars)
                    {
                        RebarGroup newRebarGroup;

                        if (groupRebars.Count >= 2
                            && this.CreateNormalOrTaperedRebarGroup(groupRebars, type, out newRebarGroup))
                        {
                            this.rebarGroups.Add(newRebarGroup);
                            this.AddRebarsToConsideredRebarsIndex(groupRebars);
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>The find reinforcement bar groups.</summary>
        /// <param name="primaryRebarIndex">The primary reinforcement bar index.</param>
        /// <param name="groupLines">The group lines.</param>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool FindRebarGroups(int primaryRebarIndex, ArrayList groupLines, out ArrayList parallelRebars)
        {
            var result = false;
            parallelRebars = new ArrayList();
            var primaryRebar = this.singleRebars[primaryRebarIndex] as SingleRebar;

            if (primaryRebar != null)
            {
                parallelRebars.Add(primaryRebar);
            }

            for (var secondaryRebarIndex = primaryRebarIndex + 1; secondaryRebarIndex < this.singleRebars.Count; secondaryRebarIndex++)
            {
                var secondaryRebar = this.singleRebars[secondaryRebarIndex] as SingleRebar;

                if (secondaryRebar != null && !this.consideredRebarsIndex.Contains(secondaryRebarIndex) &&
                    Distance.PointToLine((Point)secondaryRebar.Polygon.Points[0], (Line)groupLines[0]) < DistanceEpsilon &&
                    Distance.PointToLine((Point)secondaryRebar.Polygon.Points[1], (Line)groupLines[1]) < DistanceEpsilon)
                {
                    parallelRebars.Add(secondaryRebar);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The find reinforcement bars in same group.</summary>
        /// <param name="primaryRebarIndex">The primary reinforcement bar index.</param>
        /// <param name="groupLines">The group lines.</param>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool FindRebarsInSameGroup(int primaryRebarIndex, ArrayList groupLines, out ArrayList parallelRebars)
        {
            var result = false;
            parallelRebars = new ArrayList();

            var primaryRebar = this.singleRebars[primaryRebarIndex] as SingleRebar;
            if (primaryRebar != null)
            {
                result = this.FindRebarGroups(primaryRebarIndex, groupLines, out parallelRebars);
            }

            return result;
        }

        /// <summary>The get distance reinforcement bars.</summary>
        /// <param name="primaryRebar">The primary rebar.</param>
        /// <param name="secondaryRebar">The secondary rebar.</param>
        /// <returns>The System.Double.</returns>
        private double GetDistanceRebars(SingleRebar primaryRebar, SingleRebar secondaryRebar)
        {
            var secondaryRebarLine = new Line((Point)secondaryRebar.Polygon.Points[1], (Point)secondaryRebar.Polygon.Points[0]);
            var secondaryPoint = Projection.PointToLine((Point)primaryRebar.Polygon.Points[0], secondaryRebarLine);
            var distanceRebars = Distance.PointToPoint((Point)primaryRebar.Polygon.Points[0], secondaryPoint);

            return distanceRebars;
        }

        /// <summary>The get group line.</summary>
        /// <param name="primaryRebarIndex">The primary reinforcement bar index.</param>
        /// <param name="secondaryRebarIndex">The secondary reinforcement bar index.</param>
        /// <param name="groupLines">The group lines.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetGroupLine(int primaryRebarIndex, int secondaryRebarIndex, out ArrayList groupLines)
        {
            var result = false;
            groupLines = new ArrayList();
            var primaryRebar = this.singleRebars[primaryRebarIndex] as SingleRebar;
            var secondaryRebar = this.singleRebars[secondaryRebarIndex] as SingleRebar;

            if (primaryRebar != null && secondaryRebar != null)
            {
                groupLines.Add(new Line((Point)secondaryRebar.Polygon.Points[0], (Point)primaryRebar.Polygon.Points[0]));
                groupLines.Add(new Line((Point)secondaryRebar.Polygon.Points[1], (Point)primaryRebar.Polygon.Points[1]));

                if (
                    !(Parallel.LineToLine((Line)groupLines[0], (Line)groupLines[1], AngleEpsilon) &&
                      (DistanceEpsilon > Distance.PointToLine((Point)primaryRebar.Polygon.Points[0], (Line)groupLines[1]))))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The get group segments.</summary>
        /// <param name="groupGeometries">The group geometries.</param>
        /// <returns>The System.Collections.ArrayList.</returns>
        private ArrayList GetGroupSegments(ArrayList groupGeometries)
        {
            var groupSegments = new ArrayList();

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
                }
            }

            return groupSegments;
        }

        /// <summary>The get normal reinforcement bar group.</summary>
        /// <param name="primaryRebarIndex">The primary reinforcement bar index.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetNormalRebarGroup(int primaryRebarIndex)
        {
            var result = false;

            for (var secondaryRebarIndex = primaryRebarIndex + 1; secondaryRebarIndex < this.singleRebars.Count && !result; secondaryRebarIndex++)
            {
                if (!this.consideredRebarsIndex.Contains(secondaryRebarIndex))
                {
                    ArrayList groupLines;
                    if (this.GetGroupLine(primaryRebarIndex, secondaryRebarIndex, out groupLines))
                    {
                        var firstLine = groupLines[0] as Line;
                        var secondLine = groupLines[1] as Line;
                        var primaryRebar = this.singleRebars[primaryRebarIndex] as SingleRebar;

                        if (firstLine != null && secondLine != null && primaryRebar != null)
                        {
                            var rebarVector = new Vector((Point)primaryRebar.Polygon.Points[1] - (Point)primaryRebar.Polygon.Points[0]);

                            if (Parallel.LineToLine(firstLine, secondLine, AngleEpsilon) && Math.Abs(rebarVector.Dot(firstLine.Direction)) < DotEpsilon)
                            {
                                result = this.CreateRebarGroupBySingleRebar(primaryRebarIndex, groupLines, GroupType.Normal);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>The get parallel reinforcement bars to create new groups.</summary>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="secondaryGroupGeometries">The secondary group geometries.</param>
        /// <param name="newParallelRebars">The new parallel reinforcement bars.</param>
        /// <param name="remainingParallelRebars">The remaining parallel reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetParallelRebarsToCreateNewGroups(
            RebarGroup primaryRebarGroup, 
            ArrayList primaryGroupGeometries, 
            ArrayList secondaryGroupGeometries, 
            out ArrayList newParallelRebars, 
            out ArrayList remainingParallelRebars)
        {
            var result = false;
            newParallelRebars = new ArrayList();
            remainingParallelRebars = new ArrayList();

            foreach (RebarGeometry primaryRebarGeometry in primaryGroupGeometries)
            {
                var rebarAdded = false;
                var primaryRebarPoints = primaryRebarGeometry.Shape.Points;
                var singleRebarCreator = new CreateSingleRebar(primaryRebarGroup);
                var newRebar = singleRebarCreator.GetSingleRebar(primaryRebarPoints);

                foreach (RebarGeometry secondaryRebarGeometry in secondaryGroupGeometries)
                {
                    if (!rebarAdded && newRebar != null)
                    {
                        var secondaryRebarPoints = secondaryRebarGeometry.Shape.Points;

                        if (Distance.PointToPoint((Point)primaryRebarPoints[0], (Point)secondaryRebarPoints[0]) < DistanceEpsilon ||
                            Distance.PointToPoint((Point)primaryRebarPoints[1], (Point)secondaryRebarPoints[1]) < DistanceEpsilon)
                        {
                            var primaryRebarVector = new Vector((Point)primaryRebarPoints[1] - (Point)primaryRebarPoints[0]);
                            var secondaryRebarVector = new Vector((Point)secondaryRebarPoints[1] - (Point)secondaryRebarPoints[0]);

                            rebarAdded = this.AddRebarsIfDoNotOverlap(newRebar, primaryRebarVector, secondaryRebarVector, newParallelRebars);
                        }
                        else if (Distance.PointToPoint((Point)primaryRebarPoints[0], (Point)secondaryRebarPoints[1]) < DistanceEpsilon ||
                                 Distance.PointToPoint((Point)primaryRebarPoints[1], (Point)secondaryRebarPoints[0]) < DistanceEpsilon)
                        {
                            var primaryRebarVector = new Vector((Point)primaryRebarPoints[1] - (Point)primaryRebarPoints[0]);
                            var secondaryRebarVector = new Vector((Point)secondaryRebarPoints[0] - (Point)secondaryRebarPoints[1]);

                            rebarAdded = this.AddRebarsIfDoNotOverlap(newRebar, primaryRebarVector, secondaryRebarVector, newParallelRebars);
                        }
                    }
                }

                if (!rebarAdded)
                {
                    remainingParallelRebars.Add(newRebar);
                }
            }

            if (newParallelRebars.Count > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>The get polygon for reinforcement bar group.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <param name="polygon1">The polygon 1.</param>
        /// <param name="polygon2">The polygon 2.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetPolygonForRebarGroup(ArrayList parallelRebars, out Polygon polygon1, out Polygon polygon2)
        {
            var result = false;
            polygon1 = new Polygon();
            polygon2 = new Polygon();

            var firstRebar = parallelRebars[0] as SingleRebar;
            var lastRebar = parallelRebars[parallelRebars.Count - 1] as SingleRebar;

            var slabCoordinates = this.rebarGroupConversionData.FatherSlab.GetCoordinateSystem();

            if (firstRebar != null && lastRebar != null)
            {
                var firstRebarLine = new Line(firstRebar.Polygon.Points[0] as Point, firstRebar.Polygon.Points[1] as Point);
                var lastRebarLine = new Line(lastRebar.Polygon.Points[0] as Point, lastRebar.Polygon.Points[1] as Point);

                if (Distance.PointToLine(slabCoordinates.Origin, firstRebarLine) > Distance.PointToLine(slabCoordinates.Origin, lastRebarLine))
                {
                    var auxiliaryRebar = firstRebar;
                    firstRebar = lastRebar;
                    lastRebar = auxiliaryRebar;
                }

                polygon1.Points.Add(lastRebar.Polygon.Points[0]);
                polygon1.Points.Add(lastRebar.Polygon.Points[1]);
                polygon2.Points.Add(firstRebar.Polygon.Points[0]);
                polygon2.Points.Add(firstRebar.Polygon.Points[1]);

                result = true;
            }

            return result;
        }

        /// <summary>The get reinforcement bar group.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <param name="type">The type value.</param>
        /// <param name="polygon1">The polygon 1.</param>
        /// <param name="polygon2">The polygon 2.</param>
        /// <returns>The Tekla.Structures.Model.RebarGroup.</returns>
        private RebarGroup GetRebarGroup(ArrayList parallelRebars, GroupType type, Polygon polygon1, Polygon polygon2)
        {
            var newRebarGroup = new RebarGroup();
            var primaryRebar = parallelRebars[0] as SingleRebar;
            var secondaryRebar = parallelRebars[1] as SingleRebar;

            if (primaryRebar != null && secondaryRebar != null)
            {
                var secondaryRebarLine = new Line((Point)secondaryRebar.Polygon.Points[1], (Point)secondaryRebar.Polygon.Points[0]);
                var lineSecondPoint = Projection.PointToLine((Point)primaryRebar.Polygon.Points[0], secondaryRebarLine);
                var rebarDistance = Distance.PointToPoint((Point)primaryRebar.Polygon.Points[0], lineSecondPoint);

                newRebarGroup.Polygons.Add(polygon1);

                if (type == GroupType.Normal)
                {
                    newRebarGroup.StartPoint = (Point)polygon1.Points[0];
                    newRebarGroup.EndPoint = (Point)polygon2.Points[0];
                }
                else
                {
                    newRebarGroup.Polygons.Add(polygon2);
                }

                newRebarGroup.SpacingType = BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE;
                newRebarGroup.Spacings.Add(rebarDistance);

                newRebarGroup.ExcludeType = BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;
                newRebarGroup.Father = primaryRebar.Father;

                newRebarGroup.NumberingSeries.Prefix = primaryRebar.NumberingSeries.Prefix;
                newRebarGroup.NumberingSeries.StartNumber = primaryRebar.NumberingSeries.StartNumber;
                newRebarGroup.Name = primaryRebar.Name;
                newRebarGroup.Size = primaryRebar.Size;
                newRebarGroup.Grade = primaryRebar.Grade;
                newRebarGroup.RadiusValues = primaryRebar.RadiusValues;
                newRebarGroup.Class = primaryRebar.Class;

                newRebarGroup.StartHook.Shape = primaryRebar.StartHook.Shape;
                newRebarGroup.StartHook.Angle = primaryRebar.StartHook.Angle;
                newRebarGroup.StartHook.Radius = primaryRebar.StartHook.Radius;
                newRebarGroup.StartHook.Length = primaryRebar.StartHook.Length;
                newRebarGroup.EndHook.Shape = primaryRebar.EndHook.Shape;
                newRebarGroup.EndHook.Angle = primaryRebar.EndHook.Angle;
                newRebarGroup.EndHook.Radius = primaryRebar.EndHook.Radius;
                newRebarGroup.EndHook.Length = primaryRebar.EndHook.Length;

                newRebarGroup.OnPlaneOffsets = primaryRebar.OnPlaneOffsets;
                newRebarGroup.FromPlaneOffset = primaryRebar.FromPlaneOffset;

                newRebarGroup.StartPointOffsetValue = primaryRebar.StartPointOffsetValue;
                newRebarGroup.StartPointOffsetType = primaryRebar.StartPointOffsetType;
                newRebarGroup.EndPointOffsetValue = primaryRebar.EndPointOffsetValue;
                newRebarGroup.EndPointOffsetType = primaryRebar.EndPointOffsetType;

                if (this.RebarHooksShouldBeTurned(newRebarGroup))
                {
                    if (newRebarGroup.StartHook.Shape != RebarHookData.RebarHookShapeEnum.NO_HOOK)
                    {
                        newRebarGroup.StartHook.Shape = RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
                        newRebarGroup.StartHook.Angle = (-1) * primaryRebar.StartHook.Angle;
                    }

                    if (newRebarGroup.EndHook.Shape != RebarHookData.RebarHookShapeEnum.NO_HOOK)
                    {
                        newRebarGroup.EndHook.Shape = RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
                        newRebarGroup.EndHook.Angle = (-1) * primaryRebar.EndHook.Angle;
                    }
                }
            }

            return newRebarGroup;
        }

        /// <summary>
        /// The get reinforcement bar groups.
        /// </summary>
        private void GetRebarGroups()
        {
            if (this.singleRebars.Count > 1)
            {
                for (var primaryRebarIndex = 0; primaryRebarIndex < this.singleRebars.Count - 1; primaryRebarIndex++)
                {
                    if (!this.consideredRebarsIndex.Contains(primaryRebarIndex))
                    {
                        this.GetNormalRebarGroup(primaryRebarIndex);
                    }
                }

                for (var primaryRebarIndex = 0; primaryRebarIndex < this.singleRebars.Count - 1; primaryRebarIndex++)
                {
                    if (!this.consideredRebarsIndex.Contains(primaryRebarIndex))
                    {
                        this.GetTaperedRebarGroup(primaryRebarIndex);
                    }
                }
            }
        }

        /// <summary>The get tapered reinforcement bar group.</summary>
        /// <param name="primaryRebarIndex">The primary reinforcement bar index.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetTaperedRebarGroup(int primaryRebarIndex)
        {
            var result = false;

            for (var secondaryRebarIndex = primaryRebarIndex + 1; secondaryRebarIndex < this.singleRebars.Count && !result; secondaryRebarIndex++)
            {
                if (!this.consideredRebarsIndex.Contains(secondaryRebarIndex))
                {
                    ArrayList groupLines;
                    if (this.GetGroupLine(primaryRebarIndex, secondaryRebarIndex, out groupLines))
                    {
                        result = this.CreateRebarGroupBySingleRebar(primaryRebarIndex, groupLines, GroupType.Tapered);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get ungrouped rebars.
        /// </summary>
        private void GetUngroupedRebars()
        {
            for (var rebarIndex = 0; rebarIndex < this.singleRebars.Count; rebarIndex++)
            {
                if (!this.consideredRebarsIndex.Contains(rebarIndex))
                {
                    var ungroupedRebar = this.singleRebars[rebarIndex] as SingleRebar;

                    if (ungroupedRebar != null && this.RebarHooksShouldBeTurned(ungroupedRebar))
                    {
                        var rebarPoints = new ArrayList { ungroupedRebar.Polygon.Points[1], ungroupedRebar.Polygon.Points[0] };

                        var singleRebarCreator = new CreateSingleRebar(ungroupedRebar);
                        ungroupedRebar = singleRebarCreator.GetSingleRebar(rebarPoints);
                        SwapStartHookWithEndHook(ref ungroupedRebar);
                    }

                    this.ungroupedRebars.Add(ungroupedRebar);
                }
            }
        }

        /// <summary>The group should be splitted.</summary>
        /// <param name="primaryRebar">The primary rebar.</param>
        /// <param name="secondaryRebar">The secondary rebar.</param>
        /// <param name="previusDistanceRebars">The previus distance reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GroupShouldBeSplitted(SingleRebar primaryRebar, SingleRebar secondaryRebar, ref double previusDistanceRebars)
        {
            var result = false;
            var distanceRebars = this.GetDistanceRebars(primaryRebar, secondaryRebar);

            if (previusDistanceRebars > DistanceEpsilon
                && Math.Abs(distanceRebars - previusDistanceRebars) > DistanceEpsilon)
            {
                result = true;
            }

            previusDistanceRebars = distanceRebars;

            return result;
        }

        /// <summary>The is first reinforcement bar distance equal.</summary>
        /// <param name="firstRebar">The first rebar.</param>
        /// <param name="secondRebar">The second rebar.</param>
        /// <param name="thirdRebar">The third rebar.</param>
        /// <returns>The System.Boolean.</returns>
        private bool IsFirstRebarDistanceEqual(SingleRebar firstRebar, SingleRebar secondRebar, SingleRebar thirdRebar)
        {
            var result = false;
            var firstRebarDistance = this.GetDistanceRebars(firstRebar, secondRebar);
            var secondRebarDistance = this.GetDistanceRebars(secondRebar, thirdRebar);

            if (Math.Abs(firstRebarDistance - secondRebarDistance) < DistanceEpsilon)
            {
                result = true;
            }

            return result;
        }

        /// <summary>The line segments overlap but are not identical.</summary>
        /// <param name="primarySegment">The primary segment.</param>
        /// <param name="secondarySegment">The secondary segment.</param>
        /// <returns>The System.Boolean.</returns>
        private bool LineSegmentsOverlapButAreNotIdentical(LineSegment primarySegment, LineSegment secondarySegment)
        {
            return Parallel.LineSegmentToLineSegment(primarySegment, secondarySegment, AngleEpsilon) &&
                   (Distance.PointToLineSegment(secondarySegment.Point1, primarySegment) < DistanceEpsilon ||
                   Distance.PointToLineSegment(secondarySegment.Point2, primarySegment) < DistanceEpsilon) &&
                   !(Distance.PointToPoint(primarySegment.Point1, secondarySegment.Point1) < DistanceEpsilon &&
                   Distance.PointToPoint(primarySegment.Point2, secondarySegment.Point2) < DistanceEpsilon);
        }

        /// <summary>The primary reinforcement bar group should be split.</summary>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="secondaryGroupGeometries">The secondary group geometries.</param>
        /// <returns>The System.Boolean.</returns>
        private bool PrimaryRebarGroupShouldBeSplit(ArrayList primaryGroupGeometries, ArrayList secondaryGroupGeometries)
        {
            var result = false;

            var primaryGroupSegments = this.GetGroupSegments(primaryGroupGeometries);
            var secondaryGroupSegments = this.GetGroupSegments(secondaryGroupGeometries);

            foreach (LineSegment primarySegment in primaryGroupSegments)
            {
                foreach (LineSegment secondarySegment in secondaryGroupSegments)
                {
                    if (primarySegment.Length() > secondarySegment.Length())
                    {
                        result |= this.LineSegmentsOverlapButAreNotIdentical(primarySegment, secondarySegment)
                                  &&
                                  this.RebarGroupsDoNotOverlap(
                                      (RebarGeometry)primaryGroupGeometries[0], 
                                      (RebarGeometry)secondaryGroupGeometries[0]);
                    }
                }
            }

            return result;
        }

        /// <summary>The primary reinforcement bar group should be split.</summary>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="secondaryRebar">The secondary rebar.</param>
        /// <param name="splitRebarGeometry">The split reinforcement bar geometry.</param>
        /// <param name="remainingParallelRebars">The remaining parallel reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool PrimaryRebarGroupShouldBeSplit(
            RebarGroup primaryRebarGroup, 
            ArrayList primaryGroupGeometries, 
            SingleRebar secondaryRebar, 
            out RebarGeometry splitRebarGeometry, 
            out ArrayList remainingParallelRebars)
        {
            var result = false;
            splitRebarGeometry = null;
            remainingParallelRebars = new ArrayList();
            int rebarIndex;

            for (rebarIndex = 0; rebarIndex < primaryGroupGeometries.Count; rebarIndex++)
            {
                var singleRebarGeometry = primaryGroupGeometries[rebarIndex] as RebarGeometry;

                if (singleRebarGeometry != null)
                {
                    var primaryRebarPoints = singleRebarGeometry.Shape.Points;

                    if (Distance.PointToPoint((Point)primaryRebarPoints[0], (Point)secondaryRebar.Polygon.Points[0]) < DistanceEpsilon ||
                        Distance.PointToPoint((Point)primaryRebarPoints[0], (Point)secondaryRebar.Polygon.Points[1]) < DistanceEpsilon ||
                        Distance.PointToPoint((Point)primaryRebarPoints[1], (Point)secondaryRebar.Polygon.Points[0]) < DistanceEpsilon ||
                        Distance.PointToPoint((Point)primaryRebarPoints[1], (Point)secondaryRebar.Polygon.Points[1]) < DistanceEpsilon)
                    {
                        splitRebarGeometry = singleRebarGeometry;
                        result = true;
                    }
                    else
                    {
                        var singleRebarCreator = new CreateSingleRebar(primaryRebarGroup);
                        var newRebar = singleRebarCreator.GetSingleRebar(primaryRebarPoints);

                        if (newRebar != null)
                        {
                            remainingParallelRebars.Add(newRebar);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>The reinforcement bar array list contains single rebar.</summary>
        /// <param name="rebar">The rebar.</param>
        /// <param name="rebarsArray">The reinforcement bars array.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarArrayListContainsSingleRebar(SingleRebar rebar, ArrayList rebarsArray)
        {
            var result = false;

            foreach (SingleRebar rebarInArray in rebarsArray)
            {
                if ((Distance.PointToPoint((Point)rebar.Polygon.Points[0], (Point)rebarInArray.Polygon.Points[0]) < DistanceEpsilon &&
                     Distance.PointToPoint((Point)rebar.Polygon.Points[1], (Point)rebarInArray.Polygon.Points[1]) < DistanceEpsilon) ||
                    (Distance.PointToPoint((Point)rebar.Polygon.Points[1], (Point)rebarInArray.Polygon.Points[0]) < DistanceEpsilon &&
                     Distance.PointToPoint((Point)rebar.Polygon.Points[0], (Point)rebarInArray.Polygon.Points[1]) < DistanceEpsilon))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The reinforcement bar group array list contains reinforcement bar group.</summary>
        /// <param name="rebarGroup">The reinforcement bar group.</param>
        /// <param name="rebarGroupsArray">The reinforcement bar groups array.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarGroupArrayListContainsRebarGroup(RebarGroup rebarGroup, ArrayList rebarGroupsArray)
        {
            var result = false;

            foreach (RebarGroup groupInArray in rebarGroupsArray)
            {
                if (rebarGroup.Polygons.Count == groupInArray.Polygons.Count)
                {
                    var rebarPolygonFirst = rebarGroup.Polygons[0] as Polygon;
                    var groupPolygonFirst = groupInArray.Polygons[0] as Polygon;

                    if (rebarPolygonFirst != null && groupPolygonFirst != null &&
                        ((Distance.PointToPoint((Point)rebarPolygonFirst.Points[0], (Point)groupPolygonFirst.Points[0]) < DistanceEpsilon &&
                          Distance.PointToPoint((Point)rebarPolygonFirst.Points[1], (Point)groupPolygonFirst.Points[1]) < DistanceEpsilon) ||
                         (Distance.PointToPoint((Point)rebarPolygonFirst.Points[1], (Point)groupPolygonFirst.Points[0]) < DistanceEpsilon &&
                          Distance.PointToPoint((Point)rebarPolygonFirst.Points[0], (Point)groupPolygonFirst.Points[1]) < DistanceEpsilon)))
                    {
                        if (rebarGroup.Polygons.Count == 1)
                        {
                            if ((Distance.PointToPoint(rebarGroup.StartPoint, groupInArray.StartPoint) < DistanceEpsilon &&
                                Distance.PointToPoint(rebarGroup.EndPoint, groupInArray.EndPoint) < DistanceEpsilon) ||
                                (Distance.PointToPoint(rebarGroup.EndPoint, groupInArray.StartPoint) < DistanceEpsilon &&
                                 Distance.PointToPoint(rebarGroup.StartPoint, groupInArray.EndPoint) < DistanceEpsilon))
                            {
                                result = true;
                            }
                        }
                        else if (rebarGroup.Polygons.Count == 2)
                        {
                            var rebarPolygonSecond = rebarGroup.Polygons[1] as Polygon;
                            var groupPolygonSecond = groupInArray.Polygons[1] as Polygon;

                            if (rebarPolygonSecond != null && groupPolygonSecond != null &&
                                ((Distance.PointToPoint((Point)rebarPolygonSecond.Points[0], (Point)groupPolygonSecond.Points[0]) < DistanceEpsilon &&
                                  Distance.PointToPoint((Point)rebarPolygonSecond.Points[1], (Point)groupPolygonSecond.Points[1]) < DistanceEpsilon) ||
                                 (Distance.PointToPoint((Point)rebarPolygonSecond.Points[1], (Point)groupPolygonSecond.Points[0]) < DistanceEpsilon &&
                                  Distance.PointToPoint((Point)rebarPolygonSecond.Points[0], (Point)groupPolygonSecond.Points[1]) < DistanceEpsilon)))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>The reinforcement bar groups do not overlap.</summary>
        /// <param name="primaryRebarGeometry">The primary reinforcement bar geometry.</param>
        /// <param name="secondaryRebarGeometry">The secondary reinforcement bar geometry.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RebarGroupsDoNotOverlap(RebarGeometry primaryRebarGeometry, RebarGeometry secondaryRebarGeometry)
        {
            var result = false;
            var primaryRebarPoints = primaryRebarGeometry.Shape.Points;
            var secondaryRebarPoints = secondaryRebarGeometry.Shape.Points;
            var pointsDistance = Distance.PointToPoint((Point)primaryRebarPoints[0], (Point)secondaryRebarPoints[0]);
            int primaryPointNear = 0, secondaryPointNear = 0;

            for (var primaryPoint = 0; primaryPoint < primaryRebarPoints.Count; primaryPoint++)
            {
                for (var secondaryPoint = 0; secondaryPoint < secondaryRebarPoints.Count; secondaryPoint++)
                {
                    if (Distance.PointToPoint((Point)primaryRebarPoints[primaryPoint], (Point)secondaryRebarPoints[secondaryPoint]) < pointsDistance)
                    {
                        pointsDistance = Distance.PointToPoint((Point)primaryRebarPoints[primaryPoint], (Point)secondaryRebarPoints[secondaryPoint]);
                        primaryPointNear = primaryPoint;
                        secondaryPointNear = secondaryPoint;
                    }
                }
            }

            var primaryPointFar = primaryPointNear == 0 ? 1 : 0;
            var secondaryPointFar = secondaryPointNear == 0 ? 1 : 0;

            var primaryRebarVector = new Vector((Point)primaryRebarPoints[primaryPointFar] - (Point)primaryRebarPoints[primaryPointNear]);
            var secondaryRebarVector = new Vector((Point)secondaryRebarPoints[secondaryPointFar] - (Point)secondaryRebarPoints[secondaryPointNear]);

            if (primaryRebarVector.Dot(secondaryRebarVector) < DotEpsilon)
            {
                result = true;
            }

            return result;
        }

        /// <summary>Determines whether the hooks of the rebar should be turned.</summary>
        /// <param name="currentRebar">The reinforcement bar group.</param>
        /// <returns>True if the hooks of the group should be turned.</returns>
        private bool RebarHooksShouldBeTurned(Reinforcement currentRebar)
        {
            var result = false;

            var rebarHookDirection = GetHookDirection(currentRebar);

            if (this.rebarGroupConversionData.DepthLocation != int.MinValue && rebarHookDirection != null)
            {
                if (this.rebarGroupConversionData.DepthLocation == 0 ^ rebarHookDirection.GetAngleBetween(this.coordinateZ) < Degress90)
                {
                    result = true;
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

        /// <summary>The split groups with two reinforcement bars or more.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <param name="newRebarGroups">The new reinforcement bar groups.</param>
        private void SplitGroupsWithTwoRebarsOrMore(ArrayList parallelRebars, ArrayList newRebarGroups)
        {
            var firstRemoved = false;
            var groupCount = 0;
            var previusDistanceRebars = 0.0;

            for (var rebarIndex = 0; rebarIndex < parallelRebars.Count - 1; rebarIndex++)
            {
                var primaryRebar = parallelRebars[rebarIndex] as SingleRebar;
                var secondaryRebar = parallelRebars[rebarIndex + 1] as SingleRebar;

                if (primaryRebar != null && secondaryRebar != null)
                {
                    var newGroup = (ArrayList)newRebarGroups[groupCount];
                    newGroup.Add(primaryRebar);

                    if (firstRemoved)
                    {
                        if (!this.IsFirstRebarDistanceEqual(newGroup[0] as SingleRebar, primaryRebar, secondaryRebar))
                        {
                            newGroup.RemoveAt(0);
                        }

                        firstRemoved = false;
                    }

                    if (this.GroupShouldBeSplitted(primaryRebar, secondaryRebar, ref previusDistanceRebars))
                    {
                        if (newGroup.Count == 0)
                        {
                        }
                        else if (newGroup.Count == 1)
                        {
                            newGroup.RemoveAt(0);
                        }
                        else if (newGroup.Count == 2)
                        {
                            newGroup.RemoveAt(0);
                            firstRemoved = true;
                        }
                        else
                        {
                            groupCount++;
                            newRebarGroups.Add(new ArrayList());
                        }

                        previusDistanceRebars = 0.0;
                    }

                    if (rebarIndex == parallelRebars.Count - 2)
                    {
                        var secondaryGroup = (ArrayList)newRebarGroups[groupCount];
                        secondaryGroup.Add(secondaryRebar);
                    }
                }
            }
        }

        /// <summary>The split in different groups if needed.</summary>
        /// <param name="parallelRebars">The parallel reinforcement bars.</param>
        /// <returns>The System.Collections.ArrayList.</returns>
        private ArrayList SplitInDifferentGroupsIfNeeded(ArrayList parallelRebars)
        {
            var newRebarGroups = new ArrayList { new ArrayList() };

            if (parallelRebars.Count > 2)
            {
                this.SplitGroupsWithTwoRebarsOrMore(parallelRebars, newRebarGroups);
            }
            else if (parallelRebars.Count == 2)
            {
                var primaryRebar = parallelRebars[0] as SingleRebar;
                var secondaryRebar = parallelRebars[1] as SingleRebar;
                var newGroup = (ArrayList)newRebarGroups[0];
                newGroup.Add(primaryRebar);
                newGroup.Add(secondaryRebar);
            }
            else
            {
                newRebarGroups = null;
            }

            return newRebarGroups;
        }

        /// <summary>The split primary reinforcement bar group.</summary>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="secondaryGroupGeometries">The secondary group geometries.</param>
        /// <param name="splitRebarGroups">The split reinforcement bar groups.</param>
        /// <param name="splitUngroupedRebars">The split ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitPrimaryRebarGroup(
            RebarGroup primaryRebarGroup, 
            ArrayList primaryGroupGeometries, 
            ArrayList secondaryGroupGeometries, 
            ref ArrayList splitRebarGroups, 
            ref ArrayList splitUngroupedRebars)
        {
            var result = false;
            ArrayList newParallelRebars, remainingParallelRebars;

            if (this.GetParallelRebarsToCreateNewGroups(
                primaryRebarGroup, 
                primaryGroupGeometries, 
                secondaryGroupGeometries, 
                out newParallelRebars, 
                out remainingParallelRebars))
            {
                var type = GroupType.Undefined;

                if (primaryRebarGroup.Polygons.Count == 1)
                {
                    type = GroupType.Normal;
                }
                else if (primaryRebarGroup.Polygons.Count == 2)
                {
                    type = GroupType.Tapered;
                }

                result = this.AddParallelRebarsToGroup(
                    newParallelRebars, type, ref splitRebarGroups, ref splitUngroupedRebars);

                this.AddParallelRebarsToGroup(remainingParallelRebars, type, ref splitRebarGroups, ref splitUngroupedRebars);
            }

            return result;
        }

        /// <summary>The split primary reinforcement bar group by reinforcement bar groups.</summary>
        /// <param name="newRebarGroups">The reinforcement bar groups.</param>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="primaryGroupIndex">The primary group index.</param>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="splitRebarGroups">The split reinforcement bar groups.</param>
        /// <param name="splitUngroupedRebars">The split ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitPrimaryRebarGroupByRebarGroups(
            ArrayList newRebarGroups, 
            RebarGroup primaryRebarGroup, 
            int primaryGroupIndex, 
            ArrayList primaryGroupGeometries, 
            ref ArrayList splitRebarGroups, 
            ref ArrayList splitUngroupedRebars)
        {
            var result = false;

            for (var secondaryGroupIndex = 0; secondaryGroupIndex < newRebarGroups.Count && !result; secondaryGroupIndex++)
            {
                var secondaryRebarGroup = newRebarGroups[secondaryGroupIndex] as RebarGroup;

                if (secondaryRebarGroup != null)
                {
                    secondaryRebarGroup.Insert();
                    var secondaryGroupGeometries = secondaryRebarGroup.GetRebarGeometries(false);
                    secondaryRebarGroup.Delete();

                    if (primaryGroupIndex != secondaryGroupIndex
                        && this.PrimaryRebarGroupShouldBeSplit(primaryGroupGeometries, secondaryGroupGeometries))
                    {
                        result = this.SplitPrimaryRebarGroup(
                            primaryRebarGroup, 
                            primaryGroupGeometries, 
                            secondaryGroupGeometries, 
                            ref splitRebarGroups, 
                            ref splitUngroupedRebars);
                    }
                }
            }

            return result;
        }

        /// <summary>The split primary reinforcement bar group by single rebar.</summary>
        /// <param name="newUngroupedRebars">The ungrouped reinforcement bars.</param>
        /// <param name="primaryRebarGroup">The primary reinforcement bar group.</param>
        /// <param name="primaryGroupGeometries">The primary group geometries.</param>
        /// <param name="splitRebarGroups">The split reinforcement bar groups.</param>
        /// <param name="splitUngroupedRebars">The split ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitPrimaryRebarGroupBySingleRebar(
            ArrayList newUngroupedRebars, 
            RebarGroup primaryRebarGroup, 
            ArrayList primaryGroupGeometries, 
            ref ArrayList splitRebarGroups, 
            ref ArrayList splitUngroupedRebars)
        {
            var result = false;
            var splitGroupBySingleRebar = false;

            for (var secondaryRebarIndex = 0; secondaryRebarIndex < newUngroupedRebars.Count && !splitGroupBySingleRebar; secondaryRebarIndex++)
            {
                RebarGeometry splitRebarGeometry;
                ArrayList remainingParallelRebars;
                var secondaryRebar = newUngroupedRebars[secondaryRebarIndex] as SingleRebar;

                if (secondaryRebar != null && this.PrimaryRebarGroupShouldBeSplit(primaryRebarGroup, primaryGroupGeometries, secondaryRebar, out splitRebarGeometry, out remainingParallelRebars))
                {
                    secondaryRebar.Insert();
                    var secondaryGroupGeometries = secondaryRebar.GetRebarGeometries(false);
                    secondaryRebar.Delete();
                    result = this.SplitPrimaryRebarGroup(primaryRebarGroup, primaryGroupGeometries, secondaryGroupGeometries, ref splitRebarGroups, ref splitUngroupedRebars);

                    if ((primaryRebarGroup.Polygons.Count == 2 && remainingParallelRebars.Count == 2) || remainingParallelRebars.Count == 1)
                    {
                        foreach (SingleRebar rebar in remainingParallelRebars)
                        {
                            if (!this.RebarArrayListContainsSingleRebar(rebar, splitUngroupedRebars))
                            {
                                splitUngroupedRebars.Add(rebar);
                            }
                        }

                        splitGroupBySingleRebar = true;
                    }
                }
            }

            return result;
        }

        /// <summary>The split reinforcement bar groups.</summary>
        /// <param name="newRebarGroups">The reinforcement bar groups.</param>
        /// <param name="newUngroupedRebars">The ungrouped reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitRebarGroups(ref ArrayList newRebarGroups, ref ArrayList newUngroupedRebars)
        {
            var result = false;
            var splitRebarGroups = new ArrayList();
            var splitUngroupedRebars = new ArrayList();

            for (var primaryGroupIndex = 0; primaryGroupIndex < newRebarGroups.Count; primaryGroupIndex++)
            {
                bool splitGroup;
                var primaryRebarGroup = newRebarGroups[primaryGroupIndex] as RebarGroup;

                if (primaryRebarGroup != null)
                {
                    primaryRebarGroup.Insert();
                    var primaryGroupGeometries = primaryRebarGroup.GetRebarGeometries(false);
                    primaryRebarGroup.Delete();

                    splitGroup = this.SplitPrimaryRebarGroupByRebarGroups(
                        newRebarGroups, 
                        primaryRebarGroup, 
                        primaryGroupIndex, 
                        primaryGroupGeometries, 
                        ref splitRebarGroups, 
                        ref splitUngroupedRebars);

                    if (!splitGroup)
                    {
                        splitGroup = this.SplitPrimaryRebarGroupBySingleRebar(
                            newUngroupedRebars, 
                            primaryRebarGroup, 
                            primaryGroupGeometries, 
                            ref splitRebarGroups, 
                            ref splitUngroupedRebars);
                    }

                    if (splitGroup)
                    {
                        result = true;
                    }
                    else
                    {
                        splitRebarGroups.Add(primaryRebarGroup);
                    }
                }
            }

            newRebarGroups = splitRebarGroups;

            foreach (SingleRebar rebar in splitUngroupedRebars)
            {
                if (rebar != null && !this.RebarArrayListContainsSingleRebar(rebar, newUngroupedRebars))
                {
                    if (this.RebarHooksShouldBeTurned(rebar))
                    {
                        var rebarPoints = new ArrayList { rebar.Polygon.Points[1], rebar.Polygon.Points[0] };

                        var singleRebarCreator = new CreateSingleRebar(rebar);
                        var turnedRebar = singleRebarCreator.GetSingleRebar(rebarPoints);
                        SwapStartHookWithEndHook(ref turnedRebar);
                        newUngroupedRebars.Add(turnedRebar);
                    }
                    else
                    {
                        newUngroupedRebars.Add(rebar);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The split reinforcement bar groups for splicing.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool SplitRebarGroupsForSplicing()
        {
            var result = false;
            var rebarGroups = new ArrayList(this.rebarGroups);
            var ungroupedRebars = new ArrayList(this.ungroupedRebars);

            if (this.rebarGroups.Count > 1)
            {
                var splitGoups = true;

                for (var ii = 0; ii < MaximumIterations && splitGoups; ii++)
                {
                    splitGoups = this.SplitRebarGroups(ref rebarGroups, ref ungroupedRebars);
                }

                this.rebarGroups = rebarGroups;
                this.ungroupedRebars = ungroupedRebars;

                result = true;
            }

            return result;
        }

        #endregion
    }
}