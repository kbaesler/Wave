using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IEditor" /> object.
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
            UID uid = new UIDClass();
            uid.Value = "esriEditor.ConflictsWindow";

            return source.FindExtension(uid) as IConflictsWindow;
        }

        #endregion
    }
}