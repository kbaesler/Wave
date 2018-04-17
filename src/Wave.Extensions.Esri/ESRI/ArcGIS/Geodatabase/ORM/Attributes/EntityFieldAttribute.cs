using System;
using System.Collections.ObjectModel;

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
            this.Name = fieldName;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityFieldAttribute" /> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        public EntityFieldAttribute(string fieldName, Type sourceType, Type destinationType)
            : this(fieldName)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type of the destination.
        /// </summary>
        /// <value>
        ///     The type of the destination.
        /// </value>
        public Type DestinationType { get; private set; }

        /// <summary>
        ///     The name of the database field represented by this property.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        ///     Gets the type of the source.
        /// </summary>
        /// <value>
        ///     The type of the source.
        /// </value>
        public Type SourceType { get; private set; }

        #endregion
    }

}