using System;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;

namespace Sempra.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides conversion extensions for <see cref="IDEWorkspace" /> interface objects.
    /// </summary>
    public static class DEExtensions

    {
        #region Public Methods

        /// <summary>
        /// Converts the <see cref="IDEWorkspace" /> to a <see cref="IWorkspace" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>
        /// Returns a <see cref="IWorkspace" /> representing the converted workspace object.
        /// </returns>
        /// <exception cref="System.NullReferenceException">The database connection is invalid.</exception>
        public static IWorkspace ToWorkspace(this IDEWorkspace source, IGPMessages messages = null)
        {
            IWorkspaceFactory workspaceFactory;

            if(source == null || source.ConnectionProperties == null)
            {
                if (messages == null)
                    throw new NullReferenceException("The database connection is invalid.");

                messages.AddError((int)fdoError.FDO_E_SE_INVALID_DATABASE, "The database connection is invalid.");
                return null;
            }

            if (source.WorkspaceType == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                if (source.WorkspaceFactoryProgID == "esriDataSourcesGDB.FileGDBWorkspaceFactory.1")
                {
                    workspaceFactory = new FileGDBWorkspaceFactoryClass();
                }
                else
                {
                    workspaceFactory = new AccessWorkspaceFactoryClass();
                }
            }
            else
            {
                workspaceFactory = new SdeWorkspaceFactoryClass();
            }

            return workspaceFactory.Open(source.ConnectionProperties, 0);
        }

        #endregion
    }
}