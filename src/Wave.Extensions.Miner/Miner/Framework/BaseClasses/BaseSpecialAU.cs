using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     Abstract base class for Special AutoUpdaters.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseSpecialAU : IMMSpecialAUStrategy, IMMSpecialAUStrategyEx
    {
        #region Fields

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseSpecialAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseSpecialAU(string name)
        {
            _Name = name;
        }

        #endregion

        #region IMMSpecialAUStrategy Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     Executes the specified special AU for the ESRI object.
        /// </summary>
        /// <param name="pObj">The ESRI object.</param>
        public virtual void Execute(IObject pObj)
        {
            this.Execute(pObj, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventUnspecified);
        }

        #endregion

        #region IMMSpecialAUStrategyEx Members

        /// <summary>
        ///     Executes the specified special AU for the ESRI Object.
        /// </summary>
        /// <param name="pObject">The esri object.</param>
        /// <param name="mode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        public virtual void Execute(IObject pObject, mmAutoUpdaterMode mode, mmEditEvent eEvent)
        {
            try
            {
                if (this.ShouldExecute(mode))
                {
                    this.InternalExecute(pObject, mode, eEvent);
                }
            }
            catch (COMException e)
            {
                // If the MM_E_CANCELEDIT error is thrown, let it out.
                switch (e.ErrorCode)
                {
                    case (int) mmErrorCodes.MM_E_CANCELEDIT:
                        throw;
                    default:
                        this.LogException(e);
                        break;
                }
            }
            catch (Exception e)
            {
                this.LogException(e);
            }
        }

        /// <summary>
        ///     Gets whether the auto updater is enabled given the object class and edit event.
        /// </summary>
        /// <param name="pObjectClass">The object class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        public virtual bool get_Enabled(IObjectClass pObjectClass, mmEditEvent eEvent)
        {
            try
            {
                return this.InternalEnabled(pObjectClass, eEvent);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error(this, Document.ParentWindow, "Error Enabling Special AU " + _Name, e);
                else
                    Log.Error(this, "Error Enabling Special AU " + _Name, e);
            }

            return false;
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
            SpecialAutoUpdateStrategy.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            SpecialAutoUpdateStrategy.Unregister(registryKey);
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
        ///     Implementation of enabled method for derived classes.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     <c>true</c> if the AutoUpdater should be enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     This method will be called from IMMSpecialAUStrategy::get_Enabled
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract bool InternalEnabled(IObjectClass objectClass, mmEditEvent editEvent);

        /// <summary>
        ///     Implementation of Auto Updater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the Auto Updater.</param>
        /// <param name="mode">The auto updater mode.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <remarks>
        ///     This method will be called from IMMSpecialAUStrategy::ExecuteEx
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract void InternalExecute(IObject obj, mmAutoUpdaterMode mode, mmEditEvent editEvent);

        /// <summary>
        ///     Determines whether this instance can execute using the specified AU mode.
        /// </summary>
        /// <param name="mode">The auto updater mode.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute using the specified AU mode; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool ShouldExecute(mmAutoUpdaterMode mode)
        {
            return (mode != mmAutoUpdaterMode.mmAUMNoEvents);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        private void LogException(Exception e)
        {
            if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                Log.Error(this, Document.ParentWindow, "Error Executing Special AU " + _Name, e);
            else
                Log.Error(this, "Error Executing Special AU " + _Name, e);
        }

        #endregion
    }
}