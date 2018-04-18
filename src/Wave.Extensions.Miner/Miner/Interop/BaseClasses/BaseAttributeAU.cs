using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     Abstract base class for Attribute AutoUpdaters.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseAttributeAU : IMMAttrAUStrategy
    {
        #region Fields

        private static readonly ILog Log = LogProvider.For<BaseAttributeAU>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseAttributeAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="fieldType">Type of the field.</param>
        protected BaseAttributeAU(string name, string domainName, esriFieldType fieldType)
        {
            Name = name;
            DomainName = domainName;
            FieldType = fieldType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name of the domain.
        /// </summary>
        /// <value>The name of the domain.</value>
        public string DomainName { get; }

        /// <summary>
        ///     Gets the type of the field.
        /// </summary>
        /// <value>The type of the field.</value>
        public esriFieldType FieldType { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns the value for the field on the <paramref name="pObj" /> object.
        /// </summary>
        /// <param name="pObj">The object.</param>
        /// <returns>
        ///     A value for the assigned field.
        /// </returns>
        public virtual object GetAutoValue(IObject pObj)
        {
            try
            {
                if (InoperableAutoUpdaters.Instance.Contains(pObj.Class.ObjectClassID, ((IRowSubtypes) pObj).SubtypeCode, this.GetType()))
                    return null;

                return this.InternalExecute(pObj);
            }
            catch (COMException e)
            {
                // If the MM_S_NOCHANGE error was thrown, let it out so ArcFM will know what to do.
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

            return null;
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
            AttrAutoUpdateStrategy.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            AttrAutoUpdateStrategy.Unregister(registryKey);
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
        ///     Returns the value for the field on the <paramref name="obj" /> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A value for the assigned field.
        /// </returns>
        /// <remarks>
        ///     This method will be called from interface method
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract object InternalExecute(IObject obj);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        private void WriteError(Exception e)
        {
            if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                Log.Error("Error Executing Attribute AU " + Name, e);
            else
                Log.Error(e);
        }

        #endregion
    }
}