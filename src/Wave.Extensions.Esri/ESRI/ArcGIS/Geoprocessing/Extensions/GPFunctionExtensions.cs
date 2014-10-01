using System;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGPFunction" /> interface.
    /// </summary>
    public static class GPFunctionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates the composite parameter that allows the input values to be of different data types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataTypes">The data types.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateCompositeParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, params IGPDataType[] dataTypes)
        {
            var compositeDataTypeClass = new GPCompositeDataTypeClass();
            foreach (var dataType in dataTypes)
                compositeDataTypeClass.AddDataType(dataType);

            return new GPParameterClass
            {
                IGPParameterEdit3_DataType = compositeDataTypeClass,
                IGPParameterEdit3_DisplayName = displayName,
                IGPParameterEdit3_Name = name,
                IGPParameterEdit3_ParameterType = parameterType,
                IGPParameterEdit2_Value = new GPTableViewClass()
            };
        }

        /// <summary>
        ///     Creates the composite parameter that allows the input values to be of different data types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="category">
        ///     The category for the parameter in the tool dialog. Parameters that belong to the same category
        ///     are listed in a text box that is expandable and collapsible in the tool dialog box.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataTypes">The data types.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateCompositeParameter(this IGPFunction source, string name, string displayName, string category, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, params IGPDataType[] dataTypes)
        {
            var parameter = source.CreateCompositeParameter(name, displayName, parameterType, parameterDirection, dataTypes);
            parameter.Category = category;

            return parameter;
        }

        /// <summary>
        ///     Creates a parameter that supports multiple inputs of the same data type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="category">
        ///     The category for the parameter in the tool dialog. Parameters that belong to the same category
        ///     are listed in a text box that is expandable and collapsible in the tool dialog box.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataTypes">The data types.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateMultiValueParameter(this IGPFunction source, string name, string displayName, string category, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, params IGPDataType[] dataTypes)
        {
            var parameter = source.CreateMultiValueParameter(name, displayName, parameterType, parameterDirection, dataTypes);
            parameter.Category = category;

            return parameter;
        }

        /// <summary>
        ///     Creates a parameter that supports multiple inputs of the same data type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataTypes">The data types.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateMultiValueParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, params IGPDataType[] dataTypes)
        {
            var compositeDataTypeClass = new GPCompositeDataTypeClass();
            foreach (var dataType in dataTypes)
                compositeDataTypeClass.AddDataType(dataType);

            var multiValueType = new GPMultiValueTypeClass();
            multiValueType.MemberDataType = compositeDataTypeClass;

            return new GPParameterClass
            {
                IGPParameterEdit3_DataType = multiValueType,
                IGPParameterEdit3_DisplayName = displayName,
                IGPParameterEdit3_Name = name,
                IGPParameterEdit3_ParameterType = parameterType,
                IGPParameterEdit3_Direction = parameterDirection
            };
        }

        /// <summary>
        ///     Creates a parameter that supports multiple inputs of the same data type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="category">
        ///     The category for the parameter in the tool dialog. Parameters that belong to the same category
        ///     are listed in a text box that is expandable and collapsible in the tool dialog box.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="checkBoxes">if set to <c>true</c> if the ActiveX control should be checkboxes.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateMultiValueParameter(this IGPFunction source, string name, string displayName, string category, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, IGPDataType dataType, bool checkBoxes = false)
        {
            var parameter = source.CreateMultiValueParameter(name, displayName, parameterType, parameterDirection, dataType, checkBoxes);
            parameter.Category = category;

            return parameter;
        }

        /// <summary>
        ///     Creates a parameter that supports multiple inputs of the same data type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="checkBoxes">
        ///     if set to <c>true</c> if the values should be displayed in a list box with check boxes (the
        ///     values are driven by the Domain).
        /// </param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateMultiValueParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, IGPDataType dataType, bool checkBoxes = false)
        {
            var multiValueType = new GPMultiValueTypeClass();
            multiValueType.MemberDataType = dataType;

            IGPParameterEdit3 parameter = new GPParameterClass
            {
                IGPParameterEdit3_DataType = multiValueType,
                IGPParameterEdit3_DisplayName = displayName,
                IGPParameterEdit3_Name = name,
                IGPParameterEdit3_ParameterType = parameterType,
                IGPParameterEdit3_Direction = parameterDirection,
                IGPParameterEdit3_ControlCLSID = (checkBoxes) ? new UIDClass
                {
                    Value = "{38C34610-C7F7-11D5-A693-0008C711C8C1}"
                } : null
            };

            return parameter;
        }


        /// <summary>
        ///     Creates a parameter that supports multiple inputs of the same data type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="control">The ActiveX control should that should be used to display the contents.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateMultiValueParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, IGPDataType dataType, UID control)
        {
            var multiValueType = new GPMultiValueTypeClass();
            multiValueType.MemberDataType = dataType;

            IGPParameterEdit3 parameter = new GPParameterClass
            {
                IGPParameterEdit3_DataType = multiValueType,
                IGPParameterEdit3_DisplayName = displayName,
                IGPParameterEdit3_Name = name,
                IGPParameterEdit3_ParameterType = parameterType,
                IGPParameterEdit3_Direction = parameterDirection,
                IGPParameterEdit3_ControlCLSID = control
            };

            return parameter;
        }

        /// <summary>
        ///     Creates the a coded value domain an assigns it to the parameter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="values">The values (strings) for the domain.</param>
        /// <param name="names">The names (strings) for the domain.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     values
        ///     or
        ///     names
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     values;The values and names arrays must contain the same number of
        ///     elements.
        /// </exception>
        public static IGPParameterEdit3 CreateParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, object[] values, object[] names, IGPDataType dataType)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (names == null)
                throw new ArgumentNullException("names");

            if (values.Length != names.Length)
                throw new ArgumentOutOfRangeException("values", "The values and names arrays must contain the same number of elements.");

            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();
            for (int i = 0; i < values.Length - 1; i++)
                codedValueDomain.AddStringCode(values[i].ToString(), names[i].ToString());

            var parameter = source.CreateParameter(name, displayName, parameterType, parameterDirection, dataType);
            parameter.Domain = codedValueDomain as IGPDomain;
            return parameter;
        }

        /// <summary>
        ///     Creates a simple parameter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateParameter(this IGPFunction source, string name, string displayName, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, IGPDataType dataType)
        {
            return new GPParameterClass
            {
                IGPParameterEdit3_DataType = dataType,
                IGPParameterEdit3_DisplayName = displayName,
                IGPParameterEdit3_Name = name,
                IGPParameterEdit3_ParameterType = parameterType,
                IGPParameterEdit3_Direction = parameterDirection
            };
        }

        /// <summary>
        ///     Creates a simple parameter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">
        ///     The name is the language-independent name for the parameter (not localized) and must not contain
        ///     spaces and must be unique within a function.
        /// </param>
        /// <param name="displayName">
        ///     The display name is the localized name (as it appears in the dialog) and is contained in
        ///     resource string.
        /// </param>
        /// <param name="category">
        ///     The category for the parameter in the tool dialog. Parameters that belong to the same category
        ///     are listed in a text box that is expandable and collapsible in the tool dialog box.
        /// </param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>
        ///     Returns a <see cref="IGPParameterEdit3" /> representing the parameter.
        /// </returns>
        public static IGPParameterEdit3 CreateParameter(this IGPFunction source, string name, string displayName, string category, esriGPParameterType parameterType, esriGPParameterDirection parameterDirection, IGPDataType dataType)
        {
            var parameter = source.CreateParameter(name, displayName, parameterType, parameterDirection, dataType);
            parameter.Category = category;
            return parameter;
        }

        #endregion
    }
}