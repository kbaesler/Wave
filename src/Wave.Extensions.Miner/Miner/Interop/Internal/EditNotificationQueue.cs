using ESRI.ArcGIS.esriSystem;

using Miner.Geodatabase.Edit;

namespace Miner.Interop.Internal
{
    /// <summary>
    ///     A singleton wrapper for the <see cref="IMMEditNotificationQueue" /> for the ESRI Editor extension.
    /// </summary>
    internal static class EditNotificationQueue
    {
        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IMMEditNotificationQueue Instance
        {
            get
            {
                UID uid = new UID();
                uid.Value = "{AEFDC531-431C-4EF3-8CEC-E5BEEB4D3822}";

                IExtension extension = Editor.FindExtension(uid);
                IMMEditNotificationQueue notifyQueue = (IMMEditNotificationQueue) extension;
                return notifyQueue;
            }
        }

        #endregion
    }
}