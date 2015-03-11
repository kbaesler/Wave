using System;
using System.Linq;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMFeatureClass" /> interface
    /// </summary>
    public static class FeatureClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Changes the field visibility to the specified <paramref name="visible" /> value for the fields
        ///     that match the field name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <exception cref="System.NullReferenceException">
        ///     source
        ///     or
        ///     fieldName
        /// </exception>
        public static void ChangeVisibility(this IMMFeatureClass source, string fieldName, bool visible)
        {
            if (source == null) throw new NullReferenceException("source");
            if (fieldName == null) throw new NullReferenceException("fieldName");

            ID8List list = source as ID8List;
            if (list == null) return;

            IMMSubtype all = source.GetSubtype(ConfigTopLevelExtensions.ALL_SUBTYPES);
            if (all != null)
            {
                all.ChangeVisibility(fieldName, visible);
            }

            foreach (var subtype in list.AsEnumerable().OfType<IMMSubtype>())
            {
                subtype.ChangeVisibility(fieldName, visible);
            }
        }

        #endregion
    }
}