namespace Tekla.Logging
{
    using System;

    /// <summary>
    /// Simple logging abstraction in order to provide a layer of indirection between logging calls made by MCP
    /// and the specific logging library used in your application (log4net, EntLib logging, NLog).
    /// see: http://springframework.net/docs/1.1-RC1/reference/html/logging.html.
    /// </summary>
    public interface ILog
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether this logger is enabled for the Debug level.</summary>
        /// <value>The is debug enabled.</value>
        bool IsDebugEnabled { get; }

        /// <summary>Gets a value indicating whether this logger is enabled for the Error level.</summary>
        /// <value>The is error enabled.</value>
        bool IsErrorEnabled { get; }

        /// <summary>Gets a value indicating whether this logger is enabled for the Fatal level.</summary>
        /// <value>The is fatal enabled.</value>
        bool IsFatalEnabled { get; }

        /// <summary>Gets a value indicating whether this logger is enabled for the Info level.</summary>
        /// <value>The is info enabled.</value>
        bool IsInfoEnabled { get; }

        /// <summary>Gets a value indicating whether this logger is enabled for the Trace level.</summary>
        /// <value>The is trace enabled.</value>
        bool IsTraceEnabled { get; }

        /// <summary>Gets a value indicating whether  this logger is enabled for the Warn level.</summary>
        /// <value>The is warn enabled.</value>
        bool IsWarnEnabled { get; }

        /// <summary>Gets the path to log file.</summary>
        /// <value>The log file.</value>
        string LogFile { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Logs a message object with the Debug level.</summary>
        /// <param name="message">The message object to log.</param>
        void Debug(object message);

        /// <summary>Log a message object with the Debug level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Debug(object message, Exception exception);

        /// <summary>Logs a message object with the Error level.</summary>
        /// <param name="message">The message object to log.</param>
        void Error(object message);

        /// <summary>Log a message object with the Error level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Error(object message, Exception exception);

        /// <summary>Logs a message object with the Fatal level.</summary>
        /// <param name="message">The message object to log.</param>
        void Fatal(object message);

        /// <summary>Log a message object with the Fatal level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Fatal(object message, Exception exception);

        /// <summary>Logs a message object with the Info level.</summary>
        /// <param name="message">The message object to log.</param>
        void Info(object message);

        /// <summary>Log a message object with the Info level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Info(object message, Exception exception);

        /// <summary>Logs a message object with the Trace level.</summary>
        /// <param name="message">The message object to log.</param>
        void Trace(object message);

        /// <summary>Log a message object with the Trace level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Trace(object message, Exception exception);

        /// <summary>Logs a message object with the Warn level.</summary>
        /// <param name="message">The message object to log.</param>
        void Warn(object message);

        /// <summary>Log a message object with the Warn level including the stack trace of the Exception passed as a parameter.</summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Warn(object message, Exception exception);

        #endregion
    }
}