using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner;
using Miner.Geodatabase;
using Miner.Interop;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    ///     An abstract class used to create a geoprocessing tool that operates with AU information.
    /// </summary>
    public abstract class BaseConfigTopLevelFunction : BaseLicensedFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConfigTopLevelFunction" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="functionFactory">The function factory.</param>
        protected BaseConfigTopLevelFunction(string name, string displayName, IGPFunctionFactory functionFactory)
            : base(name, displayName, functionFactory, mmProductInstallation.mmPIArcFM)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Add the <paramref name="uids" /> from the <paramref name="list" /> that match the event and UID.
        /// </summary>
        /// <param name="uids">The dictionary of events and UIDs.</param>
        /// <param name="list">The list of events and UIDs.</param>
        /// <param name="messages">The messages.</param>
        protected void Add(Dictionary<mmEditEvent, IEnumerable<string>> uids, ID8List list, IGPMessages messages)
        {
            // Enumerate through the dictionary of events and UIDs.
            foreach (var uid in uids)
            {
                // Locate all of the "AutoValue" types.
                foreach (var item in list.AsEnumerable())
                {
                    if (item.ItemType == mmd8ItemType.mmitAutoValue)
                    {
                        IMMAutoValue autoValue = (IMMAutoValue) item;
                        if (autoValue.AutoGenID != null && autoValue.EditEvent == uid.Key)
                        {
                            // When the UID is not contained within the list.
                            if (!uid.Value.Contains(autoValue.AutoGenID.Value.ToString()))
                            {
                                // Enumerate through all of the UIDs in the collection and add them to list.
                                foreach (var id in uid.Value)
                                {
                                    IMMAutoValue newAutoValue = new MMAutoValueClass();
                                    newAutoValue.EditEvent = uid.Key;
                                    newAutoValue.AutoGenID = new UIDClass()
                                    {
                                        Value = id
                                    };

                                    ((ID8List) item).AddEx(newAutoValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the components from the registry that satisfy the predicate and selector.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="components">The components.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain<TValue>(IEnumerable<KeyValuePair<IUID, TValue>> components, Func<TValue, bool> predicate)
        {
            List<IUID> uids = new List<IUID>();

            foreach (var o in components)
            {
                if (predicate(o.Value))
                    uids.Add(o.Key);
            }

            return this.CreateDomain<TValue>(uids);
        }

        /// <summary>
        ///     Gets the coded value domain for the components assigned to the edit event for the specified
        ///     <paramref name="relationshipClass" />.
        /// </summary>
        /// <param name="relationshipClass">The relationship class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain GetComponents(IRelationshipClass relationshipClass, mmEditEvent editEvent)
        {
            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            var values = configTopLevel.GetAutoValues(relationshipClass, editEvent);

            IList<IUID> uids = new List<IUID>();
            foreach (var value in values)
            {
                uids.Add(value.AutoGenID);
            }

            return this.CreateDomain<IMMRelationshipAUStrategy>(uids);
        }

        /// <summary>
        ///     Gets the coded value domain for the components assigned to the edit event for the specified
        ///     <paramref name="table" /> of the specified <paramref name="subtype" />
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain GetComponents(IObjectClass table, IGPValue subtype, mmEditEvent editEvent)
        {
            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            var values = configTopLevel.GetAutoValues(table, editEvent);
            int subtypeCode = subtype.Cast(-1);

            IList<IUID> uids = new List<IUID>();

            if (values.ContainsKey(subtypeCode))
            {
                var ids = values[subtypeCode].Select(o => o.AutoGenID);
                foreach(var id in ids)
                    uids.Add(id);
            }

            return this.CreateDomain<IMMSpecialAUStrategyEx>(uids);
        }

        /// <summary>
        ///     Gets the coded value domain for the components assigned to the edit event for the field on the specified
        ///     <paramref name="table" /> of the specified <paramref name="subtype" />
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain GetComponents(IObjectClass table, IGPValue subtype, mmEditEvent editEvent, string fieldName)
        {
            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            var values = configTopLevel.GetAutoValues(table, editEvent, fieldName);
            int subtypeCode = subtype.Cast(-1);

            IEqualityComparer<IUID> equalityComparer = new UIDEqualityComparer();
            IList<IUID> uids = new List<IUID>();

            if (values.ContainsKey(subtypeCode))
            {
                var list = values[subtypeCode];
                if (list.ContainsKey(fieldName))
                {
                    var field = list[fieldName];

                    foreach (var o in field.Select(o => o.AutoGenID).Where(o => !uids.Contains(o, equalityComparer)))
                    {
                        uids.Add(o);
                    }
                }
            }

            return this.CreateDomain<IMMAttrAUStrategy>(uids);
        }

        /// <summary>
        ///     Gets the coded value domain that contains all of the subtypes for the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain GetSubtypes(IObjectClass table)
        {
            // Create a domain of all the subtypes.
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();
            codedValueDomain.AddStringCode("-1", "All");

            ISubtypes subtypes = (ISubtypes) table;
            if (subtypes.HasSubtype)
            {
                foreach (var o in subtypes.Subtypes.AsEnumerable())
                    codedValueDomain.AddStringCode(o.Key.ToString(CultureInfo.InvariantCulture), o.Value);
            }

            return codedValueDomain as IGPDomain;
        }

        /// <summary>
        ///     Removes the <paramref name="uids" /> from the <paramref name="list" /> that match the event and UID.
        /// </summary>
        /// <param name="uids">The dictionary of events and UIDs.</param>
        /// <param name="list">The list of events and UIDs.</param>
        /// <param name="messages">The messages.</param>
        protected void Remove(Dictionary<mmEditEvent, IEnumerable<string>> uids, ID8List list, IGPMessages messages)
        {
            // Enumerate through the dictionary of events and UIDs.
            foreach (var uid in uids)
            {
                // Locate all of the "AutoValue" types.
                foreach (var item in list.AsEnumerable())
                {
                    if (item.ItemType == mmd8ItemType.mmitAutoValue)
                    {
                        IMMAutoValue autoValue = (IMMAutoValue) item;
                        if (autoValue.AutoGenID != null && autoValue.EditEvent == uid.Key)
                        {
                            // When the UID is contained within the list it should be removed.
                            if (uid.Value.Contains(autoValue.AutoGenID.Value.ToString()))
                            {
                                list.Remove(item);
                                messages.Add(esriGPMessageType.esriGPMessageTypeInformative, "Removing the {0} from the {1} event.", autoValue.AutoGenID.Value, autoValue.EditEvent);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the variant domain by instantiating the objects from the <see cref="IUID" /> an placing them into a
        ///     <see cref="IGPString" /> value for the domain.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="uids">The uids that will be created.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        private IGPDomain CreateDomain<TValue>(IEnumerable<IUID> uids)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var o in this.CreateInstance<TValue>(uids))
            {
                string key = o.Key.Value.ToString();
                if (!list.ContainsKey(key))
                {
                    string name = this.GetComponentName(o.Value);
                    if (!string.IsNullOrEmpty(name))
                    {
                        list.Add(key, name);
                    }
                }
            }

            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();
            foreach (var o in list.OrderBy(o => o.Value))
                codedValueDomain.AddStringCode(o.Key, o.Value);

            return codedValueDomain as IGPDomain;
        }

        /// <summary>
        ///     Creates the instances from the <see cref="List{IUID}" />
        /// </summary>
        /// <param name="uids">The uids.</param>
        /// <returns>
        ///     Returns a <see cref="TValue" /> representing the object instances
        /// </returns>
        private IEnumerable<KeyValuePair<IUID, TValue>> CreateInstance<TValue>(IEnumerable<IUID> uids)
        {
            foreach (var uid in uids)
            {
                TValue value;

                try
                {
                    value = uid.Create<TValue>();

                    if (EqualityComparer<TValue>.Default.Equals(value, default(TValue)))
                        continue;
                }
                catch
                {
                    value = default(TValue);
                }

                yield return new KeyValuePair<IUID, TValue>(uid, value);
            }
        }

        /// <summary>
        ///     Gets the name of the component.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the component or "UNREGISTERED PROGRAM".
        /// </returns>
        private string GetComponentName<TValue>(TValue value)
        {
            IMMSpecialAUStrategyEx s = value as IMMSpecialAUStrategyEx;
            if (s != null) return s.Name;

            IMMRelationshipAUStrategy r = value as IMMRelationshipAUStrategy;
            if (r != null) return r.Name;

            IMMAttrAUStrategy a = value as IMMAttrAUStrategy;
            if (a != null) return a.Name;

            return "UNREGISTERED PROGRAM";
        }

        /// <summary>
        ///     Loads the components from the registry.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="categoryID">The category identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IUID}" /> representing the UIDs that are enabled.
        /// </returns>
        protected List<KeyValuePair<IUID, TValue>> LoadComponents<TValue>(string categoryID)
        {
            List<KeyValuePair<IUID, TValue>> list = new List<KeyValuePair<IUID, TValue>>();

            IMMComCategoryLookup categories = new ComCategoryLookup(categoryID);
            for (int i = 0; i < categories.Count; i++)
            {
                IUID uid = new UIDClass();
                uid.Value = categories.GetClassID(i).ToString("B");

                foreach (var o in this.CreateInstance<TValue>(new[] {uid}))
                {
                    list.Add(o);
                }
            }

            return list;
        }

        #endregion
    }
}