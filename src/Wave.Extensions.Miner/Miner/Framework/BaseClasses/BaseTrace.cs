using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;

using Miner.Interop;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     An abstract tracing strategy that can be used for network tracing.
    /// </summary>
    /// <typeparam name="TResults">The type of the results.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseTrace<TResults>
        where TResults : IMMSearchResults
    {
        #region Fields

        private readonly Dictionary<int, IEIDInfo> _EdgesByEID;
        private readonly Dictionary<int, IEIDInfo> _JunctionsByEID;

        private int _JunctionsClassID;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTrace&lt;TResults&gt;" /> class.
        /// </summary>
        protected BaseTrace()
        {
            _EdgesByEID = new Dictionary<int, IEIDInfo>();
            _JunctionsByEID = new Dictionary<int, IEIDInfo>();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the EID.
        /// </summary>
        /// <value>The EID.</value>
        protected int EID { get; private set; }

        /// <summary>
        ///     Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        protected esriElementType ElementType { get; private set; }

        /// <summary>
        ///     Gets the geometric network.
        /// </summary>
        /// <value>The geometric network.</value>
        protected IGeometricNetwork GeometricNetwork { get; private set; }

        /// <summary>
        ///     Gets the workspace.
        /// </summary>
        /// <value>The workspace.</value>
        protected IWorkspace Workspace { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initiates the tracing strategy using the <paramref name="feature" /> as the starting point.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>
        ///     The <see cref="Miner.Interop.IMMSearchResults" /> for the trace, otherwise <c>null</c>.
        /// </returns>
        public TResults Trace(IFeature feature)
        {
            try
            {
                this.OnBeforeTrace(feature);

                TResults results = this.OnTrace(feature);
                this.OnAfterTrace(results, feature);

                return results;
            }
            finally
            {
                _EdgesByEID.Clear();
                _JunctionsByEID.Clear();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the network element information for the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>
        ///     Returns the <see cref="int" /> representing the network information.
        /// </returns>
        protected int GetEID(IFeature feature, out esriElementType elementType)
        {
            elementType = esriElementType.esriETNone;

            INetworkFeature networkFeature = feature as INetworkFeature;
            if (networkFeature == null)
                return -1;

            return networkFeature.GetEID(out elementType);
        }

        /// <summary>
        ///     Returns the network element information for the specified <paramref name="eid" />.
        /// </summary>
        /// <param name="eid">The eid.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>Returns a <see cref="IEIDInfo" /> representing the network element information</returns>
        protected IEIDInfo GetEIDInfo(int eid, esriElementType elementType)
        {
            if (elementType == esriElementType.esriETEdge)
            {
                if (_EdgesByEID.ContainsKey(eid))
                    return _EdgesByEID[eid];
            }
            else if (elementType == esriElementType.esriETJunction)
            {
                if (_JunctionsByEID.ContainsKey(eid))
                    return _JunctionsByEID[eid];
            }

            IEIDInfo eidInfo = this.GeometricNetwork.GetEIDInfo(eid, elementType);

            if (elementType == esriElementType.esriETEdge)
            {
                _EdgesByEID.Add(eid, eidInfo);
            }
            else if (elementType == esriElementType.esriETJunction)
            {
                _JunctionsByEID.Add(eid, eidInfo);
            }

            return eidInfo;
        }

        /// <summary>
        ///     Determines whether the specified network element corresponds to the required generic junction feature class.
        /// </summary>
        /// <param name="junctionEID">The junction EID.</param>
        /// <returns>
        ///     <c>true</c> if the specified network element corresponds to the required generic junction feature class; otherwise,
        ///     <c>false</c>.
        /// </returns>
        protected bool IsGenericJunction(int junctionEID)
        {
            if (_JunctionsClassID < 0)
            {
                // Determine the generic junction class ID.
                IFeatureClassContainer container = (IFeatureClassContainer) this.GeometricNetwork;
                IEnumFeatureClass enumClasses = container.Classes;
                enumClasses.Reset();

                // Iterate through all of the classes that participate in the network.
                IFeatureClass networkClass;
                while ((networkClass = enumClasses.Next()) != null)
                {
                    if (this.IsGenericJunction(networkClass))
                    {
                        _JunctionsClassID = networkClass.ObjectClassID;
                        break;
                    }
                }
            }

            // Determine the class identifier for the element.
            IEIDInfo eidInfo = this.GetEIDInfo(junctionEID, esriElementType.esriETJunction);

            // When the class ID equals the generic junction.
            return (eidInfo.Feature.Class.ObjectClassID == _JunctionsClassID);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="IObjectClass" /> corresponds to the generic network junctions class.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="IObjectClass" /> corresponds to the generic network junctions class;
        ///     otherwise, <c>false</c>.
        /// </returns>
        protected bool IsGenericJunction(IFeatureClass objectClass)
        {
            if (objectClass.ShapeType != esriGeometryType.esriGeometryPoint)
                return false;

            string name = ((IDataset) objectClass).Name;
            int pos = name.IndexOf(".", StringComparison.Ordinal);
            int length = name.Length;

            string substring = name;
            if (pos > 0) substring = name.Substring(pos + 1, length - pos - 1);

            return substring.ToUpper(CultureInfo.CurrentCulture).Trim().EndsWith("_JUNCTIONS", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Called after the <see cref="OnTrace(IFeature)" /> method has executed.
        /// </summary>
        /// <param name="traceResults">The trace results.</param>
        /// <param name="feature">The feature.</param>
        protected abstract void OnAfterTrace(TResults traceResults, IFeature feature);

        /// <summary>
        ///     Called before the trace executes the <see cref="OnTrace(IFeature)" /> method.
        /// </summary>
        /// <param name="feature">The feature.</param>
        protected virtual void OnBeforeTrace(IFeature feature)
        {
            this.Workspace = ((IDataset) feature.Class).Workspace;

            esriElementType elementType;
            this.EID = this.GetEID(feature, out elementType);
            this.ElementType = elementType;

            INetworkFeature networkFeature = (INetworkFeature) feature;
            this.GeometricNetwork = networkFeature.GeometricNetwork;
        }

        /// <summary>
        ///     Initiates the tracing using the <paramref name="feature" /> as the starting point.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>
        ///     The <see cref="Miner.Interop.IMMSearchResults" /> containing the results from the trace.
        /// </returns>
        protected abstract TResults OnTrace(IFeature feature);

        #endregion
    }
}