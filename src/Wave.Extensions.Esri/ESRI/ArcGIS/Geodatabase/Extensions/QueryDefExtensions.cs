using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IQueryDef" /> interface.
    /// </summary>
    public static class QueryDefExtensions
    {
        #region Public Methods


        /// <summary>
        ///     Create a table of the results of the query definition.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="primaryKeys">The primary key field names.</param>
        /// <param name="copyLocally">if set to <c>true</c> if the data must be copied locally.</param>
        /// <param name="workspace">The workspace that contains the tables.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the results of the query definition.</returns>
        public static ITable Evaluate(this IQueryDef source, string primaryKeys, bool copyLocally, IWorkspace workspace, string tableName)
        {
            IQueryName2 query = new TableQueryNameClass();
            query.QueryDef = source;
            query.PrimaryKey = (!copyLocally) ? primaryKeys : "";
            query.CopyLocally = copyLocally;

            IDatasetName ds = (IDatasetName)query;
            ds.WorkspaceName = (IWorkspaceName)((IDataset)workspace).FullName;
            ds.Name = tableName;

            IName name = (IName)query;
            return (ITable)name.Open();
        }

        #endregion
    }
}