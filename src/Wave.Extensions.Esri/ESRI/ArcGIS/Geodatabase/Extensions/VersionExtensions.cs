using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IVersion" /> interface.
    /// </summary>
    public static class VersionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions that need to be
        ///     checked-in or exported.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="dataChangeTypes">The data change types.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DeltaRow}" /> representing the differences for the table (or
        ///     key).
        /// </returns>
        public static Dictionary<string, List<DeltaRow>> GetDataChanges(this IVersion source, IVersion target, params esriDataChangeType[] dataChangeTypes)
        {
            return source.GetDataChanges(target, (s, t) => t != null, dataChangeTypes);
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions that need to be
        ///     checked-in or exported.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="predicate">
        ///     The predicate that defines a set of criteria and determines whether the specified differences
        ///     will be loaded.
        /// </param>
        /// <param name="dataChangeTypes">The data change types.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DeltaRow}" /> representing the differences for the table (or
        ///     key).
        /// </returns>
        public static Dictionary<string, List<DeltaRow>> GetDataChanges(this IVersion source, IVersion target, Func<string, ITable, bool> predicate, params esriDataChangeType[] dataChangeTypes)
        {
            var list = new Dictionary<string, List<DeltaRow>>();


            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName) ((IDataset) source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName) ((IDataset) target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            VersionDataChanges vdc = (VersionDataChanges) vdci;
            IDataChanges dataChanges = (IDataChanges) vdci;
            IDataChangesInfo dci = (IDataChangesInfo) vdc;

            IEnumModifiedClassInfo enumMci = dataChanges.GetModifiedClassesInfo();
            enumMci.Reset();
            IModifiedClassInfo mci;
            while ((mci = enumMci.Next()) != null)
            {
                string tableName = mci.ChildClassName;
                var rows = new List<DeltaRow>();

                using (ComReleaser cr = new ComReleaser())
                {
                    // Load the copy of the table from the source version.
                    ITable table = ((IFeatureWorkspace) source).OpenTable(tableName);
                    cr.ManageLifetime(table);

                    // Validate that the table needs to be loaded.
                    if (predicate(tableName, table))
                    {
                        // Determines if the table represents a feature class.
                        bool isFeatureClass = (table is IFeatureClass);

                        // Iterate through all of the data change types.
                        foreach (var dataChangeType in dataChangeTypes)
                        {
                            // IRow objects returned from a difference cursor are meant to be a read only. Thus, only the OIDs are being loaded and
                            // the rows are hydrated from the struct.
                            IFIDSet set = dci.ChangedIDs[tableName, dataChangeType];
                            set.Reset();

                            int oid;
                            set.Next(out oid);
                            while (oid != -1)
                            {
                                var row = new DeltaRow(dataChangeType, oid, tableName, isFeatureClass, source, target);
                                rows.Add(row);

                                set.Next(out oid);
                            }
                        }
                    }
                }

                list.Add(tableName, rows);
            }

            return list;
        }

        /// <summary>
        ///     Gets the changes between the <paramref name="source" /> (or child) and <paramref name="table" /> (or parent)
        ///     version of the table.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="table">The table (or parent) version.</param>
        /// <param name="filter">The predicate used to filter the differences.</param>
        /// <param name="differenceTypes">The types of differences that are detected.</param>
        /// <returns>Returns a <see cref="IEnumerable{DifferenceRow}" /> representing the differences for the table.</returns>
        public static IEnumerable<DifferenceRow> GetDifferences(this IVersionedTable source, ITable table, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            foreach (var differenceType in differenceTypes)
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    IDifferenceCursor differenceCursor = source.Differences(table, differenceType, filter);
                    cr.ManageLifetime(differenceCursor);

                    // IRow objects returned from a difference cursor are meant to be a read only. Thus, only the OIDs are being loaded and
                    // the rows are hydrated from the table again.
                    List<int> oids = new List<int>();
                    IRow differenceRow;
                    int oid;

                    differenceCursor.Next(out oid, out differenceRow);
                    while (oid != -1)
                    {
                        oids.Add(oid);
                        differenceCursor.Next(out oid, out differenceRow);
                    }

                    if (oids.Count > 0)
                    {
                        // When the feature was deleted in the source we need to use the parent version.
                        ITable differenceTable = (differenceType == esriDifferenceType.esriDifferenceTypeDeleteNoChange || differenceType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
                            ? table
                            : (ITable) source;

                        // Fetch the rows for read-write access.
                        ICursor cursor = differenceTable.GetRows(oids.ToArray(), false);
                        cr.ManageLifetime(cursor);

                        int i = 0;
                        while ((differenceRow = cursor.NextRow()) != null)
                        {
                            yield return new DifferenceRow(differenceType, oids[i++], differenceRow);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="filter">The predicate to filter the results.</param>
        /// <param name="differenceTypes">The type of differences that will be determined.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DifferenceRow}" /> representing the differences for the
        ///     table (or key).
        /// </returns>
        public static Dictionary<string, IEnumerable<DifferenceRow>> GetDifferences(this IVersion source, IVersion target, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            return source.GetDifferences(target, filter, (s, t) => t != null, differenceTypes);
        }

        /// <summary>
        ///     Gets the differences between the <paramref name="source" /> and <paramref name="target" /> versions.
        /// </summary>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        /// <param name="filter">The predicate to filter the results.</param>
        /// <param name="predicate">
        ///     The predicate that defines a set of criteria and determines whether the specified differences
        ///     will be loaded.
        /// </param>
        /// <param name="differenceTypes">The type of differences that will be determined.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DifferenceRow}" /> representing the differences for the
        ///     table (or key).
        /// </returns>
        public static Dictionary<string, IEnumerable<DifferenceRow>> GetDifferences(this IVersion source, IVersion target, IQueryFilter filter, Func<string, ITable, bool> predicate, params esriDifferenceType[] differenceTypes)
        {
            var list = new Dictionary<string, IEnumerable<DifferenceRow>>();

            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName) ((IDataset) source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName) ((IDataset) target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            IDataChanges dataChanges = (IDataChanges) vdci;
            IEnumModifiedClassInfo enumMci = dataChanges.GetModifiedClassesInfo();
            enumMci.Reset();
            IModifiedClassInfo mci;
            while ((mci = enumMci.Next()) != null)
            {
                // The table references are not disposed due to the enumerable return which would result in an RCW exception.
                string tableName = mci.ChildClassName;
                ITable sourceTable = ((IFeatureWorkspace) source).OpenTable(tableName);
                if (predicate(tableName, sourceTable))
                {
                    IVersionedTable versionedTable = sourceTable as IVersionedTable;
                    ITable table = ((IFeatureWorkspace) target).OpenTable(tableName);
                    if (versionedTable != null && table != null)
                    {
                        var rows = versionedTable.GetDifferences(table, filter, differenceTypes);
                        list.Add(tableName, rows);
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Gets the changes (or edits) that have been made in the current edit session.
        /// </summary>
        /// <param name="source">The source version.</param>
        /// <param name="editDataChangesType">Type of the edit data changes.</param>
        /// <param name="differenceTypes">The type of differences.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{String, DifferenceRow}" /> representing the differences for the table
        ///     (or key).
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     The version must be within an edit session in order to determine the
        ///     edit changes.
        /// </exception>
        public static Dictionary<string, List<DifferenceRow>> GetEditChanges(this IVersion source, esriEditDataChangesType editDataChangesType, params esriDifferenceType[] differenceTypes)
        {
            var list = new Dictionary<string, List<DifferenceRow>>();

            IWorkspaceEdit2 workspaceEdit2 = (IWorkspaceEdit2) source;
            if (!workspaceEdit2.IsBeingEdited())
                throw new InvalidOperationException("The version must be within an edit session in order to determine the edit changes.");

            IDataChangesEx dataChanges = workspaceEdit2.EditDataChanges[editDataChangesType];
            IEnumBSTR enumMci = dataChanges.ModifiedClasses;
            enumMci.Reset();
            string tableName;
            while ((tableName = enumMci.Next()) != null)
            {
                var rows = new List<DifferenceRow>();

                foreach (var differenceType in differenceTypes)
                {
                    using (ComReleaser cr = new ComReleaser())
                    {
                        IDifferenceCursorEx cursor = dataChanges.ExtractEx(tableName, differenceType);
                        cr.ManageLifetime(cursor);

                        IRow sourceRow;
                        IRow differenceRow;
                        int oid;
                        ILongArray fieldIndices;

                        cursor.Next(out oid, out sourceRow, out differenceRow, out fieldIndices);
                        while (oid != -1)
                        {
                            var row = new DifferenceRow(differenceType, oid, differenceRow, sourceRow, fieldIndices);
                            rows.Add(row);

                            cursor.Next(out oid, out sourceRow, out differenceRow, out fieldIndices);
                        }
                    }
                }

                list.Add(tableName, rows);
            }

            return list;
        }

        #endregion
    }

    #region Nested Type: DeltaRow

    /// <summary>
    ///     A struct that represents the row differences based on the data changes for extract.
    /// </summary>
    public struct DeltaRow
    {
        #region Enumerations

        /// <summary>
        ///     The state of the row.
        /// </summary>
        public enum RowState
        {
            /// <summary>
            ///     The state of the row in the current (child) version.
            /// </summary>
            ChildVersion,

            /// <summary>
            ///     The state of the row in the target (parent) version.
            /// </summary>
            ParentVersion,

            /// <summary>
            ///     The state of the row in the common ancestor (prior to edits) version.
            /// </summary>
            CommonAncestorVersion
        }

        #endregion

        #region Fields

        /// <summary>
        ///     The common ancestor workspace.
        /// </summary>
        private readonly IWorkspace _CommonAncestorWorkspace;

        /// <summary>
        ///     The source workspace or child workspace.
        /// </summary>
        private readonly IWorkspace _SourceWorkspace;

        /// <summary>
        ///     The target workspace or parent workspace.
        /// </summary>
        private readonly IWorkspace _TargetWorkspace;

        /// <summary>
        ///     The type of data changes.
        /// </summary>
        public esriDataChangeType DataChangeType;

        /// <summary>
        ///     Indicates if the row represents a feature class.
        /// </summary>
        public bool IsFeatureClass;

        /// <summary>
        ///     The OID of the row.
        /// </summary>
        public int OID;

        /// <summary>
        ///     The name of the table.
        /// </summary>
        public string TableName;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeltaRow" /> struct.
        /// </summary>
        /// <param name="dataChangeType">Type of the data change.</param>
        /// <param name="oid">The object id of the row.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="isFeatureClass">if set to <c>true</c> if the row represents a feature class.</param>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        public DeltaRow(esriDataChangeType dataChangeType, int oid, string tableName, bool isFeatureClass, IVersion source, IVersion target)
        {
            this.DataChangeType = dataChangeType;
            this.OID = oid;
            this.TableName = tableName;
            this.IsFeatureClass = isFeatureClass;

            _TargetWorkspace = (IWorkspace) target;
            _SourceWorkspace = (IWorkspace) source;
            _CommonAncestorWorkspace = (IWorkspace) ((IVersion2) source).GetCommonAncestor(target);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the row from the specified workspace.
        /// </summary>
        /// <param name="rowState">The state representing the child or parent or common, depending on the row state needed.</param>
        /// <returns>
        ///     Returns a <see cref="IRow" /> representing the row.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The row state is not supported.</exception>
        public IRow GetRow(RowState rowState = RowState.ChildVersion)
        {
            IWorkspace workspace;

            switch (rowState)
            {
                case RowState.ChildVersion:
                    workspace = _SourceWorkspace;
                    break;

                case RowState.ParentVersion:
                    workspace = _TargetWorkspace;
                    break;

                case RowState.CommonAncestorVersion:
                    workspace = _CommonAncestorWorkspace;
                    break;

                default:
                    throw new NotSupportedException("The row state is not supported.");
            }

            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureWorkspace fws = (IFeatureWorkspace) workspace;
                ITable table = fws.OpenTable(this.TableName);
                cr.ManageLifetime(table);

                try
                {
                    return table.GetRow(this.OID);
                }
                catch (COMException ex)
                {
                    switch (ex.ErrorCode)
                    {
                        case (int) fdoError.FDO_E_FEATURE_NOT_FOUND:
                        case (int) fdoError.FDO_E_ROW_NOT_FOUND:
                            return null;
                        default:
                            throw;
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region Nested Type: DifferenceRow

    /// <summary>
    ///     A struct that represents the row differences based on the difference cursors.
    /// </summary>
    public struct DifferenceRow
    {
        #region Fields

        /// <summary>
        ///     A reference to the row that has been modified.
        /// </summary>
        public IRow Changed;

        /// <summary>
        ///     The type of difference.
        /// </summary>
        public esriDifferenceType DifferenceType;

        /// <summary>
        ///     The field indices that have been changed.
        /// </summary>
        public ILongArray FieldIndices;

        /// <summary>
        ///     The OID of the row.
        /// </summary>
        public int OID;

        /// <summary>
        ///     The reference to the row prior to the changes.
        /// </summary>
        public IRow Original;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DifferenceRow" /> struct.
        /// </summary>
        /// <param name="differenceType">Type of the difference.</param>
        /// <param name="oid">The object id of the row.</param>
        /// <param name="differenceRow">The read-only row reference, which can be null if it was deleted.</param>
        public DifferenceRow(esriDifferenceType differenceType, int oid, IRow differenceRow)
        {
            this.DifferenceType = differenceType;
            this.OID = oid;
            this.Changed = differenceRow;
            this.Original = null;
            this.FieldIndices = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DifferenceRow" /> struct.
        /// </summary>
        /// <param name="differenceType">Type of the difference.</param>
        /// <param name="oid">The object id of the row.</param>
        /// <param name="differenceRow">The read-only row reference, which can be null if it was deleted.</param>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="fieldIndices">An array with indices of fields with different values.</param>
        public DifferenceRow(esriDifferenceType differenceType, int oid, IRow differenceRow, IRow sourceRow, ILongArray fieldIndices)
            : this(differenceType, oid, differenceRow)
        {
            this.Original = sourceRow;
            this.FieldIndices = fieldIndices;
        }

        #endregion
    }

    #endregion
}