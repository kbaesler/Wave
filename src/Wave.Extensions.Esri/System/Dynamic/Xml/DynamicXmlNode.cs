using System.Xml;
using System.Xml.Linq;

namespace System.Dynamic
{
    /// <summary>
    ///     Provides a dynamic version of the <see cref="XmlNode" />
    /// </summary>
    /// <seealso cref="System.Dynamic.DynamicObject" />
    public sealed class DynamicXmlNode : DynamicObject
    {
        #region Fields

        private readonly XElement _Xml;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicXmlNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DynamicXmlNode(string name)
            : this(new XElement(name))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicXmlNode" /> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        internal DynamicXmlNode(XElement xml)
        {
            _Xml = xml;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicXmlNode" /> class.
        /// </summary>
        /// <param name="xdoc">The xdoc.</param>
        internal DynamicXmlNode(XDocument xdoc)
            : this(xdoc.Root)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _Xml.ToString();
        }

        /// <summary>
        ///     Provides the implementation for operations that get member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member on which the dynamic operation is performed. For example, for the
        ///     Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived
        ///     from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The
        ///     binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        ///     The result of the get operation. For example, if the method is called for a property, you can
        ///     assign the property value to <paramref name="result" />.
        /// </param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            switch (name)
            {
                case "Name":
                    result = _Xml.Name.LocalName;
                    return true;

                case "Value":
                    result = _Xml.Value;
                    return true;

                case "Elements":

                    result = new DynamicXmlNodeList(_Xml.Elements());
                    return true;

                case "Parent":

                    result = new DynamicXmlNode(_Xml.Parent);
                    return true;

                default:

                    XAttribute xattribute = _Xml.Attribute(name);
                    if (xattribute != null)
                    {
                        result = xattribute.Value;
                        return true;
                    }

                    XElement xelement = _Xml.Element(name);
                    if (xelement != null)
                    {
                        result = xelement.Value;
                        return true;
                    }

                    foreach (XElement element in _Xml.Elements())
                    {
                        if (MakePluralName(element.Name.LocalName) == name)
                        {
                            result = new DynamicXmlNodeList(_Xml.Elements(element.Name));
                            return true;
                        }
                    }

                    return base.TryGetMember(binder, out result);
            }
        }

        /// <summary>
        ///     Provides the implementation for operations that invoke a member. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as calling a method.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the dynamic operation. The binder.Name property provides the name of
        ///     the member on which the dynamic operation is performed. For example, for the statement
        ///     sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase
        ///     property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="args">
        ///     The arguments that are passed to the object member during the invoke operation. For example, for the
        ///     statement sampleObject.SampleMethod(100), where sampleObject is derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[0]" /> is equal to 100.
        /// </param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name != "SelectAll")
                return base.TryInvokeMember(binder, args, out result);

            result = new DynamicXmlNodeList(_Xml.Elements());

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the specified character is vowel.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the character is a vowel.</returns>
        private bool IsVowel(char c)
        {
            switch (c)
            {
                case 'o':
                case 'u':
                case 'y':
                case 'a':
                case 'e':
                case 'i':
                case 'O':
                case 'U':
                case 'Y':
                case 'A':
                case 'E':
                case 'I':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Makes the name of the plural.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string MakePluralName(string name)
        {
            if (name.EndsWith("x", StringComparison.OrdinalIgnoreCase) || name.EndsWith("ch", StringComparison.OrdinalIgnoreCase) || (name.EndsWith("ss", StringComparison.OrdinalIgnoreCase) || name.EndsWith("sh", StringComparison.OrdinalIgnoreCase)))
            {
                name += "es";
                return name;
            }
            if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase) && name.Length > 1 && !IsVowel(name[name.Length - 2]))
            {
                name = name.Remove(name.Length - 1, 1);
                name += "ies";
                return name;
            }
            if (!name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                name += "s";
            return name;
        }

        #endregion
    }
}