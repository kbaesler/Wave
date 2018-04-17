using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides property refelection for the entity object.
    /// </summary>
    class EntityAttributesCollection
    {
        #region Fields

        private readonly Dictionary<string, string> _Aliases = new Dictionary<string, string>();
        private readonly Dictionary<string, EntityFieldAttribute> _Attributes = new Dictionary<string, EntityFieldAttribute>();
        private readonly List<PropertyInfo> _Dependencies = new List<PropertyInfo>();
        private readonly ConcurrentDictionary<string, EntityAttributeProperty> _Fields = new ConcurrentDictionary<string, EntityAttributeProperty>();
        private readonly Dictionary<string, PropertyInfo> _Properties = new Dictionary<string, PropertyInfo>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityAttributesCollection" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public EntityAttributesCollection(Type type)
        {
            var table = type.GetCustomAttributes(typeof(EntityTableAttribute), true).OfType<EntityTableAttribute>().SingleOrDefault();

            foreach (var prop in type.GetProperties())
            {
                var field = Attribute.GetCustomAttributes(prop).OfType<EntityFieldAttribute>().SingleOrDefault();
                if (field != null)
                {
                    _Attributes.Add(field.Name, field);
                    _Properties.Add(field.Name, prop);

                    if (table != null)
                    {
                        _Aliases.Add(string.Format("{0}.{1}", table.FullName, field.Name), field.Name);
                    }
                }
                else if (typeof(Entity).IsAssignableFrom(prop.PropertyType))
                {
                    _Dependencies.Add(prop);
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            get { return _Attributes.Count; }
        }

        /// <summary>
        ///     Gets the dependencies.
        /// </summary>
        /// <value>
        ///     The dependencies.
        /// </value>
        public IEnumerable<PropertyInfo> Dependencies
        {
            get { return _Dependencies; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts to given object using the conversion types.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public object Convert(string fieldName, object value)
        {
            if (value == null || value == DBNull.Value)
                return value;

            var attribute = this.GetAttribute(fieldName);
            if (attribute != null && attribute.DestinationType != null && attribute.SourceType != null)
            {
                var prop = this.GetProperty(fieldName);
                if (prop != null)
                {
                    foreach (TypeConverter c in this.GetCustomTypeConverters(prop))
                    {
                        if (c.CanConvertTo(attribute.DestinationType))
                            return c.ConvertTo(value, attribute.DestinationType);
                    }
                }

                TypeConverter converter = TypeDescriptor.GetConverter(attribute.SourceType);
                if (converter.CanConvertTo(attribute.DestinationType))
                {
                    return converter.ConvertTo(value, attribute.DestinationType);
                }

                converter = TypeDescriptor.GetConverter(attribute.DestinationType);
                if (converter.CanConvertFrom(attribute.SourceType))
                {
                    return converter.ConvertFrom(value);
                }
            }

            return value;
        }


        /// <summary>
        ///     Gets the fields.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>Returns a <see cref="IDictionary{TKey,TValue}" /> representing the fields and property pairs.</returns>
        public IDictionary<string, EntityAttributeProperty> GetFields(IRow row)
        {
            if (_Fields == null || !_Fields.Any())
            {
                if (row == null) return _Fields;

                foreach (var field in row.Fields.AsEnumerable())
                {
                    var p = this.GetProperty(field.Name);
                    if (p == null) continue;

                    _Fields.TryAdd(field.Name, new EntityAttributeProperty {Property = p, Editable = field.Editable});
                }
            }

            return _Fields;
        }

        /// <summary>
        ///     Gets the property.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public PropertyInfo GetProperty(string fieldName)
        {
            if (_Properties.ContainsKey(fieldName))
                return _Properties[fieldName];

            if (_Aliases.ContainsKey(fieldName))
                return this.GetProperty(_Aliases[fieldName]);

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the attribute.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        private EntityFieldAttribute GetAttribute(string fieldName)
        {
            if (_Attributes.ContainsKey(fieldName))
                return _Attributes[fieldName];

            if (_Aliases.ContainsKey(fieldName))
                return this.GetAttribute(_Aliases[fieldName]);

            return null;
        }

        /// <summary>
        ///     Extracts and instantiates any customer type converters assigned to a
        ///     derivitive of the <see cref="System.Reflection.MemberInfo" /> property
        /// </summary>
        /// <param name="member">Any class deriving from MemberInfo</param>
        /// <returns>A list of customer type converters, empty if none found</returns>
        private List<TypeConverter> GetCustomTypeConverters(MemberInfo member)
        {
            List<TypeConverter> result = new List<TypeConverter>();

            try
            {
                foreach (TypeConverterAttribute a in member.GetCustomAttributes(typeof(TypeConverterAttribute), true))
                {
                    TypeConverter converter = Activator.CreateInstance(Type.GetType(a.ConverterTypeName)) as TypeConverter;

                    if (converter != null)
                        result.Add(converter);
                }
            }
            catch
            {
                // Let it go, there were no custom converters
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    class EntityAttributeProperty
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance can read.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance can read; otherwise, <c>false</c>.
        /// </value>
        public bool CanRead => Property.CanRead;

        /// <summary>
        ///     Gets a value indicating whether this instance can write.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance can write; otherwise, <c>false</c>.
        /// </value>
        public bool CanWrite => Property.CanWrite;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="EntityAttributeProperty" /> is editable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if editable; otherwise, <c>false</c>.
        /// </value>
        public bool Editable { get; set; }

        /// <summary>
        ///     Gets or sets the property.
        /// </summary>
        /// <value>
        ///     The property.
        /// </value>
        public PropertyInfo Property { get; set; }

        #endregion
    }
}