using System;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

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
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IEditor source, string menuText, Func<bool> operation)
        {
            if (source == null || source.Map == null) return false;

            var wse = source.EditWorkspace as IWorkspaceEdit2;
            if (wse == null) return false;

            source.Map.DelayDrawing(true);

            if (wse.IsInEditOperation)
                throw new ArgumentOutOfRangeException("source", @"An edit operation is already started.");

            source.StartOperation();

            bool flag = false;

            try
            {
                flag = operation();
            }
            catch (Exception)
            {
                if (wse.IsInEditOperation)
                    source.AbortOperation();

                throw;
            }
            finally
            {
                if (wse.IsInEditOperation)
                {
                    if (flag)
                        source.StopOperation(menuText);
                    else
                        source.AbortOperation();
                }
            }

            return flag;
        }

        #endregion
    }
}