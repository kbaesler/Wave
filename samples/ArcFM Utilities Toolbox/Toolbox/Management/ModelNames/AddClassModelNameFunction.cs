using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner.Geodatabase;
using Miner.Interop;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    ///     A geoprocessing tool that allows for assigning class model names to multiple feature or tables at once.
    /// </summary>
    public class AddClassModelNameFunction : BaseLicensedFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddClassModelNameFunction" /> class.
        /// </summary>
        /// <param name="functionFactory">The function factory.</param>
        public AddClassModelNameFunction(IGPFunctionFactory functionFactory)
            : base("AddClassModelName", "Add Class Model Name", functionFactory, mmProductInstallation.mmPIArcFM)
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
        /// </value>
        public override IArray ParameterInfo
        {
            get
            {
                IArray array = new ArrayClass();

                array.Add(this.CreateMultiValueParameter("in_tables", "Table", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new DETableTypeClass(), new DEFeatureClassTypeClass()));
                array.Add(this.CreateMultiValueParameter("in_class_model_names", "Class Model Name(s)", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass()));

                array.Add(this.CreateParameter("out_results", "Results", esriGPParameterType.esriGPParameterTypeDerived, esriGPParameterDirection.esriGPParameterDirectionOutput, new GPBooleanTypeClass()));

                return array;
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
            IGPMultiValue tables = (IGPMultiValue) parameters["in_tables"];
            IGPMultiValue modelNames = (IGPMultiValue) parameters["in_class_model_names"];
            int addedCount = 0;

            if (tables.Count > 0 && modelNames.Count > 0)
            {
                foreach (var table in tables.AsEnumerable())
                {
                    IObjectClass dataElement = utilities.OpenTable(table);

                    foreach (var modelName in modelNames.AsEnumerable().Select(o => o.GetAsText()))
                    {
                        messages.Add(esriGPMessageType.esriGPMessageTypeInformative, "Adding the {0} class model name to the {1} table.", modelName, dataElement.AliasName);

                        ModelNameManager.Instance.AddClassModelName(dataElement, modelName);
                        addedCount++;
                    }
                }
            }

            if (addedCount == tables.Count)
            {
                // Success
                parameters["out_results"].SetAsText("true");
            }
            else
            {
                // Failure.
                parameters["out_results"].SetAsText("false");
            }
        }

        #endregion
    }
}