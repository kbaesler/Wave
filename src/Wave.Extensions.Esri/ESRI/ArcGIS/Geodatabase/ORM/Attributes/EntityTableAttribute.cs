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

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityTableAttribute" /> class.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        public EntityTableAttribute(string schemaName, string tableName)
            : this(tableName)
        {
            this.Schema = schemaName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        /// <value>
        ///     The full name.
        /// </value>
        public string FullName
        {
            get { return string.IsNullOrEmpty(this.Schema) ? this.Name : string.Format("{0}.{1}", this.Schema, this.Name); }
        }

        /// <summary>
        ///     Gets or sets the name of the table.
        /// </summary>
        /// <value>
        ///     The name of the table.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the schema.
        /// </summary>
        /// <value>
        ///     The schema.
        /// </value>
        public string Schema { get; set; }

        #endregion
    }
}