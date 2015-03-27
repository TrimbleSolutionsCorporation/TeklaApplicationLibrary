namespace Tekla.Logging
{
    using System;
    using System.Diagnostics;

    using log4net.Core;

    using Tekla.Logging.Log4Net;

    /// <summary>
    ///     Helper class to trace methods' entry/exit.
    ///     This class is similar to derives Microsoft.Practices.EnterpriseLibrary.Logging Tracer class.
    /// </summary>
    public class Tracer : IDisposable
    {
        #region Fields

        /// <summary>
        ///     The _logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        ///     The _watch.
        /// </summary>
        private readonly Stopwatch watch;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracer"/> class.
        /// </summary>
        /// <param name="logger">
        /// <see cref="ILog"/> object. 
        /// </param>
        public Tracer(ILog logger)
            : this(logger, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracer"/> class with a specified message.
        /// </summary>
        /// <param name="logger">
        /// <see cref="ILog"/> object. 
        /// </param>
        /// <param name="msg">
        /// Message to log. 
        /// </param>
        public Tracer(ILog logger, object msg)
        {
            this.logger = logger;
            if (logger == null || !logger.IsTraceEnabled)
            {
                return;
            }
            else
            {
                this.watch = new Stopwatch();
                (this.logger as Log4NetAdapter).Logger.Log(
                    typeof(Tracer), Level.Trace, msg == null ? "Entering..." : msg.ToString(), null);
                this.watch.Start();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Logs the end of tracing.
        /// </summary>
        public void Dispose()
        {
/*
            if (false && _watch != null && _logger != null)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder("Execution time (ms): ");
                (_logger as Log4NetAdapter).Logger.Log(typeof(Tracer), Level.Trace, msg.Append(_watch.ElapsedMilliseconds).ToString(), null);
            }
            // You should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            // GC.SuppressFinalize(this);    //actually not needed until finalizer is not implemented
*/
        }

        #endregion
    }
}