namespace Tekla.Structures
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Helper class for executing actions in separate thread.
    /// </summary>
    public static class SeparateThread
    {
        #region Static Fields

        /// <summary>
        /// Timeout in milliseconds.
        /// </summary>
        private static int timeoutMilliseconds = 10000;

        #endregion

        #region Delegates

        /// <summary>
        /// Deferred action.
        /// </summary>
        public delegate void Action();

        /// <summary>
        /// Deferred action with result.
        /// </summary>
        /// <typeparam name="T">
        /// Result type.
        /// </typeparam>
        /// <returns>
        /// Action result.
        /// </returns>
        public delegate T Action<T>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the default timeout in milliseconds.
        /// </summary>
        /// <value> Timeout in milliseconds.</value>
        public static int TimeoutMilliseconds
        {
            get
            {
                return timeoutMilliseconds;
            }

            set
            {
                timeoutMilliseconds = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Executes an action in separate thread.
        /// </summary>
        /// <param name="action">
        /// Action to execute.
        /// </param>
        /// <exception cref="TimeoutException">
        /// Thrown if the operation times out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the operation causes an error.
        /// </exception>
        public static void Execute(Action action)
        {
            Execute(timeoutMilliseconds, action);
        }

        /// <summary>
        /// Executes an action in separate thread.
        /// </summary>
        /// <param name="timeoutMilliseconds">
        /// Timeout in milliseconds.
        /// </param>
        /// <param name="action">
        /// Action to execute.
        /// </param>
        /// <exception cref="TimeoutException">
        /// Thrown if the operation times out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the operation causes an error.
        /// </exception>
        public static void Execute(int timeoutMilliseconds, Action action)
        {
            Exception error = null;

            using (var ev = new ManualResetEvent(false))
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                        {
                            try
                            {
                                action();
                            }
                            catch (RemotingException e)
                            {
                                Trace.WriteLine(e.ToString());
                            }
                            catch (Exception e)
                            {
                                error = e;
                            }
                            finally
                            {
                                try
                                {
                                    ev.Set();
                                }
                                catch (ObjectDisposedException)
                                {
                                    // No action.
                                }
                            }
                        });

                if (!ev.WaitOne(timeoutMilliseconds, true))
                {
                    var dialog = new WaitingDialog(
                        delegate
                            {
                                try
                                {
                                    return ev.WaitOne(0, true);
                                }
                                catch (ObjectDisposedException)
                                {
                                    return true;
                                }
                            });

                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        throw new TimeoutException("Operation timed out.");
                    }
                }
                else if (error != null)
                {
                    throw new InvalidOperationException(
                        "Operation caused an error. See the inner exception for details.", error);
                }
            }
        }

        /// <summary>
        /// Executes an action in separate thread.
        /// </summary>
        /// <typeparam name="T">
        /// Result type.
        /// </typeparam>
        /// <param name="action">
        /// Action to execute.
        /// </param>
        /// <returns>
        /// Action result.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// Thrown if the operation times out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the operation causes an error.
        /// </exception>
        public static T Execute<T>(Action<T> action)
        {
            return Execute(timeoutMilliseconds, action);
        }

        /// <summary>
        /// Executes an action in separate thread.
        /// </summary>
        /// <typeparam name="T">
        /// Result type.
        /// </typeparam>
        /// <param name="timeoutMilliseconds">
        /// Timeout in milliseconds.
        /// </param>
        /// <param name="action">
        /// Action to execute.
        /// </param>
        /// <returns>
        /// Action result.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// Thrown if the operation times out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the operation causes an error.
        /// </exception>
        public static T Execute<T>(int timeoutMilliseconds, Action<T> action)
        {
            var result = default(T);
            Exception error = null;

            using (var ev = new ManualResetEvent(false))
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                        {
                            try
                            {
                                result = action();
                            }
                            catch (RemotingException e)
                            {
                                Trace.WriteLine(e.ToString());
                                result = default(T);
                            }
                            catch (Exception e)
                            {
                                error = e;
                            }
                            finally
                            {
                                try
                                {
                                    ev.Set();
                                }
                                catch (ObjectDisposedException)
                                {
                                    // No action.
                                }
                            }
                        });

                if (!ev.WaitOne(timeoutMilliseconds, true))
                {
                    var dialog = new WaitingDialog(
                        delegate
                            {
                                try
                                {
                                    return ev.WaitOne(0, true);
                                }
                                catch (ObjectDisposedException)
                                {
                                    return true;
                                }
                            });

                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        throw new TimeoutException("Operation timed out.");
                    }
                }
                else if (error != null)
                {
                    throw new InvalidOperationException(
                        "Operation caused an error. See the inner exception for details.", error);
                }
            }

            return result;
        }

        #endregion
    }
}