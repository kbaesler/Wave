using System.Collections;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace Miner.Framework
{
    /// <summary>
    ///     An abstract Relationship Auto Updater (AU) that is used to to execute multiple <see cref="BaseRelationshipAU" />
    ///     for the same relationship.
    /// </summary>
    public abstract class BaseRelationshipCollectionAU : BaseRelationshipAU, IEnumerable<IMMRelationshipAUStrategy>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseRelationshipCollectionAU" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseRelationshipCollectionAU(string name)
            : base(name)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the list of <see cref="IMMRelationshipAUStrategy" /> classes that will be executed.
        /// </summary>
        /// <returns>The list of <see cref="IMMRelationshipAUStrategy" /> classes that will be executed.</returns>
        protected abstract IEnumerable<IMMRelationshipAUStrategy> Items { get; }

        #endregion

        #region IEnumerable<IMMRelationshipAUStrategy> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IMMRelationshipAUStrategy> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Implementation of enabled method for derived classes.
        /// </summary>
        /// <param name="relClass">The relelationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destination class.</param>
        /// <param name="editEvent">The edit event.</param>
        /// <returns>
        ///     <c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     This method will be called from IMMRelationshipAUStrategyEx::get_Enabled
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent editEvent)
        {
            foreach (IMMRelationshipAUStrategy item in this.Items)
            {
                if (!item.get_Enabled(relClass, editEvent))
                {
                    return false;
                }
            }

            return true;
        }

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
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode mode, mmEditEvent editEvent)
        {
            foreach (IMMRelationshipAUStrategy item in this.Items)
            {
                item.Execute(relationship, mode, editEvent);
            }
        }

        #endregion
    }
}