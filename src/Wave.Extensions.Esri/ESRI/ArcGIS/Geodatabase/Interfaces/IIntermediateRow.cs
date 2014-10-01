using System.Collections.Generic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides the methods and properties for accessing the intermediate row information.
    /// </summary>
    public interface IIntermediateRow
    {
        #region Public Properties

        /// <summary>
        ///     Gets the destination foreign key.
        /// </summary>
        string DestinationForeignKey { get; }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        ///     Gets the origin foreign key.
        /// </summary>
        string OriginForeignKey { get; }

        /// <summary>
        ///     Gets the physical intermediate row.
        /// </summary>
        IRow Row { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Updates the field with specified <paramref name="fieldName" /> with the <paramref name="value" />.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        void Update(string fieldName, object value);

        #endregion
    }
}