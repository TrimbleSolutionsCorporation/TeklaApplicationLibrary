namespace Tekla.Logging
{
    using System;
    using System.Diagnostics;

    using log4net.Config;
    using log4net.Ext.Trace;

    using NUnit.Framework;

    using Tekla.Logging.LoggerImpl;

    /// <summary>
    /// The performance test.
    /// </summary>
    [TestFixture]
    public class PerformanceTest
    {
        #region Constants

        /// <summary>
        /// The cnt value.
        /// </summary>
        private const int Cnt = 50000;

        #endregion

        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PerformanceTest));

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ITraceLog TraceLogger = TraceLogManager.GetLogger(typeof(PerformanceTest));

        #endregion

        #region Fields

        /// <summary>
        /// The watch.
        /// </summary>
        private Stopwatch watch = new Stopwatch();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The current logging test.
        /// </summary>
        [Test]
        public void CurrentLoggingTest()
        {
            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < Cnt; i++)
            {
                var d = CurrentLoggingMethod();
            }

            var elapsed1 = watch.ElapsedMilliseconds;
            Console.WriteLine("With current tracing: {0} ms ({1} mks per call)", elapsed1, elapsed1 * 1000 / Cnt);
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            XmlConfigurator.Configure();
            Console.WriteLine("Tracing performance test with {0} iterations", Cnt);
        }

        /// <summary>
        /// The suggested logging test.
        /// </summary>
        [Test]
        public void SuggestedLoggingTest()
        {
            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < Cnt; i++)
            {
                var d = SuggestedLoggingMethod();
            }

            var elapsed2 = watch.ElapsedMilliseconds;
            Console.WriteLine("With suggested tracing: {0} ms ({1} mks per call)", elapsed2, elapsed2 * 1000 / Cnt);
        }

        /// <summary>
        /// The tear down.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        /// <summary>
        /// The without logging test.
        /// </summary>
        [Test]
        public void WithoutLoggingTest()
        {
            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < Cnt; i++)
            {
                var d = WithoutLoggingMethod();
            }

            var elapsed0 = watch.ElapsedMilliseconds;
            Console.WriteLine("Without tracing: {0} ms ({1} mks per call)", elapsed0, elapsed0 * 1000 / Cnt);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The current logging method.
        /// </summary>
        /// <returns> The System.DateTime.</returns>
        private static DateTime CurrentLoggingMethod()
        {
            using (new Tracer(Logger))
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// The suggested logging method.
        /// </summary>
        /// <returns> The System.DateTime.</returns>
        private static DateTime SuggestedLoggingMethod()
        {
            if (TraceLogger.IsTraceEnabled)
            {
                TraceLogger.Trace("Entering...");
            }

            return DateTime.Now;
        }

        /// <summary>
        /// The without logging method.
        /// </summary>
        /// <returns> The System.DateTime.</returns>
        private static DateTime WithoutLoggingMethod()
        {
            return DateTime.Now;
        }

        #endregion
    }
}