namespace Tekla.Structures.IOTools
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Calculate checksum of file
    /// </summary>
    public class FileChecksum
    {
        /// <summary>The MD5</summary>
        private readonly MD5 md5 = MD5.Create();

        /// <summary>Calculates the MD5 checksum of file contents.</summary>
        /// <summary>Checksum can be used to check if file has changed.</summary>
        /// <param name="file">The file.</param>
        /// <returns>Checksum of file</returns>
        public string Calculate(string file)
        {
            return this.CalculateChecksum(file);
        }

        /// <summary>
        /// Calculates the checksum.
        /// NOTE: Do not change the calculated checksum!
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Checksum of file</returns>
        private string CalculateChecksum(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var buffer = this.md5.ComputeHash(stream);
                var sb = new StringBuilder();

                for (int i = 0; i < buffer.Length; i++)
                {
                    sb.Append(buffer[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
