namespace Tekla.Structures
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Application interface.
    /// </summary>
    public static class TeklaStructures
    {
        #region Static Fields

        /// <summary>
        /// Common tasks library.
        /// </summary>
        private static readonly CommonTasks TeklaStructuresCommonTasks = new CommonTasks();

        /// <summary>
        /// Application connection instance.
        /// </summary>
        private static readonly Connection TeklaStructuresConnection = new Connection();

        /// <summary>
        /// Application main window.
        /// </summary>
        private static readonly MainWindow TeklaStructuresMainWindow = new MainWindow();

        /// <summary>
        /// Application registry.
        /// </summary>
        private static readonly Registry TeklaStructuresRegistry = new Registry();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="TeklaStructures"/> class. 
        /// Initializes the class.
        /// </summary>
        static TeklaStructures()
        {
            Application.ApplicationExit += delegate { Disconnect(); };
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Announces that the application has been closed.
        /// </summary>
        public static event EventHandler Closed
        {
            add
            {
                TeklaStructuresConnection.Model.ApplicationClosed += value;
            }

            remove
            {
                TeklaStructuresConnection.Model.ApplicationClosed -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the common tasks library interface.
        /// </summary>
        /// <value>
        /// Common tasks library interface instance.
        /// </value>
        public static ICommonTasks CommonTasks
        {
            get
            {
                return TeklaStructuresCommonTasks;
            }
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// Application configuration.
        /// </value>
        public static Configuration Configuration
        {
            get
            {
                return TeklaStructuresConnection.Model.Configuration;
            }
        }

        /// <summary>
        /// Gets the connection interface.
        /// </summary>
        /// <value>
        /// Connection interface instance.
        /// </value>
        public static IConnection Connection
        {
            get
            {
                return TeklaStructuresConnection;
            }
        }

        /// <summary>
        /// Gets the drawing interface.
        /// </summary>
        /// <value>
        /// Drawing interface instance.
        /// </value>
        public static IDrawing Drawing
        {
            get
            {
                return TeklaStructuresConnection.Drawing;
            }
        }

        /// <summary>
        /// Gets the application environment interface.
        /// </summary>
        /// <value>
        /// Application environment interface instance.
        /// </value>
        public static IEnvironment Environment
        {
            get
            {
                return TeklaStructuresConnection.Model;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the application is running.
        /// </summary>
        /// <value>
        /// Indicates whether the application is running.
        /// </value>
        public static bool IsRunning
        {
            get
            {
                return TeklaStructuresMainWindow.Handle != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the Tekla Structures main window interface.
        /// </summary>
        /// <value>
        /// Tekla Structures main window interface instance.
        /// </value>
        public static IMainWindow MainWindow
        {
            get
            {
                return TeklaStructuresMainWindow;
            }
        }

        /// <summary>
        /// Gets the model interface.
        /// </summary>
        /// <value>
        /// Model interface instance.
        /// </value>
        public static IModel Model
        {
            get
            {
                return TeklaStructuresConnection.Model;
            }
        }

        /// <summary>
        /// Gets the application registry interface.
        /// </summary>
        /// <value>
        /// Application registry interface.
        /// </value>
        public static IRegistry Registry
        {
            get
            {
                return TeklaStructuresRegistry;
            }
        }

        /// <summary>
        /// Gets the application version string.
        /// </summary>
        /// <value>
        /// Application version string.
        /// </value>
        public static string Version
        {
            get
            {
                return TeklaStructuresConnection.Model.Version;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Connects the interface to the application.
        /// </summary>
        /// <returns>A boolean value indicating whether the interface was connected.</returns>
        public static bool Connect()
        {
            return TeklaStructuresConnection.Connect();
        }

        /// <summary>
        /// Disconnects the interface from the application.
        /// </summary>
        public static void Disconnect()
        {
            TeklaStructuresConnection.Disconnect();
        }

        /// <summary>Executes a script.</summary>
        /// <param name="script">Script text.</param>
        public static void ExecuteScript(string script)
        {
            new MacroBuilder(script).Run(TeklaStructuresConnection);
        }

        #endregion
    }
}