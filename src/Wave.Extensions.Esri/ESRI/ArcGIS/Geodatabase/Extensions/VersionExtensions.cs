using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
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
        ///     Creates a <see cref="IEnumerable{IVersionInfo}" /> from the enumeration.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the enumeration of version information.
        /// </returns>
        public static IEnumerable<IVersionInfo> AsEnumerable(this IEnumVersionInfo source)
        {
            if (source != null)
            {
                source.Reset();
                IVersionInfo vi;
                while ((vi = source.Next()) != null)
                    yield return vi;
            }
        }

        /// <summary>
        ///     Creates an enumeration of the <see cref="IEnumModifiedClassInfo" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IModifiedClassInfo}" /> representing the items.</returns>
        public static IEnumerable<IModifiedClassInfo> AsEnumerable(this IEnumModifiedClassInfo source)
        {
            if (source != null)
            {
                source.Reset();
                IModifiedClassInfo mci;
                while ((mci = source.Next()) != null)
                {
                    yield return mci;
                }
            }
        }

        /// <summary>
        ///     Creates the version or returns the version that already exists.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="access">The access.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        ///     Returns a <see cref="IVersion" /> representing the version.
        /// </returns>
        public static IVersion CreateVersion(this IVersionedWorkspace source, string name, esriVersionAccess access, string description)
        {
            try
            {
                var version = source.DefaultVersion.CreateVersion(name);
                version.Access = access;

                if (!string.IsNullOrEmpty(description))
                    version.Description = description.Length > 64 ? description.Substring(0, 64) : description; // The size limit on the description is 62 characters.

                return version;
            }
            catch (COMException e)
            {
                if (e.ErrorCode == (int)fdoError.FDO_E_VERSION_ALREADY_EXISTS)
                    return source.GetVersion(name);

                throw;
            }
        }

        /// <summary>
        ///     Exports the version differences to the specified export file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="exportFileName">Name of the export file.</param>
        /// <param name="exportOption">The export option.</param>
        /// <param name="overwrite">if set to <c>true</c> when the delta file should be overwritten when it exists.</param>
        public static void ExportDataChanges(this IVersion source, IVersion target, string exportFileName, esriExportDataChangesOption exportOption, bool overwrite)
        {
            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            IExportDataChanges2 edc = new DataChangesExporterClass();
            edc.ExportDataChanges(exportFileName, exportOption, (IDataChanges)vdci, overwrite);
        }

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
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     dataChangeTypes
        /// </exception>
        public static Dictionary<string, DeltaRowCollection> GetDataChanges(this IVersion source, IVersion target, params esriDataChangeType[] dataChangeTypes)
        {
            if (source == null) return null;
            if (target == null) throw new ArgumentNullException("target");
            if (dataChangeTypes == null) throw new ArgumentNullException("dataChangeTypes");

            return source.GetDataChanges(target, (mci) => mci.ClassID > 0, dataChangeTypes);
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
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     predicate
        ///     or
        ///     dataChangeTypes
        /// </exception>
        public static Dictionary<string, DeltaRowCollection> GetDataChanges(this IVersion source, IVersion target, Func<IModifiedClassInfo, bool> predicate, params esriDataChangeType[] dataChangeTypes)
        {
            if (source == null) return null;
            if (target == null) throw new ArgumentNullException("target");
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (dataChangeTypes == null) throw new ArgumentNullException("dataChangeTypes");

            var list = new Dictionary<string, DeltaRowCollection>(StringComparer.Create(CultureInfo.InvariantCulture, true));

            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            IDataChanges dataChanges = (IDataChanges)vdci;
            IDataChangesInfo dci = (IDataChangesInfo)vdci;

            var tasks = new List<Action>();

            IEnumModifiedClassInfo enumMci = dataChanges.GetModifiedClassesInfo();
            foreach (var mci in enumMci.AsEnumerable())
            {
                // Validate that the table needs to be loaded.
                if (predicate(mci))
                {
                    tasks.Add(() =>
                    {
                        string tableName = mci.ChildClassName;
                        var rows = new DeltaRowCollection(mci, source, target);
                        var actions = new List<Action>();

                        // Determines if the table represents a feature class.
                        bool isFeatureClass = mci.DatasetType == esriDatasetType.esriDTFeatureClass;

                        // Iterate through all of the data change types.
                        foreach (var dataChangeType in dataChangeTypes)
                        {
                            actions.Add(() =>
                            {
                                // IRow objects returned from a difference cursor are meant to be a read only. Thus, only the OIDs are being loaded and
                                // the rows are hydrated from the struct.
                                IFIDSet set = dci.ChangedIDs[tableName, dataChangeType];
                                set.Reset();

                                int oid;
                                set.Next(out oid);
                                while (oid != -1)
                                {
                                    var row = new DeltaRow(dataChangeType, oid, tableName, isFeatureClass);
                                    rows.Add(row);

                                    set.Next(out oid);
                                }
                            });
                        }

                        if (actions.Any())
                        {
                            Task.WaitAll(actions);
                        }

                        list.Add(tableName, rows);
                    });
                }
            }

            if (tasks.Any())
            {
                Task.WaitAll(tasks);
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
        /// <returns>
        ///     Returns a <see cref="IEnumerable{DifferenceRow}" /> representing the differences for the table.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     table
        ///     or
        ///     differenceTypes
        /// </exception>
        public static IEnumerable<DifferenceRow> GetDifferences(this IVersionedTable source, ITable table, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (differenceTypes == null) throw new ArgumentNullException("differenceTypes");

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
                            : (ITable)source;

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
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     differenceTypes
        /// </exception>
        public static Dictionary<string, IEnumerable<DifferenceRow>> GetDifferences(this IVersion source, IVersion target, IQueryFilter filter, params esriDifferenceType[] differenceTypes)
        {
            if (source == null) return null;
            if (target == null) throw new ArgumentNullException("target");
            if (differenceTypes == null) throw new ArgumentNullException("differenceTypes");

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
        /// <exception cref="System.ArgumentNullException">
        ///     target
        ///     or
        ///     differenceTypes
        /// </exception>
        public static Dictionary<string, IEnumerable<DifferenceRow>> GetDifferences(this IVersion source, IVersion target, IQueryFilter filter, Func<string, ITable, bool> predicate, params esriDifferenceType[] differenceTypes)
        {
            if (source == null) return null;
            if (target == null) throw new ArgumentNullException("target");
            if (differenceTypes == null) throw new ArgumentNullException("differenceTypes");

            var list = new Dictionary<string, IEnumerable<DifferenceRow>>();

            IVersionDataChangesInit vdci = new VersionDataChangesClass();
            IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)source).FullName;
            IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)target).FullName;
            vdci.Init(wsNameSource, wsNameTarget);

            IDataChanges dataChanges = (IDataChanges)vdci;
            IEnumModifiedClassInfo enumMci = dataChanges.GetModifiedClassesInfo();
            enumMci.Reset();
            IModifiedClassInfo mci;
            while ((mci = enumMci.Next()) != null)
            {
                // The table references are not disposed due to the enumerable return which would result in an RCW exception.
                string tableName = mci.ChildClassName;
                ITable sourceTable = ((IFeatureWorkspace)source).OpenTable(tableName);
                if (predicate(tableName, sourceTable))
                {
                    IVersionedTable versionedTable = sourceTable as IVersionedTable;
                    ITable table = ((IFeatureWorkspace)target).OpenTable(tableName);
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
        ///     Finds the <see cref="IFeatureClass" /> with the specified <paramref name="tableName" /> in the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">tableName</exception>
        public static IFeatureClass GetFeatureClass(this IVersion source, string tableName)
        {
            return ((IWorkspace)source).GetFeatureClass(tableName);
        }


        /// <summary>
        ///     Gets the parent version.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IVersion" /> representing the parent version.</returns>
        public static IVersion GetParent(this IVersion source)
        {
            if (source.HasParent())
            {
                return ((IVersionedWorkspace)source).FindVersion(source.VersionInfo.Parent.VersionName);
            }

            return null;
        }


        /// <summary>
        ///     Finds the <see cref="ITable" /> with the specified <paramref name="tableName" /> in the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">tableName</exception>
        public static ITable GetTable(this IVersion source, string tableName)
        {
            return ((IWorkspace)source).GetTable(tableName);
        }


        /// <summary>
        ///     Gets the version.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns a <see cref="IVersion" /> representing the version.</returns>
        public static IVersion GetVersion(this IVersionedWorkspace source, string name)
        {
            try
            {
                var version = source.FindVersion(name);
                version.RefreshVersion();

                return version;
            }
            catch (COMException e)
            {
                if (e.ErrorCode == (int)fdoError.FDO_E_VERSION_NOT_FOUND
                    || e.ErrorCode == (int)fdoError.FDO_E_SE_VERSION_NOEXIST)
                    return null;

                throw;
            }
        }

        /// <summary>
        ///    Deletes the version (when it exists).
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        public static void DeleteVersion(this IVersionedWorkspace source, string name)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                var version = source.GetVersion(name);
                cr.ManageLifetime(version);

                if (version != null)
                {
                    version.Delete();
                }
            }
        }

        /// <summary>
        ///     Gets the locks.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ILockInfo}" /> representing the locks.</returns>
        public static IEnumerable<ILockInfo> GetVersionLocks(this IVersion source)
        {
            IEnumLockInfo enumLockInfo = source.VersionLocks;
            ILockInfo lockinfo;

            while ((lockinfo = enumLockInfo.Next()) != null)
            {
                yield return lockinfo;
            }
        }

        /// <summary>
        ///     Determines whether this instance is locked.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     <c>true</c> if the specified source is locked; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLocked(this IVersion source)
        {
            return source.GetVersionLocks().Any();
        }

        /// <summary>
        /// Encapsulates the <paramref name="operation" /> by the necessary start and stop edit constructs using the specified
        /// <paramref name="withUndoRedo" />s
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">if set to <c>true</c> when the changes are reverted when the edits are aborted.</param>
        /// <param name="operation">The edit operation delegate that handles making the necessary edits. When the delegate returns
        /// <c>true</c> the edits will be saved; otherwise they will not be saved.</param>
        /// <returns>
        /// Returns a <see cref="bool" /> representing the state of the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static bool PerformOperation(this IVersion source, bool withUndoRedo, Func<Action, bool> operation)
        {
            return ((IWorkspaceEdit)source).PerformOperation(withUndoRedo, operation, error => false);
        }

        /// <summary>
        /// Encapsulates the <paramref name="operation" /> by the necessary start and stop edit constructs using the specified
        /// <paramref name="withUndoRedo" />s
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">if set to <c>true</c> when the changes are reverted when the edits are aborted.</param>
        /// <param name="operation">The edit operation delegate that handles making the necessary edits. When the delegate returns
        /// <c>true</c> the edits will be saved; otherwise they will not be saved.</param>
        /// <returns>
        /// Returns a <see cref="bool" /> representing the state of the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public static bool PerformOperation(this IVersion source, bool withUndoRedo, Func<bool> operation)
        {
            return ((IWorkspaceEdit)source).PerformOperation(withUndoRedo, operation, error => false);
        }

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> by the necessary start and stop edit constructs using the specified
        ///     <paramref name="withUndoRedo" /> and
        ///     <paramref name="multiuserEditSessionMode" /> parameters.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">if set to <c>true</c> when the changes are reverted when the edits are aborted.</param>
        /// <param name="multiuserEditSessionMode">
        ///     The edit session mode that can be used to indicate non-versioned or versioned
        ///     editing for workspaces that support multiuser editing.
        /// </param>
        /// <param name="operation">
        ///     The edit operation delegate that handles making the necessary edits. When the delegate returns
        ///     <c>true</c> the edits will be saved; otherwise they will not be saved.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing the state of the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="System.ArgumentException">
        ///     The workspace does not support the edit session
        ///     mode.;multiuserEditSessionMode
        /// </exception>
        public static bool PerformOperation(this IVersion source, bool withUndoRedo, esriMultiuserEditSessionMode multiuserEditSessionMode, Func<bool> operation)
        {
            return ((IWorkspace)source).PerformOperation(withUndoRedo, multiuserEditSessionMode, operation, error => false);
        }

        #endregion
    }

    #region Nested Type: DeltaRowCollection

    /// <summary>
    ///     Provides a collection of <see cref="DeltaRow" /> objects.
    /// </summary>
    public class DeltaRowCollection : KeyedCollection<int, DeltaRow>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeltaRowCollection" /> class.
        /// </summary>
        /// <param name="modifiedClassInfo">The modified class information.</param>
        /// <param name="source">The source (or child) version.</param>
        /// <param name="target">The target (or parent) version.</param>
        public DeltaRowCollection(IModifiedClassInfo modifiedClassInfo, IVersion source, IVersion target)
        {
            this.ModifiedClassInfo = modifiedClassInfo;
            this.SourceVersion = source;
            this.TargetVersion = target;
            this.CommonAncestorVersion = target == null ? null : ((IVersion2)source).GetCommonAncestor(target);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeltaRowCollection" /> class.
        /// </summary>
        /// <param name="modifiedClassInfo">The modified class information.</param>
        /// <param name="source">The source.</param>
        public DeltaRowCollection(IModifiedClassInfo modifiedClassInfo, IVersion source)
            : this(modifiedClassInfo, source, null)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the data change types.
        /// </summary>
        /// <value>
        ///     The data change types.
        /// </value>
        public esriDataChangeType[] DataChangeTypes
        {
            get { return this.Select(o => o.DataChangeType).Distinct().ToArray(); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is feature class.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is feature class; otherwise, <c>false</c>.
        /// </value>
        public bool IsFeatureClass
        {
            get { return this.All(o => o.IsFeatureClass); }
        }

        /// <summary>
        ///     Gets the common ancestor version.
        /// </summary>
        /// <value>
        ///     The common ancestor version.
        /// </value>
        public IVersion CommonAncestorVersion { get; private set; }


        /// <summary>
        ///     Gets the modified class information.
        /// </summary>
        /// <value>
        ///     The modified class information.
        /// </value>
        public IModifiedClassInfo ModifiedClassInfo { get; private set; }

        /// <summary>
        ///     Gets the source (or current) version.
        /// </summary>
        /// <value>
        ///     The source (or current) version.
        /// </value>
        public IVersion SourceVersion { get; private set; }

        /// <summary>
        ///     Gets the target version.
        /// </summary>
        /// <value>
        ///     The target version.
        /// </value>
        public IVersion TargetVersion { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the rows that reside in the version that corresponds to the state.
        /// </summary>
        /// <param name="changeVersion">The change version.</param>
        /// <param name="deltaRow">The delta row.</param>
        /// <returns>
        ///     Returns a <see cref="IRow" /> representing the row for the state.
        /// </returns>
        public IRow GetRow(esriChangeVersion changeVersion, DeltaRow deltaRow)
        {
            return this.GetRow(changeVersion, deltaRow.OID);
        }

        /// <summary>
        ///     Gets the rows that reside in the version that corresponds to the state.
        /// </summary>
        /// <param name="changeVersion">The change version.</param>
        /// <param name="oid">The oid.</param>
        /// <returns>
        ///     Returns a <see cref="IRow" /> representing the row for the state.
        /// </returns>
        public IRow GetRow(esriChangeVersion changeVersion, int oid)
        {
            IFeatureWorkspace workspace = this.GetWorkspace(changeVersion);
            if (workspace == null) return null;

            using (ComReleaser cr = new ComReleaser())
            {
                ITable table = workspace.OpenTable(this.ModifiedClassInfo.ChildClassName);
                cr.ManageLifetime(table);

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = string.Format("{0} = {1}", table.OIDFieldName, oid);


                var cursor = table.Search(filter, false);
                cr.ManageLifetime(cursor);

                foreach (var row in cursor.AsEnumerable())
                    return row;
            }

            return null;
        }

        /// <summary>
        ///     Gets the rows that have differences in the given fields.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the rows that have differences.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     sourceFieldName
        ///     or
        ///     targetFieldName
        /// </exception>
        public IEnumerable<IRow> GetRows(string fieldName)
        {
            var equailityComparer = new FieldsEqualityComparer();

            foreach (var source in this.GetRows(esriChangeVersion.esriChangeSourceVersion))
            {
                var s = source.Fields.FindField(fieldName);
                if (s == -1)
                    throw new ArgumentOutOfRangeException("fieldName");

                foreach (var target in this.GetRows(esriChangeVersion.esriChangeTargetVersion))
                {
                    var t = target.Fields.FindField(fieldName);
                    if (t == -1)
                        throw new ArgumentOutOfRangeException("fieldName");

                    if (!equailityComparer.Equals(source, target, s))
                        yield return source;
                }
            }
        }

        /// <summary>
        ///     Gets the rows that reside in the version that corresponds to the state.
        /// </summary>
        /// <param name="changeVersion">The change version.</param>
        /// <param name="dataChangeTypes">The data change types.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IRow}" /> representing the rows for the state.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The row state is not supported.</exception>
        public IEnumerable<IRow> GetRows(esriChangeVersion changeVersion, params esriDataChangeType[] dataChangeTypes)
        {
            var rows = this.GetRows(dataChangeTypes);
            return rows.GetRows(changeVersion);
        }

        /// <summary>
        ///     Gets the rows that pertain to the data change type.
        /// </summary>
        /// <param name="dataChangeTypes">Type of the data changes.</param>
        /// <returns>Retursn a <see cref="DeltaRowCollection" /> representing the delta rows for the data change type.</returns>
        public DeltaRowCollection GetRows(params esriDataChangeType[] dataChangeTypes)
        {
            var rows = this.Where(deltaRow => dataChangeTypes.Contains(deltaRow.DataChangeType));

            var collection = new DeltaRowCollection(this.ModifiedClassInfo, this.SourceVersion, this.TargetVersion);
            collection.AddRange(rows);

            return collection;
        }

        /// <summary>
        ///     Combines the rows from the collections in into a single collection
        /// </summary>
        /// <param name="collections">The collections.</param>
        /// <returns>
        ///     Retursn a <see cref="DeltaRowCollection" /> representing the delta rows for the data change type.
        /// </returns>
        public DeltaRowCollection GetRows(params DeltaRowCollection[] collections)
        {
            var collection = new DeltaRowCollection(this.ModifiedClassInfo, this.SourceVersion, this.TargetVersion);

            foreach (var rows in collections)
                collection.AddRange(rows);

            return collection;
        }

        /// <summary>
        ///     Gets the rows that reside in the current version.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IRow}" /> representing the rows for the state.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The row state is not supported.</exception>
        public IEnumerable<IRow> GetRows()
        {
            return this.GetRows(esriChangeVersion.esriChangeSourceVersion);
        }

        /// <summary>
        ///     Gets the rows that reside in the version that corresponds to the state.
        /// </summary>
        /// <param name="changeVersion">The change version.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IRow}" /> representing the rows for the state.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The row state is not supported.</exception>
        public IEnumerable<IRow> GetRows(esriChangeVersion changeVersion)
        {
            IFeatureWorkspace workspace = this.GetWorkspace(changeVersion);
            if (workspace != null)
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    ITable table = workspace.OpenTable(this.ModifiedClassInfo.ChildClassName);

                    foreach (var oids in this.Select(o => o.OID.ToString(CultureInfo.InvariantCulture)).Batch(1000))
                    {
                        IQueryFilter filter = new QueryFilterClass();
                        filter.WhereClause = string.Format("{0} IN ({1})", table.OIDFieldName, string.Join(",", oids.ToArray()));

                        var cursor = table.Search(filter, false);
                        cr.ManageLifetime(cursor);

                        foreach (var row in cursor.AsEnumerable())
                            yield return row;
                    }
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the key for item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override int GetKeyForItem(DeltaRow item)
        {
            return item.OID;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the workspace for the delta row state enumeration
        /// </summary>
        /// <param name="changeVersion">The change version.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureWorkspace" /> representing the workspace for that state
        /// </returns>
        private IFeatureWorkspace GetWorkspace(esriChangeVersion changeVersion)
        {
            switch (changeVersion)
            {
                case esriChangeVersion.esriChangeSourceVersion:
                    return this.SourceVersion as IFeatureWorkspace;

                case esriChangeVersion.esriChangeTargetVersion:
                    return this.TargetVersion as IFeatureWorkspace;

                default:
                    return this.CommonAncestorVersion as IFeatureWorkspace;
            }
        }

        #endregion
    }

    #endregion

    #region Nested Type: DeltaRow

    /// <summary>
    ///     A struct that represents the row differences based on the data changes for extract.
    /// </summary>
    public struct DeltaRow
    {
        #region Fields

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
        public DeltaRow(esriDataChangeType dataChangeType, int oid, string tableName, bool isFeatureClass)
        {
            this.DataChangeType = dataChangeType;
            this.OID = oid;
            this.TableName = tableName;
            this.IsFeatureClass = isFeatureClass;
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

        #region Public Methods

        /// <summary>
        ///     Gets the row from the specified workspace.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="workspace">The workspace representing the child or parent, depending on the row state needed.</param>
        /// <returns>
        ///     Returns a <see cref="IRow" /> representing the row.
        /// </returns>
        public IRow GetRow(string tableName, IWorkspace workspace)
        {
            using (ComReleaser cr = new ComReleaser())
            {
                IFeatureWorkspace fws = (IFeatureWorkspace)workspace;
                ITable table = fws.OpenTable(tableName);
                cr.ManageLifetime(table);

                return table.GetRow(this.OID);
            }
        }

        #endregion
    }

    #endregion
}