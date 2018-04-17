using System.IO;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
    /// <summary>
    ///     A supporting class used to validate an XML file against either a document type definition
    ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
    /// </summary>
    public sealed class XmlValidation
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlValidation" /> class.
        /// </summary>
        /// <param name="schemaUri">
        ///     The schema URI which can be either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </param>
        /// <param name="validationType">Type of the validation.</param>
        /// <param name="eventHandler">
        ///     The event handler for receiving information about document type definition (DTD), XML-Data
        ///     Reduced (XDR) schema, and XML Schema definition language (XSD) schema validation errors.
        /// </param>
        public XmlValidation(Uri schemaUri, ValidationType validationType, ValidationEventHandler eventHandler)
            : this(schemaUri, null, validationType, eventHandler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlValidation" /> class.
        /// </summary>
        /// <param name="schemaUri">
        ///     The schema URI which can be either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="validationType">Type of the validation.</param>
        /// <param name="eventHandler">
        ///     The event handler for receiving information about document type definition (DTD), XML-Data
        ///     Reduced (XDR) schema, and XML Schema definition language (XSD) schema validation errors.
        /// </param>
        public XmlValidation(Uri schemaUri, string targetNamespace, ValidationType validationType, ValidationEventHandler eventHandler)
            : this(new XmlTextReader(schemaUri.AbsolutePath), targetNamespace, validationType, eventHandler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlValidation" /> class.
        /// </summary>
        /// <param name="schemaDocument">
        ///     The schema document which can be either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </param>
        /// <param name="validationType">Type of the validation.</param>
        /// <param name="eventHandler">
        ///     The event handler for receiving information about document type definition (DTD), XML-Data
        ///     Reduced (XDR) schema, and XML Schema definition language (XSD) schema validation errors.
        /// </param>
        public XmlValidation(XmlReader schemaDocument, ValidationType validationType, ValidationEventHandler eventHandler)
            : this(schemaDocument, "", validationType, eventHandler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlValidation" /> class.
        /// </summary>
        /// <param name="schemaDocument">
        ///     The schema document which can be either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="validationType">Type of the validation.</param>
        /// <param name="eventHandler">
        ///     The event handler for receiving information about document type definition (DTD), XML-Data
        ///     Reduced (XDR) schema, and XML Schema definition language (XSD) schema validation errors.
        /// </param>
        public XmlValidation(XmlReader schemaDocument, string targetNamespace, ValidationType validationType, ValidationEventHandler eventHandler)
        {
            this.SchemaDocument = schemaDocument;
            this.TargetNamespace = targetNamespace;
            this.ValidationType = validationType;
            this.EventHandler = eventHandler;
        }

        #endregion

        #region Events

        /// <summary>
        ///     The event handler for receiving information about document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, and XML Schema definition language
        ///     (XSD) schema validation errors.
        /// </summary>
        private event ValidationEventHandler EventHandler;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the schema document.
        /// </summary>
        /// <value>
        ///     The schema document.
        /// </value>
        public XmlReader SchemaDocument { get; private set; }

        /// <summary>
        ///     Gets the target namespace.
        /// </summary>
        /// <value>The target namespace.</value>
        public string TargetNamespace { get; private set; }

        /// <summary>
        ///     Gets the type of the validation.
        /// </summary>
        /// <value>The type of the validation.</value>
        public ValidationType ValidationType { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Validates the specified document with either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="navigable">The navigable.</param>
        public void Validate(IXPathNavigable navigable)
        {
            XPathNavigator nav = navigable.CreateNavigator();
            this.Validate(nav.OuterXml, new XmlReaderSettings());
        }

        /// <summary>
        ///     Validates the specified XML fragment with either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="xmlFragment">The XML fragment.</param>
        public void Validate(string xmlFragment)
        {
            this.Validate(xmlFragment, new XmlReaderSettings());
        }

        /// <summary>
        ///     Validates the specified XML fragment with either a document type definition
        ///     (DTD), XML-Data Reduced (XDR) schema, or XML Schema definition language (XSD) schema validation file.
        /// </summary>
        /// <param name="xmlFragment">The XML fragment.</param>
        /// <param name="settings">The settings.</param>
        public void Validate(string xmlFragment, XmlReaderSettings settings)
        {
            // Read the XSD into the schema set.
            XmlSchemaSet xss = new XmlSchemaSet();
            xss.Add(this.TargetNamespace, this.SchemaDocument);

            // Set the validation settings.
            settings.Schemas.Add(xss);
            settings.ValidationType = this.ValidationType;
            settings.ValidationEventHandler += this.EventHandler;

            // Load the Xml fragment into the stream.
            using (StringReader sr = new StringReader(xmlFragment))
            {
                // Load the XML reader and perform the validation.
                XmlTextReader xtr = new XmlTextReader(sr);
                XmlReader xr = XmlReader.Create(xtr, settings);
                
                while (xr.Read())
                {
                   // Read each line which will compare it against the XSD schema.
                }
            }
        }

        #endregion
    }
}