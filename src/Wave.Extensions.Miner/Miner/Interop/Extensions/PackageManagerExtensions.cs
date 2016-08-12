using System;
using System.Collections;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMPackageManager" /> interface.
    /// </summary>
    public static class PackageManagerExtensions
    {
        #region Public Methods

        /// <summary>
        /// Gets the package names.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="packageType">Type of the package.</param>
        /// <param name="packageCategory">The package category.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Returns a <see cref="IEnumerable{IMMPackageName}"/> representing the packages.</returns>
        public static IEnumerable<IMMPackageName> GetPackageNames(this IMMPackageManager source, mmPackageType packageType, mmPackageCategory packageCategory, IWorkspace workspace, string userName)
        {
            var packages = ((IMMPackageByWS) source).GetPackageNamesByWS(packageType, packageCategory, workspace, userName);
            if (packages != null)
            {
                IEnumerator enumerator = ((IEnumerable)packages).GetEnumerator();
                enumerator.Reset();
                while (enumerator.MoveNext())
                    yield return enumerator.Current as IMMPackageName;
            }
        } 

        #endregion
    }
}