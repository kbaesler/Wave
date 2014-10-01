namespace Miner.Interop.Process
{

    #region Enumerations

    /// <summary>
    ///     An enumeration of the version status.
    /// </summary>
    public enum PxVersionStatus
    {
        /// <summary>
        ///     The version doesn't exist.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The version exists.
        /// </summary>
        Exists = 1,

        /// <summary>
        ///     The version was posted.
        /// </summary>
        Posted = 2,

        /// <summary>
        ///     The version can be deleted.
        /// </summary>
        Deletable = 4
    }

    #endregion
}