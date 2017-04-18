using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner;
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
        protected void Add(Dictionary<mmEditEvent, IEnumerable<IUID>> uids, ID8List list, IGPMessages messages)
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
                            if (!uid.Value.Contains(autoValue.AutoGenID))
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
        /// <returns>
        /// Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain<TValue>(IEnumerable<GPAutoValue<TValue>> components)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();

            foreach (var o in components)
            {
                codedValueDomain.AddCode(o, o.Name);
            }        

            return (IGPDomain)codedValueDomain;
        }

        /// <summary>
        ///     Gets the components from the registry that satisfy the predicate and selector.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="components">The components.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain<TValue>(IEnumerable<GPAutoValue<TValue>> components, Func<TValue, bool> predicate)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();

            foreach (var o in components)
            {
                if (o.Value != null && predicate(o.Value))
                    codedValueDomain.AddCode(o, o.Name);
            }

            return (IGPDomain) codedValueDomain;
        }

        /// <summary>
        ///     Creates the variant domain by instantiating the objects from the <see cref="IUID" /> an placing them into a
        ///     <see cref="IGPString" /> value for the domain.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="uids">The uids that will be created.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain<TValue>(IEnumerable<IMMAutoValue> uids)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();

            foreach (var uid in uids)
            {
                GPAutoValue<TValue> value = new GPAutoValue<TValue>(uid.AutoGenID);
                codedValueDomain.AddCode(value, value.Name);
            }

            return (IGPDomain) codedValueDomain;
        }

        /// <summary>
        ///     Creates the variant domain by instantiating the objects from the <see cref="IUID" /> an placing them into a
        ///     <see cref="IGPString" /> value for the domain.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="uids">The uids that will be created.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain<TValue>(IEnumerable<IUID> uids)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();

            foreach (var uid in uids)
            {
                GPAutoValue<TValue> value = new GPAutoValue<TValue>(uid);
                codedValueDomain.AddCode(value, value.Name);
            }

            return (IGPDomain) codedValueDomain;
        }

        /// <summary>
        /// Gets the coded value domain.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain CreateDomain(string[] names, string[] values)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();
            for (int i = 0; i < values.Length; i++)
                codedValueDomain.AddStringCode(values[i], names[i]);

            return (IGPDomain)codedValueDomain;
        }

        /// <summary>
        ///     Gets the coded value domain using fields from the specified <paramref name="table" />
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        ///     Returns a <see cref="IGPDomain" /> representing the coded value domain.
        /// </returns>
        protected IGPDomain GetFields(IObjectClass table)
        {
            IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();

            foreach (var o in table.Fields.AsEnumerable())
                codedValueDomain.AddStringCode(o.Name, o.Name);

            return (IGPDomain) codedValueDomain;
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
        ///     Loads the components from the registry.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="categoryID">The category identifier.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IUID}" /> representing the UIDs that are enabled.
        /// </returns>
        protected List<GPAutoValue<TValue>> LoadComponents<TValue>(string categoryID)
        {
            List<GPAutoValue<TValue>> list = new List<GPAutoValue<TValue>>();

            IMMComCategoryLookup categories = new ComCategoryLookup(categoryID);
            for (int i = 0; i < categories.Count; i++)
            {
                IUID uid = new UIDClass();
                uid.Value = categories.GetClassID(i).ToString("B");

                GPAutoValue<TValue> value = new GPAutoValue<TValue>(uid);
                list.Add(value);
            }

            return list.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        ///     Removes the <paramref name="uids" /> from the <paramref name="list" /> that match the event and UID.
        /// </summary>
        /// <param name="uids">The dictionary of events and UIDs.</param>
        /// <param name="list">The list of events and UIDs.</param>
        /// <param name="messages">The messages.</param>
        protected void Remove(Dictionary<mmEditEvent, IEnumerable<IUID>> uids, ID8List list, IGPMessages messages)
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
                            if (uid.Value.Contains(autoValue.AutoGenID))
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
    }
}