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
        private static readonly ILog Log = LogProvider.For<IMMEditNotificationQueue>();

        #region Public Methods

        /// <summary>
        ///     Notifies ArcFM that the relationship has been modified.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Notify(this IRelationshipClass source)
        {
            if (source == null) return;

            IMMEditNotificationQueue notifyQueue = EditNotificationQueue.Instance;
            if (notifyQueue != null)
            {
                try
                {
                    notifyQueue.SetNotifyRelationshipChanged(source);
                }
                catch (COMException com)
                {
                    Log.Error("You must be in an edit operation.\n" + com.GetErrorMessage(), com);
                }
            }
        }

        #endregion
    }
}