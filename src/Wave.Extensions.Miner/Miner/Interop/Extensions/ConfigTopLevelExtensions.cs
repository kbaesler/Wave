using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop.Extensions
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMConfigTopLevel" /> interface.
    /// </summary>
    public static class ConfigTopLevelExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Changes the field visibility to the specified <paramref name="visible" /> value for all subtypes in the specified
        ///     <paramref name="table" /> object class
        ///     that match the field names.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="table">The object class.</param>
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        public static void ChangeVisibility(this IMMConfigTopLevel source, IObjectClass table, bool visible, params string[] fieldNames)
        {
            if (source == null) return;
            if (fieldNames == null) throw new ArgumentNullException("fieldNames");

            ISubtypes subtypes = (ISubtypes) table;
            foreach (var subtypeCode in subtypes.Subtypes.AsEnumerable().Select(o => o.Key))
            {
                source.ChangeVisibility(table, subtypeCode, visible, fieldNames);
            }
        }

        /// <summary>
        ///     Changes the field visibility to the specified <paramref name="visible" /> value for the subtype in the specified
        ///     <paramref name="table" /> object class
        ///     that match the field names.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="table">The object class.</param>
        /// <param name="subtypeCode">The subtype code.</param>
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <param name="fieldNames">The field names.</param>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        public static void ChangeVisibility(this IMMConfigTopLevel source, IObjectClass table, int subtypeCode, bool visible, params string[] fieldNames)
        {
            if (source == null) return;
            if (fieldNames == null) throw new ArgumentNullException("fieldNames");

            IMMSubtype subtype = source.GetSubtypeByID(table, subtypeCode, false);
            foreach (var fieldName in fieldNames)
            {
                subtype.ChangeVisibility(fieldName, visible);
            }
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified
        ///     <paramref name="editEvent" /> for all subtypes
        ///     of the <see cref="table" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="table">The object representing the specific subtype being analyzed.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{Key, Value}" /> representing the automatic values for the specified event and
        ///     object class.
        /// </returns>
        public static Dictionary<int, List<IMMAutoValue>> GetAutoValues(this IMMConfigTopLevel source, IObjectClass table, mmEditEvent editEvent)
        {
            Dictionary<int, List<IMMAutoValue>> list = new Dictionary<int, List<IMMAutoValue>>();

            ISubtypes subtypes = (ISubtypes) table;
            foreach (var subtypeCode in subtypes.Subtypes.AsEnumerable().Select(o => o.Key))
            {
                IMMSubtype subtype = source.GetSubtypeByID(table, subtypeCode, false);
                if (subtype == null) continue;

                var autoValues = subtype.GetAutoValues(table, editEvent);
                list.Add(subtypeCode, autoValues);
            }

            return list;
        }

        /// <summary>
        ///     Get the value of ArcFM Primary Display field.
        /// </summary>
        /// <param name="source">An Instance of IMMConfigTopLevel.</param>
        /// <param name="table">The object class.</param>
        /// <returns>
        ///     The value of the ArcFM Primary Display field.
        ///     If the value is null the object ID will be returned surrounded with parentheses.
        /// </returns>
        public static IField GetPrimaryDisplayField(this IMMConfigTopLevel source, IObjectClass table)
        {
            if (source == null) return null;
            if (table == null) return null;

            var featureClass = source.GetFeatureClassOnly(table);
            if (featureClass == null) return null;

            int index = table.Fields.FindField(featureClass.PriDisplayField);
            if (index == -1) return null;

            IField field = table.Fields.Field[index];
            return field;
        }

        #endregion
    }
}