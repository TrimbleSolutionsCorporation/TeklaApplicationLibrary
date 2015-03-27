namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Representation of a virtual folder.
    /// </summary>
    /// <remarks>
    /// Virtual folder represents a folder that is augmented with a search path. When 
    /// searching for files, the search is extended to all folders in the search path.
    /// </remarks>
    public class VirtualFolder
    {
        #region Fields

        /// <summary>
        /// Folder path.
        /// </summary>
        private readonly string folderPath;

        /// <summary>
        /// Search path.
        /// </summary>
        private readonly string searchPath;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="VirtualFolder"/> class. 
        /// Initializes a new instance of the class.</summary>
        /// <param name="folderPath">Folder path.</param>
        /// <param name="searchPath">Search path.</param>
        public VirtualFolder(string folderPath, string searchPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            if (searchPath == null)
            {
                throw new ArgumentNullException("searchPath");
            }

            folderPath = folderPath.Trim().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            this.folderPath = folderPath;
            this.searchPath = folderPath + SearchPath.Separator + searchPath;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the folder name.
        /// </summary>
        /// <value>
        /// Folder name.
        /// </value>
        public string FolderName
        {
            get
            {
                return Path.GetFileName(this.folderPath);
            }
        }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <value>
        /// Folder path.
        /// </value>
        public string FolderPath
        {
            get
            {
                return this.folderPath;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a writable copy of the specified file.</summary>
        /// <param name="filename">File name.</param>
        /// <returns>Writable path for the specified file.</returns>
        public string CreateWritableCopy(string filename)
        {
            var target = this.GetWritablePath(filename);

            if (!File.Exists(target))
            {
                var source = this.FindFile(filename);

                if (source != null)
                {
                    File.Copy(source, target, true);
                }
            }

            return target;
        }

        /// <summary>Finds a file in the folder.</summary>
        /// <param name="filename">File name.</param>
        /// <returns>Path to the file or null if the file was not found.</returns>
        public string FindFile(string filename)
        {
            return SearchPath.FindFile(this.searchPath, filename);
        }

        /// <summary>Finds the files matching the specified pattern.</summary>
        /// <param name="pattern">Find pattern.</param>
        /// <returns>Enumerable collection of found files.</returns>
        public IEnumerable<string> FindFiles(string pattern)
        {
            return SearchPath.FindFiles(this.searchPath, pattern);
        }

        /// <summary>Finds the files with the specified extension.</summary>
        /// <param name="extension">File extension.</param>
        /// <returns>Enumerable collection of found files.</returns>
        public IEnumerable<string> FindFilesWithExtension(string extension)
        {
            return SearchPath.FindFilesWithExtension(this.searchPath, extension);
        }

        /// <summary>Returns the writable path for the specified file.</summary>
        /// <param name="filename">File name.</param>
        /// <returns>Writable path for the specified file.</returns>
        public string GetWritablePath(string filename)
        {
            return Path.Combine(this.folderPath, Path.GetFileName(filename));
        }

        /// <summary>Determines whether the specified file is writable.</summary>
        /// <param name="filename">File name.</param>
        /// <returns>A boolean value indicating whether the file is writable.</returns>
        public bool IsWritable(string filename)
        {
            return File.Exists(this.GetWritablePath(filename));
        }

        #endregion
    }
}