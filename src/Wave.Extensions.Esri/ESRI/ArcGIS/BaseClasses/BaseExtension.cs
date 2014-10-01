using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An interface that exposes the state changes for the extension enabled states.
    /// </summary>
    public interface INotifyExtensionStateChanged
    {
        #region Events

        /// <summary>
        ///     Raised when the state changes.
        /// </summary>
        event EventHandler<ExtensionStateChangedEventArgs> StateChanged;

        #endregion
    }

    /// <summary>
    ///     An abstract extension for running within ArcGIS Engine or Desktop.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseExtension : IExtension
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseExtension" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected BaseExtension(string extensionName)
        {
            this.Name = extensionName;
        }

        #endregion

        #region IExtension Members

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        ///     Cleanup function for extension.
        /// </summary>
        public virtual void Shutdown()
        {
        }

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public virtual void Startup(ref object initializationData)
        {
        }

        #endregion
    }

    /// <summary>
    ///     The instance event data for the event that is raised.
    /// </summary>
    public class ExtensionStateChangedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ExtensionStateChangedEventArgs(esriExtensionState oldValue, esriExtensionState newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the new value.
        /// </summary>
        /// <value>
        ///     The new value.
        /// </value>
        public esriExtensionState NewValue { get; set; }

        /// <summary>
        ///     Gets or sets the old value.
        /// </summary>
        public esriExtensionState OldValue { get; set; }

        #endregion
    }
}