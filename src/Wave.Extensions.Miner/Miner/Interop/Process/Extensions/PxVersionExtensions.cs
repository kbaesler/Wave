using System;
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
        ///     Gets the version status as an enumeration.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="PxVersionStatus" /> representing the enumeration value for the version status.
        /// </returns>
        public static PxVersionStatus GetVersionStatus(this IMMPxSDEVersion source)
        {
            var value = Enum.ToObject(typeof (PxVersionStatus), source.get_Status());
            return (PxVersionStatus) value;
        }

        #endregion
    }
}