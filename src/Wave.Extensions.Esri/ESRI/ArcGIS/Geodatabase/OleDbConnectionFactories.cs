using System.Data.OleDb;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides a factory for accessing the OLE DB providers for the ESRI connections.
    /// </summary>
    public static class OleDbConnectionFactories
    {
        #region Public Methods

        /// <summary>
        ///     Gets the access connection.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>
        ///     Returns a <see cref="OleDbConnection" /> representing the OLE DB connection.
        /// </returns>
        public static OleDbConnection GetAccessConnection(string fileName, OleDbGeometryType geometryType = OleDbGeometryType.Wkb)
        {
            string connectionString = string.Format("Provider=ESRI.GeoDB.OleDB.1;Data Source={0};Extended Properties=WorkspaceType=esriDataSourcesGDB.AccessWorkspaceFactory.1;Geometry={1}", fileName, geometryType);
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        ///     Gets the coverage connection.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>
        ///     Returns a <see cref="OleDbConnection" /> representing the OLE DB connection.
        /// </returns>
        public static OleDbConnection GetCoverageConnection(string fileName, OleDbGeometryType geometryType = OleDbGeometryType.Wkb)
        {
            string connectionString = string.Format("Provider=ESRI.GeoDB.OleDB.1;Data Source={0};Extended Properties=WorkspaceType=esriDataSourcesFile.ArcInfoWorkspaceFactory.1;Geometry={1}", fileName, geometryType.ToString().ToUpperInvariant());
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        ///     Gets the file geodatabase connection.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>
        ///     Returns a <see cref="OleDbConnection" /> representing the OLE DB connection.
        /// </returns>
        public static OleDbConnection GetFileGdbConnection(string fileName, OleDbGeometryType geometryType = OleDbGeometryType.Wkb)
        {
            string connectionString = string.Format("Provider=ESRI.GeoDB.OleDB.1;Data Source={0};Extended Properties=WorkspaceType=esriDataSourcesGDB.FileGDBWorkspaceFactory.1;Geometry={1}", fileName, geometryType.ToString().ToUpperInvariant());
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        ///     Gets the SDE connection.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>
        ///     Returns a <see cref="OleDbConnection" /> representing the OLE DB connection.
        /// </returns>
        public static OleDbConnection GetSdeConnection(string fileName, OleDbGeometryType geometryType = OleDbGeometryType.Wkb)
        {
            string connectionString = string.Format("Provider=ESRI.GeoDB.OleDB.1;Data Source={0};Extended Properties=WorkspaceType=esriDataSourcesGDB.SdeWorkspaceFactory.1;Geometry={1}", fileName, geometryType.ToString().ToUpperInvariant());
            return new OleDbConnection(connectionString);
        }

        /// <summary>
        ///     Gets the shapefile connection.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>
        ///     Returns a <see cref="OleDbConnection" /> representing the OLE DB connection.
        /// </returns>
        public static OleDbConnection GetShapefileConnection(string fileName, OleDbGeometryType geometryType = OleDbGeometryType.Wkb)
        {
            string connectionString = string.Format("Provider=ESRI.GeoDB.OleDB.1;Data Source={0};Extended Properties=WorkspaceType=esriDataSourcesFile.ShapefileWorkspaceFactory.1;Geometry={1}", fileName, geometryType.ToString().ToUpperInvariant());
            return new OleDbConnection(connectionString);
        }

        #endregion
    }

    /// <summary>
    ///     The enumerations of the supported geometry types.
    /// </summary>
    public enum OleDbGeometryType
    {
        /// <summary>
        ///     The well-known binary representation is a contiguous stream of bytes.
        /// </summary>
        Wkb,

        /// <summary>
        ///     The object representing is in ESRI formatted bytes.
        /// </summary>
        Object,
    }
}