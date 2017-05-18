using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Editor
{
    /// <summary>
    ///     Provides extension methods forthe <see cref="IEditor" /> extension.
    /// </summary>
    public static class EditorExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Provides access to the conflict display environment after performing a reconcile in the Editor.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns the <see cref="IConflictsWindow" /> representing the conflict display environment.
        /// </returns>
        /// <remarks>
        ///     If conflicts were detected and further post processing of conflicts is required,
        ///     the methods to work with the classes that contain conflicts and the individual rows that are conflicts.
        ///     Removing conflicting rows from the selection sets and resetting the conflicts window
        ///     allows you to programmatically remove rows and classes from the dialog.
        /// </remarks>
        public static IConflictsWindow GetConflictsWindow(this IEditor source)
        {
            if (source == null) return null;

            UID uid = new UIDClass();
            uid.Value = "esriEditor.ConflictsWindow";

            return source.FindExtension(uid) as IConflictsWindow;
        }

        /// <summary>
        /// Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="workspace">The workspace you wish to edit and this workspace must be represented in the focus map.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>
        /// Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IEditor source, IWorkspace workspace, string menuText, Func<bool> operation)
        {
            return source.PerformOperation(menuText, workspace, operation, error => false);
        }

        /// <summary>
        /// Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="workspace">The workspace you wish to edit and this workspace must be represented in the focus map.</param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <param name="error">The error handling action that occurred during commit when true is returned the error has been
        /// handled.</param>
        /// <returns>
        /// Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IEditor source, string menuText, IWorkspace workspace, Func<bool> operation, Func<COMException, bool> error)
        {
            if (source == null || source.Map == null) return false;

            source.StartEditing(workspace);
            source.StartOperation();
            source.Map.DelayDrawing(true);

            bool saveEdits = false;
            bool editOperation = true;

            try
            {
                saveEdits = operation();
            }
            catch (Exception)
            {
                source.AbortOperation();
                editOperation = false;

                throw;
            }
            finally
            {
                if (editOperation)
                {
                    if (saveEdits)
                        source.StopOperation(menuText);
                    else
                        source.AbortOperation();
                }

                try
                {
                    source.StopEditing(saveEdits);
                }
                catch (COMException com)
                {
                    if (!error(com))
                        throw;

                    source.StopEditing(saveEdits);
                }
                finally
                {
                    source.Map.DelayDrawing(false);
                }
            }

            return saveEdits;
        }

        #endregion
    }
}