using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides a custom attribute for specifying the source table.  This is used to map a database table to a
    ///     class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityTableAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityTableAttribute" /> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public EntityTableAttribute(string tableName)
        {
            this.Name = tableName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the table.
        /// </summary>
        /// <value>
        ///     The name of the table.
        /// </value>
        public string Name { get; set; }

        #endregion
    }
}