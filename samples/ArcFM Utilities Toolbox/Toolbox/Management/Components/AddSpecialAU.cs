using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    ///     A geoprocessing tool that allows for assigning "Special" AUs to object classes.
    /// </summary>
    public class AddSpecialAU : BaseConfigTopLevelFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddSpecialAU" /> class.
        /// </summary>
        /// <param name="functionFactory">The function factory.</param>
        public AddSpecialAU(IGPFunctionFactory functionFactory)
            : base("AddSpecialAU", "Add Special AU", functionFactory)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the list of parameters accepted by the geoprocessing function.
        /// </summary>
        /// <value>
        ///     The ParameterInfo property is the place where a function tool's parameters are defined. It returns an IArray of
        ///     parameter objects (IGPParameter); these objects define the characteristics of the input and output parameters.
        ///     At the minimum, your function should output a Boolean value containing success or failure.
        /// </value>
        public override IArray ParameterInfo
        {
            get
            {
                IArray list = new ArrayClass();

                list.Add(this.CreateCompositeParameter("in_table", "Table or Feature Class", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new DETableTypeClass(), new DEFeatureClassTypeClass()));

                list.Add(this.CreateParameter("in_subtype", "Subtype", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass()));

                list.Add(this.CreateMultiValueParameter("in_create", "Create", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));
                list.Add(this.CreateMultiValueParameter("in_update", "Update", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));
                list.Add(this.CreateMultiValueParameter("in_delete", "Delete", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));

                list.Add(this.CreateMultiValueParameter("in_before", "Before Split", "Advanced", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));
                list.Add(this.CreateMultiValueParameter("in_split", "Split", "Advanced", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));
                list.Add(this.CreateMultiValueParameter("in_after", "After Split", "Advanced", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPAutoValueType<IMMSpecialAUStrategy>()));

                list.Add(this.CreateParameter("out_results", "Results", esriGPParameterType.esriGPParameterTypeDerived, esriGPParameterDirection.esriGPParameterDirectionOutput, new GPBooleanTypeClass()));

                return list;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Executes the geoprocessing function using the given array of parameter values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <param name="messages">The messages that are reported to the user.</param>
        /// <param name="utilities">
        ///     The utilities object that provides access to the properties and methods of a geoprocessing
        ///     objects.
        /// </param>
        protected override void Execute(Dictionary<string, IGPValue> parameters, ITrackCancel trackCancel, IGPEnvironmentManager environmentManager, IGPMessages messages, IGPUtilities2 utilities)
        {
            IGPValue value = parameters["in_table"];
            IObjectClass table = utilities.OpenTable(value);
            if (table != null)
            {
                IGPMultiValue onCreate = (IGPMultiValue)parameters["in_create"];
                IGPMultiValue onUpdate = (IGPMultiValue)parameters["in_update"];
                IGPMultiValue onDelete = (IGPMultiValue)parameters["in_delete"];

                IGPMultiValue onBeforeSplit = (IGPMultiValue)parameters["in_before"];
                IGPMultiValue onSplit = (IGPMultiValue)parameters["in_split"];
                IGPMultiValue onAfterSplit = (IGPMultiValue)parameters["in_after"];

                // Load "Special" AUs.
                var uids = new Dictionary<mmEditEvent, IEnumerable<IUID>>();
                uids.Add(mmEditEvent.mmEventFeatureCreate, onCreate.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));
                uids.Add(mmEditEvent.mmEventFeatureUpdate, onUpdate.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));
                uids.Add(mmEditEvent.mmEventFeatureDelete, onDelete.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));
                uids.Add(mmEditEvent.mmEventBeforeFeatureSplit, onBeforeSplit.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));
                uids.Add(mmEditEvent.mmEventFeatureSplit, onSplit.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));
                uids.Add(mmEditEvent.mmEventAfterFeatureSplit, onAfterSplit.AsEnumerable().Cast<IGPAutoValue>().Select(o => o.UID));

                IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
                configTopLevel.Workspace = utilities.GetWorkspace(value);

                // Load all of the subtypes when the user specified "All" or "-1".
                int subtype = parameters["in_subtype"].Cast(-1);
                var subtypeCodes = new List<int>(new[] { subtype });
                if (subtype == -1)
                {
                    ISubtypes subtypes = (ISubtypes)table;
                    subtypeCodes.AddRange(subtypes.Subtypes.AsEnumerable().Select(o => o.Key));
                }

                // Enumerate through all of the subtypes making changes.
                foreach (var subtypeCode in subtypeCodes)
                {
                    // Load the configurations for the table and subtype.
                    ID8List list = (ID8List)configTopLevel.GetSubtypeByID(table, subtypeCode, false);

                    // Update the list to have these UIDs removed.
                    this.Add(uids, list, messages);
                }

                // Commit the changes to the database.
                configTopLevel.SaveFeatureClassToDB(table);

                // Success.
                parameters["out_results"].SetAsText("true");
            }
            else
            {
                // Failure.
                parameters["out_results"].SetAsText("false");
            }
        }
        
        /// <summary>
        ///     Pre validates the given set of values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <param name="utilities">
        ///     The utilities object that provides access to the properties and methods of a geoprocessing
        ///     objects.
        /// </param>
        protected override void UpdateParameters(Dictionary<string, IGPParameter> parameters, IGPEnvironmentManager environmentManager, IGPUtilities2 utilities)
        {
            IGPValue value = utilities.UnpackGPValue(parameters["in_table"]);
            if (!value.IsEmpty())
            {
                IObjectClass table = utilities.OpenTable(value);
                if (table != null)
                {
                    IGPParameterEdit3 subtypeParameter = (IGPParameterEdit3)parameters["in_subtype"];
                    subtypeParameter.Domain = this.GetSubtypes(table);

                    var components = this.LoadComponents<IMMSpecialAUStrategyEx>(SpecialAutoUpdateStrategy.CatID);

                    IGPParameterEdit3 prameter = (IGPParameterEdit3)parameters["in_create"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventFeatureCreate]);

                    // Load the "OnUpdate" components for table.
                    prameter = (IGPParameterEdit3)parameters["in_update"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventFeatureUpdate]);

                    // Load the "OnDelete" components for table.
                    prameter = (IGPParameterEdit3)parameters["in_delete"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventFeatureDelete]);

                    // Load the "OnBeforeSplit" components for table.
                    prameter = (IGPParameterEdit3)parameters["in_before"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventBeforeFeatureSplit]);

                    // Load the "OnSplit" components for table.
                    prameter = (IGPParameterEdit3)parameters["in_split"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventFeatureSplit]);

                    // Load the "OnAfterSplit" components for table.
                    prameter = (IGPParameterEdit3)parameters["in_after"];
                    prameter.Domain = base.CreateDomain(components, o => o.Enabled[table, mmEditEvent.mmEventAfterFeatureSplit]);
                }
            }
        }

        #endregion
    }
}