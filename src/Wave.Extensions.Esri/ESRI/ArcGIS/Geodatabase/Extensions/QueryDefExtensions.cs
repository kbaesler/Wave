using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.Geodatabase.Extensions
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IQueryDef" /> interface.
    /// </summary>
    public static class QueryDefExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the number of records affected by executing the query definition.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of records affected.
        /// </returns>
        public static int Count(this IQueryDef source)
        {
            string subFields = source.SubFields;

            try
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    source.SubFields = "COUNT(*)";

                    ICursor cursor = source.Evaluate();
                    cr.ManageLifetime(cursor);

                    IRow row = cursor.NextRow();
                    if (row != null) return row.GetValue(0, 0);
                }

                return 0;
            }
            finally
            {
                source.SubFields = subFields;
            }
        }

        #endregion
    }
}