namespace Tekla.Logging
{
    using System;

    using Tekla.Logging.Log4Net;

    /// <summary>
    /// Logging manager class to be used as a factory and repository to get reference to actual logger instance.
    /// </summary>
    public sealed class LogManager
    {
        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="LogManager"/> class from being created. 
        /// Private constructor to prevent object creation.</summary>
        private LogManager()
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Get a ILog instance by type.</summary>
        /// <param name="type">The type value.</param>
        /// <returns>The logger instance.</returns>
        public static ILog GetLogger(Type type)
        {
            return new Log4NetAdapter(log4net.LogManager.GetLogger(type));
        }

        #endregion
    }
}