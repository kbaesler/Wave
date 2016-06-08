using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;

namespace Miner.Process
{
    /// <summary>
    ///     An abstract class used for creating a Px Action Handler
    /// </summary>
    public abstract class BasePxActionHandler : PxActionHandler
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxActionHandler" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        protected BasePxActionHandler(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the process framework application.
        /// </summary>
        /// <value>
        ///     The process framework application.
        /// </value>
        protected IMMPxApplication PxApplication
        {
            get { return PxServiceManager.PxApplication; }
        }

        #endregion
    }
}