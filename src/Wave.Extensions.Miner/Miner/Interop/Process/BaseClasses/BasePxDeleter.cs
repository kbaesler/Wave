using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     The base deleter class used to delete custom data from workflow manager when a WorkRequest, Design, WorkLocation or
    ///     CU is deleted.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    [ComVisible(true)]
    public abstract class BasePxDeleter : IMMPxDeleter, IMMPxDisplayName
    {
        #region Fields

        private static readonly ILog Log = LogProvider.For<BasePxDeleter>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxDeleter" /> class.
        /// </summary>
        /// <param name="deleterName">Name of the deleter.</param>
        protected BasePxDeleter(string deleterName)
        {
            this.DisplayName = deleterName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; private set; }

        /// <summary>
        ///     Gets or sets the px application.
        /// </summary>
        /// <value>
        ///     The px application.
        /// </value>
        public IMMPxApplication PxApplication { set; protected get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Deletes the specified px node from the process framework database table.
        /// </summary>
        /// <param name="pPxNode">The node.</param>
        /// <param name="sMsg">The message.</param>
        /// <param name="status">The status.</param>
        public virtual void Delete(IMMPxNode pPxNode, ref string sMsg, ref int status)
        {
            try
            {
                this.InternalDelete(pPxNode, ref sMsg, ref status);

                if (string.IsNullOrEmpty(sMsg) && this.PxApplication != null)
                    sMsg = string.Format("{0} {1} deleted successfully.", this.PxApplication.GetNodeTypeName(pPxNode), pPxNode.Id);
            }
            catch (Exception e)
            {
                Log.Error("Error Executing Deleter " + this.DisplayName, e);
                this.Notify(e.Message, mmUserMessageType.mmUMTDataError);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MMProcessMgrDeleter.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMProcessMgrDeleter.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the deleter that is used to clean up the associated parent node (i.e. the Session or Design,  Work Request).
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     Returns the <see cref="Miner.Interop.Process.IMMPxDeleter" /> representing the deleter for the node.
        /// </returns>
        protected IMMPxDeleter GetNodeDeleter(IMMPxNode node)
        {
            if (this.PxApplication == null || node == null) return null;

            if (node.NodeType == this.PxApplication.Helper.GetNodeTypeID(Design.NodeTypeName))
            {
                return new clsDNDeleterClass();
            }

            if (node.NodeType == this.PxApplication.Helper.GetNodeTypeID(WorkRequest.NodeTypeName))
            {
                return new clsWRDeleterClass();
            }

            if (node.NodeType == this.PxApplication.Helper.GetNodeTypeID(Session.NodeTypeName))
            {
                Type t = Type.GetTypeFromProgID("mmSessionManager.clsSessionDeleter");
                object obj = Activator.CreateInstance(t);
                return (IMMPxDeleter) obj;
            }

            return null;
        }

        /// <summary>
        ///     Deletes the specified <paramref name="node" />from the process framework database table
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="message">The message.</param>
        /// <param name="status">The status.</param>
        protected abstract void InternalDelete(IMMPxNode node, ref string message, ref int status);

        /// <summary>
        ///     Logs the message to callback associated with the current process framework application.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        protected virtual void Notify(string message)
        {
            this.Notify(message, mmUserMessageType.mmUMTUserInfo);
        }

        /// <summary>
        ///     Logs the message to callback associated with the current process framework application.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="messageType">The User message type, if applicable.</param>
        protected virtual void Notify(string message, mmUserMessageType messageType)
        {
            try
            {
                var cleanup = this.PxApplication as IMMPxCleanup;
                if (cleanup != null)
                {
                    IMMMessageCallback callback = cleanup.CleanupCallback;
                    if (callback == null) return;

                    callback.StatusMessage = message;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion
    }
}