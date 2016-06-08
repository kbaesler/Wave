using System;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace Miner.Geodatabase
{
    /// <summary>
    ///     Provides access to operations that interact with the Geodatabase Manager (Gdbm).
    /// </summary>
    public static class Gdbm
    {
        #region Public Methods

        /// <summary>
        ///     Adds the version information to the GDBM_POST_QUEUE table.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="nodeID">The node ID.</param>
        /// <param name="nodeTypeName">Name of the node type.</param>
        /// <param name="nodeTypeID">The node type ID.</param>
        /// <param name="nonPxPostCode">The non px post code.</param>
        /// <remarks>
        ///     You must write a row into the GDBM_POST_QUEUE table and populate the NON_PX_POST_CODE field with some value (it
        ///     doesn't matter what).
        ///     You must also have a corresponding Post Type configured in the GDBM for the "Plain Version" node.
        ///     This Post Type must have the appropriate Post Code you used in the post queue table (this is how GDBM matches the
        ///     version in the posting queue
        ///     table to the correct posting behavior).
        /// </remarks>
        public static bool Enqueue(IVersion version, int priority, int nodeID, string nodeTypeName, int nodeTypeID, string nonPxPostCode)
        {
            if (!version.HasParent()) return false;

            IWorkspace workspace = (IWorkspace) version;
            IMMTableUtils tableUtils = new MMTableUtilsClass();
            var table = tableUtils.OpenSystemTable(workspace, "GDBM_POST_QUEUE");
            if (table == null) return false;

            var indexes = table.Fields.ToDictionary();

            int index = version.VersionName.IndexOf(".", StringComparison.Ordinal);
            string versionOwner = version.VersionName.Substring(0, index);
            string versionName = version.VersionName.Substring(index + 1, version.VersionName.Length - index - 1);

            IRow row = table.CreateRow();
            row.Value[indexes["CURRENTUSER"]] = workspace.ConnectionProperties.GetProperty("USER", Environment.UserName);
            row.Value[indexes["VERSION_OWNER"]] = versionOwner;
            row.Value[indexes["VERSION_NAME"]] = versionName;
            row.Value[indexes["DESCRIPTION"]] = version.Description;
            row.Value[indexes["SUBMIT_TIME"]] = DateTime.Now;
            row.Value[indexes["PRIORITY"]] = priority;
            row.Value[indexes["PX_NODE_ID"]] = nodeID;
            row.Value[indexes["NODE_TYPE_NAME"]] = nodeTypeName;
            row.Value[indexes["NODE_TYPE_ID"]] = nodeTypeID;
            row.Value[indexes["NON_PX_POST_CODE"]] = nonPxPostCode;
            row.Store();

            return true;
        }        

        #endregion
    }
}