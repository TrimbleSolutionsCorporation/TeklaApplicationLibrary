namespace Tekla.Structures
{
    using System.Drawing;

    using Microsoft.Win32;

    /// <summary>
    /// Provides access to application registry keys and values.
    /// </summary>
    internal class Registry : IRegistry
    {
        #region Fields

        /// <summary>
        /// Registry key for current version.
        /// </summary>
        private RegistryKey currentVersion;

        /// <summary>
        /// Application root key.
        /// </summary>
        private RegistryKey root;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the registry key for current application version.
        /// </summary>
        /// <value> Registry key.</value>
        public RegistryKey CurrentVersion
        {
            get
            {
                if (this.currentVersion == null)
                {
                    this.currentVersion = this.GetVersion(TeklaStructures.Version);
                }

                return this.currentVersion;
            }
        }

        /// <summary>
        /// Gets the application root registry key.
        /// </summary>
        /// <value> Registry key.</value>
        public RegistryKey Root
        {
            get
            {
                if (this.root == null)
                {
                    this.root = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Tekla\Structures");
                }

                return this.root;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the registry key for the specified version.
        /// </summary>
        /// <param name="version">
        /// Version string. 
        /// </param>
        /// <returns>
        /// Registry key. 
        /// </returns>
        public RegistryKey GetVersion(string version)
        {
            // Ftrs and TPs write registry values to Next branch
            if (version.ToLower().Contains("99.") || version.ToLower().Contains("test package"))
            {
                version = "Next";
            }
            else if (version.IndexOf(' ') > 0)
            {
                version = version.Split(' ')[0];
            }

            if (version.Contains("."))
            {
                var parts = version.Split('.');
                var major = parts[0];
                var minor = parts[1];

                version = major + "." + minor;
            }

            return this.Root.OpenSubKey(version);
        }

        /// <summary>
        /// Loads dialog bounds rectangle from registry.
        /// </summary>
        /// <param name="dialogName">
        /// Dialog name. 
        /// </param>
        /// <returns>
        /// Loaded dialog bounds or <see cref="Rectangle.Empty"/> if not available. 
        /// </returns>
        public Rectangle LoadDialogBounds(string dialogName)
        {
            return this.LoadDialogBounds(dialogName, TeklaStructures.Version);
        }

        /// <summary>
        /// Loads dialog bounds rectangle from registry.
        /// </summary>
        /// <param name="dialogName">
        /// Dialog name. 
        /// </param>
        /// <param name="version">
        /// Version string. 
        /// </param>
        /// <returns>
        /// Loaded dialog bounds or <see cref="Rectangle.Empty"/> if not available. 
        /// </returns>
        public Rectangle LoadDialogBounds(string dialogName, string version)
        {
            using (var versionRoot = this.GetVersion(version))
            {
                using (var dialogs = versionRoot.OpenSubKey("Dialogs"))
                {
                    using (var key = dialogs.OpenSubKey(dialogName))
                    {
                        var bounds = Rectangle.Empty;

                        if (key != null)
                        {
                            bounds.X = (int)key.GetValue("x", 0);
                            bounds.Y = (int)key.GetValue("y", 0);
                            bounds.Width = (int)key.GetValue("width", 0);
                            bounds.Height = (int)key.GetValue("height", 0);
                        }

                        return bounds;
                    }
                }
            }
        }

        /// <summary>
        /// Saves dialog bounds rectangle to registry.
        /// </summary>
        /// <param name="dialogName">
        /// Dialog name. 
        /// </param>
        /// <param name="bounds">
        /// Bounds rectangle. 
        /// </param>
        public void SaveDialogBounds(string dialogName, Rectangle bounds)
        {
            this.SaveDialogBounds(dialogName, TeklaStructures.Version, bounds);
        }

        /// <summary>
        /// Saves dialog bounds rectangle to registry.
        /// </summary>
        /// <param name="dialogName">
        /// Dialog name. 
        /// </param>
        /// <param name="version">
        /// Version string. 
        /// </param>
        /// <param name="bounds">
        /// Bounds rectangle. 
        /// </param>
        public void SaveDialogBounds(string dialogName, string version, Rectangle bounds)
        {
            using (var versionRoot = this.GetVersion(version))
            {
                using (var dialogs = versionRoot.OpenSubKey("Dialogs", true))
                {
                    using (var key = dialogs.CreateSubKey(dialogName))
                    {
                        key.SetValue("x", bounds.X);
                        key.SetValue("y", bounds.Y);
                        key.SetValue("width", bounds.Width);
                        key.SetValue("height", bounds.Height);
                    }
                }
            }
        }

        #endregion
    }
}