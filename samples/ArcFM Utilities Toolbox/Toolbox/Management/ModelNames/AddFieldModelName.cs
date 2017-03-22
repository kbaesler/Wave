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
    ///     A geoprocessing tool that allows for assigning field model names to multiple feature or tables at once.
    /// </summary>
    public class AddFieldModelName : BaseLicensedFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddFieldModelName" /> class.
        /// </summary>
        /// <param name="functionFactory">The function factory.</param>
        public AddFieldModelName(IGPFunctionFactory functionFactory)
            : base("AddFieldModelName", "Add Field Model Name", functionFactory, mmProductInstallation.mmPIArcFM)
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

                array.Add(this.CreateCompositeParameter("in_table", "Table", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new DETableTypeClass(), new DEFeatureClassTypeClass()));
                array.Add(this.CreateMultiValueParameter("in_fields", "Field", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass(), true));
                array.Add(this.CreateMultiValueParameter("in_field_model_names", "Field Model Name(s)", esriGPParameterType.esriGPParameterTypeRequired, esriGPParameterDirection.esriGPParameterDirectionInput, new GPStringTypeClass()));

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
            IGPMultiValue fieldNames = (IGPMultiValue) parameters["in_fields"];
            IGPMultiValue modelNames = (IGPMultiValue) parameters["in_field_model_names"];
            IObjectClass oclass = utilities.OpenTable(parameters["in_table"]);

            if (fieldNames.Count > 0 && modelNames.Count > 0)
            {
                foreach (var field in fieldNames.AsEnumerable())
                {
                    var fieldName = field.GetAsText();
                    int index = oclass.FindField(fieldName);

                    foreach (var modelName in modelNames.AsEnumerable().Select(o => o.GetAsText()))
                    {
                        messages.Add(esriGPMessageType.esriGPMessageTypeInformative, "Adding the {0} field model name to the {1} field.", modelName, fieldName);

                        ModelNameManager.Instance.AddFieldModelName(oclass, oclass.Fields.Field[index], modelName);
                    }
                }

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
            // Retrieve the input parameter value.
            IGPValue value = utilities.UnpackGPValue(parameters["in_table"]);
            if (!value.IsEmpty())
            {
                // Create the domain based on the fields on the table.
                IDETable table = value as IDETable;
                if (table != null)
                {
                    IFields fields = table.Fields;
                    if (fields != null)
                    {
                        IGPCodedValueDomain codedValueDomain = new GPCodedValueDomainClass();
                        foreach (var field in fields.AsEnumerable())
                            codedValueDomain.AddStringCode(field.Name, field.Name);

                        IGPParameterEdit3 derivedFields = (IGPParameterEdit3) parameters["in_fields"];
                        derivedFields.Domain = (IGPDomain) codedValueDomain;
                    }
                }
            }
        }

        #endregion
    }
}