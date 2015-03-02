using System;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IVersion" /> object.
    /// </summary>
    public static class VersionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Reconciles the current version with a target version.
        /// </summary>
        /// <param name="source">The current version.</param>
        /// <param name="targetVersionName">
        ///     The target version name passed in is case-sensitive and should take the form
        ///     {owner}.{version_name} for example, SDE.DEFAULT.
        /// </param>
        /// <param name="acquireLock">Indicates if locks should be obtained or not.</param>
        /// <param name="abortIfConflicts">
        ///     Indicates if the reconcile process shuld abort the reconcile if conflicts are detected
        ///     for any class.
        /// </param>
        /// <param name="childWins">Indicates if all conflicts detected would be resolved in favor of the source version.</param>
        /// <param name="columnLevel">
        ///     Indicates if conflicts are detected only when the same attribute is updated in the source and
        ///     target versions.
        /// </param>
        /// <param name="autoUpdaterMode">The ArcFM Auto Updater mode that is used during the reconcile.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when conflicts were detected; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">targetVersionName</exception>
        /// <remarks>
        ///     The Reconcile4 function reconciles the current source version with the specified target version.
        ///     The target version must be an ancestor of the current version or an error will be returned.
        /// </remarks>
        public static bool Reconcile(this IVersion source, string targetVersionName, bool acquireLock, bool abortIfConflicts, bool childWins, bool columnLevel, mmAutoUpdaterMode autoUpdaterMode)
        {
            if (source == null) return false;
            if (targetVersionName == null) throw new ArgumentNullException("targetVersionName");

            using (new AutoUpdaterModeReverter(autoUpdaterMode))
            {
                IVersionEdit4 versionEdit = (IVersionEdit4) source;
                bool hasConflicts = versionEdit.Reconcile4(targetVersionName, acquireLock, abortIfConflicts, childWins, columnLevel);
                return hasConflicts;
            }
        }

        #endregion
    }
}