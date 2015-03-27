namespace Tekla.Structures.Concrete
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows.Forms;

    using Tekla.Structures.Dialog;
    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    using TSMUI = Tekla.Structures.Model.UI;

    /// <summary>
    /// The classificator calculator.
    /// </summary>
    public class ClassificatorCalculator
    {
        #region Static Fields

        /// <summary>
        /// The uda value.
        /// </summary>
        public const string UDA = "Hierarchy";

        /// <summary>
        /// The uda 2.
        /// </summary>
        public const string UDA2 = "Depth";

        /// <summary>
        /// The back value.
        /// </summary>
        public const string Back = "Back-";

        /// <summary>
        /// The bot value.
        /// </summary>
        public const string Bot = "Bot-";

        /// <summary>
        /// The front value.
        /// </summary>
        public const string Front = "Front-";

        /// <summary>
        /// The top value.
        /// </summary>
        public const string Top = "Top-";

        /// <summary>
        /// The m_ prefix back.
        /// </summary>
        public readonly string PrefixBack;

        /// <summary>
        /// The m_ prefix bot.
        /// </summary>
        public readonly string PrefixBot;

        /// <summary>
        /// The m_ prefix front.
        /// </summary>
        public readonly string PrefixFront;

        /// <summary>
        /// The m_ prefix top.
        /// </summary>
        public readonly string PrefixTop;

        #endregion

        #region Constants

        /// <summary>
        /// The deeper.
        /// </summary>
        private const int Deeper = 2;

        /// <summary>
        /// The epsilon.
        /// </summary>
        private const double Epsilon = 0.0001;

        /// <summary>
        /// The equal.
        /// </summary>
        private const int Equal = 1;

        /// <summary>
        /// The upper.
        /// </summary>
        private const int Upper = 0;

        #endregion

        #region Fields

        /// <summary>
        /// The m_ localization.
        /// </summary>
        private readonly Localization localization;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ClassificatorCalculator"/> class.</summary>
        /// <param name="localize">The localize.</param>
        public ClassificatorCalculator(Localization localize)
        {
            this.localization = localize;
            this.PrefixTop = Top;
            this.PrefixBot = Bot;
            this.PrefixBack = Back;
            this.PrefixFront = Front;
        }

        /// <summary>Initializes a new instance of the <see cref="ClassificatorCalculator"/> class.</summary>
        /// <param name="top">The top value.</param>
        /// <param name="bot">The bot value.</param>
        /// <param name="back">The back value.</param>
        /// <param name="front">The front value.</param>
        /// <param name="localize">The localize value.</param>
        public ClassificatorCalculator(string top, string bot, string back, string front, Localization localize)
        {
            this.localization = localize;
            this.PrefixTop = top;
            this.PrefixBot = bot;
            this.PrefixBack = back;
            this.PrefixFront = front;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The classify rebar.</summary>
        /// <param name="singleObject">The single object.</param>
        /// <param name="classifiedRebars">The classified reinforcement bars.</param>
        public void ClassifyRebar(ModelObject singleObject, ref ArrayList classifiedRebars)
        {
            var rebar = singleObject as Reinforcement;

            if (rebar != null && !this.ArrayContainsRebar(rebar, classifiedRebars))
            {
                var rebarsLevel = this.SetRebarsLevel(rebar);
                rebar.SetUserProperty(UDA, rebarsLevel);
                classifiedRebars.Add(rebar);
            }
        }

        /// <summary>The classify reinforcement bars.</summary>
        /// <param name="allRebars">The all reinforcement bars.</param>
        /// <param name="rebars">The reinforcement bars.</param>
        /// <param name="progress">The progress.</param>
        public void ClassifyRebars(bool allRebars, ref ArrayList rebars, ref ProgressBar progress)
        {
            var model = new Model();

            var modelObjects = allRebars ? model.GetModelObjectSelector().GetAllObjects() : new TSMUI.ModelObjectSelector().GetSelectedObjects();

            progress.Maximum = modelObjects.GetSize();
            var count = 0;
            while (modelObjects.MoveNext())
            {
                try
                {
                    rebars = this.HandleNextObject(allRebars, progress, modelObjects.Current, rebars);
                    ++progress.Value;
                }
                catch (ApplicationException)
                {
                    ++count;
                }
            }

            if (count > 0)
            {
                this.ShowMessage("albl_Corrupt_Object_Selection");
            }

            this.SortRebarsLevels(ref rebars, ref progress);
        }

        /// <summary>The set reinforcement bars level.</summary>
        /// <param name="rebar">The rebar.</param>
        /// <returns>The System.String.</returns>
        public string SetRebarsLevel(Reinforcement rebar)
        {
            var result = string.Empty;

            var rebarGeometries = rebar.GetRebarGeometries(false);
            var fatherPart = rebar.Father as Part;
            if (fatherPart != null)
            {
                var slabsSolid = fatherPart.GetSolid();
                var maxDepth = Math.Abs(slabsSolid.MaximumPoint.Z);
                var minDepth = Math.Abs(slabsSolid.MinimumPoint.Z);

                // Is it a slab or wall
                if (!this.PartIsSlab(rebar.Father))
                {
                    // Walls reinforcement bars geometry have to be rotated and shifted to run
                    // same methods as for slabs.
                    var csys1 = fatherPart.GetCoordinateSystem();
                    var rotMat = MatrixFactory.Rotate(Math.PI / 2, csys1.AxisX);
                    maxDepth = this.RotateSlabPoint(slabsSolid.MaximumPoint, rotMat, csys1.Origin);
                    minDepth = this.RotateSlabPoint(slabsSolid.MinimumPoint, rotMat, csys1.Origin);
                    this.RotateRebarsGeometries(ref rebarGeometries, rotMat, csys1.Origin);
                    if (maxDepth > 0 && minDepth < 0)
                    {
                        this.ShiftRebars(ref rebarGeometries, ref maxDepth, ref minDepth);
                    }
                }
                else
                {
                    this.ShiftRebars(ref rebarGeometries, ref maxDepth, ref minDepth);
                }

                var confine = maxDepth > Epsilon ? maxDepth : minDepth;
                var rebarsLevel = Math.Abs(GetRebarsPredominantLevel(rebarGeometries));

                if (this.PartIsSlab(rebar.Father))
                {
                    if (rebarsLevel < confine / 2)
                    {
                        result = this.PrefixTop + Math.Floor(rebarsLevel);
                    }
                    else
                    {
                        result = this.PrefixBot + Math.Floor(rebarsLevel);
                    }
                }
                else
                {
                    if (rebarsLevel < confine / 2)
                    {
                        result = this.PrefixFront + Math.Floor(rebarsLevel);
                    }
                    else
                    {
                        result = this.PrefixBack + Math.Floor(rebarsLevel);
                    }
                }
            }

            return result;
        }

        /// <summary>The sort reinforcement bars levels.</summary>
        /// <param name="classifiedRebars">The classified reinforcement bars.</param>
        /// <param name="progressBar">The progress bar.</param>
        public void SortRebarsLevels(ref ArrayList classifiedRebars, ref ProgressBar progressBar)
        {
            foreach (Reinforcement rebar in classifiedRebars)
            {
                var rebarsIndex = 1;
                string rebarsLevel = string.Empty, prevLevel = string.Empty;
                rebar.GetUserProperty(UDA, ref rebarsLevel);

                foreach (Reinforcement nextRebar in classifiedRebars)
                {
                    var nextRebarsLevel = string.Empty;
                    nextRebar.GetUserProperty(UDA, ref nextRebarsLevel);

                    if (rebar.Identifier.ID != nextRebar.Identifier.ID
                        && rebar.Father.Identifier.ID == nextRebar.Father.Identifier.ID)
                    {
                        if (this.RaiseRebarsIndex(rebarsLevel, nextRebarsLevel)
                            && !this.AreLevelsEqual(nextRebarsLevel, prevLevel))
                        {
                            rebarsIndex++;
                            prevLevel = nextRebarsLevel;
                        }
                    }
                }

                rebar.SetUserProperty(UDA, this.AssembleRebarsLevel(rebarsLevel, rebarsIndex));
                if (progressBar.Maximum > progressBar.Value)
                {
                    ++progressBar.Value;
                }
            }

            this.ReviseClassifiedRebars(classifiedRebars);
            if (progressBar.Maximum > progressBar.Value)
            {
                ++progressBar.Value;
            }
        }

        #endregion

        #region Methods

        /// <summary>The part is slab.</summary>
        /// <param name="singlePart">The single part.</param>
        /// <returns>The System.Boolean.</returns>
        internal bool PartIsSlab(ModelObject singlePart)
        {
            var result = false;
            var conPlate = singlePart as ContourPlate;

            if (conPlate != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>The add value to level list.</summary>
        /// <param name="levelPoints">The level points.</param>
        /// <param name="currentZ">The current z.</param>
        /// <param name="lengthBetweenPoints">The length between points.</param>
        private static void AddValueToLevelList(ref ArrayList levelPoints, double currentZ, double lengthBetweenPoints)
        {
            var result = false;
            double[] level = { currentZ, lengthBetweenPoints };

            for (var ii = 0; ii < levelPoints.Count; ii++)
            {
                var singleLevel = levelPoints[ii] as double[];

                if (singleLevel != null && Math.Abs(singleLevel[0] - currentZ) < Epsilon)
                {
                    singleLevel[1] += lengthBetweenPoints;
                    result = true;
                }
            }

            if (!result)
            {
                levelPoints.Add(level);
            }
        }

        /// <summary>The get length between points.</summary>
        /// <param name="pointA">The point a.</param>
        /// <param name="pointB">The point b.</param>
        /// <returns>The System.Double.</returns>
        private static double GetLengthBetweenPoints(Point pointA, Point pointB)
        {
            var hypotenusa = new Point(pointB.X - pointA.X, pointB.Y - pointA.Y, pointB.Z - pointA.Z);
            return Math.Sqrt(Math.Pow(hypotenusa.X, 2.0) + Math.Pow(hypotenusa.Y, 2.0) + Math.Pow(hypotenusa.Z, 2.0));
        }

        /// <summary>The get reinforcement bars predominant level.</summary>
        /// <param name="rebarGeometries">The reinforcement bar geometries.</param>
        /// <returns>The System.Double.</returns>
        private static double GetRebarsPredominantLevel(ArrayList rebarGeometries)
        {
            var levelPoints = new ArrayList();

            foreach (RebarGeometry rg in rebarGeometries)
            {
                var rebarsPoints = rg.Shape.Points;

                for (var ii = 0; ii < rebarsPoints.Count - 1; ii++)
                {
                    var coords = rebarsPoints[ii] as Point;
                    var nextCoords = rebarsPoints[ii + 1] as Point;

                    if (coords != null && nextCoords != null)
                    {
                        if (Math.Abs(coords.Z) - Math.Abs(nextCoords.Z) < Epsilon)
                        {
                            AddValueToLevelList(ref levelPoints, coords.Z, GetLengthBetweenPoints(coords, nextCoords));
                        }
                    }
                }
            }

            return RebarsPredominantLevel(levelPoints);
        }

        /// <summary>The reinforcement bars predominant level.</summary>
        /// <param name="levelPoints">The level points.</param>
        /// <returns>The System.Double.</returns>
        private static double RebarsPredominantLevel(ArrayList levelPoints)
        {
            double[] predominantLevel = { 0.0, 0.0 };

            foreach (double[] level in levelPoints)
            {
                if (Math.Abs(predominantLevel[1]) < Math.Abs(level[1]))
                {
                    predominantLevel[0] = level[0];
                    predominantLevel[1] = level[1];
                }
            }

            return predominantLevel[0];
        }

        /// <summary>The are levels equal.</summary>
        /// <param name="rebarsLevel">The reinforcement bars level.</param>
        /// <param name="prevLevel">The prev level.</param>
        /// <returns>The System.Boolean.</returns>
        private bool AreLevelsEqual(string rebarsLevel, string prevLevel)
        {
            var result = false;

            if (rebarsLevel.Contains(this.PrefixTop) && prevLevel.Contains(this.PrefixTop))
            {
                if (this.CompareLevels(rebarsLevel, prevLevel, this.PrefixTop) == Equal)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixBot) && prevLevel.Contains(this.PrefixBot))
            {
                if (this.CompareLevels(rebarsLevel, prevLevel, this.PrefixBot) == Equal)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixFront) && prevLevel.Contains(this.PrefixFront))
            {
                if (this.CompareLevels(rebarsLevel, prevLevel, this.PrefixFront) == Equal)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixBack) && prevLevel.Contains(this.PrefixBack))
            {
                if (this.CompareLevels(rebarsLevel, prevLevel, this.PrefixBack) == Equal)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The array contains reinforcement bar.</summary>
        /// <param name="rebar">The reinforcement bars.</param>
        /// <param name="classifiedRebars">The classified reinforcement bars.</param>
        /// <returns>The System.Boolean.</returns>
        private bool ArrayContainsRebar(Reinforcement rebar, ArrayList classifiedRebars)
        {
            var result = false;

            foreach (Reinforcement rebarInArray in classifiedRebars)
            {
                if (rebar.Identifier.ID == rebarInArray.Identifier.ID)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The assemble reinforcement bars level.</summary>
        /// <param name="rebarsLevel">The reinforcement bars level.</param>
        /// <param name="rebarsIndex">The reinforcement bars index.</param>
        /// <returns>The System.String.</returns>
        private string AssembleRebarsLevel(string rebarsLevel, int rebarsIndex)
        {
            string result;

            if (rebarsLevel.Contains(this.PrefixTop))
            {
                result = this.PrefixTop + rebarsIndex + "*" + rebarsLevel.Substring(this.PrefixTop.Length);
            }
            else if (rebarsLevel.Contains(this.PrefixBot))
            {
                result = this.PrefixBot + rebarsIndex + "*" + rebarsLevel.Substring(this.PrefixBot.Length);
            }
            else if (rebarsLevel.Contains(this.PrefixFront))
            {
                result = this.PrefixFront + rebarsIndex + "*" + rebarsLevel.Substring(this.PrefixFront.Length);
            }
            else
            {
                result = this.PrefixBack + rebarsIndex + "*" + rebarsLevel.Substring(this.PrefixBack.Length);
            }

            return result;
        }

        /// <summary>The compare levels.</summary>
        /// <param name="rebarsLevel">The reinforcement bars level.</param>
        /// <param name="nextRebarsLevel">The next reinforcement bars level.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The integer.</returns>
        private int CompareLevels(string rebarsLevel, string nextRebarsLevel, string prefix)
        {
            var result = Upper;

            var thisLevel = this.ExtractLevel(rebarsLevel, prefix);
            var nextLevel = this.ExtractLevel(nextRebarsLevel, prefix);

            if (thisLevel == nextLevel)
            {
                result = Equal;
            }

            if (thisLevel > nextLevel)
            {
                result = Deeper;
            }

            return result;
        }

        /// <summary>The extract level.</summary>
        /// <param name="rebarsLevel">The reinforcement bars level.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The System.Int32.</returns>
        private int ExtractLevel(string rebarsLevel, string prefix)
        {
            return int.Parse(rebarsLevel.Contains("*") ? rebarsLevel.Substring(rebarsLevel.LastIndexOf("*") + 1) : rebarsLevel.Substring(prefix.Length));
        }

        /// <summary>The handle next object.</summary>
        /// <param name="allRebars">The all reinforcement bars.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="nextObj">The next obj.</param>
        /// <param name="rebars">The reinforcement bars.</param>
        /// <returns>The System.Collections.ArrayList.</returns>
        private ArrayList HandleNextObject(bool allRebars, ProgressBar progress, ModelObject nextObj, ArrayList rebars)
        {
            var singlePart = nextObj as Part;
            if (singlePart != null && (this.PartIsSlab(nextObj) || this.PartIsWall(nextObj)))
            {
                var partsReinforcement = singlePart.GetReinforcements();
                progress.Maximum += (partsReinforcement.GetSize() * 2) + 1;
                while (partsReinforcement.MoveNext())
                {
                    this.ClassifyRebar(partsReinforcement.Current, ref rebars);
                    ++progress.Value;
                }
            }
            else
            {
                var singleRebar = nextObj as Reinforcement;
                if (!allRebars && singleRebar != null)
                {
                    this.ClassifyRebar(nextObj, ref rebars);
                }
            }

            return rebars;
        }

        /// <summary>The is it wall.</summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="length">The length.</param>
        /// <param name="count">The count.</param>
        /// <returns>The System.Boolean.</returns>
        private bool IsItWall(double width, double height, double length, int count)
        {
            if (height / width > 5 && length / width > 5)
            {
                return true;
            }

            if (count > 2)
            {
                return false;
            }

            return this.IsItWall(height, length, width, ++count);
        }

        /// <summary>The part is wall.</summary>
        /// <param name="modelObject">The model object.</param>
        /// <returns>The System.Boolean.</returns>
        private bool PartIsWall(ModelObject modelObject)
        {
            var result = false;
            var wall = modelObject as Part;
            if (wall != null)
            {
                double width = 0.0, height = 0.0, length = 0.0;
                wall.GetReportProperty("WIDTH", ref width);
                wall.GetReportProperty("HEIGHT", ref height);
                wall.GetReportProperty("LENGTH", ref length);
                result = this.IsItWall(width, height, length, 0);
            }

            return result;
        }

        /// <summary>The raise reinforcement bars index.</summary>
        /// <param name="rebarsLevel">The reinforcement bars level.</param>
        /// <param name="nextRebarsLevel">The next reinforcement bars level.</param>
        /// <returns>The System.Boolean.</returns>
        private bool RaiseRebarsIndex(string rebarsLevel, string nextRebarsLevel)
        {
            var result = false;
            if (rebarsLevel.Contains(this.PrefixTop) && nextRebarsLevel.Contains(this.PrefixTop))
            {
                if (this.CompareLevels(rebarsLevel, nextRebarsLevel, this.PrefixTop) == Deeper)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixBot) && nextRebarsLevel.Contains(this.PrefixBot))
            {
                if (this.CompareLevels(rebarsLevel, nextRebarsLevel, this.PrefixBot) == Upper)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixFront) && nextRebarsLevel.Contains(this.PrefixFront))
            {
                if (this.CompareLevels(rebarsLevel, nextRebarsLevel, this.PrefixFront) == Deeper)
                {
                    result = true;
                }
            }
            else if (rebarsLevel.Contains(this.PrefixBack) && nextRebarsLevel.Contains(this.PrefixBack))
            {
                if (this.CompareLevels(rebarsLevel, nextRebarsLevel, this.PrefixBack) == Upper)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>The revise classified reinforcement bars.</summary>
        /// <param name="classifiedRebars">The classified reinforcement bars.</param>
        private void ReviseClassifiedRebars(ArrayList classifiedRebars)
        {
            try
            {
                foreach (Reinforcement rebar in classifiedRebars)
                {
                    var rebarsLevel = string.Empty;
                    rebar.GetUserProperty(UDA, ref rebarsLevel);
                    rebar.SetUserProperty(UDA2, rebarsLevel);
                    rebar.SetUserProperty(UDA, rebarsLevel.Remove(rebarsLevel.LastIndexOf("*")));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Trace.WriteLine("Exception in ReviseClassifiedRebars:\n " + ex.Message);
            }
        }

        /// <summary>The rotate reinforcement bars geometries.</summary>
        /// <param name="rebarGeometries">The reinforcement bar geometries.</param>
        /// <param name="rotateMatrix">The rotate matrix.</param>
        /// <param name="origin">The origin.</param>
        private void RotateRebarsGeometries(ref ArrayList rebarGeometries, Matrix rotateMatrix, Point origin)
        {
            foreach (RebarGeometry rg in rebarGeometries)
            {
                var rebarsPoints = rg.Shape.Points;

                for (var ii = 0; ii < rebarsPoints.Count; ii++)
                {
                    var coords = rebarsPoints[ii] as Point;
                    coords = coords - origin;
                    rebarsPoints[ii] = rotateMatrix.Transform(coords);
                }
            }
        }

        /// <summary>The rotate slab point.</summary>
        /// <param name="slabsPoint">The slabs point.</param>
        /// <param name="rotMat">The rot mat.</param>
        /// <param name="origin">The origin.</param>
        /// <returns>The System.Double.</returns>
        private double RotateSlabPoint(Point slabsPoint, Matrix rotMat, Point origin)
        {
            var shiftedPoint = slabsPoint - origin;
            shiftedPoint = rotMat.Transform(shiftedPoint);
            return shiftedPoint.Z;
        }

        /// <summary>The shift reinforcement bars.</summary>
        /// <param name="rebarGeometries">The reinforcement bar geometries.</param>
        /// <param name="shift">The shift.</param>
        /// <param name="minDepth">The min depth.</param>
        private void ShiftRebars(ref ArrayList rebarGeometries, ref double shift, ref double minDepth)
        {
            if (shift > Epsilon)
            {
                var shiftingVector = new Vector(0.0, 0.0, shift);
                var mat = new Matrix();
                this.RotateRebarsGeometries(ref rebarGeometries, mat, shiftingVector);
                shift -= minDepth;
                minDepth = 0.0;
            }
        }

        /// <summary>The show message.</summary>
        /// <param name="message">The message.</param>
        private void ShowMessage(string message)
        {
            Trace.WriteLine("Exception: " + this.localization.GetText(message));
            MessageBox.Show(
                this.localization.GetText(message), 
                this.localization.GetText("albl_Rebar_Classificator"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning);
        }

        #endregion
    }
}