using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMField" /> interface.
    /// </summary>
    public static class FieldExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the automatic value (i.e ArcFM Auto Updater) to the field source for the specified event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IMMAutoValue" /> representing the value that was added; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">guid</exception>
        public static IMMAutoValue AddAutoValue(this IMMField source, mmEditEvent editEvent, Guid guid)
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
        /// <param name="visible">if set to <c>true</c> if the field is visible.</param>
        /// <exception cref="System.ArgumentNullException">fieldName</exception>
        /// <exception cref="ArgumentNullException">fieldName</exception>
        public static void ChangeVisibility(this IMMField source, bool visible)
        {
            if (source == null) return;

            var list = source as ID8List;
            if (list == null) return;

            // There are multiple settings per field, thus only change the visibility setting.
            foreach (IMMSimpleSetting simple in list.AsEnumerable().OfType<IMMSimpleSetting>().Where(o => o.SettingType == mmFieldSettingType.mmFSVisible))
            {
                simple.SettingValue = visible;
            }

            // Update the custom settings (i.e. ArcInfo, etc).
            foreach (ID8ListItem custom in list.AsEnumerable().Where(o => o.ItemType == mmd8ItemType.mmitCustomSetting))
            {
                IMMSimpleSetting simple = custom as IMMSimpleSetting;
                if (simple != null && simple.SettingType == mmFieldSettingType.mmFSVisible)
                {
                    var index = simple.DisplayOrder;

                    // Remove custom setting
                    var containedBy = custom.ContainedBy;
                    list.Remove(custom);

                    MMSimpleSetting setting = new MMSimpleSettingClass();
                    setting.ContainedBy = containedBy;

                    ((IMMSimpleSetting) setting).SettingType = mmFieldSettingType.mmFSVisible;
                    ((IMMSimpleSetting) setting).SettingValue = visible;

                    // Replace with simple setting
                    list.AddSorted(setting, index);
                }
            }
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
        public static IMMAutoValue GetAutoValue(this IMMField source, mmEditEvent editEvent, Guid guid)
        {
            if (source == null) return null;
            if (guid == Guid.Empty) throw new ArgumentNullException("guid");

            ID8List list = source as ID8List;
            if (list == null) return null;

            foreach (IMMAutoValue autoValue in list.AsEnumerable().OfType<IMMAutoValue>().Where(o => o != null && o.EditEvent == editEvent && o.AutoGenID != null))
            {
                if (autoValue.AutoGenID.Value == null) continue;

                string autoGenID = autoValue.AutoGenID.Value.ToString();
                if (string.Equals(autoGenID, guid.ToString("B"), StringComparison.InvariantCultureIgnoreCase))
                    return autoValue;
            }

            return null;
        }


        /// <summary>
        ///     Gets all of the automatic values (i.e. ArcFM Auto Updaters) that have been configured for the specified field
        /// </summary>
        /// <param name="source">The object representing the specific subtype being analyzed.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IMMAutoValue}" /> objects that have been assigned to the field.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public static IEnumerable<IMMAutoValue> GetAutoValues(this IMMField source, mmEditEvent editEvent)
        {
            if (source == null) return null;

            var list = source as ID8List;
            if (list == null) return null;

            return list.AsEnumerable().OfType<IMMAutoValue>().Where(o => o.AutoGenID != null && o.EditEvent == editEvent);
        }

        /// <summary>
        ///     Removes the automatic value (i.e ArcFM Auto Updater) to the field source for the specified event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the value was removed; otherwise <C>false</C>
        /// </returns>
        /// <exception cref="ArgumentNullException">guid</exception>
        public static bool RemoveAutoValue(this IMMField source, mmEditEvent editEvent, Guid guid)
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