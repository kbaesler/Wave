using System;

namespace ESRI.ArcGIS.Geodatabase.Internal
{
    /// <summary>
    ///     A wrapper to provide a managed workspace editing operations.
    /// </summary>
    internal class EditableWorkspace : IEditableWorkspace
    {
        #region Fields

        private readonly IWorkspace _Workspace;
        private readonly IWorkspaceEdit2 _WorkspaceEdit;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EditableWorkspace" /> class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public EditableWorkspace(IWorkspace workspace)
        {
            _Workspace = workspace;
            _WorkspaceEdit = (IWorkspaceEdit2) workspace;
        }

        #endregion

        #region IEditableWorkspace Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets a value indicating whether workspace is being edited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if workspace is being edited; otherwise, <c>false</c>.
        /// </value>
        public bool IsBeingEdited
        {
            get { return _WorkspaceEdit.IsBeingEdited(); }
        }

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
        public void StartEditing(bool withUndoRedo = true, esriMultiuserEditSessionMode multiuserEditSessionMode = esriMultiuserEditSessionMode.esriMESMVersioned)
        {
            IMultiuserWorkspaceEdit multiuserWorkspaceEdit = _Workspace as IMultiuserWorkspaceEdit;
            if (multiuserWorkspaceEdit != null)
            {
                if (!multiuserWorkspaceEdit.SupportsMultiuserEditSessionMode(multiuserEditSessionMode))
                    throw new ArgumentException(@"The workspace does not support the edit session mode.", "multiuserEditSessionMode");

                multiuserWorkspaceEdit.StartMultiuserEditing(multiuserEditSessionMode);
            }
            else
            {
                if (!_WorkspaceEdit.IsBeingEdited())
                    _WorkspaceEdit.StartEditing(withUndoRedo);
            }

            _WorkspaceEdit.StartEditOperation();
        }

        /// <summary>
        ///     Aborts the editing operations and stops editing.
        /// </summary>
        public void AbortEditing()
        {
            if (_WorkspaceEdit.IsInEditOperation)
                _WorkspaceEdit.AbortEditOperation();

            if (_WorkspaceEdit.IsBeingEdited())
                _WorkspaceEdit.StopEditing(false);
        }


        /// <summary>
        ///     Stops editing the workspace.
        /// </summary>
        /// <param name="saveEdits">if set to <c>true</c> to commit the edits.</param>
        public void StopEditing(bool saveEdits)
        {
            if (_WorkspaceEdit.IsBeingEdited())
            {
                if (_WorkspaceEdit.IsInEditOperation)
                    _WorkspaceEdit.StopEditOperation();

                _WorkspaceEdit.StopEditing(saveEdits);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the workspace is in the middle of an edit operation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the workspace is in the middle of an edit operation.; otherwise, <c>false</c>.
        /// </value>
        public bool IsInEditOperation
        {
            get { return _WorkspaceEdit.IsInEditOperation; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.AbortEditing();
            }
        }

        #endregion
    }
}