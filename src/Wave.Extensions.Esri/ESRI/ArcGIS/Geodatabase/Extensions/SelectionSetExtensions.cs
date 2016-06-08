using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ISelectionSet" /> interface.
    /// </summary>
    public static class SelectionSetExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the specified oids to the selection set.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The oids.</param>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        public static void Add(this ISelectionSet source, params int[] oids)
        {
            if (source == null) return;
            if (oids == null) throw new ArgumentNullException("oids");

            IGeoDatabaseBridge2 bridge = new GeoDatabaseHelperClass();
            bridge.AddList(source, ref oids);
        }

        /// <summary>
        ///     Removes the specified oids from the selection set.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oids">The oids.</param>
        /// <exception cref="System.ArgumentNullException">oids</exception>
        /// <remarks>
        ///     ESRI states that the RemoveList method on the ISelectionSet should not used from .NET. Instead, call
        ///     IGeoDatabaseBridge2.RemoveList.
        /// </remarks>
        public static void Remove(this ISelectionSet source, params int[] oids)
        {
            if (source == null) return;
            if (oids == null) throw new ArgumentNullException("oids");

            IGeoDatabaseBridge2 bridge = new GeoDatabaseHelperClass();
            bridge.RemoveList(source, ref oids);
        }

        #endregion
    }
}