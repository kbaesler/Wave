using System.IO;

namespace System.Reflection
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Assembly" /> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the copyright notice as defined by the AssemblyCopyright
        ///     attribute in the AssemblyInfo.cs file of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns a <see cref="string" /> representing the description; otherwise <c>null</c>.</returns>
        public static string GetCopyrights(this Assembly assembly)
        {
            AssemblyCopyrightAttribute[] array = (AssemblyCopyrightAttribute[]) assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
            return (array.Length > 0) ? array[0].Copyright : null;
        }

        /// <summary>
        ///     Returns the directory that contains the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns a <see cref="string" /> representing the directory that contains the assembly; otherwise <c>null</c>.</returns>
        public static string GetDirectory(this Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            return path;
        }

        /// <summary>
        ///     Returns a description of this product as defined by the AssemblyDescription attribute in the
        ///     AssemblyInfo.cs file of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns a <see cref="string" /> representing the description; otherwise <c>null</c>.</returns>
        public static string GetProductDescription(this Assembly assembly)
        {
            AssemblyDescriptionAttribute[] array = (AssemblyDescriptionAttribute[]) assembly.GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
            return (array.Length > 0) ? array[0].Description : null;
        }

        /// <summary>
        ///     Returns the name of this product as defined by the AssemblyProduct attribute in the
        ///     AssemblyInfo.cs file of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns a <see cref="string" /> representing the description; otherwise <c>null</c>.</returns>
        public static string GetProductName(this Assembly assembly)
        {
            AssemblyProductAttribute[] array = (AssemblyProductAttribute[]) assembly.GetCustomAttributes(typeof (AssemblyProductAttribute), false);
            return (array.Length > 0) ? array[0].Product : null;
        }

        #endregion
    }
}