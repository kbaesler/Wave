namespace System.IO
{
    /// <summary>
    ///     Provides extension methose for the <see cref="DirectoryInfo" />
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        #region Public Methods

        /// <summary>
        /// Copies the specified source directory (and files) to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destDirName">Name of the destination directory.</param>
        /// <param name="recrusive">if set to <c>true</c> when sub directories are copied.</param>
        /// <param name="overwrite">if set to <c>true</c> when overwritting existing files.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">Source directory does not exist or could not be found: "
        /// + sourceDirName</exception>
        /// <exception cref="DirectoryNotFoundException">Source directory does not exist or could not be found: "
        /// + sourceDirName</exception>
        public static DirectoryInfo Copy(this DirectoryInfo source, string destDirName, bool recrusive, bool overwrite)
        {
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + source.FullName);
            }

            DirectoryInfo[] dirs = source.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destDirName, file.Name), overwrite);
            }

            if (recrusive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    subdir.Copy(Path.Combine(destDirName, subdir.Name), recrusive, overwrite);
                }
            }

            return new DirectoryInfo(destDirName);
        }

        #endregion
    }
}