using System.Globalization;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxSDEVersion" /> interface.
    /// </summary>
    public static class PxVersionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the fully qualified name of the version.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="node">The node.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the version.</returns>
        public static string GetVersionName(this IMMPxSDEVersionNamer source, IMMPxNode node)
        {
            var baseVersion = source.GetBaseVersionName(node.Id);
            var versionName = source.GetVersionName(node.Id);

            if (string.IsNullOrEmpty(baseVersion))
                return versionName;

            return string.Format("{0}{1}", baseVersion, versionName);
        }        

        /// <summary>
        ///     Determines whether the version is in the state specified by the version status.
        /// </summary>
        /// <param name="source">The version.</param>
        /// <param name="versionStatus">The version status.</param>
        /// <returns>
        ///     <c>true</c> if the version is in the state specified by the version status; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStatus(this IMMPxSDEVersion source, PxVersionStatus versionStatus)
        {
            if (source == null) return false;

            return source.get_Status() == (short) versionStatus;
        }

        #endregion
    }
}