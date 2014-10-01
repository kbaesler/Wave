namespace System.Configuration
{
    /// <summary>
    ///     Represents a configuration element within a configuration file that is based on a key.
    /// </summary>
    [Serializable]
    public abstract class KeyedElement : ConfigurationElement
    {
        #region Public Properties

        /// <summary>
        ///     Gets the element key.
        /// </summary>
        /// <value>The element key.</value>
        public abstract string ElementKey { get; }

        #endregion
    }
}