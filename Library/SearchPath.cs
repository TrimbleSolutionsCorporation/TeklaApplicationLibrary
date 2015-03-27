namespace Tekla.Structures
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Operations for working with search paths.
    /// </summary>
    public static class SearchPath
    {
        #region Static Fields

        /// <summary>
        /// Separator character in search path.
        /// </summary>
        public const char Separator = ';';

        /// <summary>
        /// Separator characters.
        /// </summary>
        private static readonly char[] Separators = new[] { Separator };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Finds the specified file in the search path.
        /// </summary>
        /// <param name="searchPath">
        /// Search path.
        /// </param>
        /// <param name="filename">
        /// File name.
        /// </param>
        /// <returns>
        /// Full path of the file or null if the file was not found.
        /// </returns>
        public static string FindFile(string searchPath, string filename)
        {
            filename = Path.GetFileName(filename);

            if (!string.IsNullOrEmpty(filename))
            {
                foreach (var directory in GetDirectories(searchPath))
                {
                    var path = Path.Combine(directory, filename);

                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the files matching the specified search pattern.
        /// </summary>
        /// <param name="searchPath">
        /// Search path.
        /// </param>
        /// <param name="searchPattern">
        /// Search pattern.
        /// </param>
        /// <returns>
        /// Enumerable collection of file paths.
        /// </returns>
        public static IEnumerable<string> FindFiles(string searchPath, string searchPattern)
        {
            var files = new Dictionary<string, string>();

            foreach (var directory in GetDirectories(searchPath))
            {
                if (Directory.Exists(directory))
                {
                    foreach (var path in Directory.GetFiles(directory, searchPattern))
                    {
                        var file = Path.GetFileName(path);

                        if (!files.ContainsKey(file))
                        {
                            files.Add(file, path);

                            yield return path;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the files with the specified extension.
        /// </summary>
        /// <param name="searchPath">
        /// Search path.
        /// </param>
        /// <param name="extension">
        /// File extension.
        /// </param>
        /// <returns>
        /// Enumerable collection of file paths.
        /// </returns>
        public static IEnumerable<string> FindFilesWithExtension(string searchPath, string extension)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.ToLowerInvariant();

                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                foreach (var path in FindFiles(searchPath, "*" + extension))
                {
                    if (Path.GetExtension(path).ToLowerInvariant() == extension)
                    {
                        yield return path;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the directories specified in the search path.
        /// </summary>
        /// <param name="searchPath">
        /// Search path.
        /// </param>
        /// <returns>
        /// Enumerable collection of directory paths.
        /// </returns>
        private static IEnumerable<string> GetDirectories(string searchPath)
        {
            if (!string.IsNullOrEmpty(searchPath))
            {
                foreach (var directory in searchPath.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return Path.GetFullPath(directory);
                }
            }
        }

        #endregion
    }
}