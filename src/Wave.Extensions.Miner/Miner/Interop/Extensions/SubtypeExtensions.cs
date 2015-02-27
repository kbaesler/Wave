using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop.Extensions
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
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the value was added; otherwise <C>false</C>
        /// </returns>
        /// <exception cref="ArgumentNullException">guid</exception>
        public static bool AddAutoValue(this IMMSubtype source, mmEditEvent editEvent, Guid guid)
        {
            if (source == null) return false;
            if (guid == Guid.Empty) throw new ArgumentNullException("guid");

            if (source.GetAutoValue(editEvent, guid) != null)
                return false;

            var list = source as ID8List;
            if (list == null) return false;

            UID uid = new UIDClass();
            uid.Value = guid.ToString("B");

            list.Add(new MMAutoValueClass
            {
                AutoGenID = uid,
                EditEvent = editEvent
            });

            return true;
        }

        /// <summary>
        ///     Gets the automatic value (i.e. ArcFM Auto Updater) for the specified <paramref name="editEvent" /> and
        ///     <paramref name="guid" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IMMAutoValue" /> representign the automatic value; otherwise <c>null</c>.
        /// </returns>
        public static IMMAutoValue GetAutoValue(this IMMSubtype source, mmEditEvent editEvent, Guid guid)
        {
            ID8List list = (ID8List) source;

            foreach (IMMAutoValue autoValue in list.AsEnumerable().OfType<IMMAutoValue>())
            {
                if (autoValue == null) continue;
                if (autoValue.EditEvent != editEvent) continue;
                if (autoValue.AutoGenID == null) continue;
                if (autoValue.AutoGenID.Value == null) continue;

                string autoGenID = autoValue.AutoGenID.Value.ToString();

                if (string.Equals(autoGenID, guid.ToString("B"), StringComparison.InvariantCultureIgnoreCase))
                    return autoValue;
            }

            return null;
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified
        ///     <paramref name="objectClass" />
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="objectClass">The object class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="List{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        public static List<IMMAutoValue> GetAutoValues(this IMMSubtype source, IObjectClass objectClass, mmEditEvent editEvent)
        {
            if (source == null) return null;

            List<IMMAutoValue> list = ((ID8List) source).AsEnumerable().OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent).ToList();
            return list;
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified
        ///     <paramref name="index" />
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="objectClass">The object class.</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="List{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static List<IMMAutoValue> GetAutoValues(this IMMSubtype source, IObjectClass objectClass, int index, mmEditEvent editEvent)
        {
            if (source == null) return null;
            if (index < 0 || objectClass.Fields.FieldCount > 0)
                throw new IndexOutOfRangeException();

            IMMField mmfield = null;
            source.GetField(index, ref mmfield);
            if (mmfield == null) return null;

            List<IMMAutoValue> list = ((ID8List) mmfield).AsEnumerable().OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent).ToList();
            return list;
        }

        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified
        ///     <paramref name="field" />
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="objectClass">The object class.</param>
        /// <param name="field">The field in the object class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="List{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        public static List<IMMAutoValue> GetAutoValues(this IMMSubtype source, IObjectClass objectClass, IField field, mmEditEvent editEvent)
        {
            if (source == null) return null;

            int index = objectClass.FindField(field.Name);
            return source.GetAutoValues(objectClass, index, editEvent);
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

            IMMAutoValue item = source.GetAutoValue(editEvent, guid);
            if (item != null)
            {
                var list = source as ID8List;
                if (list == null) return false;

                list.Remove((ID8ListItem) item);
            }

            return true;
        }

        /// <summary>
        ///     Sets the field visibility.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        public static void SetFieldVisibility(this IMMSubtype source, string fieldName, bool visible)
        {
            if (source == null) return;
            if (fieldName == null) throw new ArgumentNullException("fieldName");

            var list = source as ID8List;
            if (list == null) return;

            var item = list.Where(o => o.ItemType == mmd8ItemType.mmitField && ((IMMField) o).FieldName == fieldName).FirstOrDefault();
            if (item != null)
            {
                ID8List fields = item.Value as ID8List;
                if (fields != null)
                {
                    // Update the simple cases for true and false.
                    foreach (IMMSimpleSetting simple in fields.AsEnumerable().OfType<IMMSimpleSetting>().Where(setting => setting.SettingType == mmFieldSettingType.mmFSVisible))
                    {
                        simple.SettingValue = visible;
                    }

                    // Update the custom cases for ArcMap Only, Phase A, etc.
                    foreach (ID8ListItem custom in fields.AsEnumerable().Where(o => o.ItemType == mmd8ItemType.mmitCustomSetting))
                    {
                        IMMSimpleSetting simple = custom as IMMSimpleSetting;
                        if (simple != null && simple.SettingType == mmFieldSettingType.mmFSVisible)
                        {
                            var index = simple.DisplayOrder;

                            // Remove custom setting
                            var containedBy = custom.ContainedBy;
                            fields.Remove(custom);

                            MMSimpleSetting setting = new MMSimpleSettingClass();
                            setting.ContainedBy = containedBy;

                            ((IMMSimpleSetting) setting).SettingType = mmFieldSettingType.mmFSVisible;
                            ((IMMSimpleSetting) setting).SettingValue = visible;

                            // Replace with simple setting
                            fields.AddSorted(setting, index);
                        }
                    }
                }
            }
        }

        #endregion
    }
}