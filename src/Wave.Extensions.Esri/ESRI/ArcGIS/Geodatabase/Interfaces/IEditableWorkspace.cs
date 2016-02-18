using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides the methods for editing on a workspace.
    /// </summary>
    public interface IEditableWorkspace : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether workspace is being edited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if workspace is being edited; otherwise, <c>false</c>.
        /// </value>
        bool IsBeingEdited { get; }

        /// <summary>
        ///     Gets a value indicating whether the workspace is in the middle of an edit operation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the workspace is in the middle of an edit operation.; otherwise, <c>false</c>.
        /// </value>
        bool IsInEditOperation { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Aborts the edit operations and stops editing.
        /// </summary>
        void AbortEditing();

        /// <summary>
        ///     Starts editing the workspace
        /// </summary>
        /// <param name="withUndoRedo">if set to <c>true</c> when the changes are reverted when the edits are aborted.</param>
        /// <param name="multiuserEditSessionMode">
        ///     The edit session mode that can be used to indicate non-versioned or versioned
        ///     editing for workspaces that support multiuser editing.
        /// </param>
        /// <exception cref="System.ArgumentException">
        ///     The workspace does not support the edit session
        ///     mode.;multiuserEditSessionMode
        /// </exception>
        void StartEditing(bool withUndoRedo, esriMultiuserEditSessionMode multiuserEditSessionMode);

        /// <summary>
        ///     Stops editing the workspace.
        /// </summary>
        /// <param name="saveEdits">if set to <c>true</c> to commit the edits.</param>
        void StopEditing(bool saveEdits);

        #endregion
    }
}