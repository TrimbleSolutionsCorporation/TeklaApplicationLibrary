namespace Tekla.Structures.Concrete
{
    using System;
    using System.Collections;

    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    /// <summary>
    /// The split reinforcement bars in part.
    /// </summary>
    public class SplitRebarsInPart
    {
        #region Constants

        /// <summary>
        /// The additiona l_ reba r_ length.
        /// </summary>
        private const double AdditionalRebarLength = 0.05;

        /// <summary>
        /// The angl e_ epsilon.
        /// </summary>
        private const double AngleEpsilon = 0.0001;

        /// <summary>
        /// The distanc e_ epsilon.
        /// </summary>
        private const double DistanceEpsilon = 0.001;

        /// <summary>
        /// The reba r_ lengt h_ epsilon.
        /// </summary>
        private const double RebarLengthEpsilon = 12.5;

        /// <summary>
        /// The standar d_ hoo k_ angl e_135.
        /// </summary>
        private const double StandardHookAngle135 = 135;

        /// <summary>
        /// The standar d_ hoo k_ angl e_180.
        /// </summary>
        private const double StandardHookAngle180 = 180;

        /// <summary>
        /// The standar d_ hoo k_ angl e_90.
        /// </summary>
        private const double StandardHookAngle90 = 90;

        /// <summary>
        /// The standar d_ hoo k_ length.
        /// </summary>
        private const double StandardHookLength = 120;

        /// <summary>
        /// The standar d_ hoo k_ radius.
        /// </summary>
        private const double StandardHookRadius = 30;

        #endregion

        #region Fields

        /// <summary>
        /// The m_ original rebar.
        /// </summary>
        private SingleRebar originalRebar;

        /// <summary>
        /// The m_ single rebars.
        /// </summary>
        private ArrayList singleRebars;

        /// <summary>
        /// The m_ split data.
        /// </summary>
        private SplitData splitData;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SplitRebarsInPart"/> class.</summary>
        /// <param name="splitData">The split data.</param>
        /// <param name="singleRebars">The single reinforcement bars.</param>
        public SplitRebarsInPart(SplitData splitData, ArrayList singleRebars)
        {
            this.splitData = splitData;
            this.singleRebars = singleRebars;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitRebarsInPart"/> class.
        /// </summary>
        public SplitRebarsInPart()
        {
        }

        #endregion

        #region Enums

        /// <summary>
        /// The bar positions.
        /// </summary>
        private enum BarPositions
        {
            /// <summary>
            /// The first.
            /// </summary>
            First, 

            /// <summary>
            /// The second.
            /// </summary>
            Second, 

            /// <summary>
            /// The third.
            /// </summary>
            Third, 

            /// <summary>
            /// The forth.
            /// </summary>
            Forth, 

            /// <summary>
            /// The undefined.
            /// </summary>
            Undefined, 
        }

        /// <summary>
        /// The first or last enum.
        /// </summary>
        private enum FirstOrLastEnum
        {
            /// <summary>
            /// The undefined.
            /// </summary>
            Undefined, 

            /// <summary>
            /// The firstrebar.
            /// </summary>
            FirstRebar, 

            /// <summary>
            /// The lastrebar.
            /// </summary>
            LastRebar, 

            /// <summary>
            /// The middlerebar.
            /// </summary>
            MiddleRebar, 
        }

        /// <summary>
        /// The splice section.
        /// </summary>
        private enum SpliceSection
        {
            /// <summary>
            /// The ever y_ single.
            /// </summary>
            EverySingle, 

            /// <summary>
            /// The ever y_ second.
            /// </summary>
            EverySecond, 

            /// <summary>
            /// The ever y_ third.
            /// </summary>
            EveryThird, 

            /// <summary>
            /// The ever y_ forth.
            /// </summary>
            EveryForth, 

            /// <summary>
            /// The undefined.
            /// </summary>
            Undefined, 
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The split reinforcement bars if needed.</summary>
        /// <param name="newSplitData">The split data.</param>
        /// <param name="newSingleRebars">The single reinforcement bars.</param>
        /// <param name="splitRebars">The split reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SplitRebarsIfNeeded(SplitData newSplitData, ArrayList newSingleRebars, out ArrayList splitRebars)
        {
            var result = false;
            splitRebars = new ArrayList();

            if (newSplitData != null)
            {
                this.splitData = newSplitData;
            }

            if (newSingleRebars != null)
            {
                this.singleRebars = newSingleRebars;
            }

            if (this.splitData != null && this.singleRebars != null)
            {
                result = this.SplitRebarsIfNeeded(out splitRebars);
            }

            return result;
        }

        /// <summary>The split reinforcement bars if needed.</summary>
        /// <param name="newSplitRebars">The split reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SplitRebarsIfNeeded(out ArrayList newSplitRebars)
        {
            var result = false;
            var remainingOffset = 0.0;
            newSplitRebars = new ArrayList();

            if (this.singleRebars.Count > 0)
            {
                for (var rebarIndex = 0; rebarIndex < this.singleRebars.Count; rebarIndex++)
                {
                    this.originalRebar = this.singleRebars[rebarIndex] as SingleRebar;
                    if (this.originalRebar != null
                        &&
                        Distance.PointToPoint(
                            (Point)this.originalRebar.Polygon.Points[1], (Point)this.originalRebar.Polygon.Points[0])
                        > this.splitData.MaxLength + this.GetRebarLengthEpsilon())
                    {
                        bool rebarSplitted;
                        ArrayList splitRebarSet;

                        switch (this.splitData.SpliceSection)
                        {
                            case 0:
                                rebarSplitted = this.SplitEveryRebarInSameLocation(
                                    rebarIndex, ref remainingOffset, out splitRebarSet);
                                break;
                            case 1:
                                rebarSplitted = this.SplitEverySecondRebarInSameLocation(
                                    rebarIndex, ref remainingOffset, out splitRebarSet);
                                break;
                            case 2:
                                rebarSplitted = this.SplitEveryThirdRebarInSameLocation(
                                    rebarIndex, ref remainingOffset, out splitRebarSet);
                                break;
                            case 3:
                                rebarSplitted = this.SplitEveryFourthRebarInSameLocation(
                                    rebarIndex, ref remainingOffset, out splitRebarSet);
                                break;
                            default:
                                rebarSplitted = this.SplitEveryRebarInSameLocation(
                                    rebarIndex, ref remainingOffset, out splitRebarSet);
                                break;
                        }

                        if (rebarSplitted)
                        {
                            foreach (SingleRebar splitRebar in splitRebarSet)
                            {
                                newSplitRebars.Add(splitRebar);
                            }
                        }
                    }
                    else
                    {
                        newSplitRebars.Add(this.originalRebar);
                    }
                }

                result = true;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>The calculate total offset.</summary>
        /// <param name="totalRebarLength">The total reinforcement bar length.</param>
        /// <param name="section">The section.</param>
        /// <param name="barPosition">The bar position.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="totalOffset">The total offset.</param>
        /// <param name="remainingLength">The remaining length.</param>
        /// <param name="hookLength">The hook length.</param>
        private void CalculateTotalOffset(
            double totalRebarLength, 
            SpliceSection section, 
            BarPositions barPosition, 
            double remainingOffset, 
            out double totalOffset, 
            out double remainingLength, 
            out double hookLength)
        {
            double maxLength;
            hookLength = 0.0;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);
            remainingLength = totalRebarLength - (Math.Floor(totalRebarLength / maxLength) * maxLength);

            totalOffset = barPosition ==
                BarPositions.First ?
                this.CalculateTotalOffsetForFirstRebar(totalRebarLength, maxLength, ref remainingLength, out hookLength) :
                this.CalculateTotalOffsetForFollowingRebar(totalRebarLength, maxLength, remainingOffset, section, barPosition, ref remainingLength);
        }

        /// <summary>The calculate total offset for first rebar.</summary>
        /// <param name="totalRebarLength">The total reinforcement bar length.</param>
        /// <param name="maxLength">The max length.</param>
        /// <param name="remainingLength">The remaining length.</param>
        /// <param name="hookLength">The hook length.</param>
        /// <returns>The System.Double.</returns>
        private double CalculateTotalOffsetForFirstRebar(
            double totalRebarLength, double maxLength, ref double remainingLength, out double hookLength)
        {
            var totalOffset = 0.0;
            double startHookLength = 0.0, endHookLength = 0.0;
            hookLength = 0.0;
            var centralBarsCount = (int)Math.Floor(totalRebarLength / maxLength);

            switch (this.splitData.SpliceSymmetry)
            {
                case 0:
                    totalOffset = this.splitData.SpliceOffset;
                    break;
                case 1:
                    if (centralBarsCount % 2 == 0)
                    {
                        totalOffset = remainingLength / 2.0;
                    }
                    else
                    {
                        totalOffset = (maxLength / 2.0) + (remainingLength / 2.0);
                    }

                    break;
                case 2:
                    if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES ||
                        this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_135_DEGREES ||
                        this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_180_DEGREES)
                    {
                        startHookLength += 120;
                        startHookLength += Math.PI * this.originalRebar.StartHook.Radius / 2.0;
                        startHookLength -= this.originalRebar.StartHook.Radius;
                    }
                    else if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        startHookLength += this.originalRebar.StartHook.Length;
                        startHookLength += Math.PI * this.originalRebar.StartHook.Radius * this.originalRebar.StartHook.Angle
                                           / 180.0;
                        startHookLength -= this.originalRebar.StartHook.Radius;
                    }

                    if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES ||
                        this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_135_DEGREES ||
                        this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_180_DEGREES)
                    {
                        endHookLength += 120;
                        endHookLength += Math.PI * this.originalRebar.EndHook.Radius / 2.0;
                        endHookLength -= this.originalRebar.EndHook.Radius;
                    }
                    else if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        endHookLength += this.originalRebar.EndHook.Length;
                        endHookLength += Math.PI * this.originalRebar.EndHook.Radius * this.originalRebar.EndHook.Angle
                                         / 180.0;
                        endHookLength -= this.originalRebar.EndHook.Radius;
                    }

                    hookLength = (startHookLength > endHookLength) ? startHookLength : endHookLength;
                    remainingLength += 2 * hookLength;

                    if (centralBarsCount == 1)
                    {
                        totalOffset = remainingLength / 2.0;
                    }
                    else
                    {
                        totalOffset = 0.0;
                    }

                    break;
                default:
                    break;
            }

            totalOffset = totalOffset - (Math.Floor(totalOffset / maxLength) * maxLength);

            return totalOffset;
        }

        /// <summary>The calculate total offset for following rebar.</summary>
        /// <param name="totalRebarLength">The total reinforcement bar length.</param>
        /// <param name="maxLength">The max length.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="section">The section.</param>
        /// <param name="barPosition">The bar position.</param>
        /// <param name="remainingLength">The remaining length.</param>
        /// <returns>The System.Double.</returns>
        private double CalculateTotalOffsetForFollowingRebar(
            double totalRebarLength, 
            double maxLength, 
            double remainingOffset, 
            SpliceSection section, 
            BarPositions barPosition, 
            ref double remainingLength)
        {
            var totalOffset = 0.0;
            double shortRebarOffset;

            switch (section)
            {
                case SpliceSection.EverySingle:
                    Console.WriteLine("Error");
                    break;
                case SpliceSection.EverySecond:
                    shortRebarOffset = this.GetShortRebarLength(maxLength, remainingOffset, 2.0);
                    switch (barPosition)
                    {
                        case BarPositions.Second:
                            var centralBarsCount = (int)Math.Floor(totalRebarLength / maxLength);
                            switch (this.splitData.SpliceSymmetry)
                            {
                                case 0:
                                    totalOffset = shortRebarOffset;
                                    break;
                                case 1:
                                    if (centralBarsCount % 2 == 1)
                                    {
                                        totalOffset = remainingLength / 2.0;
                                    }
                                    else
                                    {
                                        totalOffset = (maxLength / 2.0) + (remainingLength / 2.0);
                                    }

                                    break;
                                case 2:
                                    if (centralBarsCount % 2 == 0)
                                    {
                                        totalOffset = remainingLength / 2.0;
                                    }
                                    else
                                    {
                                        totalOffset = (maxLength / 2.0) + (remainingLength / 2.0);
                                    }

                                    break;
                                default:
                                    break;
                            }

                            break;
                        default:
                            Console.WriteLine("Error");
                            break;
                    }

                    break;
                case SpliceSection.EveryThird:
                    shortRebarOffset = this.GetShortRebarLength(maxLength, remainingOffset, 3.0);
                    switch (barPosition)
                    {
                        case BarPositions.Second:
                            totalOffset = (shortRebarOffset + maxLength) / 2.0;
                            break;
                        case BarPositions.Third:
                            totalOffset = shortRebarOffset;
                            break;
                        default:
                            Console.WriteLine("Error");
                            break;
                    }

                    break;
                case SpliceSection.EveryForth:
                    shortRebarOffset = this.GetShortRebarLength(maxLength, remainingOffset, 4.0);
                    switch (barPosition)
                    {
                        case BarPositions.Second:
                            totalOffset = (2 * (maxLength - shortRebarOffset) / 3.0) + shortRebarOffset;
                            break;
                        case BarPositions.Third:
                            totalOffset = ((maxLength - shortRebarOffset) / 3.0) + shortRebarOffset;
                            break;
                        case BarPositions.Forth:
                            totalOffset = shortRebarOffset;
                            break;
                        default:
                            Console.WriteLine("Error");
                            break;
                    }

                    break;
                default:
                    break;
            }

            return totalOffset;
        }

        /// <summary>The create different middle rebar in center symmetry.</summary>
        /// <param name="remainingLength">The remaining length.</param>
        /// <param name="currentOriginalRebar">The original rebar.</param>
        /// <param name="splitPattern">The split pattern.</param>
        /// <param name="incrementalLength">The incremental length.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateDifferentMiddleRebarInCenterSymmetry(
            double remainingLength, 
            SingleRebar currentOriginalRebar, 
            ref Vector splitPattern, 
            ref double incrementalLength, 
            ref ArrayList splitRebarSet)
        {
            double rebarLength, finalRebarLength;
            var rebarPoints = new ArrayList();

            if (this.splitData.SpliceType == 0)
            {
                rebarLength = remainingLength - this.splitData.LappingLength;
            }
            else
            {
                rebarLength = remainingLength;
            }

            splitPattern.Normalize(incrementalLength);
            var firstPoint = new Vector(splitPattern);
            splitPattern.Normalize(rebarLength);
            var secondPoint = new Vector(splitPattern);

            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + firstPoint);
            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + firstPoint + secondPoint);
            bool result = this.CreateSplittedRebars(FirstOrLastEnum.MiddleRebar, rebarPoints, splitRebarSet, out finalRebarLength);
            incrementalLength += finalRebarLength;

            return result;
        }

        /// <summary>The create end rebar.</summary>
        /// <param name="currentOriginalRebar">The original rebar.</param>
        /// <param name="maxLength">The max length.</param>
        /// <param name="splitPattern">The split pattern.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <param name="incrementalLength">The incremental length.</param>
        /// <param name="lastSegment">The last segment.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateEndRebar(
            SingleRebar currentOriginalRebar, 
            double maxLength, 
            Vector splitPattern, 
            ArrayList splitRebarSet, 
            ref double incrementalLength, 
            out double lastSegment)
        {
            var rebarPoints = new ArrayList();
            double finalRebarLength;

            splitPattern.Normalize(incrementalLength);
            var firstPoint = new Vector(splitPattern);
            lastSegment = Distance.PointToPoint(
                (Point)currentOriginalRebar.Polygon.Points[0] + firstPoint, (Point)currentOriginalRebar.Polygon.Points[1]);

            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + firstPoint);
            rebarPoints.Add(currentOriginalRebar.Polygon.Points[1]);
            bool result = this.CreateSplittedRebars(FirstOrLastEnum.LastRebar, rebarPoints, splitRebarSet, out finalRebarLength);
            incrementalLength += maxLength + (this.splitData.LappingLength / 2.0);

            return result;
        }

        /// <summary>The create middle rebar.</summary>
        /// <param name="currentOriginalRebar">The original rebar.</param>
        /// <param name="splitPattern">The split pattern.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <param name="incrementalLength">The incremental length.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateMiddleRebar(SingleRebar currentOriginalRebar, Vector splitPattern, ArrayList splitRebarSet, ref double incrementalLength)
        {
            var rebarPoints = new ArrayList();
            double rebarLength, finalRebarLength;

            splitPattern.Normalize(incrementalLength);
            var firstPoint = new Vector(splitPattern);
            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out rebarLength);
            splitPattern.Normalize(rebarLength);
            var secondPoint = new Vector(splitPattern);

            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + firstPoint);
            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + firstPoint + secondPoint);
            bool result = this.CreateSplittedRebars(FirstOrLastEnum.MiddleRebar, rebarPoints, splitRebarSet, out finalRebarLength);
            incrementalLength += finalRebarLength;

            return result;
        }

        /// <summary>The create splitted reinforcement bars.</summary>
        /// <param name="position">The position.</param>
        /// <param name="rebarPoints">The reinforcement bar points.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <param name="finalRebarLength">The final reinforcement bar length.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateSplittedRebars(FirstOrLastEnum position, ArrayList rebarPoints, ArrayList splitRebarSet, out double finalRebarLength)
        {
            var result = false;
            finalRebarLength = 0.0;

            var singleRebarCreator = new CreateSingleRebar(this.originalRebar);
            var newRebar = singleRebarCreator.GetSingleRebar(rebarPoints);

            if (newRebar != null)
            {
                // Rounding the lenth of the rebar
                var reportLength = 0.0;
                newRebar.Insert();
                newRebar.GetReportProperty("LENGTH", ref reportLength);
                newRebar.Delete();

                var rebarLength = Distance.PointToPoint(newRebar.Polygon.Points[1] as Point, newRebar.Polygon.Points[0] as Point);

                if (rebarLength > reportLength)
                {
                    var rebarVector = new Vector((Point)newRebar.Polygon.Points[1] - (Point)newRebar.Polygon.Points[0]);
                    rebarVector.Normalize(rebarLength + AdditionalRebarLength);
                    rebarPoints[1] = (Point)rebarPoints[0] + rebarVector;
                    newRebar = singleRebarCreator.GetSingleRebar(rebarPoints);
                }

                if (newRebar != null)
                {
                    finalRebarLength = Distance.PointToPoint(
                        newRebar.Polygon.Points[1] as Point, newRebar.Polygon.Points[0] as Point);

                    if (position == FirstOrLastEnum.FirstRebar)
                    {
                        newRebar.EndHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
                    }
                    else if (position == FirstOrLastEnum.MiddleRebar)
                    {
                        newRebar.StartHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
                        newRebar.EndHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
                    }
                    else if (position == FirstOrLastEnum.LastRebar)
                    {
                        newRebar.StartHook.Shape = RebarHookData.RebarHookShapeEnum.NO_HOOK;
                    }

                    splitRebarSet.Add(newRebar);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The create start rebar.</summary>
        /// <param name="currentOriginalRebar">The original rebar.</param>
        /// <param name="splitPattern">The split pattern.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <param name="totalOffset">The total offset.</param>
        /// <param name="hookLength">The hook length.</param>
        /// <param name="incrementalLength">The incremental length.</param>
        /// <returns>The System.Boolean.</returns>
        private bool CreateStartRebar(
            SingleRebar currentOriginalRebar, 
            Vector splitPattern, 
            ArrayList splitRebarSet, 
            double totalOffset, 
            double hookLength, 
            ref double incrementalLength)
        {
            var rebarPoints = new ArrayList();
            double finalRebarLength;

            if (totalOffset < DistanceEpsilon)
            {
                double rebarLength;

                if (this.splitData.SpliceSymmetry == 2)
                {
                    rebarLength = this.splitData.MaxLength;

                    if (this.splitData.SpliceType == 0)
                    {
                        rebarLength -= this.splitData.LappingLength / 2.0;
                    }

                    rebarLength -= hookLength;
                }
                else
                {
                    this.GetRebarMaximumLength(FirstOrLastEnum.FirstRebar, out rebarLength);
                }

                splitPattern.Normalize(rebarLength);
            }
            else
            {
                splitPattern.Normalize(totalOffset);
            }

            var secondPoint = new Vector(splitPattern);
            rebarPoints.Add(currentOriginalRebar.Polygon.Points[0]);
            rebarPoints.Add((Point)currentOriginalRebar.Polygon.Points[0] + secondPoint);
            bool result = this.CreateSplittedRebars(FirstOrLastEnum.FirstRebar, rebarPoints, splitRebarSet, out finalRebarLength);
            incrementalLength += finalRebarLength;

            return result;
        }

        /// <summary>
        /// The get reinforcement bar length epsilon.
        /// </summary>
        /// <returns>
        /// The System.Double.
        /// </returns>
        private double GetRebarLengthEpsilon()
        {
            var rebarLengthEpsilon = RebarLengthEpsilon;

            if (this.splitData.SpliceType == 0)
            {
                rebarLengthEpsilon += this.splitData.LappingLength / 2.0;
            }

            return rebarLengthEpsilon;
        }

        /// <summary>The get reinforcement bar maximum length.</summary>
        /// <param name="position">The position.</param>
        /// <param name="maxLength">The max length.</param>
        /// <returns>The System.Boolean.</returns>
        private bool GetRebarMaximumLength(FirstOrLastEnum position, out double maxLength)
        {
            maxLength = this.splitData.MaxLength;

            switch (position)
            {
                case FirstOrLastEnum.Undefined:
                    break;

                case FirstOrLastEnum.FirstRebar:
                    if (this.splitData.SpliceType == 0)
                    {
                        maxLength -= this.splitData.LappingLength / 2.0;
                    }

                    if (this.originalRebar.StartHook.Shape != RebarHookData.RebarHookShapeEnum.NO_HOOK
                        && this.originalRebar.StartHook.Shape != RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        var startAngle = 0.0;

                        if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES)
                        {
                            startAngle = StandardHookAngle90;
                        }
                        else if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_135_DEGREES)
                        {
                            startAngle = StandardHookAngle135;
                        }
                        else if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_180_DEGREES)
                        {
                            startAngle = StandardHookAngle180;
                        }

                        maxLength -= StandardHookLength;
                        maxLength -= Math.PI * StandardHookRadius * startAngle / 180.0;
                        maxLength += StandardHookRadius;
                    }
                    else if (this.originalRebar.StartHook.Shape == RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        maxLength -= this.originalRebar.StartHook.Length;
                        maxLength -= Math.PI * this.originalRebar.StartHook.Radius * this.originalRebar.StartHook.Angle
                                     / 180.0;
                        maxLength += this.originalRebar.StartHook.Radius;
                    }

                    break;

                case FirstOrLastEnum.MiddleRebar:
                    if (this.splitData.SpliceType == 0)
                    {
                        maxLength -= this.splitData.LappingLength;
                    }

                    break;

                case FirstOrLastEnum.LastRebar:
                    if (this.splitData.SpliceType == 0)
                    {
                        maxLength -= this.splitData.LappingLength / 2.0;
                    }

                    if (this.originalRebar.EndHook.Shape != RebarHookData.RebarHookShapeEnum.NO_HOOK
                        && this.originalRebar.EndHook.Shape != RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        var endAngle = 0.0;

                        if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES)
                        {
                            endAngle = StandardHookAngle90;
                        }
                        else if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_135_DEGREES)
                        {
                            endAngle = StandardHookAngle135;
                        }
                        else if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.HOOK_180_DEGREES)
                        {
                            endAngle = StandardHookAngle180;
                        }

                        maxLength -= StandardHookLength;
                        maxLength -= Math.PI * StandardHookRadius * endAngle / 180.0;
                        maxLength += StandardHookRadius;
                    }
                    else if (this.originalRebar.EndHook.Shape == RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        maxLength -= this.originalRebar.EndHook.Length;
                        maxLength -= Math.PI * this.originalRebar.EndHook.Radius * this.originalRebar.EndHook.Angle / 180.0;
                        maxLength += this.originalRebar.EndHook.Radius;
                    }

                    break;

                default:
                    break;
            }

            return true;
        }

        /// <summary>The get short reinforcement bar length.</summary>
        /// <param name="maxLength">The max length.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="barCount">The bar count.</param>
        /// <returns>The System.Double.</returns>
        private double GetShortRebarLength(double maxLength, double remainingOffset, double barCount)
        {
            double totalOffset;

            if (remainingOffset > (this.splitData.MinSplitDistance + (this.splitData.LappingLength / 2.0))
                && remainingOffset < maxLength - (this.splitData.MinSplitDistance + (this.splitData.LappingLength / 2.0)))
            {
                totalOffset = remainingOffset;
            }
            else
            {
                totalOffset = remainingOffset + (maxLength / barCount);

                if (totalOffset > maxLength)
                {
                    totalOffset -= maxLength;
                }
            }

            return totalOffset;
        }

        /// <summary>The split every fourth rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryFourthRebarInSameLocation(int rebarIndex, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                switch (rebarIndex % 4)
                {
                    case 0:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.First, ref remainingOffset, out splitRebarSet);
                        break;
                    case 1:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Second, ref remainingOffset, out splitRebarSet);
                        break;
                    case 2:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Third, ref remainingOffset, out splitRebarSet);
                        break;
                    case 3:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Forth, ref remainingOffset, out splitRebarSet);
                        break;
                    default:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.First, ref remainingOffset, out splitRebarSet);
                        break;
                }
            }

            return result;
        }

        /// <summary>The split every rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryRebarInSameLocation(int rebarIndex, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();

            if (singleRebar != null)
            {
                result = this.SplitRebar(singleRebar, SpliceSection.EverySingle, BarPositions.First, ref remainingOffset, out splitRebarSet);
            }

            return result;
        }

        /// <summary>The split every second rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEverySecondRebarInSameLocation(int rebarIndex, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                result =
                    rebarIndex % 2 == 0 ?
                    this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.First, ref remainingOffset, out splitRebarSet) :
                    this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.Second, ref remainingOffset, out splitRebarSet);
            }

            return result;
        }

        /// <summary>The split every third rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryThirdRebarInSameLocation(int rebarIndex, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                switch (rebarIndex % 3)
                {
                    case 0:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.First, ref remainingOffset, out splitRebarSet);
                        break;
                    case 1:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.Second, ref remainingOffset, out splitRebarSet);
                        break;
                    case 2:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.Third, ref remainingOffset, out splitRebarSet);
                        break;
                    default:
                        result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.First, ref remainingOffset, out splitRebarSet);
                        break;
                }
            }

            return result;
        }

        /// <summary>The split rebar.</summary>
        /// <param name="originalRebar">The original rebar.</param>
        /// <param name="section">The section.</param>
        /// <param name="barPosition">The bar position.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitRebar(
            SingleRebar originalRebar, 
            SpliceSection section, 
            BarPositions barPosition, 
            ref double remainingOffset, 
            out ArrayList splitRebarSet)
        {
            var result = true;
            double maxLength, totalOffset, remainingLength, hookLength, incrementalLength = 0.0;
            splitRebarSet = new ArrayList();
            var totalRebarLength = Distance.PointToPoint((Point)originalRebar.Polygon.Points[1], (Point)originalRebar.Polygon.Points[0]);
            var splitPattern = new Vector((Point)originalRebar.Polygon.Points[1] - (Point)originalRebar.Polygon.Points[0]);

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);
            this.CalculateTotalOffset(
                totalRebarLength, 
                section, 
                barPosition, 
                remainingOffset, 
                out totalOffset, 
                out remainingLength, 
                out hookLength);

            for (var ii = 0; incrementalLength + this.GetRebarLengthEpsilon() < totalRebarLength; ii++)
            {
                if (ii == 0)
                {
                    result &= this.CreateStartRebar(originalRebar, splitPattern, splitRebarSet, totalOffset, hookLength, ref incrementalLength);
                }
                else if (incrementalLength + maxLength + this.GetRebarLengthEpsilon() < totalRebarLength)
                {
                    var centralBarsCount = (int)Math.Floor(totalRebarLength / maxLength);

                    if (this.splitData.SpliceSymmetry == 2 && barPosition == BarPositions.First && centralBarsCount > 1
                        && incrementalLength < totalRebarLength / 2.0
                        && incrementalLength + maxLength > totalRebarLength / 2.0)
                    {
                        result &= this.CreateDifferentMiddleRebarInCenterSymmetry(remainingLength, originalRebar, ref splitPattern, ref incrementalLength, ref splitRebarSet);
                    }
                    else
                    {
                        result &= this.CreateMiddleRebar(originalRebar, splitPattern, splitRebarSet, ref incrementalLength);
                    }
                }
                else
                {
                    double lastSegment;
                    result &= this.CreateEndRebar(originalRebar, maxLength, splitPattern, splitRebarSet, ref incrementalLength, out lastSegment);

                    if (barPosition == BarPositions.First)
                    {
                        remainingOffset = lastSegment;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}