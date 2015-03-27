namespace Tekla.Logging.Log4Net
{
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Appender for desktop applications (including ClickOnce) that rolls log files based on size or date or both.
    /// The <see cref="RollingFileAppender"/> extends the <seealso cref="log4net.Appender.RollingFileAppender"/> to override relative file names processing. 
    /// </summary>
    public class RollingFileAppender : log4net.Appender.RollingFileAppender
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the path to the file that logging will be written to. 
        /// </summary>
        /// <value>The path to the file that logging will be written to.</value>
        /// <remarks>
        /// If the file path is relative it is taken as relative from the <seealso cref="System.Windows.Forms.Application.LocalUserAppDataPath"/>
        /// instead of the application base directory - this is the difference with <seealso cref="M:log4net.Appender.RollingFileAppender.File"/> property.
        /// </remarks>
        public override string File
        {
            get
            {
                return base.File;
            }

            set
            {
                if (Path.IsPathRooted(value))
                {
                    base.File = value;
                }
                else
                {
                    base.File = Path.Combine(Application.LocalUserAppDataPath, value);
                }
            }
        }

        #endregion
    }
}