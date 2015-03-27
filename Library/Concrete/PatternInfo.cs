namespace Tekla.Structures.Concrete
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;

    using Tekla.Structures.Geometry3d;
    using Tekla.Structures.Model;

    /// <summary>
    /// The pattern info.
    /// </summary>
    public class PatternInfo
    {
        #region Constants

        /// <summary>
        /// The beam tag.
        /// </summary>
        private const string BeamTag = "PATTERN_TYPE";

        /// <summary>
        /// The bottom class tag.
        /// </summary>
        private const string BottomClassTag = "BOT_CLASS";

        /// <summary>
        /// The bottom name tag.
        /// </summary>
        private const string BottomNameTag = "BOT_NAME";

        /// <summary>
        /// The bottom xy tag.
        /// </summary>
        private const string BottomXyTag = "BOT_XY";

        /// <summary>
        /// The pattern begin tag.
        /// </summary>
        private const string PatternBeginTag = "PATTERN_BEGIN";

        /// <summary>
        /// The pattern end tag.
        /// </summary>
        private const string PatternEndTag = "PATTERN_END";

        /// <summary>
        /// The symmtery tag.
        /// </summary>
        private const string SymmteryTag = "SYMMETRY";

        /// <summary>
        /// The top class tag.
        /// </summary>
        private const string TopClassTag = "TOP_CLASS";

        /// <summary>
        /// The top name tag.
        /// </summary>
        private const string TopNameTag = "TOP_NAME";

        /// <summary>
        /// The top xy tag.
        /// </summary>
        private const string TopXyTag = "TOP_XY";

        #endregion

        #region Static Fields

        /// <summary>
        /// The pattern data.
        /// </summary>
        private static ArrayList patternData;

        /// <summary>
        /// The bottom class.
        /// </summary>
        private static int bottomClass = 3;

        /// <summary>
        /// The bottom name.
        /// </summary>
        private static string bottomName = "BOTTOM";

        /// <summary>
        /// The top class.
        /// </summary>
        private static int topClass = 3;

        /// <summary>
        /// The top name.
        /// </summary>
        private static string topName = "TOP";

        /// <summary>
        /// The bottom pos.
        /// </summary>
        private ArrayList bottomPos;

        /// <summary>
        /// The top pos.
        /// </summary>
        private ArrayList topPos;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternInfo"/> class.
        /// </summary>
        public PatternInfo()
        {
            this.BeamType = string.Empty;
            this.Height = 0.0;
            this.IsSymmetrical = true;
            this.Name = string.Empty;
            this.Width = this.Height = 0;

            this.bottomPos = new ArrayList();
            this.topPos = new ArrayList();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the bottom name.</summary>
        /// <value>The bottom name.</value>
        public static string BottomName
        {
            get
            {
                return bottomName;
            }
        }

        /// <summary>Gets the top class.</summary>
        /// <value>The top class.</value>
        public static int TopClass
        {
            get
            {
                return topClass;
            }
        }

        /// <summary>Gets the top name.</summary>
        /// <value>The top name.</value>
        public static string TopName
        {
            get
            {
                return topName;
            }
        }

        /// <summary>Gets the bottom class.</summary>
        /// <value>The bottom class.</value>
        public static int BottomClass
        {
            get
            {
                return bottomClass;
            }
        }

        /// <summary>Gets or sets the beam type.</summary>
        /// <value>The beam type.</value>
        public string BeamType { get; set; }

        /// <summary>Gets or sets the height.</summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        /// <summary>Gets or sets a value indicating whether this is symmetrical.</summary>
        /// <value>The is symmetrical.</value>
        public bool IsSymmetrical { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the width.</summary>
        /// <value>The width value.</value>
        public double Width { get; set; }

        /// <summary>Gets the max number bottom.</summary>
        /// <value>The max number bottom.</value>
        public int MaxNumberBottom
        {
            get
            {
                return this.IsSymmetrical ? 2 * this.bottomPos.Count : this.bottomPos.Count;
            }
        }

        /// <summary>Gets the max number top.</summary>
        /// <value>The max number top.</value>
        public int MaxNumberTop
        {
            get
            {
                return this.IsSymmetrical ? 2 * this.topPos.Count : this.topPos.Count;
            }
        }

        #endregion Public Properties

        #region Public Methods and Operators

        /// <summary>
        /// The get all beam types.
        /// </summary>
        /// <returns>
        /// The System.Collections.ArrayList.
        /// </returns>
        public static ArrayList GetAllBeamTypes()
        {
            var result = new ArrayList();
            if (patternData == null)
            {
                LoadPatternData();
            }

            if (patternData != null)
            {
                foreach (PatternInfo pi in patternData)
                {
                    if (!result.Contains(pi.BeamType))
                    {
                        result.Add(pi.BeamType);
                    }
                }
            }

            return result;
        }

        /// <summary>The get pattern info.</summary>
        /// <param name="name">The name value.</param>
        /// <returns>The Tekla.Structures.Concrete.PatternInfo.</returns>
        public static PatternInfo GetPatternInfo(string name)
        {
            PatternInfo result = null;
            if (patternData == null)
            {
                LoadPatternData();
            }

            if (patternData != null)
            {
                foreach (PatternInfo pi in patternData)
                {
                    if (pi.Name == name)
                    {
                        result = pi;
                        break;
                    }
                }

                if (result == null)
                {
                    result = (PatternInfo)patternData[0];
                }
            }

            return result;
        }

        /// <summary>The get patterns by beam type.</summary>
        /// <param name="type">The type value.</param>
        /// <returns>The System.Collections.ArrayList.</returns>
        public static ArrayList GetPatternsByBeamType(string type)
        {
            var result = new ArrayList();
            if (patternData == null)
            {
                LoadPatternData();
            }

            if (patternData != null)
            {
                foreach (PatternInfo pi in patternData)
                {
                    if (pi.BeamType == type)
                    {
                        result.Add(pi);
                    }
                }
            }

            return result;
        }

        /// <summary>The get sys file path name.</summary>
        /// <param name="m">The m value.</param>
        /// <param name="name">The name value.</param>
        /// <returns>The System.String.</returns>
        public static string GetSysFilePathName(Model m, string name)
        {
            var result = string.Empty;
            string[] items = null;
            var ii = 0;

            while (true)
            {
                var path = string.Empty;

                if (ii == 0)
                {
                    path = m.GetInfo().ModelPath;
                }
                else if (ii == 1)
                {
                    TeklaStructuresSettings.GetAdvancedOption("XS_PROJECT", ref path);
                }
                else if (ii == 2)
                {
                    TeklaStructuresSettings.GetAdvancedOption("XS_FIRM", ref path);
                }
                else if (ii == 3)
                {
                    TeklaStructuresSettings.GetAdvancedOption("XS_SYSTEM", ref path);

                    // this might contain several folders
                    items = path.Split(new[] { ';' });
                    if (items.GetLength(0) > 0)
                    {
                        path = items[0];
                    }
                }
                else if (items.GetLength(0) > ii - 3)
                {
                    path = items[ii - 3];
                }
                else
                {
                    break;
                }

                if (path != string.Empty)
                {
                    var fullName = path + @"\" + name;
                    if (File.Exists(fullName))
                    {
                        result = fullName;
                        break;
                    }
                }

                ii++;
            }

            return result;
        }

        /// <summary>The add bottom pos.</summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public void AddBottomPos(double x, double y)
        {
            var newp = new Point(x, y, 0);
            this.bottomPos.Add(newp);
            this.SetWidthHeight(x, y);
        }

        /// <summary>The add top pos.</summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public void AddTopPos(double x, double y)
        {
            var newp = new Point(x, y, 0);
            this.topPos.Add(newp);
        }

        /// <summary>The get bottom xy.</summary>
        /// <param name="index">The index.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>The System.Boolean.</returns>
        public bool GetBottomXy(int index, ref double x, ref double y)
        {
            var result = false;
            var jj = this.IsSymmetrical ? index / 2 : index;
            if (jj < this.bottomPos.Count)
            {
                var pp = (Point)this.bottomPos[jj];
                x = pp.X;
                y = pp.Y;
                if (this.IsSymmetrical && 2 * jj != index)
                {
                    x = -x;
                }

                result = true;
            }

            return result;
        }

        /// <summary>The get top xy.</summary>
        /// <param name="index">The index.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>The System.Boolean.</returns>
        public bool GetTopXy(int index, ref double x, ref double y)
        {
            var result = false;
            var jj = this.IsSymmetrical ? index / 2 : index;
            if (jj < this.topPos.Count)
            {
                var pp = (Point)this.topPos[jj];
                x = pp.X;
                y = pp.Y;
                if (this.IsSymmetrical && 2 * jj != index)
                {
                    x = -x;
                }

                result = true;
            }

            return result;
        }

        /// <summary>The read pattern block.</summary>
        /// <param name="sr">The sr value.</param>
        public void ReadPatternBlock(StreamReader sr)
        {
            string line;
            this.bottomPos = new ArrayList();
            this.topPos = new ArrayList();
            while ((line = sr.ReadLine()) != null && line != PatternEndTag)
            {
                var items = line.Split(new[] { ';' }, 3);
                if (items.GetLength(0) > 1)
                {
                    if (items[0] == SymmteryTag && items.GetLength(0) > 1)
                    {
                        this.IsSymmetrical = items[1] != "0";
                    }
                    else if (items[0] == BottomXyTag && items.GetLength(0) > 2)
                    {
                        var np = new Point(
                            Convert.ToDouble(items[1], CultureInfo.InvariantCulture), 
                            Convert.ToDouble(items[2], CultureInfo.InvariantCulture), 
                            0);
                        this.SetWidthHeight(np.X, np.Y);
                        this.bottomPos.Add(np);
                    }
                    else if (items[0] == TopXyTag && items.GetLength(0) > 2)
                    {
                        var np = new Point(
                            Convert.ToDouble(items[1], CultureInfo.InvariantCulture), 
                            Convert.ToDouble(items[2], CultureInfo.InvariantCulture), 
                            0);
                        this.topPos.Add(np);
                    }
                }
            }
        }

        /// <summary>The set width height.</summary>
        /// <param name="pointX">The point x.</param>
        /// <param name="pointY">The point y.</param>
        /// <returns>The System.Boolean.</returns>
        public bool SetWidthHeight(double pointX, double pointY)
        {
            var result = false;

            if (this.bottomPos != null)
            {
                for (var ii = 0; ii < this.bottomPos.Count; ii++)
                {
                    var pp = (Point)this.bottomPos[ii];

                    if (pp.X < 2 * Math.Abs(pointX))
                    {
                        this.Width = 2 * Math.Abs(pointX);
                    }

                    if (pp.X < 2 * Math.Abs(pointY))
                    {
                        this.Height = 2 * Math.Abs(pointY);
                    }
                    else
                    {
                        this.Height = this.Width;
                    }

                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get pattern file name.
        /// </summary>
        /// <returns>
        /// The System.String.
        /// </returns>
        private static string GetPatternFileName()
        {
            var result = string.Empty;
            var m = new Model();
            var modelInfo = m.GetInfo();
            if (modelInfo != null && !modelInfo.ModelPath.Equals(string.Empty))
            {
                result = GetSysFilePathName(m, @"\StrandPattern.dat");
            }

            return result;
        }

        /// <summary>
        /// The load pattern data.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception.
        /// </exception>
        private static bool LoadPatternData()
        {
            patternData = new ArrayList();

            try
            {
                var path = GetPatternFileName();
                if (!path.Equals(string.Empty))
                {
                    var sr = new StreamReader(path);
                    using (sr)
                    {
                        string line;
                        var currentBeam = string.Empty;
                        PatternInfo pi;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var items = line.Split(new[] { ';' }, 3);
                            if (items.GetLength(0) > 1)
                            {
                                if (items[0] == BeamTag)
                                {
                                    currentBeam = items[1];
                                }
                                else if (items[0] == BottomNameTag)
                                {
                                    bottomName = items[1];
                                }
                                else if (items[0] == BottomClassTag)
                                {
                                    try
                                    {
                                        bottomClass = Convert.ToInt32(items[1]);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else if (items[0] == TopNameTag)
                                {
                                    topName = items[1];
                                }
                                else if (items[0] == TopClassTag)
                                {
                                    try
                                    {
                                        topClass = Convert.ToInt32(items[1]);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else if (items[0] == PatternBeginTag)
                                {
                                    pi = new PatternInfo { Name = items[1], BeamType = currentBeam };
                                    pi.ReadPatternBlock(sr);
                                    patternData.Add(pi);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Exception in LoadPatternData of PatternInfo.");
            }

            return patternData.Count < 0;
        }

        #endregion
    }
}