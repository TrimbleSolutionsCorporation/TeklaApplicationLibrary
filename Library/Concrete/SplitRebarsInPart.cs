namespace Tekla.Structures.Concrete
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
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

        /// <summary>
        /// Length of the start hook.
        /// </summary>
        private double startHookLength;

        /// <summary>
        /// Length of the start hook.
        /// </summary>
        private double endHookLength;

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
                    double length = Distance.PointToPoint((Point)this.originalRebar.Polygon.Points[1], (Point)this.originalRebar.Polygon.Points[0]);
                    double diameter = this.GetRebarDiameter(this.originalRebar.Size, this.originalRebar.Grade);
                    this.startHookLength = this.GetHookRealLength(this.originalRebar.StartHook, diameter);
                    this.endHookLength = this.GetHookRealLength(this.originalRebar.EndHook, diameter);

                    if (this.originalRebar != null && length > this.splitData.MaxLength + this.GetRebarLengthEpsilon())
                    {
                        bool rebarSplitted;
                        ArrayList splitRebarSet;

                        switch (this.splitData.SpliceSection)
                        {
                            case 0:
                                rebarSplitted = this.SplitEveryRebarInSameLocation(rebarIndex, ref remainingOffset, new List<double>(), out splitRebarSet);
                                break;
                            case 1:
                                rebarSplitted = this.SplitEverySecondRebarInSameLocation(rebarIndex, ref remainingOffset, new List<double>(), out splitRebarSet);
                                break;
                            case 2:
                                rebarSplitted = this.SplitEveryThirdRebarInSameLocation(rebarIndex, new List<double>(), ref remainingOffset, out splitRebarSet);
                                break;
                            case 3:
                                rebarSplitted = this.SplitEveryFourthRebarInSameLocation(rebarIndex, new List<double>(), ref remainingOffset, out splitRebarSet);
                                break;
                            default:
                                rebarSplitted = this.SplitEveryRebarInSameLocation(rebarIndex, ref remainingOffset, new List<double>(), out splitRebarSet);
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
                    hookLength = (this.startHookLength > this.endHookLength) ? this.startHookLength : this.endHookLength;
                    remainingLength += 2.0 * hookLength;

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

                    maxLength -= this.startHookLength;

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

                    maxLength -= this.endHookLength;

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
        /// <param name="segmentLengths">Lengths of bars segments.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryFourthRebarInSameLocation(int rebarIndex, List<double> segmentLengths, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                if (segmentLengths.Count != 0)
                {
                    switch (rebarIndex % 4)
                    {
                        case 0:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        case 1:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Second, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        case 2:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Third, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        case 3:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.Forth, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        default:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryForth, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                    }
                }
                else
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
            }

            return result;
        }

        /// <summary>The split every rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="segmentLengths">Lengths of bars segments.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryRebarInSameLocation(int rebarIndex, ref double remainingOffset, List<double> segmentLengths, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();

            if (singleRebar != null)
            {
                if (segmentLengths.Count == 0)
                {
                    result = this.SplitRebar(singleRebar, SpliceSection.EverySingle, BarPositions.First, ref remainingOffset, out splitRebarSet);
                }
                else
                {
                    result = this.SplitRebar(singleRebar, SpliceSection.EverySingle, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet);
                }
            }

            return result;
        }

        /// <summary>The split every second rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="segmentLengths">Lengths of bars segments.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEverySecondRebarInSameLocation(int rebarIndex, ref double remainingOffset, List<double> segmentLengths, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                if (segmentLengths.Count != 0)
                {
                    result =
                        rebarIndex % 2 == 0 ?
                        this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet) :
                        this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.Second, segmentLengths, ref remainingOffset, out splitRebarSet);
                }
                else
                {
                    result =
                        rebarIndex % 2 == 0 ?
                        this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.First, ref remainingOffset, out splitRebarSet) :
                        this.SplitRebar(singleRebar, SpliceSection.EverySecond, BarPositions.Second, ref remainingOffset, out splitRebarSet);
                }
            }

            return result;
        }

        /// <summary>The split every third rebar in same location.</summary>
        /// <param name="rebarIndex">The reinforcement bar index.</param>
        /// <param name="segmentLengths">Lengths of bars segments.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitEveryThirdRebarInSameLocation(int rebarIndex, List<double> segmentLengths, ref double remainingOffset, out ArrayList splitRebarSet)
        {
            var result = false;
            var singleRebar = this.singleRebars[rebarIndex] as SingleRebar;
            splitRebarSet = new ArrayList();
            double maxLength;

            this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);

            if (singleRebar != null)
            {
                if (segmentLengths.Count != 0)
                {
                    switch (rebarIndex % 3)
                    {
                        case 0:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        case 1:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.Second, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        case 2:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.Third, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                        default:
                            result = this.SplitRebar(singleRebar, SpliceSection.EveryThird, BarPositions.First, segmentLengths, ref remainingOffset, out splitRebarSet);
                            break;
                    }
                }
                else
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

        /// <summary>The split rebar.</summary>
        /// <param name="originalRebar">The original rebar.</param>
        /// <param name="section">The section.</param>
        /// <param name="barPosition">The bar position.</param>
        /// <param name="rebarSegmentLengths">Lengths of all rebars segments.</param>
        /// <param name="remainingOffset">The remaining offset.</param>
        /// <param name="splitRebarSet">The split reinforcement bar set.</param>
        /// <returns>The System.Boolean.</returns>
        private bool SplitRebar(
            SingleRebar originalRebar,
            SpliceSection section,
            BarPositions barPosition,
            List<double> rebarSegmentLengths,
            ref double remainingOffset,
            out ArrayList splitRebarSet)
        {
            bool result = true;
            double maxLength;
            double totalOffset;
            double remainingLength;
            double hookLength;
            double incrementalLength = 0.0;
            double totalRebarLength = 0.0;
            double previousLength = 0.0;
            splitRebarSet = new ArrayList();

            ArrayList previousPoints = new ArrayList();

            foreach (double length in rebarSegmentLengths)
            {
                totalRebarLength += length;
            }

            CreateSingleRebar createSingleRebar = new CreateSingleRebar(originalRebar);

            for (int jj = 0; jj < originalRebar.Polygon.Points.Count - 1; jj++)
            {
                Vector splitPattern = new Vector((Point)originalRebar.Polygon.Points[jj + 1] - (Point)originalRebar.Polygon.Points[jj]);

                this.GetRebarMaximumLength(FirstOrLastEnum.MiddleRebar, out maxLength);
                this.CalculateTotalOffset(
                    totalRebarLength,
                    section,
                    barPosition,
                    remainingOffset,
                    out totalOffset,
                    out remainingLength,
                    out hookLength);

                ArrayList splitRebarSegment = new ArrayList();
                ArrayList rebarPoints = new ArrayList();
                rebarPoints.Add(originalRebar.Polygon.Points[jj]);
                rebarPoints.Add(originalRebar.Polygon.Points[jj + 1]);

                SingleRebar segmentToSplit = createSingleRebar.GetSingleRebar(rebarPoints);

                if ((previousLength + rebarSegmentLengths[jj]) > maxLength)
                {
                    for (int ii = 0; incrementalLength + this.GetRebarLengthEpsilon() < totalRebarLength; ii++)
                    {
                        if (ii == 0 && jj == 0)
                        {
                            result &= this.CreateStartRebar(segmentToSplit, splitPattern, splitRebarSegment, totalOffset, hookLength, ref incrementalLength);
                        }
                        else if (incrementalLength + maxLength + this.GetRebarLengthEpsilon() < totalRebarLength)
                        {
                            int centralBarsCount = (int)Math.Floor(totalRebarLength / maxLength);

                            if (this.splitData.SpliceSymmetry == 2 &&
                                barPosition == BarPositions.First &&
                                centralBarsCount > 1 &&
                                incrementalLength < totalRebarLength / 2.0 &&
                                incrementalLength + maxLength > totalRebarLength / 2.0)
                            {
                                result &= this.CreateDifferentMiddleRebarInCenterSymmetry(remainingLength, segmentToSplit, ref splitPattern, ref incrementalLength, ref splitRebarSegment);
                            }
                            else
                            {
                                result &= this.CreateMiddleRebar(segmentToSplit, splitPattern, splitRebarSegment, ref incrementalLength);
                            }
                        }
                        else
                        {
                            double lastSegment;
                            double segmentLength;

                            if (incrementalLength < maxLength + DistanceEpsilon)
                            {
                                segmentLength = rebarSegmentLengths[jj] - (maxLength - incrementalLength);
                            }
                            else
                            {
                                int multiplayer = (int)(incrementalLength / maxLength);
                                segmentLength = rebarSegmentLengths[jj] - ((multiplayer + 1) * maxLength - incrementalLength);
                            }

                            result &= this.CreateEndRebar(segmentToSplit, maxLength, splitPattern, splitRebarSegment, ref segmentLength, out lastSegment);
                            incrementalLength = segmentLength;

                            if (barPosition == BarPositions.First)
                            {
                                remainingOffset = lastSegment;
                            }
                        }
                    }
                }
                else
                {
                    incrementalLength += rebarSegmentLengths[jj];
                    previousLength += rebarSegmentLengths[jj];
                    this.AddRebarPoint(originalRebar.Polygon.Points[jj] as Point, previousPoints);
                    this.AddRebarPoint(originalRebar.Polygon.Points[jj + 1] as Point, previousPoints);

                    DrawPoint(originalRebar.Polygon.Points[jj] as Point, jj.ToString());
                    DrawPoint(originalRebar.Polygon.Points[jj + 1] as Point, (jj + 1).ToString());
                }

                if (splitRebarSegment.Count != 0 && previousPoints.Count > 0)
                {
                    DrawPoint(((SingleRebar)splitRebarSegment[0]).Polygon.Points[0] as Point, "SP" + jj.ToString());
                    DrawPoint(((SingleRebar)splitRebarSegment[0]).Polygon.Points[1] as Point, "SP" + (jj + 1).ToString());

                    previousPoints.Add(((SingleRebar)splitRebarSegment[0]).Polygon.Points[0]);
                    SingleRebar previousBar = createSingleRebar.GetSingleRebar(previousPoints);
                    splitRebarSegment.Insert(0, previousBar);

                    previousPoints.Clear();
                    previousLength = 0.0;
                }

                splitRebarSet.AddRange(splitRebarSegment);
            }

            return result;
        }

        private void DrawPoint(Point inputPoint, string comment)
        {


            Tekla.Structures.Model.UI.GraphicsDrawer drawer = new Tekla.Structures.Model.UI.GraphicsDrawer();
            drawer.DrawText(inputPoint, comment, new Tekla.Structures.Model.UI.Color(1, 0, 0));
            
        }

        /// <summary>
        /// Add rebar point into the list, if point is not already within.
        /// </summary>
        /// <param name="rebarPoint">Rebar point.</param>
        /// <param name="list">List of rebar points.</param>
        private void AddRebarPoint(Point rebarPoint, ArrayList list)
        {
            if (rebarPoint != null && !list.Contains(rebarPoint))
            {
                list.Add(rebarPoint);
            }
        }

        /// <summary>
        /// Get start hook real length
        /// </summary>
        /// <param name="rebarHookData">Hook data</param>
        /// <param name="actualRebarSize">Actual rebar size</param>
        /// <returns>Real length of start hook</returns>
        private double GetHookRealLength(RebarHookData rebarHookData, double actualRebarSize)
        {
            double radius = rebarHookData.Radius;
            double length = rebarHookData.Length;
            double angle = rebarHookData.Angle;

            double hookAngleRadians = (Math.Abs(angle) / 180.0) * Math.PI;
            double r = radius + actualRebarSize / 2.0;
            double legnthOfBend = hookAngleRadians * r;
            double startHookRealLength = length + legnthOfBend - (r + actualRebarSize / 2.0);

            if (length < DistanceEpsilon)
            {
                startHookRealLength = 0.0;
            }

            return startHookRealLength;
        }

        /// <summary>
        /// Get rebar diameter.
        /// </summary>
        /// <param name="size">Rebar size</param>
        /// <param name="grade">Rebar grade</param>
        /// <returns>Rebar diameter</returns>
        private double GetRebarDiameter(string size, string grade)
        {
            Catalogs.RebarItem rebar = new Catalogs.RebarItem();
            rebar.Select(grade, size);

            return rebar.ActualDiameter;
        }

        #endregion
    }
}