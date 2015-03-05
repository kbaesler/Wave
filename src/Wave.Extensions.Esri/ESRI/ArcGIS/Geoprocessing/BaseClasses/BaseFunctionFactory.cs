using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing.BaseClasses
{
    /// <summary>
    ///     An abstract class that provides access to the collection of <see cref="IGPFunction" /> objects.
    /// </summary>
    /// <remarks>The FunctionFactory object manages the function tools based on the FunctionName object.</remarks>
    public abstract class BaseFunctionFactory : IGPFunctionFactory, IGPFunctionFactory2
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseFunctionFactory" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
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
        ///     Gets the functions for the given factory organized by the key being the <see cref="IGPFunction2" /> and the value
        ///     being the category for the function (if necessary, otherwise supply null).
        /// </summary>
        /// <value>
        ///     The functions.
        /// </value>
        public Dictionary<IGPFunction2, string> Functions { get; private set; }

        #endregion

        #region IGPFunctionFactory Members

        /// <summary>
        ///     Gets or sets the alias.
        /// </summary>
        /// <value>
        ///     The alias.
        /// </value>
        public string Alias { get; protected set; }

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
        ///     Gets the geoprocessing function name object with the given name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns <see cref="IGPFunction" /> of the geoprocessing function object with the given name.</returns>
        public virtual IGPFunction GetFunction(string name)
        {
            return this.Functions.Keys.FirstOrDefault(o => o.Name.Equals(name));
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
            IGPFunctionName gpFunctionName = new GPFunctionNameClass();
            gpFunctionName.MinimumProduct = esriProductCode.esriProductCodeEditor;

            IGPFunction2 gpFunction = this.GetFunction(name) as IGPFunction2;
            if (gpFunction == null) return null;

            var gpName = (IGPName) gpFunctionName;
            gpName.Name = gpFunction.Name;
            gpName.Description = gpFunction.DisplayName;
            gpName.DisplayName = gpFunction.DisplayName;
            gpName.Factory = this;

            if (this.Functions.ContainsKey(gpFunction))
                gpName.Category = this.Functions[gpFunction];

            return gpName;
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
            IArray array = new EnumGPNameClass();

            foreach (IGPFunction function in this.Functions.Keys)
            {
                IGPName gpName = this.GetFunctionName(function.Name);
                if (gpName != null) array.Add(gpName);
            }

            return (IEnumGPName) array;
        }

        /// <summary>
        ///     Name of the geoprocessing function factory.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        #endregion

        #region IGPFunctionFactory2 Members

        /// <summary>
        ///     Release the functions cached by the geoprocessing function factory.
        /// </summary>
        public virtual void ReleaseInternals()
        {
        }

        #endregion        
    }
}