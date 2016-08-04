using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            if (assembly == null) return null;

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
            if (assembly == null) return null;

            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            return path;
        }

        /// <summary>
        ///     Gets the manifest resource streams.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey, TValue}" /> repersenting the manifest resources by names.
        /// </returns>
        public static Dictionary<string, Stream> GetManifestResourceStreams(this Assembly source, string path)
        {
            var list = GetManifestResourceStreams(source).Where(o => o.Key.StartsWith(path)).ToDictionary(pair =>
            {
                string[] names = pair.Key.Split(new[] {path}, StringSplitOptions.RemoveEmptyEntries);
                return names.First();
            },
                pair => pair.Value);

            return list;
        }

        /// <summary>
        ///     Gets the manifest resource streams.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="Dictionary{TKey, TValue}" /> repersenting the manifest resources by names.</returns>
        public static Dictionary<string, Stream> GetManifestResourceStreams(this Assembly source)
        {
            Dictionary<string, Stream> list = new Dictionary<string, Stream>();
            var names = source.GetManifestResourceNames();
            foreach (var name in names)
            {
                var stream = source.GetManifestResourceStream(name);
                list.Add(name, stream);
            }

            return list;
        }

        /// <summary>
        ///     Returns a description of this product as defined by the AssemblyDescription attribute in the
        ///     AssemblyInfo.cs file of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns a <see cref="string" /> representing the description; otherwise <c>null</c>.</returns>
        public static string GetProductDescription(this Assembly assembly)
        {
            if (assembly == null) return null;

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
            if (assembly == null) return null;

            AssemblyProductAttribute[] array = (AssemblyProductAttribute[]) assembly.GetCustomAttributes(typeof (AssemblyProductAttribute), false);
            return (array.Length > 0) ? array[0].Product : null;
        }

        #endregion
    }
}