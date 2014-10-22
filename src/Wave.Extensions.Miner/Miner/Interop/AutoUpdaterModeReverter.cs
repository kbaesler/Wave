using System;

namespace Miner.Interop
{
    /// <summary>
    ///     A supporting class that will revert the mode of the <see cref="IMMAutoUpdater" /> extension on the disposing of the
    ///     class.
    /// </summary>
    public class AutoUpdaterModeReverter : IDisposable
    {
        #region Fields

        private readonly IMMAutoUpdater _Instance;
        private readonly mmAutoUpdaterMode _PreviousMode;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoUpdaterModeReverter" /> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public AutoUpdaterModeReverter(mmAutoUpdaterMode mode)
        {
#if ARCGIS_10
            _Instance = AutoUpdater.Instance;
#else
            _Instance = Instance;
#endif
            _PreviousMode = _Instance.AutoUpdaterMode;
            _Instance.AutoUpdaterMode = mode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the current mode.
        /// </summary>
        public mmAutoUpdaterMode CurrentMode
        {
            get { return _Instance.AutoUpdaterMode; }
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IMMAutoUpdater Instance
        {
            get
            {
#if ARCGIS_10
                return AutoUpdater.Instance;
#else
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                IMMAutoUpdater autoupdater = obj as IMMAutoUpdater;
                return autoupdater;
#endif
            }
        }

        /// <summary>
        ///     Gets the previous mode.
        /// </summary>
        public mmAutoUpdaterMode PreviousMode
        {
            get { return _PreviousMode; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Instance != null)
                    _Instance.AutoUpdaterMode = _PreviousMode;
            }
        }

        #endregion
    }
}