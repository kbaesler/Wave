using System.Collections.Generic;
using System.Runtime.InteropServices;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides the ability to extend the advanced reconcile process using conflict filters.
    /// </summary>
    public interface IConflictWorkspaceExtension
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the auto updater mode.
        /// </summary>
        /// <value>
        ///     The auto updater mode.
        /// </value>
        [ComVisible(false)]
        mmAutoUpdaterMode AutoUpdaterMode { get; set; }

        /// <summary>
        ///     Gets or sets the callback that is used to notify the caller of progress changes.
        /// </summary>
        /// <value>
        ///     The callback.
        /// </value>
        IMMMessageCallback Callback { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the child conflicts should over rule the targets.
        /// </summary>
        /// <value>
        ///     <c>true</c> if child conflicts should over rule the targets; otherwise, <c>false</c>.
        /// </value>
        bool ChildWins { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether conflicts will be defined at the column level.
        /// </summary>
        /// <value>
        ///     <c>true</c> if conflicts will be defined at the column level; otherwise, <c>false</c>.
        /// </value>
        bool ColumnLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is rebuilding the connectivity of the network features before
        ///     reconcile.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is rebuilding the connectivity of the network features before reconcile; otherwise,
        ///     <c>false</c>.
        /// </value>
        bool IsRebuildingConnectivity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the conflicts that have been resolved are removed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the conflicts that have been resolved are removed; otherwise, <c>false</c>.
        /// </value>
        bool IsRemovedAfterResolved { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the conflicts that have been resolved should be saved again (with AUs)
        ///     after the reconcile completes.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the conflicts that have been resolved should be saved again (with AUs) after the reconcile
        ///     completes; otherwise, <c>false</c>.
        /// </value>
        bool IsSavedAfterReconcile { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether target version is locked during reconcile.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the target version is locked during reconcile; otherwise, <c>false</c>.
        /// </value>
        bool LockTarget { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns the filters that are used for resolving the reconcile conflicts.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>
        ///     Returns a
        ///     <see cref="T:System.Collections.Generic.IList{ESRI.ArcGIS.Geodatabase.IConflictFilter}" />
        ///     implementations that are used to resolve the row conflicts.
        /// </returns>
        IList<IConflictFilter> GetFilters(IWorkspace workspace);

        #endregion
    }
}