using System;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides methods for executing versioned edits via SQL for Oracle.
    /// </summary>
    public class OracleSqlVersionedEditing : SqlVersionedEditing<IWorkspace, IWorkspaceEdit>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OracleSqlVersionedEditing" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="versionName">Name of the version.</param>
        public OracleSqlVersionedEditing(IWorkspace connection, string versionName)
            : base(connection, (IWorkspaceEdit) connection, versionName)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Commits the changes to the version
        /// </summary>
        public override void Commit()
        {
            if (this.IsBeingEdited && this.IsCurrentVersion)
            {
                this.Connection.ExecuteSQL("BEGIN COMMIT; END;");
                this.IsCommitted = true;
            }
        }

        /// <summary>
        ///     Creates the version.
        /// </summary>
        /// <param name="description">The description.</param>
        public override void CreateVersion(string description)
        {
            this.Connection.ExecuteSQL(string.Format("DECLARE v_versionName VARCHAR2 (50) := '{0}'; BEGIN SDE.VERSION_USER_DDL.CREATE_VERSION ('SDE.DEFAULT', v_versionName, SDE.VERSION_UTIL.C_TAKE_NAME_AS_GIVEN, SDE.VERSION_UTIL.C_VERSION_PUBLIC, '{1}'); END;", this.VersionName, description));
            this.Connection.ExecuteSQL(string.Format("BEGIN SDE.VERSION_UTIL.SET_CURRENT_VERSION('{0}'); END;", this.VersionName));
            this.IsCurrentVersion = true;
        }

        /// <summary>
        ///     Deletes the version.
        /// </summary>
        public override void DeleteVersion()
        {
            this.Connection.ExecuteSQL(string.Format("BEGIN SDE.DELETE_VERSION('{0}'); END;", this.VersionName));
        }


        /// <summary>
        ///     Rollbacks the changes to the version.
        /// </summary>
        public override void Rollback()
        {
            if (this.IsBeingEdited && this.IsCurrentVersion)
            {
                this.Connection.ExecuteSQL("BEGIN ROLLBACK; END;");
                this.IsCommitted = false;
            }
        }

        /// <summary>
        ///     Starts the editing.
        /// </summary>
        public override void Start()
        {
            this.Connection.ExecuteSQL(string.Format("BEGIN SDE.VERSION_USER_DDL.EDIT_VERSION('{0}', 1); END;", this.VersionName));
            this.IsBeingEdited = true;
        }

        /// <summary>
        ///     Stops the edting.
        /// </summary>
        public override void Stop()
        {
            this.Connection.ExecuteSQL(string.Format("BEGIN SDE.VERSION_USER_DDL.EDIT_VERSION('{0}', 2); END;", this.VersionName));
            this.IsBeingEdited = false;
        }

        #endregion
    }

    /// <summary>
    ///     Provides a framework for editing a version via SQL statements.
    /// </summary>
    /// <typeparam name="TConnection">The type of the connection.</typeparam>
    /// <typeparam name="TTransaction">The type of the transaction.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public abstract class SqlVersionedEditing<TConnection, TTransaction> : IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlVersionedEditing{TConnection, TTransaction}" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="versionName">Name of the version.</param>
        protected SqlVersionedEditing(TConnection connection, TTransaction transaction, string versionName)
        {
            this.Connection = connection;
            this.Transaction = transaction;
            this.VersionName = versionName;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the connection.
        /// </summary>
        /// <value>
        ///     The connection.
        /// </value>
        protected TConnection Connection { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is being edited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is being edited; otherwise, <c>false</c>.
        /// </value>
        protected bool IsBeingEdited { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is committed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is committed; otherwise, <c>false</c>.
        /// </value>
        protected bool IsCommitted { get; set; }


        /// <summary>
        ///     Gets a value indicating whether this instance is current version.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is current version; otherwise, <c>false</c>.
        /// </value>
        protected bool IsCurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the transaction.
        /// </summary>
        /// <value>
        ///     The transaction.
        /// </value>
        protected TTransaction Transaction { get; set; }


        /// <summary>
        ///     Gets the name of the version.
        /// </summary>
        /// <value>
        ///     The name of the version.
        /// </value>
        protected string VersionName { get; set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Commits the changes to the version
        /// </summary>
        public abstract void Commit();

        /// <summary>
        ///     Creates the version.
        /// </summary>
        /// <param name="description">The description.</param>
        public abstract void CreateVersion(string description);

        /// <summary>
        ///     Deletes the version.
        /// </summary>
        public abstract void DeleteVersion();

        /// <summary>
        ///     Rollbacks the changes to the version.
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        ///     Starts the editing.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stops the edting.
        /// </summary>
        public abstract void Stop();

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.IsBeingEdited && this.IsCurrentVersion)
                {
                    if (!this.IsCommitted)
                        this.Rollback();

                    this.Stop();
                }
            }
        }

        #endregion
    }
}