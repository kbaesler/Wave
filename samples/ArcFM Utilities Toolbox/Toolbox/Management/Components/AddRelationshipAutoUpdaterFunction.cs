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
    ///     A geoprocessing tool that allows for un-assigning "Relationship" AUs from a relationship class.
    /// </summary>
    public class AddRelationshipAutoUpdaterFunction : BaseConfigTopLevelFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoveRelationshipAutoUpdaterFunction" /> class.
        /// </summary>
        /// <param name="functionFactory">The function factory.</param>
        public AddRelationshipAutoUpdaterFunction(IGPFunctionFactory functionFactory)
            : base("AddRelationshipAU", "Add Relationship AU", functionFactory)
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

                list.Add(this.CreateCompositeParameter("in_table", "Relationship Class", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new DERelationshipClassTypeClass()));
                list.Add(this.CreateMultiValueParameter("in_create", "Create", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass(), true));
                list.Add(this.CreateMultiValueParameter("in_delete", "Delete", esriGPParameterType.esriGPParameterTypeOptional, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass(), true));
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
            IRelationshipClass relClass = utilities.OpenRelationshipClass(value);
            if (relClass != null)
            {
                IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
                configTopLevel.Workspace = utilities.GetWorkspace(value);

                IGPMultiValue onCreate = (IGPMultiValue) parameters["in_create"];
                IGPMultiValue onDelete = (IGPMultiValue) parameters["in_delete"];

                var uids = new Dictionary<mmEditEvent, IEnumerable<string>>();
                uids.Add(mmEditEvent.mmEventRelationshipCreated, onCreate.AsEnumerable().Select(o => o.GetAsText()));
                uids.Add(mmEditEvent.mmEventRelationshipDeleted, onDelete.AsEnumerable().Select(o => o.GetAsText()));

                // Update the list to have these UIDs removed.
                ID8List list = (ID8List) configTopLevel.GetRelationshipClass(relClass);
                base.Add(uids, list, messages);

                // Commit the changes to the database.
                configTopLevel.SaveRelationshipClasstoDB(relClass);

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
                IRelationshipClass relClass = utilities.OpenRelationshipClass(value);
                if (relClass != null)
                {
                    // Load the components.
                    var components = this.LoadComponents<IMMRelationshipAUStrategy>(RelationshipAutoupdateStrategy.CatID);

                    // Load the "OnRelationshipCreated" components for the relationship.
                    IGPParameterEdit3 createParameter = (IGPParameterEdit3) parameters["in_create"];
                    createParameter.Domain = base.CreateDomain(components, o => o.Enabled[relClass, mmEditEvent.mmEventRelationshipCreated]);

                    // Load the "OnRelationshipDeleted" components for the relationship.
                    IGPParameterEdit3 deleteParameter = (IGPParameterEdit3) parameters["in_delete"];
                    deleteParameter.Domain = base.CreateDomain(components, o => o.Enabled[relClass, mmEditEvent.mmEventRelationshipDeleted]);
                }
            }
        }

        #endregion
    }
}