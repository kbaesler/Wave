namespace Miner.Interop.Process
{
    /// <summary>
    ///     A partial extension of the EDMTable element.
    /// </summary>
    public partial class EdmTable
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="EdmTable" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public bool Valid
        {
            get { return !string.IsNullOrEmpty(this.TableName); }
        }

        #endregion
    }
}