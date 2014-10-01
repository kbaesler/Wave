using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop.Internal;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IRelationshipClass" /> interface.
    /// </summary>
    public static class RelationshipClassExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Notifies ArcFM that the relationship has been modified.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Notify(this IRelationshipClass source)
        {
            IMMEditNotificationQueue notifyQueue = EditNotificationQueue.Instance;
            if (notifyQueue != null)
            {
                try
                {
                    notifyQueue.SetNotifyRelationshipChanged(source);
                }
                catch (COMException com)
                {
                    Log.Error(typeof (RelationshipClassExtensions), "You must be in an edit operation.\n" + com.GetErrorMessage(), com);
                }
            }
        }

        #endregion
    }
}