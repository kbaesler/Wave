using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geodatabase
{

    #region Enumerations

    /// <summary>
    ///     Enumeration of the various supported relational (DBMS) geodatabase.
    /// </summary>
    public enum DBMS
    {
        /// <summary>
        ///     A local Microsoft Access geodatabase.
        /// </summary>
        Access,

        /// <summary>
        ///     A local ESRI File geodatabase.
        /// </summary>
        File,

        /// <summary>
        ///     A remote Oracle geodatabase.
        /// </summary>
        Oracle,

        /// <summary>
        ///     A remote Microsoft SQL Server geodatabase.
        /// </summary>
        SqlServer,

        /// <summary>
        ///     A remote IBM DB2 geodatabase.
        /// </summary>
        Db2,

        /// <summary>
        ///     A remote Informix geodatabase.
        /// </summary>
        Informix,

        /// <summary>
        ///     A remote Postgre SQL geodatabase.
        /// </summary>
        PostgreSql,

        /// <summary>
        ///     An unknown geodatabase.
        /// </summary>
        Unknown
    }

    #endregion

    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IWorkspace" /> interface.
    /// </summary>
    public static class WorkspaceExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Escaping quote characters by use two quotes for every one displayed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns a <see cref="string" /> representing the escaped value.</returns>
        public static string Escape(this ISQLSyntax source, string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("'", "''");
        }

#if V10
        /// <summary>
        ///     Executes the specified query (SQL) and returns the results as a <see cref="ICursor" />
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Returns a <see cref="ICursor" /> representing the results of the query.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static ICursor ExecuteReader(this IWorkspace source, string commandText)
        {
            ISqlWorkspace sw = source as ISqlWorkspace;
            if (sw == null) throw new NotSupportedException();

            return sw.OpenQueryCursor(commandText);
        }
#endif

        /// <summary>
        ///     Gets the database management system that is used with conjunction of the <paramref name="source" />.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <returns>
        ///     The <see cref="DBMS" /> enumeration value.
        /// </returns>
        public static DBMS GetDBMS(this IWorkspace source)
        {
            if (source == null) return DBMS.Unknown;

            if (source.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                UID uid = source.WorkspaceFactory.GetClassID();
                if (uid.Value.ToString() == "{71FE75F0-EA0C-4406-873E-B7D53748AE7E}")
                    return DBMS.File;

                return DBMS.Access;
            }

            IDatabaseConnectionInfo2 dci = source as IDatabaseConnectionInfo2;
            if (dci != null)
            {
                switch (dci.ConnectionDBMS)
                {
                    case esriConnectionDBMS.esriDBMS_DB2:
                        return DBMS.Db2;

                    case esriConnectionDBMS.esriDBMS_Informix:
                        return DBMS.Informix;

                    case esriConnectionDBMS.esriDBMS_Oracle:
                        return DBMS.Oracle;

                    case esriConnectionDBMS.esriDBMS_PostgreSQL:
                        return DBMS.PostgreSql;

                    case esriConnectionDBMS.esriDBMS_SQLServer:
                        return DBMS.SqlServer;
                }
            }

            return DBMS.Unknown;
        }

        /// <summary>
        ///     Finds the <see cref="IDomain" /> that equals the specified <paramref name="domainName" /> using a non case
        ///     sensitive comparison.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>
        ///     Returns a <see cref="IDomain" /> representing the domain with the given name; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">domainName</exception>
        public static IDomain GetDomain(this IWorkspace source, string domainName)
        {
            if (source == null) return null;
            if (domainName == null) throw new ArgumentNullException("domainName");

            foreach (var domain in source.GetDomains())
            {
                if (domain.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase))
                    return domain;
            }

            return null;
        }

        /// <summary>
        ///     Gets all of the <see cref="IDomain" /> values that have been configured in the workspace.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IDomain}" /> representing the domains available; otherwise <c>null</c>.
        /// </returns>
        public static IEnumerable<IDomain> GetDomains(this IWorkspace source)
        {
            if (source == null) return null;

            IWorkspaceDomains wd = (IWorkspaceDomains) source;
            IEnumDomain domains = wd.Domains;
            return domains.AsEnumerable();
        }

        /// <summary>
        ///     Gets the changes (or edits) that have been made in the current edit session.
        /// </summary>
        /// <param name="source">The source workspace.</param>
        /// <param name="editDataChangesType">Type of the edit data changes.</param>
        /// <param name="func">The function used to determine if the differences should be determined for the specified table.</param>
        /// <param name="predicate">The predicate used to determine if the differences are returned.</param>
        /// <param name="differenceTypes">The type of differences.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey,TValue}" /> representing the differences for the table
        ///     (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">differenceTypes</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The workspace must be within an edit session in order to determine the
        ///     edit changes.
        /// </exception>
        public static Dictionary<string, List<DifferenceRow>> GetEditChanges(this IWorkspace source, esriEditDataChangesType editDataChangesType, Func<string, bool> func, Predicate<DifferenceRow> predicate, params esriDifferenceType[] differenceTypes)
        {
            if (source == null) return null;
            if (differenceTypes == null) throw new ArgumentNullException("differenceTypes");
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (func == null) throw new ArgumentNullException("func");

            var list = new Dictionary<string, List<DifferenceRow>>();

            IWorkspaceEdit2 workspaceEdit2 = (IWorkspaceEdit2) source;
            if (!workspaceEdit2.IsBeingEdited())
                throw new InvalidOperationException("The workspace must be within an edit session in order to determine the edit changes.");

            IDataChangesEx dataChanges = workspaceEdit2.EditDataChanges[editDataChangesType];
            IEnumBSTR enumMci = dataChanges.ModifiedClasses;
            enumMci.Reset();
            string tableName;
            while ((tableName = enumMci.Next()) != null)
            {
                if (func(tableName))
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

                                if (predicate(row))
                                    rows.Add(row);

                                cursor.Next(out oid, out sourceRow, out differenceRow, out fieldIndices);
                            }
                        }
                    }

                    list.Add(tableName, rows);
                }
            }

            return list;
        }

        /// <summary>
        ///     Gets the changes (or edits) that have been made in the current edit session.
        /// </summary>
        /// <param name="source">The source workspace.</param>
        /// <param name="editDataChangesType">Type of the edit data changes.</param>
        /// <param name="predicate">The predicate used to determine if the differences are returned.</param>
        /// <param name="differenceTypes">The type of differences.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey,TValue}" /> representing the differences for the table
        ///     (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">differenceTypes</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The workspace must be within an edit session in order to determine the
        ///     edit changes.
        /// </exception>
        public static Dictionary<string, List<DifferenceRow>> GetEditChanges(this IWorkspace source, esriEditDataChangesType editDataChangesType, Predicate<DifferenceRow> predicate, params esriDifferenceType[] differenceTypes)
        {
            return source.GetEditChanges(editDataChangesType, func => true, predicate, differenceTypes);
        }

        /// <summary>
        ///     Gets the changes (or edits) that have been made in the current edit session.
        /// </summary>
        /// <param name="source">The source workspace.</param>
        /// <param name="editDataChangesType">Type of the edit data changes.</param>
        /// <param name="func">The function used to determine if the differences should be determined for the specified table.</param>
        /// <param name="differenceTypes">The type of differences.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey,TValue}" /> representing the differences for the table
        ///     (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">differenceTypes</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The workspace must be within an edit session in order to determine the
        ///     edit changes.
        /// </exception>
        public static Dictionary<string, List<DifferenceRow>> GetEditChanges(this IWorkspace source, esriEditDataChangesType editDataChangesType, Func<string, bool> func, params esriDifferenceType[] differenceTypes)
        {
            return source.GetEditChanges(editDataChangesType, func, predicate => true, differenceTypes);
        }

        /// <summary>
        ///     Gets the changes (or edits) that have been made in the current edit session.
        /// </summary>
        /// <param name="source">The source workspace.</param>
        /// <param name="editDataChangesType">Type of the edit data changes.</param>
        /// <param name="differenceTypes">The type of differences.</param>
        /// <returns>
        ///     Returns a <see cref="Dictionary{TKey,TValue}" /> representing the differences for the table
        ///     (or key).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">differenceTypes</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The workspace must be within an edit session in order to determine the
        ///     edit changes.
        /// </exception>
        public static Dictionary<string, List<DifferenceRow>> GetEditChanges(this IWorkspace source, esriEditDataChangesType editDataChangesType, params esriDifferenceType[] differenceTypes)
        {
            return source.GetEditChanges(editDataChangesType, func => true, predicate => true, differenceTypes);
        }

        /// <summary>
        ///     Finds the <see cref="IFeatureClass" /> with the specified <paramref name="tableName" /> in the
        ///     <paramref name="schemaName" /> that resides within the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="schemaName">Name of the schema (optional).</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">tableName</exception>
        public static IFeatureClass GetFeatureClass(this IWorkspace source, string schemaName, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            string name = (string.IsNullOrEmpty(schemaName)) ? tableName : schemaName + "." + tableName;
            return ((IFeatureWorkspace) source).OpenFeatureClass(name);
        }

        /// <summary>
        ///     Gets all of the feature classes in the workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureClass}" /> representing the feature
        ///     classes.
        /// </returns>
        public static IEnumerable<IFeatureClass> GetFeatureClasses(this IWorkspace source)
        {
            var datasets = source.Datasets[esriDatasetType.esriDTFeatureDataset];
            foreach (var featureDataset in datasets.AsEnumerable().Cast<IFeatureDataset>())
            {
                foreach (var featureClass in featureDataset.Subsets.AsEnumerable().OfType<IFeatureClass>())
                {
                    yield return featureClass;
                }
            }

            datasets = source.Datasets[esriDatasetType.esriDTFeatureClass];
            foreach (var dataset in datasets.AsEnumerable().OfType<IFeatureClass>())
            {
                yield return dataset;
            }
        }


        /// <summary>
        ///     Gets the formatted date time for the workspace.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        ///     Returns <see cref="String" /> representing the formatted date time for the workspace.
        /// </returns>
        public static string GetFormattedDate(this IWorkspace source, DateTime dateTime)
        {
            if (source == null) return null;

            // Depending on the workspace we need to format the date time differently.
            DBMS dbms = GetDBMS(source);
            switch (dbms)
            {
                case DBMS.Access:

                    // Access - #3/11/2005#
                    return string.Format(CultureInfo.InvariantCulture, "#{0}#", dateTime.ToShortDateString());

                case DBMS.File:

                    // FileGeodatabase - date '3/11/2005'
                    return string.Format(CultureInfo.InvariantCulture, "date '{0}'", dateTime.ToShortDateString());

                case DBMS.Oracle:

                    // Oracle - 01-NOV-2005
                    return string.Format(CultureInfo.InvariantCulture, "{0}", dateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));

                case DBMS.SqlServer:

                    // SqlServer - '3/11/2005'
                    return string.Format(CultureInfo.InvariantCulture, "{0}", dateTime.ToShortDateString());
            }

            return dateTime.ToShortTimeString();
        }

        /// <summary>
        ///     Gets the name of the SQL function.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sqlFunctionName">Name of the SQL function.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the name of the function.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The function is not supported.</exception>
        public static string GetFunctionName(this IWorkspace source, esriSQLFunctionName sqlFunctionName)
        {
            if (source == null) return null;

            ISQLSyntax sqlSyntax = (ISQLSyntax) source;
            string functionName = sqlSyntax.GetFunctionName(sqlFunctionName);
            if (!string.IsNullOrEmpty(functionName))
                return functionName;

            throw new NotSupportedException("The function is not supported.");
        }

        /// <summary>
        ///     Finds the <see cref="IRelationshipClass" /> with the specified <paramref name="relationshipName" /> in the
        ///     <paramref name="schemaName" /> that resides within the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="schemaName">Name of the schema (optional).</param>
        /// <param name="relationshipName">Name of the relationship table.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationshipClass" /> representing the relationship that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">tableName</exception>
        public static IRelationshipClass GetRelationshipClass(this IWorkspace source, string schemaName, string relationshipName)
        {
            if (source == null) return null;
            if (relationshipName == null) throw new ArgumentNullException("relationshipName");

            string name = (string.IsNullOrEmpty(schemaName)) ? relationshipName : schemaName + "." + relationshipName;
            return ((IFeatureWorkspace) source).OpenRelationshipClass(name);
        }

        /// <summary>
        ///     Gets all of the feature classes in the workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IFeatureClass}" /> representing the feature classes.</returns>
        public static IEnumerable<IRelationshipClass> GetRelationshipClasses(this IWorkspace source)
        {
            var datasets = source.Datasets[esriDatasetType.esriDTFeatureDataset];
            foreach (var featureDataset in datasets.AsEnumerable().Cast<IFeatureDataset>())
            {
                foreach (var featureClass in featureDataset.Subsets.AsEnumerable().OfType<IRelationshipClass>())
                {
                    yield return featureClass;
                }
            }

            datasets = source.Datasets[esriDatasetType.esriDTRelationshipClass];
            foreach (var dataset in datasets.AsEnumerable())
            {
                yield return (IRelationshipClass) dataset;
            }
        }

        /// <summary>
        ///     Deletes the specified data set, table or feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="datasetName">Name of the table.</param>
        public static void Delete(this IWorkspace source, IDatasetName datasetName)
        {
            if (((IWorkspace2) source).NameExists[datasetName.Type, datasetName.Name])
            {
                var table = source.GetTable("", datasetName.Name);
                table.Delete();
            }
        }

        /// <summary>
        ///     Finds the <see cref="ITable" /> with the specified <paramref name="tableName" /> in the
        ///     <paramref name="schemaName" /> that resides within the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="schemaName">Name of the schema (optional).</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">tableName</exception>
        public static ITable GetTable(this IWorkspace source, string schemaName, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            string name = (string.IsNullOrEmpty(schemaName)) ? tableName : schemaName + "." + tableName;
            return ((IFeatureWorkspace) source).OpenTable(name);
        }

#if V10
        /// <summary>
        ///     Gets all of the tables in the workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ITable}" /> representing the feature classes.</returns>
        public static IEnumerable<string> GetTableNames(this IWorkspace source)
        {
            var sw = (ISqlWorkspace) source;
            return sw.GetTables().AsEnumerable();
        }
#endif

        /// <summary>
        ///     Gets all of the tables in the workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ITable}" /> representing the feature classes.</returns>
        public static IEnumerable<ITable> GetTables(this IWorkspace source)
        {
            var datasets = source.Datasets[esriDatasetType.esriDTTable];
            return datasets.AsEnumerable().Cast<ITable>();
        }

        /// <summary>
        ///     Determines whether the specified workspace is the  <paramref name="database" /> system.
        /// </summary>
        /// <param name="source">The workspace.</param>
        /// <param name="database">The database.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the specified workspace is DBMS; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDBMS(this IWorkspace source, DBMS database)
        {
            if (source == null) return false;

            return GetDBMS(source) == database;
        }


        /// <summary>
        ///     Determines whether the predicate is supported by the workspace.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns the <see cref="bool" /> representing <c>true</c> when the predicate is supported; otherwise
        ///     <c>false</c>
        /// </returns>
        public static bool IsPredicateSupported(this IWorkspace source, esriSQLPredicates predicate)
        {
            if (source == null) return false;

            // Cast to the ISQLSyntax interface and get the supportedPredicates value.
            ISQLSyntax sqlSyntax = (ISQLSyntax) source;
            int supportedPredicates = sqlSyntax.GetSupportedPredicates();

            // Cast the predicate value to an integer and use bitwise arithmetic to check for support.
            int predicateValue = (int) predicate;
            int supportedValue = predicateValue & supportedPredicates;

            return supportedValue > 0;
        }

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> by the necessary start and stop edit constructs using the specified
        ///     <paramref name="withUndoRedo" /> and
        ///     <paramref name="multiuserEditSessionMode" /> parameters.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">
        ///     if set to <c>true</c> the undo/redo logging is supressed (if the workspace supports such
        ///     suppression).
        /// </param>
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
        ///     The workspace does not support the edit session mode.;multiuserEditSessionMode
        /// </exception>
        public static bool PerformOperation(this IWorkspace source, bool withUndoRedo, esriMultiuserEditSessionMode multiuserEditSessionMode, Func<bool> operation)
        {
            if (source == null) return false;
            if (operation == null) throw new ArgumentNullException("operation");

            IWorkspaceEdit wse = source as IWorkspaceEdit;
            if (wse == null) return false;

            IMultiuserWorkspaceEdit multiuserWorkspaceEdit = source as IMultiuserWorkspaceEdit;
            if (multiuserWorkspaceEdit != null)
            {
                if (!multiuserWorkspaceEdit.SupportsMultiuserEditSessionMode(multiuserEditSessionMode))
                    throw new ArgumentException(@"The workspace does not support the edit session mode.", "multiuserEditSessionMode");

                multiuserWorkspaceEdit.StartMultiuserEditing(multiuserEditSessionMode);
            }
            else
            {
                if (!wse.IsBeingEdited())
                    wse.StartEditing(withUndoRedo);
            }

            bool saveEdits = false;

            try
            {
                saveEdits = wse.PerformOperation(withUndoRedo, operation);
            }
            finally
            {
                wse.StopEditing(saveEdits);
            }

            return saveEdits;
        }

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">
        ///     if set to <c>true</c> the undo/redo logging is supressed (if the workspace supports such
        ///     suppression).
        /// </param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IWorkspace source, bool withUndoRedo, Func<bool> operation)
        {
            return source.PerformOperation(withUndoRedo, esriMultiuserEditSessionMode.esriMESMVersioned, operation);
        }

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">
        ///     if set to <c>true</c> the undo/redo logging is supressed (if the workspace supports such
        ///     suppression).
        /// </param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">operation</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IWorkspaceEdit source, bool withUndoRedo, Func<bool> operation)
        {
            if (source == null) return false;
            if (operation == null) throw new ArgumentNullException("operation");

            var wse = source as IWorkspaceEdit2;
            if (wse == null) return false;

            if (wse.IsInEditOperation)
                throw new ArgumentOutOfRangeException("source", "An edit operation is already started.");

            if (!wse.IsBeingEdited())
                wse.StartEditing(withUndoRedo);

            source.StartEditOperation();

            bool saveEdits = false;

            try
            {
                saveEdits = operation();
            }
            catch (Exception)
            {
                if (wse.IsInEditOperation)
                    source.AbortEditOperation();

                throw;
            }
            finally
            {
                if (wse.IsInEditOperation)
                {
                    if (saveEdits)
                        source.StopEditOperation();
                    else
                        source.AbortEditOperation();
                }

                if (!wse.IsBeingEdited())
                    wse.StopEditing(saveEdits);
            }

            return saveEdits;
        }

        /// <summary>
        ///     Updates the Multiversioned views to point to the current version.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <remarks>
        ///     Before issuing any queries against the view, you must ensure that they will take place against the correct version.
        /// </remarks>
        public static void SetCurrentVersion(this IWorkspace source)
        {
            IVersion version = source as IVersion;
            if (version != null)
            {
                string versionName = version.VersionName;

                if (source.IsDBMS(DBMS.Oracle))
                {
                    string commandText = string.Format("begin sde.version_util.set_current_version('{0}'); end;", versionName);
                    source.ExecuteSQL(commandText);
                }
                else if (source.IsDBMS(DBMS.SqlServer))
                {
                    string commandText = string.Format("begin sde.set_current_version '{0}'; end;", versionName);
                    source.ExecuteSQL(commandText);
                }
            }
        }

        #endregion
    }
}