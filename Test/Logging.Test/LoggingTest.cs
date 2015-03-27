namespace Tekla.Logging
{
    using log4net.Config;

    using NUnit.Framework;

    /// <summary>
    /// The logging test.
    /// </summary>
    [TestFixture]
    public class LoggingTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The log test.
        /// </summary>
        [Test]
        public void LogTest()
        {
            var logger = LogManager.GetLogger(typeof(LoggingTest));

            using (new Tracer(logger))
            {
                logger.Info("Logger test!");
            }
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [TestFixtureSetUp]
        public void SetUp()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// The tear down.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        #endregion
    }
}