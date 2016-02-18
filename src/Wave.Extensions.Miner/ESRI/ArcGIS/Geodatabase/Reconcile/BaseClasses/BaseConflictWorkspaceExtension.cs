using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.BaseClasses;
using ESRI.ArcGIS.Geodatabase.Internal;
using ESRI.ArcGIS.GeoDatabaseDistributed;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An abstract workpace extension that extends the ESRI reconcile process to allow for resolving conflicts during
    ///     reconcile.
    /// </summary>
    public abstract class BaseConflictWorkspaceExtension : BaseWorkspaceExtension, IConflictWorkspaceExtension, IVersionEvents, IVersionEvents2
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConflictWorkspaceExtension" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        /// <param name="extensionGuid">The extension GUID.</param>
        protected BaseConflictWorkspaceExtension(string extensionName, string extensionGuid)
            : base(extensionName, extensionGuid)
        {
            this.Rows = new List<IConflictRow>();
            this.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            this.IsRemovedAfterResolved = true;
            this.IsSavedAfterReconcile = false;
            this.IsRebuildingConnectivity = false;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the rows that were (or still are) in conflict.
        /// </summary>
        /// <value>
        ///     The rows.
        /// </value>
        protected IList<IConflictRow> Rows { get; private set; }

        #endregion

        #region IConflictWorkspaceExtension Members

        /// <summary>
        ///     Gets or sets the auto updater mode.
        /// </summary>
        /// <value>
        ///     The auto updater mode.
        /// </value>
        public mmAutoUpdaterMode AutoUpdaterMode { get; set; }

        /// <summary>
        ///     Gets or sets the callback that is used to notify the caller of progress changes.
        /// </summary>
        /// <value>
        ///     The callback.
        /// </value>
        public IMMMessageCallback Callback { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the child conflicts should over rule the targets.
        /// </summary>
        /// <value>
        ///     <c>true</c> if child conflicts should over rule the targets; otherwise, <c>false</c>.
        /// </value>
        public bool ChildWins { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether conflicts will be defined at the column level.
        /// </summary>
        /// <value>
        ///     <c>true</c> if conflicts will be defined at the column level; otherwise, <c>false</c>.
        /// </value>
        public bool ColumnLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is rebuildingin the connectivity of the network features
        ///     before reconcile.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is rebuildingin the connectivity of the network features before reconcile; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool IsRebuildingConnectivity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the conflicts that have been resolved are removed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the conflicts that have been resolved are removed; otherwise, <c>false</c>.
        /// </value>
        public bool IsRemovedAfterResolved { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the conflicts that have been resolved should be saved again (with AUs)
        ///     after the reconcile completes.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the conflicts that have been resolved should be saved again (with AUs) after the reconcile
        ///     completes; otherwise, <c>false</c>.
        /// </value>
        public bool IsSavedAfterReconcile { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether target version is locked during reconcile.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the target version is locked during reconcile; otherwise, <c>false</c>.
        /// </value>
        public bool LockTarget { get; set; }

        /// <summary>
        ///     Returns the filters that will be used for resolving the reconcile conflicts.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>
        ///     Returns a
        ///     <see cref="T:System.Collections.Generic.IList{ESRI.ArcGIS.Geodatabase.IConflictFilter}" />
        ///     implementations that are used to resolve the row conflicts.
        /// </returns>
        /// <remarks>
        ///     This method will only be called, if there are conflicts detected by ESRI.
        /// </remarks>
        public abstract IList<IConflictFilter> GetFilters(IWorkspace workspace);

        #endregion

        #region IVersionEvents Members

        /// <summary>
        ///     This event is fired during reconciliation, after conflicts are detected. It can be used by application developers
        ///     to filter found conflicts.
        /// </summary>
        /// <param name="conflictsRemoved">if set to <c>true</c> if conflicts were removed.</param>
        /// <param name="errorOccurred">if set to <c>true</c> an error occurred.</param>
        /// <param name="errorString">The error string.</param>
        /// <remarks>
        ///     When this method is called the ESRI reconcile has already been completed.
        ///     Meaning that ESRI has already automatically resolved any conflicts.
        ///     These resolutions can potentially delete or add records.
        ///     Thus, the records that have been added or edited in the edit version could be not found during this process.
        /// </remarks>
        public void OnConflictsDetected(ref bool conflictsRemoved, ref bool errorOccurred, ref string errorString)
        {
            try
            {
                IList<IConflictFilter> filters = this.GetFilters(this.Workspace);
                if (filters == null || filters.Count == 0)
                    return;

                IVersionEdit3 versionEdit = (IVersionEdit3) this.Workspace;
                IFeatureWorkspace currentWorkspace = (IFeatureWorkspace) versionEdit; // The version being edited.
                IFeatureWorkspace preReconcileWorkspace = (IFeatureWorkspace) versionEdit.PreReconcileVersion; // The version prior to reconciliation.
                IFeatureWorkspace reconcileWorkspace = (IFeatureWorkspace) versionEdit.ReconcileVersion; // The version that the current version is reconciling against.
                IFeatureWorkspace commonAncestorWorkspace = (IFeatureWorkspace) versionEdit.CommonAncestorVersion; // The common ancestor of this version and the reconcile version.

                using (new AutoUpdaterModeReverter(this.AutoUpdaterMode))
                {
                    var conflictsResolved = true;
                    var conflictClasses = this.GetResolutionOrder(versionEdit.ConflictClasses);
                    var conflictCount = this.GetConflictCount(conflictClasses);

                    this.NotifyCallback(3, "Resolving {0} row conflict(s) spread across {1} classes.", conflictCount, conflictClasses.Count);

                    foreach (var conflictClass in conflictClasses)
                    {
                        using (ComReleaser cr = new ComReleaser())
                        {
                            string tableName = ((IDataset) conflictClass).Name;

                            ITable currentTable = currentWorkspace.OpenTable(tableName);
                            cr.ManageLifetime(currentTable);

                            ITable preReconcileTable = preReconcileWorkspace.OpenTable(tableName);
                            cr.ManageLifetime(preReconcileTable);

                            ITable reconcileTable = reconcileWorkspace.OpenTable(tableName);
                            cr.ManageLifetime(reconcileTable);

                            ITable commonAncestorTable = commonAncestorWorkspace.OpenTable(tableName);
                            cr.ManageLifetime(commonAncestorTable);

                            conflictsResolved = conflictsResolved & this.ResolveConflicts(conflictClass, currentTable, preReconcileTable, reconcileTable, commonAncestorTable, TableConflictType.DeleteUpdates, filters);

                            conflictsResolved = conflictsResolved & this.ResolveConflicts(conflictClass, currentTable, preReconcileTable, reconcileTable, commonAncestorTable, TableConflictType.UpdateDeletes, filters);

                            conflictsResolved = conflictsResolved & this.ResolveConflicts(conflictClass, currentTable, preReconcileTable, reconcileTable, commonAncestorTable, TableConflictType.UpdateUpdates, filters);
                        }
                    }

                    conflictsRemoved = conflictsResolved;
                }
            }
            catch (Exception ex)
            {
                conflictsRemoved = false;
                errorOccurred = true;
                errorString = ex.Message;

                Log.Error(this, ex);
            }
        }

        /// <summary>
        ///     This event is fired after the version is reconciled, associating it with a new database state. Applications must
        ///     discard or refresh any cached row objects.
        /// </summary>
        /// <param name="targetVersionName">Name of the target version.</param>
        /// <param name="hasConflicts">if set to <c>true</c> if there are conflicts.</param>
        public void OnReconcile(string targetVersionName, bool hasConflicts)
        {
            ConflictResolution[] values = (ConflictResolution[]) Enum.GetValues(typeof (ConflictResolution));
            foreach (var value in values)
            {
                Log.Info(this, "There were {0} conflict(s) marked as '{1}' for the resolution.", this.Rows.Count(o => o.Resolution == value), value);
            }

            if (this.IsSavedAfterReconcile)
            {
                this.SaveAfterReconcile();
            }

            this.NotifyCallback(5, "The version has been reconciled.");
            this.AfterReconcile(this.Rows, targetVersionName, hasConflicts);
        }

        /// <summary>
        ///     This event is fired after the version is changed in place to represent a different version, associating it with a
        ///     new database state. Applications must discard or refresh any cached row objects.
        /// </summary>
        /// <param name="oldVersionName">Old name of the version.</param>
        /// <param name="newVersionName">New name of the version.</param>
        public virtual void OnRedefineVersion(string oldVersionName, string newVersionName)
        {
        }

        /// <summary>
        ///     This event is fired after the version is refreshed, associating it with a new database state. Applications must
        ///     discard or refresh any cached row objects.
        /// </summary>
        public virtual void OnRefreshVersion()
        {
        }

        #endregion

        #region IVersionEvents2 Members

        /// <summary>
        ///     This event is fired before a version is reconciled.
        /// </summary>
        /// <param name="targetVersionName">Name of the target version.</param>
        /// <remarks>The version will be reconciled after this method.</remarks>
        public void OnBeginReconcile(string targetVersionName)
        {
            this.BeforeReconcile(targetVersionName);

            this.NotifyCallback();

            if (this.IsRebuildingConnectivity)
            {
                this.RebuildConnectivity(targetVersionName);
            }

            this.NotifyCallback(2, "The version is being reconciled.");
        }

        /// <summary>
        ///     This event is fired after the historical archive has been updated with changes saved or posted to the DEFAULT
        ///     version.
        /// </summary>
        /// <param name="archiveTransactionTime">The archive transaction time.</param>
        public virtual void OnArchiveUpdated(object archiveTransactionTime)
        {
        }

        /// <summary>
        ///     This event is fired before a version is deleted
        /// </summary>
        /// <param name="versionName">Name of the version.</param>
        public virtual void OnDeleteVersion(string versionName)
        {
        }

        /// <summary>
        ///     This event is fired after a version is posted
        /// </summary>
        /// <param name="targetVersionName">Name of the target version.</param>
        public virtual void OnPost(string targetVersionName)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Occurs once the reconciliation has been completed.
        /// </summary>
        /// <param name="rows">The rows that were (or still are) in conflict.</param>
        /// <param name="targetVersionName">Name of the target version.</param>
        /// <param name="hasConflicts">if set to <c>true</c> if there are conflicts.</param>
        protected virtual void AfterReconcile(IList<IConflictRow> rows, string targetVersionName, bool hasConflicts)
        {
        }

        /// <summary>
        ///     Gets the selection set that corresponds to the conflict class.
        /// </summary>
        /// <param name="conflictClass">The conflict class.</param>
        /// <param name="conflictType">Type of the conflict.</param>
        /// <returns>Returns the <see cref="ISelectionSet" /> representing the set of conflicts.</returns>
        protected ISelectionSet GetConflictSet(IConflictClass conflictClass, TableConflictType conflictType)
        {
            if (conflictClass == null) throw new ArgumentNullException("conflictClass");

            ISelectionSet set = null;
            switch (conflictType)
            {
                case TableConflictType.DeleteUpdates:
                    set = conflictClass.DeleteUpdates;
                    break;

                case TableConflictType.UpdateDeletes:
                    set = conflictClass.UpdateDeletes;
                    break;

                case TableConflictType.UpdateUpdates:
                    set = conflictClass.UpdateUpdates;
                    break;
            }

            return set;
        }

        /// <summary>
        ///     Returns a list of the classes that are in conflict in the recommended resolution order.
        /// </summary>
        /// <param name="enumConflictClasses">The enumeration of the classes in conflict.</param>
        /// <returns>Returns an enumerable list of the classes in recommended order.</returns>
        protected IList<IConflictClass> GetResolutionOrder(IEnumConflictClass enumConflictClasses)
        {
            List<ConflictClass> list = new List<ConflictClass>();
            if (enumConflictClasses == null) return new IConflictClass[] {};

            enumConflictClasses.Reset();
            IConflictClass conflictClass;
            while ((conflictClass = enumConflictClasses.Next()) != null)
            {
                if (conflictClass.HasConflicts)
                {
                    list.Add(new ConflictClass(conflictClass));
                }
            }

            // Order by Non-Network Junctions and Edges.
            var nonnetwork = list.Where(o => (o.IsFeatureClass && !o.IsNetworkClass) && (o.Type == ClassType.Point || o.Type == ClassType.Line))
                .OrderBy(o => o.Type == ClassType.Line)
                .ThenBy(o => o.Type == ClassType.Point)
                .Select(o => o.Class);

            // Order by Network Junctions and Edges.
            var network = list.Where(o => o.IsNetworkClass && (o.Type == ClassType.Point || o.Type == ClassType.Line))
                .OrderBy(o => o.Type == ClassType.Line)
                .ThenBy(o => o.Type == ClassType.Point)
                .Select(o => o.Class);

            // Order by Stand-alone Tables.
            var table = list.Where(o => o.Type == ClassType.Table)
                .Select(o => o.Class);

            // Order by Relationships.
            var relationship = list.Where(o => o.Type == ClassType.Relationship)
                .Select(o => o.Class);

            // Order by Annotation.
            var anno = list.Where(o => o.Type == ClassType.Annotation)
                .Select(o => o.Class);

            // Return a list of the conflict classes in the "ArcFM" recommended conflict resolution order.
            var array = new List<IConflictClass>();
            array.AddRange(nonnetwork);
            array.AddRange(network);
            array.AddRange(table);
            array.AddRange(relationship);
            array.AddRange(anno);

            return array;
        }

        /// <summary>
        ///     Gets the type of the row conflict at a granular level.
        /// </summary>
        /// <param name="preReconcileRow">The row prior to reconciliation or edit (child) version (these are edits that you made).</param>
        /// <param name="reconcileRow">The row that the current version is reconciling against or target (parent) version.</param>
        /// <param name="commonAncestorRow">
        ///     The common ancestor row of this version and the reconcile version (as they are in the
        ///     database; this is what the row and attributes were before any edits were made).
        /// </param>
        /// <param name="comparer">
        ///     The comparer used to compare the <paramref name="preReconcileRow" />,
        ///     <paramref name="reconcileRow" /> and <paramref name="commonAncestorRow" /> rows.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="RowConflictType" /> enumeration representing the type of conflict at the granular level.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">comparer</exception>
        protected virtual RowConflictType GetRowConflictType(IRow preReconcileRow, IRow reconcileRow, IRow commonAncestorRow, IEqualityComparer<IRow> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");

            // When the common ancestor row is null (meaning it never existed in the database prior to editing); It must be an "new" insert.
            if (commonAncestorRow == null)
            {
                if (preReconcileRow != null && reconcileRow == null)
                {
                    // When the target row is null; It must be an "new" insert in the edit version.
                    return RowConflictType.InsertPreReconcile;
                }

                if (preReconcileRow == null && reconcileRow != null)
                {
                    // When the edit row is null; It must be an "new" insert in the target version.
                    return RowConflictType.InsertReconcile;
                }

                // When they are all null; It must be a "delete" in both versions.
                return RowConflictType.DeletePreReconcileDeleteReconcile;
            }

            if (preReconcileRow == null && reconcileRow != null)
            {
                // When the edit row is null and the target row is provided.
                if (comparer.Equals(reconcileRow, commonAncestorRow))
                {
                    // When the target row and common ancestor row have the same field values; It must be a "delete" in edit.
                    return RowConflictType.DeletePreReconcile;
                }

                // When the target row and common ancestor row have the differing field values; It must be a "delete" in edit and "update" in target.
                return RowConflictType.DeletePreReconcileUpdateReconcile;
            }

            if (preReconcileRow != null && reconcileRow == null)
            {
                // When the target row is null and edit row is provided.
                if (comparer.Equals(preReconcileRow, commonAncestorRow))
                {
                    // When the edit row and common ancestor row have the same field values; It must be a "delete" in target.
                    return RowConflictType.DeleteReconcile;
                }

                // When the edit row and common ancestor row have differing field values; It must be an "update" in edit and "delete" in target.
                return RowConflictType.UpdatePreReconcileDeleteReconcile;
            }

            // It must be an "update" in edit and "update" in target.
            return RowConflictType.UpdatePreReconcileUpdateReconcile;
        }

        /// <summary>
        ///     Initializes the workspace extension before the conflict reconcilition begins.
        /// </summary>
        /// <param name="targetVersionName">Name of the target version.</param>
        protected virtual void BeforeReconcile(string targetVersionName)
        {
            Log.Debug(this, "The {0} will be reconciled with the {1} version will use the following parameters:", ((IVersion) this.Workspace).VersionName, targetVersionName);
            Log.Debug(this, "\t- AutoUpdaterMode = {0}.", this.AutoUpdaterMode);
            Log.Debug(this, "\t- ChildWins = {0}.", this.ChildWins);
            Log.Debug(this, "\t- ColumnLevel = {0}.", this.ColumnLevel);
            Log.Debug(this, "\t- LockTarget = {0}.", this.LockTarget);
            Log.Debug(this, "\t- IsRemovedAfterResolved = {0}.", this.IsRemovedAfterResolved);
            Log.Debug(this, "\t- IsSavedAfterReconcile = {0}.", this.IsSavedAfterReconcile);
            Log.Debug(this, "\t- IsRebuildingConnectivity = {0}.", this.IsRebuildingConnectivity);
        }

        /// <summary>
        ///     Notifies the callback of the progress change and allows the callback to refresh.
        /// </summary>
        /// <param name="progressValue">The progress value.</param>
        protected virtual void NotifyCallback(int progressValue)
        {
            if (this.Callback != null)
            {
                this.Callback.ProgressValue = progressValue;
            }
        }

        /// <summary>
        ///     Notifies the callback of the progress change and the updated status message.
        /// </summary>
        /// <param name="progressValue">The progress value.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        protected virtual void NotifyCallback(int progressValue, string message, params object[] args)
        {
            if (this.Callback != null)
            {
                this.Callback.ProgressValue = progressValue;
                this.Callback.StatusMessage = string.Format(CultureInfo.CurrentCulture, message, args);
            }

            Log.Debug(this, message, args);
        }

        /// <summary>
        ///     Notifies the callback of the potential steps of the progress (some of them are optional).
        /// </summary>
        protected virtual void NotifyCallback()
        {
            if (this.Callback != null)
            {
                // There are potentially 5 different steps in the reconcile process.
                // 1. Rebuilding Connectivity (ArcFM).
                // 2. Reconcile. (ESRI)
                // 3. Conflict Resolution (ArcFM).
                // 4. Save After Reconcile (ArcFM).
                // 5. Done.
                this.Callback.InitProgressBar(0, 5);
            }
        }

        /// <summary>
        ///     Rebuilds the connectivity for the differences between the version with specified
        ///     <paramref name="targetVersionName" /> and the workspace
        ///     associated with the extension.
        /// </summary>
        /// <param name="targetVersionName">Name of the target version.</param>
        protected void RebuildConnectivity(string targetVersionName)
        {
            IVersion targetVersion = this.Workspace.FindVersion(targetVersionName);
            IVersion sourceVersion = (IVersion) this.Workspace;

            this.NotifyCallback(1, "Rebuilding the network connectivity in the version.");

            // Load the differences in the versions (but only those that are network feature classes).
            var dataChanges = sourceVersion.GetDataChanges(targetVersion, (s, t) => (t is INetworkClass), esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate);
            Log.Info(this, "Rebuilding the connectivity of {0} feature classes.", dataChanges.Keys.Count);

            // Iterate through all of the collection of changes.
            foreach (var dc in dataChanges)
            {
                Log.Info(this, "Rebuilding the connectivity of {0} feature(s) in the {1} feature class.", dc.Value.Count, dc.Key);
                this.NotifyCallback(1);

                foreach (var delta in dc.Value)
                {
                    IRow row = delta.GetRow();
                    this.RebuildConnectivity(row as INetworkFeature);
                }
            }
        }

        /// <summary>
        ///     Rebuilds the connectivity for the <paramref name="networkFeature" /> network feature.
        /// </summary>
        /// <param name="networkFeature">The network feature.</param>
        protected virtual void RebuildConnectivity(INetworkFeature networkFeature)
        {
            try
            {
                if (networkFeature == null)
                    return;

                // If network connectivity errors are found within the geometric network, they can generally be corrected through
                // the use of the RebuildConnectivity2 method.
                //
                // The RebuildConnectivity2 method does not check for invalid connectivity, it will remove and then rebuild the
                // of any feature contained within or intersecting the specified envelope.
                //
                // It is generally faster to call RebuildConnectivity2 on small individual areas rather than one large area that
                // encompasses the smaller areas.
                IFeature feature = (IFeature) networkFeature;
                Log.Info(this, "Rebuilding the connectivity for the network feature with the OID of {0}.", feature.OID);

                IGeometricNetworkConnectivity2 networkConnectivity = (IGeometricNetworkConnectivity2) networkFeature.GeometricNetwork;
                networkConnectivity.RebuildConnectivity2(feature.Shape.Envelope);
            }
            catch (COMException e)
            {
                Log.Error(this, "Attempted to rebuild connectivity on a network feature that is outside of the edit session.", e);
            }
        }

        /// <summary>
        ///     The main method for resolving conflicts for all of the rows for the <paramref name="conflictClass" />
        ///     that have the conflict type matching the <paramref name="conflictType" /> enumeration.
        /// </summary>
        /// <param name="conflictClass">The class that has the conflicts.</param>
        /// <param name="currentTable">The table in the current version.</param>
        /// <param name="preReconcileTable">The table prior to reconciliation.</param>
        /// <param name="reconcileTable">The table that the current version is reconciling against.</param>
        /// <param name="commonAncestorTable">The common ancestor table of this version and the reconcile version.</param>
        /// <param name="conflictType">Type of the conflict.</param>
        /// <param name="filters">The conflict filters.</param>
        /// <returns>
        ///     A boolean indicating if all conflicts have been resolved.
        /// </returns>
        protected bool ResolveConflicts(IConflictClass conflictClass, ITable currentTable, ITable preReconcileTable, ITable reconcileTable, ITable commonAncestorTable, TableConflictType conflictType, IEnumerable<IConflictFilter> filters)
        {
            ISelectionSet set = this.GetConflictSet(conflictClass, conflictType);
            if (set == null) return false;

            string tableName = ((IDataset) conflictClass).Name;
            Log.Info(this, "Resolving the '{0}' type of conflicts for {1} row(s) in the {2} class.", conflictType, set.Count, tableName);

            var list = filters.Where(o => o.CanResolve(conflictType, conflictClass)).ToArray();
            if (list.Length == 0)
            {
                Log.Warn(this, "There's no conflict filters available that support the conflict type of {0} for the {1} class.", conflictType, tableName);
                return false;
            }

            FieldsEqualityComparer equalityComparer = new FieldsEqualityComparer();
            List<IConflictRow> rows = new List<IConflictRow>();
            IEnumIDs enumIDs = set.IDs;
            enumIDs.Reset();

            int oid;
            while ((oid = enumIDs.Next()) != -1)
            {
                using (ComReleaser cr = new ComReleaser())
                {
                    // The row from the edited (current) version.
                    IRow currentRow = currentTable.Fetch(oid);
                    cr.ManageLifetime(currentRow);

                    // The row from the edit (child) version.
                    IRow preReconcileRow = preReconcileTable.Fetch(oid);
                    cr.ManageLifetime(preReconcileRow);

                    // The row from the target (parent) version.
                    IRow reconcileRow = reconcileTable.Fetch(oid);
                    cr.ManageLifetime(reconcileRow);

                    // The row from the common ancestor (as is in the database) version.
                    IRow commonAncestorRow = commonAncestorTable.Fetch(oid);
                    cr.ManageLifetime(commonAncestorRow);

                    // Determine the row conflict type at a granular level.
                    RowConflictType rowConflictType = this.GetRowConflictType(preReconcileRow, reconcileRow, commonAncestorRow, equalityComparer);
                    Log.Info(this, "Resolving the '{0}' conflict type for the {1} row using {2} filter(s).", rowConflictType, oid, list.Length);

                    // Use the filters to "attempt" to resolve the conflicts.
                    IConflictRow conflictRow = new ConflictRow(oid, tableName, rowConflictType);
                    foreach (var filter in list.OrderBy(o => o.Priority))
                    {
                        conflictRow.Resolution = filter.Resolve(conflictRow, conflictClass, currentRow, preReconcileRow, reconcileRow, commonAncestorRow, this.ChildWins, this.ColumnLevel);
                    }

                    Log.Info(this, "The resolution of the {0} row has been marked as '{1}'.", oid, conflictRow.Resolution);

                    // Save the changes when a resolution was determined.
                    if (conflictRow.Resolution != ConflictResolution.None)
                    {
                        this.SaveAndRebuild(currentRow);
                    }

                    // Add to the list of rows.
                    rows.Add(conflictRow);
                }
            }

            // Add the rows to the extension collection.
            this.Rows.AddRange(rows);

            // Remove the OIDs of the conflicts that have been resolved.
            int[] oids = rows.Where(o => o.Resolution != ConflictResolution.None).Select(o => o.OID).ToArray();
            if (this.IsRemovedAfterResolved)
            {
                // ESRI states that the RemoveList method on the ISelectionSet should not used from .NET. Instead, call IGeoDatabaseBridge2.RemoveList.
                set.Remove(oids);
            }

            // Output the statistics for the conflicts.
            int remainder = rows.Count - oids.Length;
            Log.Info(this, "{0} of the {1} row(s) have been resolved and {2} remain in conflict.", oids.Length, rows.Count, remainder);

            // Return true when all of the conflicts have been resolved.
            return remainder == 0;
        }

        /// <summary>
        ///     Iterates through all of those rows that have been resolved forcing a save (aka call store) with AUs enabled.
        /// </summary>
        protected void SaveAfterReconcile()
        {
            this.NotifyCallback(4, "Triggering AUs on the conflicts that are not resolved.");

            // Toggle the AU framework.
            using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMArcMap))
            {
                // Iterate through all of the rows that have been marked as 'None' and group by the table.
                foreach (var rows in this.Rows.Where(o => o.Resolution == ConflictResolution.None).GroupBy(o => o.TableName))
                {
                    Log.Info(this, "Calling store on {0} row(s) within the {1} class.", rows.Count(), rows.Key);

                    using (ComReleaser cr = new ComReleaser())
                    {
                        IFeatureWorkspace currentWorkspace = (IFeatureWorkspace) this.Workspace;
                        ITable currentTable = currentWorkspace.OpenTable(rows.Key);
                        cr.ManageLifetime(currentTable);

                        foreach (var o in rows)
                        {
                            IRow currentRow = currentTable.Fetch(o.OID);
                            cr.ManageLifetime(currentRow);

                            // Call Store to Trigger the AUs.
                            this.SaveAndRebuild(currentRow);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Commits the changes to the specified <paramref name="currentRow" /> and rebuilds connectivity of the
        ///     specified <paramref name="currentRow" /> when there are network connectivity issues.
        /// </summary>
        /// <param name="currentRow">The current row.</param>
        protected virtual void SaveAndRebuild(IRow currentRow)
        {
            if (currentRow == null)
                return;

            INetworkFeature networkFeature = currentRow as INetworkFeature;

            if (networkFeature == null)
            {
                currentRow.Store();
            }
            else
            {
                try
                {
                    // Disconnect from network.
                    networkFeature.Disconnect();

                    // It is not necessary to explicitly call Connect on network features or create and associate network elements
                    // with network features. All network behavior is handled by the object behavior when Store is called on the feature.
                    IFeature feature = (IFeature) currentRow;
                    feature.Store();
                }
                catch (COMException ex)
                {
                    switch (ex.ErrorCode)
                    {
                            // A requested feature object could not be located.
                        case (int) fdoError.FDO_E_FEATURE_NOT_FOUND:

                            Log.Error(this, "The requested feature object could not be located.");
                            this.RebuildConnectivity(networkFeature);

                            break;

                            // Invalid network element id.
                        case (int) esriNetworkErrors.NETWORK_E_INVALID_ELEMENT_ID:

                            Log.Error(this, "The network element id is invalid.");
                            this.RebuildConnectivity(networkFeature);

                            break;

                        // SDE Error. (Shape or row not found)
                        // The feature has been deleted.
                        case (int)fdoError.FDO_E_SE_ROW_NOEXIST:                            
                        case (int) fdoError.FDO_E_FEATURE_DELETED:
                            break;

                        default:
                            throw;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the number of rows that have conflicts.
        /// </summary>
        /// <param name="conflictClasses">The conflict classes.</param>
        /// <returns>
        ///     Returns an <see cref="int" /> representing the number of rows that have conflicts.
        /// </returns>
        private int GetConflictCount(IEnumerable<IConflictClass> conflictClasses)
        {
            int conflictCount = 0;

            foreach (var conflictClass in conflictClasses)
            {
                conflictCount += conflictClass.DeleteUpdates.Count;
                conflictCount += conflictClass.UpdateDeletes.Count;
                conflictCount += conflictClass.UpdateUpdates.Count;
            }

            return conflictCount;
        }

        #endregion
    }
}