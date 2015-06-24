using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace System.Forms.ComponentModel
{
    /// <summary>
    ///     Provides the means for supporting XML with the <see cref="System.Windows.Forms.PropertyGrid" /> control.
    /// </summary>
    [TypeConverter(typeof (XmlNodeWrapperConverter))]
    public class XmlNodeWrapper
    {
        #region Fields

        private readonly XmlNode _Node;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlNodeWrapper" /> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public XmlNodeWrapper(XmlNode node)
        {
            _Node = node;
        }

        #endregion

        #region Nested Type: XmlNodeWrapperConverter

        /// <summary>
        ///     Provides a type converter to convert expandable objects to and from various an XmlNode.
        /// </summary>
        private class XmlNodeWrapperConverter : ExpandableObjectConverter
        {
            #region Public Methods

            /// <summary>
            ///     Converts the given value object to the specified type, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
            /// <param name="culture">
            ///     A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is
            ///     assumed.
            /// </param>
            /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
            /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
            /// <returns>
            ///     An <see cref="T:System.Object" /> that represents the converted value.
            /// </returns>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return destinationType == typeof (string)
                    ? ((XmlNodeWrapper) value)._Node.InnerXml
                    : base.ConvertTo(context, culture, value, destinationType);
            }

            /// <summary>
            ///     Gets a collection of properties for the type of object specified by the value parameter.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
            /// <param name="value">An <see cref="T:System.Object" /> that specifies the type of object to get the properties for.</param>
            /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that will be used as a filter.</param>
            /// <returns>
            ///     A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties that are exposed for the
            ///     component, or null if there are no properties.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                List<PropertyDescriptor> props = new List<PropertyDescriptor>();
                XmlNodeWrapper wrapper = (XmlNodeWrapper) value;

                if (wrapper != null)
                {
                    XmlElement el = wrapper._Node as XmlElement;
                    if (el != null)
                    {
                        foreach (XmlAttribute attr in el.Attributes)
                        {
                            props.Add(new XmlNodeWrapperPropertyDescriptor(attr));
                        }
                    }

                    foreach (XmlNode child in wrapper._Node.ChildNodes)
                    {
                        props.Add(new XmlNodeWrapperPropertyDescriptor(child));
                    }
                }

                return new PropertyDescriptorCollection(props.ToArray(), true);
            }

            #endregion
        }

        #endregion

        #region Nested Type: XmlNodeWrapperPropertyDescriptor

        /// <summary>
        ///     The property descriptor for the XmlNode.
        /// </summary>
        private class XmlNodeWrapperPropertyDescriptor : PropertyDescriptor
        {
            #region Fields

            private static readonly Attribute[] _Attributes = new Attribute[0];

            private readonly XmlNode _Node;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="XmlNodeWrapperPropertyDescriptor" /> class.
            /// </summary>
            /// <param name="node">The node.</param>
            public XmlNodeWrapperPropertyDescriptor(XmlNode node)
                : base(GetName(node), _Attributes)
            {
                this._Node = node;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the type of the component.
            /// </summary>
            /// <value>
            ///     The type of the component.
            /// </value>
            public override Type ComponentType
            {
                get { return typeof (XmlNodeWrapper); }
            }

            /// <summary>
            ///     Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
            /// </value>
            public override bool IsReadOnly
            {
                get
                {
                    switch (_Node.NodeType)
                    {
                        case XmlNodeType.Attribute:
                        case XmlNodeType.Text:
                            return false;
                        default:
                            return true;
                    }
                }
            }

            /// <summary>
            ///     Gets the type of the property.
            /// </summary>
            /// <value>
            ///     The type of the property.
            /// </value>
            public override Type PropertyType
            {
                get
                {
                    switch (_Node.NodeType)
                    {
                        case XmlNodeType.Element:
                            return typeof (XmlNodeWrapper);
                        default:
                            return typeof (string);
                    }
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///     Determines whether this instance [can reset value] the specified component.
            /// </summary>
            /// <param name="component">The component.</param>
            /// <returns>
            ///     <c>true</c> if this instance [can reset value] the specified component; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return !IsReadOnly;
            }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            /// <param name="component">The component.</param>
            /// <returns></returns>
            public override object GetValue(object component)
            {
                switch (_Node.NodeType)
                {
                    case XmlNodeType.Element:
                        return new XmlNodeWrapper(_Node);
                    default:
                        return _Node.Value;
                }
            }

            /// <summary>
            ///     Resets the value.
            /// </summary>
            /// <param name="component">The component.</param>
            public override void ResetValue(object component)
            {
                SetValue(component, "");
            }

            /// <summary>
            ///     Sets the value.
            /// </summary>
            /// <param name="component">The component.</param>
            /// <param name="value">The value.</param>
            public override void SetValue(object component, object value)
            {
                _Node.Value = (string) value;
            }

            /// <summary>
            ///     Shoulds the serialize value.
            /// </summary>
            /// <param name="component">The component.</param>
            /// <returns></returns>
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            #endregion

            #region Private Methods

            /// <summary>
            ///     Gets the name.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns></returns>
            private static string GetName(XmlNode node)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        return "@" + node.Name;
                    case XmlNodeType.Element:
                        return node.Name;
                    case XmlNodeType.Comment:
                        return "<!-- -->";
                    case XmlNodeType.Text:
                        return "(text)";
                    default:
                        return node.NodeType + ":" + node.Name;
                }
            }

            #endregion
        }

        #endregion
    }
}