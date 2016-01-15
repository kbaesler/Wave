using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     Base class for Relationship Auto Updaters.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseRelationshipAU : IMMRelationshipAUStrategy, IMMRelationshipAUStrategyEx
    {
        #region Fields

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseRelationshipAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseRelationshipAU(string name)
        {
            _Name = name;
        }

        #endregion

        #region IMMRelationshipAUStrategy Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     Executes the specified relationship AU.
        /// </summary>
        /// <param name="pRelationship">The relationship.</param>
        /// <param name="mode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        public void Execute(IRelationship pRelationship, mmAutoUpdaterMode mode, mmEditEvent eEvent)
        {
            try
            {
                if (this.CanExecute(mode))
                {
                    this.InternalExecute(pRelationship, mode, eEvent);
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
                        this.WriteError(e);
                        break;
                }
            }
            catch (Exception e)
            {
                this.WriteError(e);
            }
        }

        /// <summary>
        ///     Gets whether the specified AU is enabled.
        /// </summary>
        /// <param name="pRelClass">The relationship class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        public bool get_Enabled(IRelationshipClass pRelClass, mmEditEvent eEvent)
        {
            try
            {
                if (pRelClass == null) return false;

                return this.InternalEnabled(pRelClass, pRelClass.OriginClass, pRelClass.DestinationClass, eEvent);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error(this, Document.ParentWindow, "Error Enabling Relationship AU " + _Name, e);
                else
                    Log.Error(this, "Error Enabling Relationship AU " + _Name, e);
            }

            return false;
        }

        #endregion

        #region IMMRelationshipAUStrategyEx Members

        /// <summary>
        ///     Gets whether the specified AU is enabled.
        /// </summary>
        /// <param name="pRelationshipClass">The relationship class.</param>
        /// <param name="pOriginClass">The origin class.</param>
        /// <param name="pDestClass">The destination class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        public bool get_Enabled(IRelationshipClass pRelationshipClass, IObjectClass pOriginClass, IObjectClass pDestClass, mmEditEvent eEvent)
        {
            try
            {
                return this.InternalEnabled(pRelationshipClass, pOriginClass, pDestClass, eEvent);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error(this, Document.ParentWindow, "Error Enabling Relationship AU " + _Name, e);
                else
                    Log.Error(this, "Error Enabling Relationship AU " + _Name, e);
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
            RelationshipAutoupdateStrategy.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            RelationshipAutoupdateStrategy.Unregister(registryKey);
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
        ///     Determines whether this instance can execute using the specified AU mode.
        /// </summary>
        /// <param name="mode">The AU mode.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute using the specified AU mode; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanExecute(mmAutoUpdaterMode mode)
        {
            return (mode != mmAutoUpdaterMode.mmAUMNoEvents);
        }

        /// <summary>
        ///     Implementation of enabled method for derived classes.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destination class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns><c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c></returns>
        /// <remarks>
        ///     This method will be called from IMMRelationshipAUStrategyEx::get_Enabled
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent editEvent);

        /// <summary>
        ///     Implementation of execute method for derived classes.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <remarks>
        ///     This method will be called from IMMRelationshipAUStrategy::Execute
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract void InternalExecute(IRelationship relationship, mmAutoUpdaterMode mode, mmEditEvent editEvent);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        private void WriteError(Exception e)
        {
            if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                Log.Error(this, Document.ParentWindow, "Error Executing Relationship AU " + _Name, e);
            else
                Log.Error(this, "Error Executing Relationship AU " + _Name, e);
        }

        #endregion
    }
}