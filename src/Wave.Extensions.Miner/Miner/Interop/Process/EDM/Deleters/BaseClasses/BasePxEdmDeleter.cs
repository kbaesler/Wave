using System;
using System.Globalization;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A process framework deleter that will delete the EDM values stored in the custom tables
    ///     for the given node.
    /// </summary>
    public abstract class BasePxEdmDeleter : BasePxDeleter
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxEdmDeleter" /> class.
        /// </summary>
        /// <param name="deleterName">Name of the deleter.</param>
        protected BasePxEdmDeleter(string deleterName)
            : base(deleterName)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the <see cref="BasePxEdmRepository" /> repository used to manage the extended data.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <returns>
        /// Returns the <see cref="BasePxEdmRepository" /> representing the EDM management controller.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pxApp</exception>
        /// <exception cref="System.NullReferenceException">The WMSEDM process framework configuration value is null.</exception>
        protected virtual BasePxEdmRepository GetEdmRepository(IMMPxApplication pxApp)
        {
            if(pxApp == null) throw new ArgumentNullException("pxApp");

            IMMPxHelper2 helper = (IMMPxHelper2)pxApp.Helper;
            string progId = helper.GetConfigValue("WMSEDM", string.Empty);
            if (string.IsNullOrEmpty(progId))
            {
                throw new NullReferenceException("The WMSEDM process framework configuration value is null.");
            }

            try
            {
                Type type = Type.GetTypeFromProgID(progId);
                object obj = Activator.CreateInstance(type);

                BasePxEdmRepository extendedData = obj as BasePxEdmRepository;
                return extendedData;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Deletes the specified <paramref name="node" />from the process framework database table
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="message">The message.</param>
        /// <param name="status">The status.</param>
        protected override void InternalDelete(IMMPxNode node, ref string message, ref int status)
        {
            if (node == null) return;

            IMMPxDeleter deleter = this.GetBaseDeleter(node);
            if (deleter != null)
            {
                deleter.PxApplication = base.PxApplication;
                deleter.Delete(node, ref message, ref status);
            }

            BasePxEdmRepository edm = this.GetEdmRepository(base.PxApplication);
            if (edm != null && edm.Initialize(base.PxApplication))
            {
                edm.Delete(node.Id);
                message += string.Format(CultureInfo.CurrentCulture, ". Deleted the EDM values for the node with ID: {0}.", node.Id);
            }
        }

        #endregion
    }
}