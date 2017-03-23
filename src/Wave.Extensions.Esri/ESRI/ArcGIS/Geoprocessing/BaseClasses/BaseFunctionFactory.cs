using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     An abstract class that provides access to the collection of <see cref="IGPFunction" /> objects.
    /// </summary>
    /// <remarks>The FunctionFactory object manages the function tools based on the FunctionName object.</remarks>
    [ComVisible(true)]
    public abstract class BaseFunctionFactory : IGPFunctionFactory, IGPFunctionFactory2
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseFunctionFactory" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name of the toolbox. This is used when generating the Toolbox containing the function tools of
        ///     the factory.
        /// </param>
        /// <param name="alias">The alias.</param>
        protected BaseFunctionFactory(string name, string alias)
        {
            this.Name = name;
            this.Alias = alias;
            this.Functions = new Dictionary<IGPFunction2, string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The class identifier (CLSID) of the geoprocessing function factory.
        /// </summary>
        /// <value>
        ///     The CLSID.
        /// </value>
        public UID CLSID
        {
            get
            {
                UID id = new UIDClass();
                id.Value = GetType().GUID.ToString("B");
                return id;
            }
        }

        /// <summary>
        ///     Gets or sets the alias.
        /// </summary>
        /// <value>
        ///     The alias.
        /// </value>
        public string Alias { get; protected set; }

        /// <summary>
        ///     Name of the geoprocessing function factory.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the functions for the given factory organized by the key being the <see cref="IGPFunction2" /> and the value
        ///     being the category for the function (if necessary, otherwise supply null).
        /// </summary>
        /// <value>
        ///     The functions.
        /// </value>
        private Dictionary<IGPFunction2, string> Functions { get; set; }

        #endregion

        #region IGPFunctionFactory Members

        /// <summary>
        ///     Gets the geoprocessing function name object with the given name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns <see cref="IGPFunction" /> of the geoprocessing function object with the given name.</returns>
        public virtual IGPFunction GetFunction(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return this.Functions.Keys.First(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Gets all of the geoprocessing environments that the geoprocessing functions managed by this function factory use
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IEnumGPEnvironment" /> representing all of the geoprocessing environments that the
        ///     geoprocessing functions managed by this function factory use.
        /// </returns>
        public virtual IEnumGPEnvironment GetFunctionEnvironments()
        {
            return new EnumGPEnvironmentClass();
        }

        /// <summary>
        ///     Gets the geoprocessing function name object with the given name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns <see cref="IGPName" /> of the geoprocessing function name object with the given name.</returns>
        public virtual IGPName GetFunctionName(string name)
        {
            var function = (IGPFunction2) this.GetFunction(name);
            var functionName = (IGPName) this.GetFunctionName(function);

            if (this.Functions.ContainsKey(function))
                functionName.Category = this.Functions[function];

            return functionName;
        }

        /// <summary>
        ///     Gets all of function name objects of all the geoprocessing functions managed by this function factory.
        /// </summary>
        /// <returns>
        ///     Returns an <see cref="IEnumGPName" /> representing all of function name objects of all the geoprocessing
        ///     functions managed by this function factory.
        /// </returns>
        public virtual IEnumGPName GetFunctionNames()
        {
            IArray list = new EnumGPNameClass();

            foreach (IGPFunction function in this.Functions.Keys)
            {
                IGPName functionName = this.GetFunctionName(function.Name);
                list.Add(functionName);
            }

            return (IEnumGPName) list;
        }

        #endregion

        #region IGPFunctionFactory2 Members

        /// <summary>
        ///     Release the functions cached by the geoprocessing function factory.
        /// </summary>
        public virtual void ReleaseInternals()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the specified function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="category">The category.</param>
        protected void Add(IGPFunction2 function, string category)
        {
            this.Functions.Add(function, category);
        }

        /// <summary>
        ///     Gets the name of the function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        protected virtual IGPFunctionName GetFunctionName(IGPFunction2 function)
        {
            IGPFunctionName functionName = new GPFunctionNameClass();
            functionName.MinimumProduct = esriProductCode.esriProductCodeBasic;
            functionName.HelpFile = function.HelpFile;
            functionName.HelpContext = function.HelpContext;

            IGPName name = (IGPName) functionName;
            name.Name = function.Name;
            name.Description = function.DisplayName;
            name.DisplayName = function.DisplayName;
            name.Factory = this;

            return functionName;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        static void Register(string regKey)
        {
            GPFunctionFactories.Register(regKey);
        }

        /// <summary>
        ///     Unregisters the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        static void Unregister(string regKey)
        {
            GPFunctionFactories.Unregister(regKey);
        }

        #endregion
    }
}