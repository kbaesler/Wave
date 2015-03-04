namespace ESRI.ArcGIS.Geodatabase
{

    #region Enumerations

    /// <summary>
    ///     An enumeration of the type of conflicts.
    /// </summary>
    public enum TableConflictType
    {
        /// <summary>
        ///     Indicates the delete/update conflicts.
        /// </summary>
        DeleteUpdates = 1,

        /// <summary>
        ///     Indicates the update/delete conflicts.
        /// </summary>
        UpdateDeletes = 2,

        /// <summary>
        ///     Indicates the update/update conflicts.
        /// </summary>
        UpdateUpdates = 3,

        /// <summary>
        ///     The conflict type is unknown.
        /// </summary>
        Unknown = 0
    }

    #endregion

    /// <summary>
    ///     An interface the provides the ability to resolve the conflicts.
    /// </summary>
    public interface IConflictFilter
    {
        #region Public Properties

        /// <summary>
        ///     Gets the name of the conflict filter.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        int Priority { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether this instance can resolve the specified conflict type.
        /// </summary>
        /// <param name="conflictType">Type of the conflict.</param>
        /// <param name="conflictClass">The conflict class.</param>
        /// <returns>
        ///     <c>true</c> if this instance can resolve the specified conflict type; otherwise, <c>false</c>.
        /// </returns>
        bool CanResolve(TableConflictType conflictType, IConflictClass conflictClass);

        /// <summary>
        /// Resolves the reconcile conflicts that have been detected.
        /// </summary>
        /// <param name="conflictRow">The conflict row.</param>
        /// <param name="conflictClass">The conflict class.</param>
        /// <param name="currentRow">The row in the current version that is being edited.</param>
        /// <param name="preReconcileRow">The row prior to reconciliation or edit (child) version (these are edits that you made).</param>
        /// <param name="reconcileRow">The row that the current version is reconciling against or target (parent) version.</param>
        /// <param name="commonAncestorRow">The common ancestor row of this version and the reconcile version (as they are in the
        /// database; this is what the feature and attributes were before any edits were made).</param>
        /// <param name="childWins">if set to <c>true</c> indicating whether the child conflicts should over rule the targets.</param>
        /// <param name="columnLevel">if set to <c>true</c> indicating whether conflicts will be defined at the column level.</param>
        /// <returns>
        /// Returns a <see cref="ConflictResolution" /> enumeration representing the state of the resolution.
        /// </returns>
        ConflictResolution Resolve(IConflictRow conflictRow, IConflictClass conflictClass, IRow currentRow, IRow preReconcileRow, IRow reconcileRow, IRow commonAncestorRow, bool childWins, bool columnLevel);

        #endregion
    }
}