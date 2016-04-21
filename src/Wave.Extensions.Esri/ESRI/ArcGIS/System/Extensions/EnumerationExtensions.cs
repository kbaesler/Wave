namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     Provides extension methods for the <see cref="esriUnits" /> enumeration.
    /// </summary>
    public static class EnumerationExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the specified numeric data value from one unit of measure to another unit of measure.
        /// </summary>
        /// <param name="source">The source units of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="target">The target units that that the source units should be converted to.</param>
        /// <returns>Returns a <see cref="double" /> representing the converted units.</returns>
        public static double ConvertTo(this esriUnits source, double value, esriUnits target)
        {
            IUnitConverter converter = new UnitConverterClass();
            return converter.ConvertUnits(value, source, target);
        }

        /// <summary>
        ///     Convert ESRI unit enumerations to lowercase strings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="plural">
        ///     if set to <c>true</c> the
        ///     string specifies a many unit(s) otherwise single unit.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents the enumeration as a string.
        /// </returns>
        public static string ToLower(this esriUnits source, bool plural)
        {
            return source.ToString(esriCaseAppearance.esriCaseAppearanceLower, plural);
        }

        /// <summary>
        ///     Convert ESRI unit enumerations to strings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="plural">
        ///     if set to <c>true</c> the
        ///     string specifies a many unit(s) otherwise single unit.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents the enumeration as a string.
        /// </returns>
        public static string ToString(this esriUnits source, bool plural)
        {
            return source.ToString(esriCaseAppearance.esriCaseAppearanceUnchanged, plural);
        }

        /// <summary>
        ///     Convert ESRI unit enumerations to strings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="appearance">The appearance to specify the case (eg. lower or upper case) of the string.</param>
        /// <param name="plural">
        ///     if set to <c>true</c> the
        ///     string specifies a many unit(s) otherwise single unit.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents the enumeration as a string.
        /// </returns>
        public static string ToString(this esriUnits source, esriCaseAppearance appearance, bool plural)
        {
            IUnitConverter converter = new UnitConverterClass();
            return converter.EsriUnitsAsString(source, appearance, plural);
        }

        /// <summary>
        ///     Convert ESRI unit enumerations to uppercase strings.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="plural">
        ///     if set to <c>true</c> the
        ///     string specifies a many unit(s) otherwise single unit.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents the enumeration as a string.
        /// </returns>
        public static string ToUpper(this esriUnits source, bool plural)
        {
            return source.ToString(esriCaseAppearance.esriCaseAppearanceUpper, plural);
        }

        #endregion
    }
}