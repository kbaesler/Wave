﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

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
        ///     Determines whether the workspace contains the table name and type combination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The type.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the workspace contains the table name and type.
        /// </returns>
        public static bool Contains(this IWorkspace source, esriDatasetType type, string tableName)
        {
            return ((IWorkspace2) source).NameExists[type, tableName];
        }

        /// <summary>
        /// Defines the data set definition in the specified workspace.
        /// </summary>
        /// <typeparam name="T">The type of dataset.</typeparam>
        /// <param name="source">The output workspace.</param>
        /// <param name="name">The name of the dataset.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>
        /// Returns a <see cref="IDatasetName" /> representing the definition for the dataset.
        /// </returns>
        public static T Define<T>(this IWorkspace source, string name, T definition)
            where T : IDatasetName
        {
            var ds = (IDataset) source;
            var workspaceName = (IWorkspaceName) ds.FullName;

            definition.WorkspaceName = workspaceName;
            definition.Name = name;

            return definition;
        }

        /// <summary>
        ///     Deletes the specified data set, table or feature class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="datasetName">Name of the table.</param>
        public static void Delete(this IWorkspace source, IDatasetName datasetName)
        {
            if (source.Contains(datasetName.Type, datasetName.Name))
            {
                var table = source.GetTable(datasetName.Name);
                table.Delete();
            }
        }

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

        /// <summary>
        ///     Executes the specified query (SQL) and returns the results of the single column
        ///     returned.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns a <see cref="ICursor" /> representing the results of the query.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static TValue ExecuteScalar<TValue>(this IWorkspace source, string commandText, TValue fallbackValue)
        {
            ISqlWorkspace sw = source as ISqlWorkspace;
            if (sw == null) throw new NotSupportedException();

            using (var cr = new ComReleaser())
            {
                var cursor = sw.OpenQueryCursor(commandText);
                cr.ManageLifetime(cursor);

                var row = cursor.AsEnumerable().FirstOrDefault();
                if (row != null)
                {
                    TValue value;
                    if (row.TryGetValue(0, fallbackValue, out value))
                        return value;
                }
            }

            return fallbackValue;
        }

        /// <summary>
        ///     Finds the dataset using the specified dataset type and name
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="datasetType">Type of the dataset.</param>
        /// <param name="name">The name.</param>
        /// <returns>Returns a <see cref="IDatasetName" /> representing the dataset name that matches the name specified.</returns>
        public static IDatasetName Find(this IWorkspace source, esriDatasetType datasetType, string name)
        {
            Predicate<IDatasetName> predicate =
                ds =>
                {
                    if (ds.Name == null) return false;

                    if (name.IndexOf(".", StringComparison.OrdinalIgnoreCase) > 0 && ds.Name.IndexOf(".", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        return string.Equals(ds.Name, name, StringComparison.OrdinalIgnoreCase);
                    }

                    return (source.Type == esriWorkspaceType.esriLocalDatabaseWorkspace ||
                            source.Type == esriWorkspaceType.esriFileSystemWorkspace)
                        ? string.Equals(ds.Name, name, StringComparison.OrdinalIgnoreCase)
                        : ds.Name.EndsWith("." + name, StringComparison.OrdinalIgnoreCase);
                };

            return source.Find(datasetType, predicate);
        }

        /// <summary>
        ///     Finds the dataset using the specified dataset type and name
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="datasetType">Type of the dataset.</param>
        /// <param name="predicate">The function delegate that determines it should be returned.</param>
        /// <returns>
        ///     Returns a <see cref="IDatasetName" /> representing the dataset name that matches the name specified.
        /// </returns>
        public static IDatasetName Find(this IWorkspace source, esriDatasetType datasetType, Predicate<IDatasetName> predicate)
        {
            return source.DatasetNames[datasetType].Find(predicate, datasetType != esriDatasetType.esriDTTable);
        }

        /// <summary>
        ///     Finds the dataset that satisfies the specified function predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The function delegate that determines it should be returned.</param>
        /// <param name="dfs">if set to <c>true</c> when a depth first search should be used.</param>
        /// <returns>
        ///     Returns a <see cref="IDatasetName" /> representing the dataset that satisfiied the predicate; otherwise <c>null</c>
        ///     .
        /// </returns>
        public static IDatasetName Find(this IEnumDatasetName source, Predicate<IDatasetName> predicate, bool dfs = true)
        {
            if (source != null)
            {
                source.Reset();
                IDatasetName dataset;
                while ((dataset = source.Next()) != null)
                {
                    if (predicate(dataset))
                        return dataset;

                    if (dfs)
                    {
                        var ds = dataset.SubsetNames.Find(predicate);
                        if (ds != null) return ds;
                    }
                }

                if (!dfs)
                {
                    source.Reset();
                    while ((dataset = source.Next()) != null)
                    {
                        var ds = dataset.SubsetNames.Find(predicate, false);
                        if (ds != null) return ds;
                    }
                }
            }

            return null;
        }

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
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="ArgumentOutOfRangeException">tableName</exception>
        public static IFeatureClass GetFeatureClass(this IWorkspace source, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            if (source.Contains(esriDatasetType.esriDTFeatureClass, tableName))
                return ((IFeatureWorkspace) source).OpenFeatureClass(tableName);

            var ds = source.Find(esriDatasetType.esriDTFeatureClass, tableName);
            if (ds != null)
                return ((IFeatureWorkspace) source).OpenFeatureClass(ds.Name);

            throw new ArgumentOutOfRangeException("tableName");
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
                    return string.Format(CultureInfo.InvariantCulture, "'{0}'", dateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));

                case DBMS.SqlServer:

                    // SqlServer - '3/11/2005'
                    return string.Format(CultureInfo.InvariantCulture, "'{0}'", dateTime.ToShortDateString());
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
        ///     Finds the <see cref="IRelationshipClass" /> with the specified <paramref name="tableName" /> in the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="tableName">Name of the relationship table.</param>
        /// <returns>
        ///     Returns a <see cref="IRelationshipClass" /> representing the relationship that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">relationshipName</exception>
        /// <exception cref="ArgumentOutOfRangeException">tableName</exception>
        public static IRelationshipClass GetRelationshipClass(this IWorkspace source, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            if (source.Contains(esriDatasetType.esriDTRelationshipClass, tableName))
                return ((IFeatureWorkspace) source).OpenRelationshipClass(tableName);

            var ds = source.Find(esriDatasetType.esriDTRelationshipClass, tableName);
            if (ds != null)
                return ((IFeatureWorkspace) source).OpenRelationshipClass(ds.Name);

            throw new ArgumentOutOfRangeException("tableName");
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
        ///     Finds the <see cref="ITable" /> with the specified <paramref name="tableName" /> in the
        ///     specified <paramref name="source" /> workspace.
        /// </summary>
        /// <param name="source">The workspace</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the table that has the name,
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        /// <exception cref="ArgumentOutOfRangeException">tableName</exception>
        public static ITable GetTable(this IWorkspace source, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            esriDatasetType[] types = {esriDatasetType.esriDTTable, esriDatasetType.esriDTFeatureClass};
            foreach (var type in types)
            {
                if (source.Contains(type, tableName))
                    return ((IFeatureWorkspace) source).OpenTable(tableName);
            }

            foreach (var type in types)
            {
                var ds = source.Find(type, tableName);
                if (ds != null)
                    return ((IFeatureWorkspace) source).OpenTable(ds.Name);
            }

            throw new ArgumentOutOfRangeException("tableName", $@"The {tableName} table was not found.");
        }

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
        ///     The workspace does not support the edit session
        ///     mode.;multiuserEditSessionMode
        /// </exception>
        public static bool PerformOperation(this IWorkspace source, bool withUndoRedo, esriMultiuserEditSessionMode multiuserEditSessionMode, Func<bool> operation)
        {
            return source.PerformOperation(withUndoRedo, multiuserEditSessionMode, operation, handled => false);
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
        /// <param name="error">
        ///     The error handling action that occurred during commit when true is returned the error has been
        ///     handled.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing the state of the operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        /// <exception cref="System.ArgumentException">
        ///     The workspace does not support the edit session
        ///     mode.;multiuserEditSessionMode
        /// </exception>
        public static bool PerformOperation(this IWorkspace source, bool withUndoRedo, esriMultiuserEditSessionMode multiuserEditSessionMode, Func<bool> operation, Func<COMException, bool> error)
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

            return wse.PerformOperation(withUndoRedo, operation, error);
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
            return source.PerformOperation(withUndoRedo, esriMultiuserEditSessionMode.esriMESMVersioned, operation, error => false);
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
        /// <param name="error">
        ///     The error handling action that occurred during commit when true is returned the error has been
        ///     handled.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IWorkspace source, bool withUndoRedo, Func<bool> operation, Func<COMException, bool> error)
        {
            return source.PerformOperation(withUndoRedo, esriMultiuserEditSessionMode.esriMESMVersioned, operation, error);
        }

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="withUndoRedo">
        ///     if set to <c>true</c> the undo/redo logging is supressed (if the workspace supports such
        ///     suppression).
        /// </param>
        /// <param name="operation">
        ///     The delegate that performs the operation and the action delegate used to commit the transaction
        ///     (i.e. edit operation).
        /// </param>
        /// <param name="error">
        ///     The error handling action that occurred during commit when true is returned the error has been
        ///     handled.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">operation</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IWorkspaceEdit source, bool withUndoRedo, Func<Action, bool> operation, Func<COMException, bool> error)
        {
            if (source == null) return false;
            if (operation == null) throw new ArgumentNullException("operation");

            if (!source.IsBeingEdited())
                source.StartEditing(withUndoRedo);

            source.StartEditOperation();

            bool saveEdits = false;
            bool editOperation = true;

            Action commit = () =>
            {
                source.StopEditOperation();
                editOperation = false;

                source.StopEditing(true);

                source.StartEditing(withUndoRedo);
                saveEdits = false;

                source.StartEditOperation();
                editOperation = true;
            };

            try
            {
                saveEdits = operation(commit);
            }
            catch
            {
                source.AbortEditOperation();
                editOperation = false;

                throw;
            }
            finally
            {
                if (editOperation)
                {
                    if (saveEdits)
                        source.StopEditOperation();
                    else
                        source.AbortEditOperation();
                }

                if (source.IsBeingEdited())
                {
                    try
                    {
                        source.StopEditing(saveEdits);
                    }
                    catch (COMException com)
                    {
                        if (!error(com))
                            throw;

                        source.StopEditing(saveEdits);
                    }
                }
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
        /// <param name="error">
        ///     The error handling action that occurred during commit when true is returned the error has been
        ///     handled.
        /// </param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">operation</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IWorkspaceEdit source, bool withUndoRedo, Func<bool> operation, Func<COMException, bool> error)
        {
            if (source == null) return false;
            if (operation == null) throw new ArgumentNullException("operation");

            if (!source.IsBeingEdited())
                source.StartEditing(withUndoRedo);

            source.StartEditOperation();

            bool saveEdits = false;
            bool editOperation = true;

            try
            {
                saveEdits = operation();
            }
            catch
            {
                source.AbortEditOperation();
                editOperation = false;

                throw;
            }
            finally
            {
                if (editOperation)
                {
                    if (saveEdits)
                        source.StopEditOperation();
                    else
                        source.AbortEditOperation();
                }

                if (source.IsBeingEdited())
                {
                    try
                    {
                        source.StopEditing(saveEdits);
                    }
                    catch (COMException com)
                    {
                        if (!error(com))
                            throw;

                        source.StopEditing(saveEdits);
                    }
                }
            }

            return saveEdits;
        }

        /// <summary>
        ///     Peforms the transaction and comits it on completion of the action, and should only be used for direct updates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transaction">The transaction.</param>
        /// <remarks>
        ///     Applications can use transactions to manage direct updates, for example, updates made outside of an edit
        ///     session, on object and feature classes that are tagged as not requiring an edit session.
        /// </remarks>
        public static void PerformTransaction(this ITransactions source, Action transaction)
        {
            source.StartTransaction();

            try
            {
                transaction();

                source.CommitTransaction();
            }
            catch (Exception)
            {
                source.AbortTransaction();

                throw;
            }
        }

        /// <summary>
        ///     Peforms the transaction and comits it on completion of the action, and should only be used for direct updates.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Returns the result of the transaction.</returns>
        /// <remarks>
        ///     Applications can use transactions to manage direct updates, for example, updates made outside of an edit
        ///     session, on object and feature classes that are tagged as not requiring an edit session.
        /// </remarks>
        public static TResult PerformTransaction<TResult>(this ITransactions source, Func<TResult> transaction)
        {
            TResult result;
            source.StartTransaction();

            try
            {
                result = transaction();

                source.CommitTransaction();
            }
            catch (Exception)
            {
                source.AbortTransaction();

                throw;
            }

            return result;
        }

        /// <summary>
        ///     Check the status of an open workspace. If a workspace is disconnected,
        ///     the method will ping it until it's once again available, or a maximum number of checks are exhausted.
        ///     Between checking a workspace's status, it will sleep for a number of milliseconds specified.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="disconnected">if set to <c>true</c> the workspace has been disconnected.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <param name="sleepTime">The sleep time.</param>
        /// <returns>
        ///     The return parameter is null if the workspace was never disconnected, or if it was disconnected and
        ///     could not be reconnected. If it has been disconnected but was successfully reconnected, the return
        ///     value is the newly-reconnected workspace. The outbound parameter can be used to disambiguate a null
        ///     return value. It's important to note that if a workspace was disconnected and reconnected, all of the
        ///     workspace objects - i.e. feature classes and tables - must be reopened.
        /// </returns>
        /// <exception cref="System.ArgumentException">The workspace could not be found.</exception>
        /// <remarks>
        ///     This method should be used as an error handling routine when it's suspected problems are being
        ///     caused due to workspace disconnection.
        /// </remarks>
        public static IWorkspace Reconnect(this IWorkspace source, out bool disconnected, string fileName = null, int retryAttempts = 3, int sleepTime = 250)
        {
            // At this point, assume disconnection hasn't taken place.
            disconnected = false;

            try
            {
                // Attempt to locate a matching workspace.
                IWorkspaceFactoryStatus workspaceFactoryStatus = (IWorkspaceFactoryStatus) source.WorkspaceFactory;
                IEnumWorkspaceStatus enumWorkspaceStatus = workspaceFactoryStatus.WorkspaceStatus;
                IWorkspaceStatus workspaceStatus;
                while ((workspaceStatus = enumWorkspaceStatus.Next()) != null)
                {
                    if (source.Equals(workspaceStatus.Workspace))
                    {
                        break;
                    }
                }

                // When there is no matching status object.
                if (workspaceStatus == null)
                {
                    throw new ArgumentException("The workspace could not be found.");
                }

                // Check the workspace's connection status.
                if (workspaceStatus.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSDown)
                {
                    // Indicate that disconnection has occurred.
                    disconnected = true;

                    // Ping the workspace up to a maximum number of times.
                    for (int i = 0; i < retryAttempts; i++)
                    {
                        // PingWorkspaceStatus should only be used on a workspace that is known to be down.
                        IWorkspaceStatus pingStatus = workspaceFactoryStatus.PingWorkspaceStatus(source);

                        // If the workspace becomes available, reopen it and break out of the loop.
                        if (pingStatus.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSAvailable)
                        {
                            return workspaceFactoryStatus.OpenAvailableWorkspace(pingStatus);
                        }

                        Thread.Sleep(sleepTime);
                    }
                }
            }
            catch (InvalidComObjectException)
            {
                // The reconnect is optional.
                if (string.IsNullOrEmpty(fileName))
                    throw;

                // Indicate that disconnection has occurred.
                disconnected = true;

                // Reconnect to the workspace.
                source = WorkspaceFactories.Open(fileName);
            }

            return source;
        }

        /// <summary>
        ///     Transfers one or more datasets from one geodatabase to another geodatabase, which includes tables, feature classes,
        ///     feature datasets, or any other kind of dataset and a set containing
        ///     different types of datasets.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="fromNames">The names of the source objects.</param>
        /// <param name="conflicts">if set to <c>true</c> has conflicts with the transfer.</param>
        /// <param name="enumNameMapping">The enumeration of the name mappings.</param>
        public static void Transfer(this IWorkspace source, IWorkspace workspace, IEnumName fromNames, out bool conflicts, out IEnumNameMapping enumNameMapping)
        {
            source.Transfer(workspace, fromNames, out conflicts, out enumNameMapping, (mapping, name) => mapping.GetSuggestedName(name));
        }

        /// <summary>
        ///     Transfers one or more datasets from one geodatabase to another geodatabase, which includes tables, feature classes,
        ///     feature datasets, or any other kind of dataset and a set containing
        ///     different types of datasets.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="fromNames">The names of the source objects.</param>
        /// <param name="conflicts">if set to <c>true</c> has conflicts with the transfer.</param>
        /// <param name="enumNameMapping">The enumeration of the name mappings.</param>
        /// <param name="resolveNameConflict">The resolve name conflict function.</param>
        public static void Transfer(this IWorkspace source, IWorkspace workspace, IEnumName fromNames, out bool conflicts, out IEnumNameMapping enumNameMapping, Func<INameMapping, IName, string> resolveNameConflict)
        {
            IWorkspaceName targetWorkspaceName = (IWorkspaceName) ((IDataset) workspace).FullName;
            IName targetName = (IName) targetWorkspaceName;

            IGeoDBDataTransfer2 transfer = new GeoDBDataTransferClass();
            conflicts = transfer.GenerateNameMapping(fromNames, targetName, out enumNameMapping);
            enumNameMapping.Reset();

            if (!conflicts)
            {
                transfer.Transfer(enumNameMapping, targetName);
            }
            else
            {
                INameMapping nameMapping;
                while ((nameMapping = enumNameMapping.Next()) != null)
                {
                    if (nameMapping.NameConflicts)
                    {
                        nameMapping.TargetName = resolveNameConflict(nameMapping, targetName);
                    }

                    IEnumNameMapping childEnumNameMapping = nameMapping.Children;
                    if (childEnumNameMapping != null)
                    {
                        childEnumNameMapping.Reset();

                        INameMapping childNameMapping;
                        while ((childNameMapping = childEnumNameMapping.Next()) != null)
                        {
                            if (childNameMapping.NameConflicts)
                            {
                                childNameMapping.TargetName = resolveNameConflict(childNameMapping, targetName);
                            }
                        }
                    }
                }

                transfer.Transfer(enumNameMapping, targetName);
            }
        }

        #endregion
    }
}