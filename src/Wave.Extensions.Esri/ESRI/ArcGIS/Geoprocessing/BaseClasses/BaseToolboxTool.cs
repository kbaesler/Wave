using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     An abstract class used to create geoprocessing tools for ArcMap or ArcCatalog.
    /// </summary>
    /// <remarks>
    ///     There are a few informal rules you should keep in mind when building geoprocessing function tools.
    ///     These rules are useful to begin or conclude discussions about writing functions, and they are listed as follows:
    ///     - Always create a new array in ParameterInfo()
    ///     - Always have an output
    ///     - Do not localize keywords
    ///     - Use a coded value domain for a Boolean parameter
    /// </remarks>
    public abstract class BaseToolboxTool : IGPFunction2
    {
        #region Constants

        /// <summary>
        ///     The output parameter
        /// </summary>
        protected const string OUTPUT_PARAMETER = "results";

        #endregion

        #region Fields

        /// <summary>
        ///     The cancelled results code (-1)
        /// </summary>
        public static IGPValue Cancelled = new GPLongClass {Value = -1};

        /// <summary>
        ///     The failed results code (1)
        /// </summary>
        public static IGPValue Failed = new GPLongClass {Value = 1};

        /// <summary>
        ///     The succeeded results code (0)
        /// </summary>
        public static IGPValue Succeeded = new GPLongClass {Value = 0};

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseToolboxTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="toolboxFactory">The function factory.</param>
        /// <exception cref="System.ArgumentNullException">functionFactory</exception>
        protected BaseToolboxTool(string name, string displayName, IGPFunctionFactory toolboxFactory)
        {
            if (toolboxFactory == null)
                throw new ArgumentNullException("toolboxFactory");

            this.Name = name;
            this.DisplayName = displayName;
            this.MetadataFile = string.Format("{0}_metadata.xml", name);
            this.ToolboxFactory = toolboxFactory;
            this.Utilities = new GPUtilitiesClass();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the function name object of the geoprocessing function.
        /// </summary>
        public virtual IName FullName
        {
            get
            {
                IGPName name = this.ToolboxFactory.GetFunctionName(Name);
                return name as IName;
            }
        }

        /// <summary>
        ///     Gets the list of parameters accepted by the geoprocessing function.
        /// </summary>
        /// <value>
        ///     The ParameterInfo property is the place where a function tool's parameters are defined. It returns an IArray of
        ///     parameter objects (IGPParameter); these objects define the characteristics of the input and output parameters.
        ///     At the minimum, your function should output a Boolean value containing success or failure.
        /// </value>
        public abstract IArray ParameterInfo { get; }

        /// <summary>
        ///     The class identifier (CLSID) of the custom dialog object to use when invoking the geoprocessing function.
        /// </summary>
        /// <value>
        ///     The dialog CLSID.
        /// </value>
        /// <remarks>
        ///     The DialogCLSID property is only for internal use. This is used to overwrite the default system tool dialog's look
        ///     and feel.
        ///     By default, Toolbox creates a dialog based upon the parameters returned by the ParameterInfo property.
        /// </remarks>
        public UID DialogCLSID { get; protected set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     Displayed name of the geoprocessing function.
        /// </value>
        public string DisplayName { get; protected set; }

        /// <summary>
        ///     Gets or sets the context identifier of the topic within the help file for this function object.
        /// </summary>
        /// <value>
        ///     The HelpContext property is a unique ID for the help topic from a HelpFile.
        /// </value>
        public int HelpContext { get; protected set; }

        /// <summary>
        ///     Gets or sets the name of the (CHM) file containing help information for this function object.
        /// </summary>
        /// <value>
        ///     The HelpFile property stores the path to a .chm file which contains a description of the tool parameters and
        ///     explains the tool's operation and usage.
        /// </value>
        public string HelpFile { get; protected set; }

        /// <summary>
        ///     Gets or sets the name of the (XML) file containing the default metadata for this function object.
        /// </summary>
        /// <value>
        ///     The MetadataFile property stores the name of a .xml file with the default metadata for a function tool. The .xml
        ///     file supplies parameter descriptions in the help panel of a tool dialog. If no .chm file is provided through the
        ///     HelpFile property, a tool's help is based on the .xml file's content.
        /// </value>
        public string MetadataFile { get; protected set; }

        /// <summary>
        ///     Gets or sets name of the geoprocessing function.
        /// </summary>
        /// <value>
        ///     The Name property sets the name of a function tool. This name appears when using the function tool at the command
        ///     line or in scripting. It must be unique within a given toolbox and must not contain any spaces.
        /// </value>
        public string Name { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the function factory.
        /// </summary>
        /// <value>
        ///     The function factory.
        /// </value>
        protected IGPFunctionFactory ToolboxFactory { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the utilities.
        /// </summary>
        /// <value>
        ///     The utilities.
        /// </value>
        private IGPUtilities2 Utilities { get; set; }

        #endregion

        #region IGPFunction2 Members

        /// <summary>
        ///     Executes the geoprocessing function using the given array of parameter values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <param name="messages">The messages.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     parameters;A function tool must always have an output. At the minimum,
        ///     your function should output a Boolean value containing success or failure.
        /// </exception>
        public virtual void Execute(IArray parameters, ITrackCancel trackCancel, IGPEnvironmentManager environmentManager, IGPMessages messages)
        {
            try
            {
                if (parameters.AsEnumerable<IGPParameter>().All(o => o.Direction != esriGPParameterDirection.esriGPParameterDirectionOutput))
                    throw new ArgumentOutOfRangeException(nameof(parameters), @"A function tool must always have an output. At the minimum, your function should output a Boolean value containing success or failure.");

                var list = this.Unpack(parameters);

                this.Execute(list, trackCancel, environmentManager, messages, this.Utilities);
            }
            catch (Exception ex)
            {
                messages.AddError(-1, ex.StackTrace);
            }
        }

        /// <summary>
        ///     Gets the custom renderer to use for the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        /// <remarks>Used to set a custom renderer for a function tool's output.</remarks>
        public virtual object GetRenderer(IGPParameter parameter)
        {
            return null;
        }

        /// <summary>
        ///     Determines whether the geoprocessing function has all necessary licenses in order to execute.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the function is licensed.
        /// </returns>
        /// <remarks>
        ///     The IsLicensed property is used to check if a function tool is licensed to execute in the active application.
        /// </remarks>
        public virtual bool IsLicensed()
        {
            return true;
        }

        /// <summary>
        ///     Post validates the given set of values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <param name="messages">The messages that are reported to the user.</param>
        /// <remarks>
        ///     This method is called after returning from the internal validation routine performed by the geoprocessing
        ///     framework. This method is where you can examine the messages created from internal validation and change them if
        ///     desired.
        ///     You should only change existing messages here and should not add any new messages.
        /// </remarks>
        public void UpdateMessages(IArray parameters, IGPEnvironmentManager environmentManager, IGPMessages messages)
        {
            try
            {
                var list = parameters.AsEnumerable<IGPParameter>().ToDictionary(o => o.Name, o => o);

                this.UpdateMessages(list, environmentManager, messages, this.Utilities);
            }
            catch (Exception ex)
            {
                messages.AddError(-1, ex.StackTrace);
            }
        }

        /// <summary>
        ///     Pre validates the given set of values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        public void UpdateParameters(IArray parameters, IGPEnvironmentManager environmentManager)
        {
            var list = parameters.AsEnumerable<IGPParameter>().ToDictionary(o => o.Name, o => o);

            this.UpdateParameters(list, environmentManager, this.Utilities);
        }

        /// <summary>
        ///     Validates that a function tool's set of parameter values are of the expected number, data type, and value.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="updateValues">
        ///     if set to <c>true</c> the function should populate the properties of the output value. This
        ///     is important for proper chaining of tools in ModelBuilder.
        /// </param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <returns>
        ///     Returns a <see cref="IGPMessages" /> representing the validation errors.
        /// </returns>
        /// <remarks>
        ///     At ArcGIS 9.3, this method has been replaced by UpdateParameters() and UpdateMessages() and no longer needs to be
        ///     implemented.
        /// </remarks>
        public virtual IGPMessages Validate(IArray parameters, bool updateValues, IGPEnvironmentManager environmentManager)
        {
            // Only Call if updatevalues is true.
            if (updateValues)
            {
                UpdateParameters(parameters, environmentManager);
            }

            // Call InternalValidate (Basic Validation). Are all the required parameters supplied?
            // Are the Values to the parameters the correct data type?
            IGPMessages validateMessages = this.Utilities.InternalValidate(this.ParameterInfo, parameters, false, false, environmentManager);

            // Call UpdateMessages();
            UpdateMessages(parameters, environmentManager, validateMessages);

            return validateMessages;
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
        protected abstract void Execute(Dictionary<string, IGPValue> parameters, ITrackCancel trackCancel, IGPEnvironmentManager environmentManager, IGPMessages messages, IGPUtilities2 utilities);

        /// <summary>
        ///     Unpacks the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns a <see cref="Dictionary{TKey,TValue}" /> representing the unpacked parameters.</returns>
        protected Dictionary<string, IGPValue> Unpack(Dictionary<string, IGPParameter> parameters)
        {
            return parameters.ToDictionary(o => o.Key, o => this.Utilities.UnpackGPValue(o.Value));
        }

        /// <summary>
        ///     Unpacks the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns a <see cref="Dictionary{TKey,TValue}" /> representing the unpacked parameters.</returns>
        protected Dictionary<string, IGPValue> Unpack(IArray parameters)
        {
            return parameters.AsEnumerable<IGPParameter>().ToDictionary(o => o.Name, o => this.Utilities.UnpackGPValue(o));
        }

        /// <summary>
        ///     Post validates the given set of values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="environmentManager">Provides access to all the current environments and settings of the current client.</param>
        /// <param name="messages">The messages that are reported to the user.</param>
        /// <param name="utilities">
        ///     The utilities object that provides access to the properties and methods of a geoprocessing
        ///     objects.
        /// </param>
        protected virtual void UpdateMessages(Dictionary<string, IGPParameter> parameters, IGPEnvironmentManager environmentManager, IGPMessages messages, IGPUtilities2 utilities)
        {
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
        protected virtual void UpdateParameters(Dictionary<string, IGPParameter> parameters, IGPEnvironmentManager environmentManager, IGPUtilities2 utilities)
        {
        }

        #endregion
    }
}