namespace ESRI.ArcGIS.Geodatabase
{

    #region Enumerations

    /// <summary>
    ///     An enumeration indicating the resolution to the conflict.
    /// </summary>
    public enum ConflictResolution
    {
        /// <summary>
        ///     The was no resolution to the conflict.
        /// </summary>
        None,

        /// <summary>
        ///     The conflict was resolved in favor of the pre-reconcile (edit) version.
        /// </summary>
        PreReconcile,

        /// <summary>
        ///     The conflict was resolved in favor of the reconcile (target) version.
        /// </summary>
        Reconcile
    }

    /// <summary>
    ///     An enumeration of the granular row conflict type.
    /// </summary>
    public enum RowConflictType
    {
        /// <summary>
        ///     Indicates a delete was done in the pre-reconcile (edit) version.
        /// </summary>
        DeletePreReconcile,

        /// <summary>
        ///     Indicates an insert was done in the pre-reconcile (edit) version.
        /// </summary>
        InsertPreReconcile,

        /// <summary>
        ///     Indicates an update was done in the pre-reconcile (edit) version.
        /// </summary>
        UpdatePreReconcile,

        /// <summary>
        ///     Indicates an update was done in the pre-reconcile (edit) version and an update was done in the reconcile (target)
        ///     version.
        /// </summary>
        UpdatePreReconcileUpdateReconcile,

        /// <summary>
        ///     Indicates an update was done in the pre-reconcile (edit) version and a delete was done in the reconcile (target)
        ///     version.
        /// </summary>
        UpdatePreReconcileDeleteReconcile,

        /// <summary>
        ///     Indicates a delete was done in the reconcile (target) version.
        /// </summary>
        DeleteReconcile,

        /// <summary>
        ///     Indicates a delete was done in the pre-reconcile (edit) version and an update was done in the reconcile (target)
        ///     version.
        /// </summary>
        DeletePreReconcileUpdateReconcile,

        /// <summary>
        ///     Indicates an insert was done in the reconcile (target) version.
        /// </summary>
        InsertReconcile,

        /// <summary>
        ///     Indicates an update was done in the reconcile (target) version.
        /// </summary>
        UpdateReconcile,

        /// <summary>
        ///     Indicates the pre-reconcile (edit) version and the reconcile (target) version were deleted.
        /// </summary>
        DeletePreReconcileDeleteReconcile
    }

    #endregion

    /// <summary>
    ///     Provides the methods and properties for a row that has conflicts.
    /// </summary>
    public interface IConflictRow
    {
        #region Public Properties

        /// <summary>
        ///     Gets the row OID that is in conflict.
        /// </summary>
        int OID { get; }

        /// <summary>
        ///     Gets or sets a value indicating the resolution to the conflict.
        /// </summary>
        /// <value>
        ///     The conflict resolution.
        /// </value>
        ConflictResolution Resolution { get; set; }

        /// <summary>
        ///     Gets the name of the table.
        /// </summary>
        /// <value>
        ///     The name of the table.
        /// </value>
        string TableName { get; }

        /// <summary>
        ///     Gets the type of the row conflict.
        /// </summary>
        /// <value>
        ///     The type of the row conflict.
        /// </value>
        RowConflictType Type { get; }

        #endregion
    }
}