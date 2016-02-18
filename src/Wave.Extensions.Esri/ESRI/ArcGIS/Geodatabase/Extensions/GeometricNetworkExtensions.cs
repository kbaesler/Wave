using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.NetworkAnalysis;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGeometricNetwork" /> interface
    /// </summary>
    public static class GeometricNetworkExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Builds the <see cref="IEnumNetEID" /> from the specified list of elements.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="eids">The eids.</param>
        /// <returns>
        ///     A <see cref="IEnumNetEID" /> from the list of elements.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">eids</exception>
        public static IEnumNetEID CreateEnumNetEID(this IGeometricNetwork source, esriElementType elementType, List<int> eids)
        {
            if (source == null) return null;
            if (eids == null) throw new ArgumentNullException("eids");

            return source.CreateEnumNetEID(elementType, eids.ToArray());
        }

        /// <summary>
        ///     Builds the <see cref="IEnumNetEID" /> from the specified list of elements.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="eids">The network element IDs.</param>
        /// <returns>
        ///     A <see cref="IEnumNetEID" /> from the list of elements.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">eids</exception>
        public static IEnumNetEID CreateEnumNetEID(this IGeometricNetwork source, esriElementType elementType, params int[] eids)
        {
            if (source == null) return null;
            if (eids == null) throw new ArgumentNullException("eids");

            IEnumNetEIDBuilderGEN builder = new EnumNetEIDArrayClass();
            builder.ElementType = elementType;
            builder.Network = source.Network;
            builder.set_EIDs(ref eids);

            return (IEnumNetEID) builder;
        }

        /// <summary>
        ///     Gets the network element identifier for the network feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the network element identifier.
        /// </returns>
        public static int GetEID(this INetworkFeature source, out esriElementType elementType)
        {
            IGeometricNetwork geometricNetwork = source.GeometricNetwork;
            INetwork network = geometricNetwork.Network;
            INetElements netElements = (INetElements) network;

            ISimpleJunctionFeature sjf = source as ISimpleJunctionFeature;
            elementType = sjf != null ? esriElementType.esriETJunction : esriElementType.esriETEdge;

            IFeature feature = (IFeature) source;
            int eid = netElements.GetEID(feature.Class.ObjectClassID, feature.OID, -1, elementType);
            return eid;
        }

        /// <summary>
        ///     Returns the <see cref="IEIDInfo" /> for the specified network feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>
        ///     Returns a <see cref="IEIDInfo" /> representing the network information.
        /// </returns>
        public static IEIDInfo GetEIDInfo(this INetworkFeature source, out esriElementType elementType)
        {
            int eid = source.GetEID(out elementType);
            return source.GeometricNetwork.GetEIDInfo(eid, elementType);
        }

        /// <summary>
        ///     Returns the <see cref="IEIDInfo" /> for the specified network element.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="eid">The element ID.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="returnFeatures">if set to <c>true</c> if the created IEIDInfo should contain features.</param>
        /// <param name="returnGeometries">if set to <c>true</c> if the created EIDInfo should contain geometries.</param>
        /// <returns>
        ///     Returns the <see cref="IEIDInfo" /> interface for the network element.
        /// </returns>
        public static IEIDInfo GetEIDInfo(this IGeometricNetwork source, int eid, esriElementType elementType, bool returnFeatures = true, bool returnGeometries = true)
        {
            if (source == null) return null;

            IEnumNetEID enumNetEID = source.CreateEnumNetEID(elementType, eid);

            IEIDHelper eidHelper = new EIDHelperClass();
            eidHelper.GeometricNetwork = source;
            eidHelper.ReturnFeatures = returnFeatures;
            eidHelper.ReturnGeometries = returnGeometries;

            IEnumEIDInfo enumEIDInfo = eidHelper.CreateEnumEIDInfo(enumNetEID);
            IEIDInfo eidInfo = enumEIDInfo.Next();
            return eidInfo;
        }

        /// <summary>
        ///     Checks the if the trace weight is set on the geometric network
        /// </summary>
        /// <param name="source">The geometric network.</param>
        /// <param name="weightName">Name of the trace weight.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the weight exists; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">weightName</exception>
        public static bool IsAssignedWeightName(this IGeometricNetwork source, string weightName)
        {
            if (source == null) return false;
            if (weightName == null) throw new ArgumentNullException("weightName");

            string[] names = { weightName, weightName.ToLowerInvariant(), weightName.ToLowerInvariant() };
            INetSchema netSchema = (INetSchema) source.Network;
            return names.Select(name => netSchema.WeightByName[name]).Any(netWeight => netWeight != null);
        }

        #endregion
    }
}