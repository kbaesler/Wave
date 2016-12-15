using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract class used for creating an Abandon AU.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseAbandonAU : IMMAbandonAUStrategy
    {
        #region Fields

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseAbandonAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseAbandonAU(string name)
        {
            _Name = name;
        }

        #endregion

        #region IMMAbandonAUStrategy Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     Executes the additional abandoned logic using the (pre-abandoned) <paramref name="pObj" /> object
        ///     and the new (abandoned) <paramref name="pNewObj" /> object.
        /// </summary>
        /// <param name="pObj">The original (pre-abandoned) object.</param>
        /// <param name="pNewObj">The new (abandoned) object.</param>
        public virtual void Execute(IObject pObj, IObject pNewObj)
        {
            try
            {
                if (InoperableAutoUpdaters.Instance.Contains(pObj.Class, this.GetType()))
                    return;

                this.InternalExecute(pObj, pNewObj);
            }
            catch (COMException e)
            {
                // If the MM_S_NOCHANGE error was thrown, let it out so ArcFM will
                // know what to do.
                switch (e.ErrorCode)
                {
                    case (int) mmErrorCodes.MM_S_NOCHANGE:
                        throw;
                    default:
                        this.WriteError(e);
                        break;
                }
            }
            catch (Exception e)
            {
                this.WriteError(e);
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
            MMAbandonStrategy.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMAbandonStrategy.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Aborts the execution by causing an exception to be raised that notifies ArcFM to rollback the edits.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="Miner.CancelEditException"></exception>
        protected void Abort(string message)
        {
            throw new CancelEditException(message);
        }

        /// <summary>
        ///     Executes the additional abandoned logic using the (pre-abandoned) <paramref name="origObj" /> object
        ///     and the new (abandoned) <paramref name="newObj" /> object.
        /// </summary>
        /// <param name="origObj">The original (pre-abandoned) object.</param>
        /// <param name="newObj">The new (abandoned) object.</param>
        protected abstract void InternalExecute(IObject origObj, IObject newObj);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        private void WriteError(Exception e)
        {
            if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                Log.Error(this, Document.ParentWindow, "Error Executing Abandon AU " + _Name, e);
            else
                Log.Error(this, "Error Executing Abandon AU " + _Name, e);
        }

        #endregion
    }
}