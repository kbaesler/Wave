namespace Miner.Interop.Process
{
    /// <summary>
    ///     Interface to be implemented by Form classes used as UIs for Px controls.
    /// </summary>
    public interface IPxControlUI
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the handle of the control.
        /// </summary>
        /// <value>
        ///     The handle.
        /// </value>
        int Handle { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the object displayed in the control is locked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        bool Locked { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether there are pending updates.
        /// </summary>
        /// <value>
        ///     <c>true</c> if there are pending updates; otherwise, <c>false</c>.
        /// </value>
        bool PendingUpdates { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IPxControlUI" /> is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        bool Visible { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Applies the updates.
        /// </summary>
        void ApplyUpdates();

        /// <summary>
        ///     Loads the control.
        /// </summary>
        /// <param name="pxApp">The process framework application reference.</param>
        /// <param name="node">The node.</param>
        void LoadControl(IMMPxApplication pxApp, IMMPxNode node);

        #endregion
    }
}