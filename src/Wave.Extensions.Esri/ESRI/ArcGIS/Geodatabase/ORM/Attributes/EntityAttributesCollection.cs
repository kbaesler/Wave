using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ESRI.ArcGIS.Geodatabase
{
    class EntityAttributesCollection
    {
        #region Fields

        private readonly Dictionary<string, string> _Aliases = new Dictionary<string, string>();
        private readonly Dictionary<string, EntityFieldAttribute> _Attributes = new Dictionary<string, EntityFieldAttribute>();
        private readonly List<PropertyInfo> _Dependencies = new List<PropertyInfo>();
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
                        _Aliases.Add(string.Format("{0}.{1}", table.Name, field.Name), field.Name);
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
            var attribute = this.GetAttribute(fieldName);
            if (attribute != null && attribute.DestinationType != null && attribute.SourceType != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(value);
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
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> GetFields(IRow row)
        {
            var fields = new Dictionary<string, PropertyInfo>();

            foreach (var f in row.Fields.AsEnumerable())
            {
                var p = this.GetProperty(f.Name);
                if (p == null) continue;

                if (!fields.ContainsKey(f.Name))
                    fields.Add(f.Name, p);
            }

            return fields;
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

        #endregion
    }
}