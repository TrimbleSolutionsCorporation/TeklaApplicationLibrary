namespace Tekla.Logging.Log4Net
{
    using System;

    using log4net.Core;

    /// <summary>
    /// The log 4 net adapter.
    /// </summary>
    /// <remarks>
    /// Log4net is capable of outputting extended debug information about where the current
    /// message was generated: class name, method name, file, line, etc. Log4net assumes that the location
    /// information should be gathered relative to where Debug() was called.
    /// When using Common.Logging, Debug() is called in Common.Logging.Log4Net.Log4NetLogger. This means that
    /// the location information will indicate that Common.Logging.Log4Net.Log4NetLogger always made
    /// the call to Debug(). We need to know where Common.Logging.ILog.Debug()
    /// was called. To do this we need to use the log4net.ILog.Logger.Log method and pass in a Type telling
    /// log4net where in the stack to begin looking for location information.
    /// </remarks>
    internal class Log4NetAdapter : ILog
    {
        #region Static Fields

        /// <summary>
        /// The declaring type.
        /// </summary>
        private static readonly Type DeclaringType = typeof(Log4NetAdapter);

        #endregion

        #region Fields

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Log4NetAdapter"/> class.</summary>
        /// <param name="log">The log value.</param>
        /// <exception cref="ArgumentException">Throws a argument exception.</exception>
        public Log4NetAdapter(log4net.ILog log)
        {
            if (log == null)
            {
                throw new ArgumentException("log");
            }

            this.logger = log.Logger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the log file.
        /// </summary>
        public string LogFile
        {
            get
            {
                var apnds = this.logger.Repository.GetAppenders();
                foreach (var apnd in apnds)
                {
                    if (apnd is log4net.Appender.RollingFileAppender)
                    {
                        return (apnd as log4net.Appender.RollingFileAppender).File;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return this.logger;
            }
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets a value indicating whether is debug enabled.
        /// </summary>
        bool ILog.IsDebugEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Debug);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is error enabled.
        /// </summary>
        bool ILog.IsErrorEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Error);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is fatal enabled.
        /// </summary>
        bool ILog.IsFatalEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Fatal);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is info enabled.
        /// </summary>
        bool ILog.IsInfoEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Info);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is trace enabled.
        /// </summary>
        bool ILog.IsTraceEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Trace);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is warn enabled.
        /// </summary>
        bool ILog.IsWarnEnabled
        {
            get
            {
                return this.logger.IsEnabledFor(Level.Warn);
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>The debug.</summary>
        /// <param name="message">The message.</param>
        void ILog.Debug(object message)
        {
            this.logger.Log(DeclaringType, Level.Debug, message, null);
        }

        /// <summary>The debug.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Debug(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Debug, message, exception);
        }

        /// <summary>The error.</summary>
        /// <param name="message">The message.</param>
        void ILog.Error(object message)
        {
            this.logger.Log(DeclaringType, Level.Error, message, null);
        }

        /// <summary>The error.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Error(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Error, message, exception);
        }

        /// <summary>The fatal.</summary>
        /// <param name="message">The message.</param>
        void ILog.Fatal(object message)
        {
            this.logger.Log(DeclaringType, Level.Fatal, message, null);
        }

        /// <summary>The fatal.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Fatal(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Fatal, message, exception);
        }

        /// <summary>The info value.</summary>
        /// <param name="message">The message.</param>
        void ILog.Info(object message)
        {
            this.logger.Log(DeclaringType, Level.Info, message, null);
        }

        /// <summary>The info value.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Info(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Info, message, exception);
        }

        /// <summary>The trace.</summary>
        /// <param name="message">The message.</param>
        void ILog.Trace(object message)
        {
            this.logger.Log(DeclaringType, Level.Trace, message, null);
        }

        /// <summary>The trace.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Trace(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Trace, message, exception);
        }

        /// <summary>The warn value.</summary>
        /// <param name="message">The message.</param>
        void ILog.Warn(object message)
        {
            this.logger.Log(DeclaringType, Level.Warn, message, null);
        }

        /// <summary>The warn value.</summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void ILog.Warn(object message, Exception exception)
        {
            this.logger.Log(DeclaringType, Level.Warn, message, exception);
        }

        #endregion
    }
}