using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMSubtype" /> interface.
    /// </summary>
    public static class SubtypeExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the automatic value (i.e ArcFM Auto Updater) to the subtype source for the specified event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IMMAutoValue" /> representing the value that was added; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">guid</exception>
        public static IMMAutoValue AddAutoValue(this IMMSubtype source, mmEditEvent editEvent, Guid guid)
        {
            if (source == null) return null;
            if (guid == Guid.Empty) throw new ArgumentNullException("guid");

            var list = source as ID8List;
            if (list == null) return null;

            var item = source.GetAutoValue(editEvent, guid);
            if (item != null)
                return item;

            UID uid = new UIDClass();
            uid.Value = guid.ToString("B");

            item = new MMAutoValueClass
            {
                AutoGenID = uid,
                EditEvent = editEvent
            };

            list.Add((ID8ListItem) item);

            return item;
        }

        /// <summary>
        ///     Changes the field visibility to the specified <paramref name="visible" /> value for the fields
        ///     that match the field name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        public static void ChangeVisibility(this IMMSubtype source, string fieldName, bool visible)
        {
            if (source == null) return;
            if (fieldName == null) throw new ArgumentNullException("fieldName");

            var list = source as ID8List;
            if (list == null) return;

            var field = list.AsEnumerable().OfType<IMMField>().FirstOrDefault(o => o.FieldName == fieldName);
            if (field == null) return;

            field.ChangeVisibility(visible);
        }

        /// <summary>
        ///     Gets the automatic value (i.e. ArcFM Auto Updater) for the specified <paramref name="editEvent" /> and
        ///     <paramref name="guid" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IMMAutoValue" /> representing the automatic value; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">guid</exception>
        public static IMMAutoValue GetAutoValue(this IMMSubtype source, mmEditEvent editEvent, Guid guid)
        {
            if (source == null) return null;
            if (guid == Guid.Empty) throw new ArgumentNullException("guid");

            ID8List list = source as ID8List;
            if (list == null) return null;

            var values = list.Where(i => i.ItemType == mmd8ItemType.mmitAutoValue, 0).Select(o => o.Value);            
            foreach (IMMAutoValue autoValue in values.OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent))
            {
                if (autoValue.AutoGenID.Value == null) continue;

                string autoGenID = autoValue.AutoGenID.Value.ToString();
                if (string.Equals(autoGenID, guid.ToString("B"), StringComparison.InvariantCultureIgnoreCase))
                    return autoValue;
            }

            return null;
        }

        /// <summary>
        /// Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the subtype.
        /// </summary>
        /// <param name="source">The relationship class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        /// Returns a <see cref="IEnumerable{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        public static IEnumerable<IMMAutoValue> GetAutoValues(this IMMRelationshipClass source, mmEditEvent editEvent)
        {
            if (source == null) return null;

            var list = source as ID8List;
            if (list == null) return null;

            var values = list.Where(i => i.ItemType == mmd8ItemType.mmitAutoValue, 0).Select(o => o.Value);
            return values.OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent);
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the subtype.
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        public static IEnumerable<IMMAutoValue> GetAutoValues(this IMMSubtype source, mmEditEvent editEvent)
        {
            if (source == null) return null;

            var list = source as ID8List;
            if (list == null) return null;

            var values = list.Where(i => i.ItemType == mmd8ItemType.mmitAutoValue, 0).Select(o => o.Value);
            return values.OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent);
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified
        ///     <paramref name="index" />
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public static IEnumerable<IMMAutoValue> GetAutoValues(this IMMSubtype source, int index, mmEditEvent editEvent)
        {
            if (source == null) return null;

            IMMField field = null;
            source.GetField(index, ref field);
            if (field == null) return null;

            var list = field as ID8List;
            if (list == null) return null;

            var values = list.Where(i => i.ItemType == mmd8ItemType.mmitAutoValue, 0).Select(o => o.Value);
            return values.OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent);
        }

        /// <summary>
        ///     Removes the automatic value (i.e ArcFM Auto Updater) to the subtype source for the specified event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the value was removed; otherwise <C>false</C>
        /// </returns>
        /// <exception cref="ArgumentNullException">guid</exception>
        public static bool RemoveAutoValue(this IMMSubtype source, mmEditEvent editEvent, Guid guid)
        {
            if (source == null) return false;
            if (guid == Guid.Empty) throw new ArgumentNullException("guid");

            var list = source as ID8List;
            if (list == null) return false;

            IMMAutoValue item = source.GetAutoValue(editEvent, guid);
            if (item != null)
            {
                list.Remove((ID8ListItem) item);
            }

            return true;
        }

        #endregion
    }
}