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
        public EntityFieldAttribute(string fieldName, int length)
            : this(fieldName)
        {
            this.Length = length;
        }

        #endregion

        #region Public Properties

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