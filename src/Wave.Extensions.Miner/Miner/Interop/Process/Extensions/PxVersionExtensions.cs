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
        ///     Returns the name of the version.
        /// </summary>
        /// <param name="source">The version.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the version.</returns>
        public static string GetVersionName(this IMMPxSDEVersion source)
        {
            if (source == null) return null;

            string ownerName = source.get_UserName();
            if (string.IsNullOrEmpty(ownerName))
                return source.get_Name();

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ownerName, source.get_Name());
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