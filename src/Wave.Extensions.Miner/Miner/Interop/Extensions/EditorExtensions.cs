using System;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods forthe <see cref="IMMEditor" /> extension.
    /// </summary>
    public static class EditorExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="mode">The mode used for the ArcFM Auto Updaters.</param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IMMEditor source, string menuText, mmAutoUpdaterMode mode, Func<bool> operation)
        {
            using (new AutoUpdaterModeReverter(mode))
            {
                bool flag = false;
                if (source == null || source.Map == null) return false;


                if (source.IsOperationInProgress())
                    throw new ArgumentOutOfRangeException("source", "An edit operation is already started.");

                source.Map.DelayDrawing(true);
                source.StartOperation();

                try
                {
                    flag = operation();
                }
                catch (Exception)
                {
                    if (source.IsOperationInProgress())
                        source.AbortOperation();

                    throw;
                }
                finally
                {
                    if (source.IsOperationInProgress())
                    {
                        if (flag)
                            source.StopOperation(menuText);
                        else
                            source.AbortOperation();
                    }

                    source.Map.DelayDrawing(false);
                }

                return flag;
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:IMMLoginEvents_Event.LoginChanged" /> event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="loginObject">The login object.</param>
        public static void RaiseLoginChanged(this IMMEditor source, IMMLoginObject loginObject)
        {
            IMMFireLoginEvents eventHandler = source as IMMFireLoginEvents;
            if (eventHandler != null)
                eventHandler.FireLoginChanged(loginObject);
        }

        #endregion
    }
}