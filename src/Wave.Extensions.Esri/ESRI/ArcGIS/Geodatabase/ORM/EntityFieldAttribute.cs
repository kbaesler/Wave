using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides a custom attribute for specifying the source field name.  This is used to map a database field to a
    ///     property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityFieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public EntityFieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityFieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="length">The length.</param>
        public EntityFieldAttribute(string fieldName, int? length)
            : this(fieldName)
        {
            this.Length = length;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityFieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="length">The length.</param>
        /// <param name="dataType">Type of the data.</param>
        public EntityFieldAttribute(string fieldName, int? length, Type dataType)
            : this(fieldName, length)
        {
            this.DataType = dataType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type of the data.
        /// </summary>
        /// <value>
        ///     The type of the data.
        /// </value>
        public Type DataType { get; private set; }

        /// <summary>
        ///     The name of the database field represented by this property.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        ///     The length of the field.
        /// </summary>
        public int? Length { get; private set; }

        #endregion
    }
}