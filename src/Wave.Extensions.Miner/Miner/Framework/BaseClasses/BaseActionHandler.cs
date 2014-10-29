using Miner.Geodatabase.GeodatabaseManager.ActionHandlers;

namespace Miner.Framework.BaseClasses
{
    /// <summary>
    ///     An abstract class used for creating an Action Handler
    /// </summary>
    public abstract class BaseActionHandler : ActionHandler
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseActionHandler" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        protected BaseActionHandler(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        #endregion
    }
}