namespace ESRI.ArcGIS.Geodatabase.Internal
{
    /// <summary>
    ///     A lightweight struct that holds the row in conflict.
    /// </summary>
    internal class ConflictRow : IConflictRow
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictRow" /> class.
        /// </summary>
        /// <param name="conflictOID">The conflict OID.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowConflictType">Type of the row conflict.</param>
        internal ConflictRow(int conflictOID, string tableName, RowConflictType rowConflictType)
        {
            this.OID = conflictOID;
            this.TableName = tableName;
            this.Type = rowConflictType;
            this.Resolution = ConflictResolution.None;
        }

        #endregion

        #region IConflictRow Members

        /// <summary>
        ///     Gets the row OID that is in conflict.
        /// </summary>
        public int OID { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating the resolution to the conflict.
        /// </summary>
        /// <value>
        ///     The conflict resolution.
        /// </value>
        public ConflictResolution Resolution { get; set; }

        /// <summary>
        ///     Gets the name of the table.
        /// </summary>
        /// <value>
        ///     The name of the table.
        /// </value>
        public string TableName { get; private set; }

        /// <summary>
        ///     Gets the type of the row conflict.
        /// </summary>
        /// <value>
        ///     The type of the row conflict.
        /// </value>
        public RowConflictType Type { get; private set; }

        #endregion
    }
}