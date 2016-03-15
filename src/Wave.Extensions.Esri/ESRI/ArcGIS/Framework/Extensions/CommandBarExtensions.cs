using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ICommandBars" /> interface
    /// </summary>
    public static class CommandBarsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the command to the <paramref name="toolbarUid" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="toolbarUid">The toolbar uid.</param>
        /// <param name="commandUid">The command uid.</param>
        public static void Add(this ICommandBars source, UID toolbarUid, UID commandUid)
        {
            ICommandBar toolbar = source.Find(toolbarUid) as ICommandBar;
            if (toolbar != null)
            {
                toolbar.Add(commandUid);
            }
        }

        /// <summary>
        ///     Adds the command to the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandUid">The command uid.</param>
        public static void Add(this ICommandBar source, UID commandUid)
        {
            if (source != null)
            {
                ICommandItem command = source.Find(commandUid);
                if (command == null)
                {
                    source.Add(commandUid);
                }
            }
        }

        /// <summary>
        ///     Replaces the <paramref name="oldCommandUid" /> on the toolbar with the <paramref name="newCommandUid" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="toolbarUid">The toolbar uid.</param>
        /// <param name="newCommandUid">The new command uid.</param>
        /// <param name="oldCommandUid">The old command uid.</param>
        public static void Replace(this ICommandBars source, UID toolbarUid, UID newCommandUid, UID oldCommandUid)
        {
            ICommandBar toolbar = source.Find(toolbarUid) as ICommandBar;
            if (toolbar != null)
            {
                toolbar.Replace(newCommandUid, oldCommandUid);
            }
        }

        /// <summary>
        ///     Replaces the <paramref name="oldCommandUid" /> on the toolbar with the <paramref name="newCommandUid" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="newCommandUid">The new command uid.</param>
        /// <param name="oldCommandUid">The old command uid.</param>
        public static void Replace(this ICommandBar source, UID newCommandUid, UID oldCommandUid)
        {
            if (source != null)
            {
                ICommandItem command = source.Find(newCommandUid);
                if (command == null)
                {
                    ICommandItem item = source.Find(oldCommandUid);
                    if (item != null)
                    {
                        source.Add(newCommandUid, item.Index);
                        item.Delete();
                    }
                }
            }
        }

        #endregion
    }
}