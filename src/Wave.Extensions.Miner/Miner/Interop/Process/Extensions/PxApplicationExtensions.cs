using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;

using ADODB;

using ESRI.ArcGIS.ADF;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxApplication" /> interface.
    /// </summary>
    public static class PxApplicationExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the history record for the specified node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="node">The node.</param>
        /// <param name="description">The description.</param>
        /// <param name="extraData">The extra data.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxNodeHistory" /> representing the history records.
        /// </returns>
        public static IMMPxNodeHistory AddHistory(this IMMPxApplication source, IMMPxNode node, string description, string extraData)
        {
            return source.AddHistory(node.Id, node.NodeType, description, extraData);
        }

        /// <summary>
        ///     Adds the history record for the specified node id and type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="description">The description.</param>
        /// <param name="extraData">The extra data.</param>
        /// <returns>Returns a <see cref="IMMPxNodeHistory" /> representing the history records.</returns>
        public static IMMPxNodeHistory AddHistory(this IMMPxApplication source, int nodeId, int nodeType, string description, string extraData)
        {
            var list = source.GetHistory(nodeId, nodeType);
            source.AddHistory(list, nodeId, nodeType, description, extraData);
            return list;
        }

        /// <summary>
        ///     Adds the history record for the specified node id and type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="list">The list of history records.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="description">The description.</param>
        /// <param name="extraData">The extra data.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxHistory" /> representing the history record.
        /// </returns>
        public static IMMPxHistory AddHistory(this IMMPxApplication source, IMMPxNodeHistory list, int nodeId, int nodeType, string description, string extraData)
        {
            IMMPxHistory history = new PxHistoryClass();
            history.CurrentUser = source.User.Id;
            history.CurrentUserName = source.User.Name;
            history.Date = DateTime.Now;
            history.Description = description;
            history.NodeId = nodeId;
            history.nodeTypeId = nodeType;
            history.ExtraData = extraData;

            var property = source.Connection.Properties["Data Source Name"];
            if (property != null)
            {
                string server = (!Convert.IsDBNull(property.Value)) ? Convert.ToString(property.Value, CultureInfo.InvariantCulture) : string.Empty;
                history.Server = server;
            }

            list.Add(history);

            return history;
        }

        /// <summary>
        ///     Determines if there are any process framework tasks being executed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Retuns <see cref="bool" /> representing <c>true</c> when there are tasks executing.
        /// </returns>
        public static bool AnyTasksExecuting(this IMMPxApplication source)
        {
            if (source == null) return false;

            return ((IMMPxApplicationEx3) source).TasksExecuting > 0;
        }

        /// <summary>
        ///     Creates a copy of the specified history.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IMMPxHistory" /> representing a copy of the history.</returns>
        public static IMMPxHistory Copy(this IMMPxHistory source)
        {
            return new PxHistoryClass
            {
                CurrentUser = source.CurrentUser,
                CurrentUserName = source.CurrentUserName,
                Date = source.Date,
                Description = source.Description,
                ExtraData = source.ExtraData,
                Server = source.Server,
                Xml = source.Xml,
                NodeId = source.NodeId,
                nodeTypeId = source.nodeTypeId
            };
        }


        /// <summary>
        ///     Executes the given statement which is usually an Insert, Update or Delete statement and returns the number of rows
        ///     affected.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     Returns a <see cref="int" /> representing the number of rows affected.
        /// </returns>
        /// <exception cref="ArgumentNullException">commandText</exception>
        public static int ExecuteNonQuery(this IMMPxApplication source, string commandText)
        {
            if (source == null) return -1;
            if (commandText == null) throw new ArgumentNullException("commandText");

            object recordsEffected;
            source.Connection.Execute(commandText, out recordsEffected, (int) CommandTypeEnum.adCmdText | (int) ExecuteOptionEnum.adExecuteNoRecords);

            return TypeCast.Cast(recordsEffected, -1);
        }

        /// <summary>
        ///     Queries the database and populates a <see cref="DataTable" /> with the resulting data from the specified SQL
        ///     statement.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        ///     Returns a <see cref="DataTable" /> representing the records returned by the command text.
        /// </returns>
        /// <exception cref="ArgumentNullException">commandText</exception>
        public static DataTable ExecuteQuery(this IMMPxApplication source, string commandText)
        {
            if (source == null) return null;
            if (commandText == null) throw new ArgumentNullException("commandText");

            var table = new DataTable();
            table.Locale = CultureInfo.InvariantCulture;

            using (var cr = new ComReleaser())
            {
                Recordset recordset = new RecordsetClass();
                recordset.Open(commandText, source.Connection, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 0);
                cr.ManageLifetime(recordset);

                var adapter = new OleDbDataAdapter();
                adapter.Fill(table, recordset);
                recordset.Close();
            }

            return table;
        }

        /// <summary>
        ///     Retrieves a single value (for example, an aggregate value) from a database.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The process application reference.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     Returns an <see cref="object" /> representing the results of the single value from the database, or the fallback
        ///     value.
        /// </returns>
        /// <exception cref="ArgumentNullException">commandText</exception>
        public static TValue ExecuteScalar<TValue>(this IMMPxApplication source, string commandText, TValue fallbackValue)
        {
            if (source == null) return fallbackValue;
            if (commandText == null) throw new ArgumentNullException("commandText");

            TValue value = fallbackValue;
            object parameters = Type.Missing;

            Command command = new CommandClass();
            command.ActiveConnection = source.Connection;
            command.CommandType = CommandTypeEnum.adCmdText;
            command.CommandText = commandText;

            var table = new DataTable();
            table.Locale = CultureInfo.InvariantCulture;

            using (var cr = new ComReleaser())
            {
                object recordsAffected;
                Recordset recordset = command.Execute(out recordsAffected, ref parameters, (int) CommandTypeEnum.adCmdText);
                cr.ManageLifetime(recordset);

                var adapter = new OleDbDataAdapter();
                adapter.Fill(table, recordset);
                recordset.Close();

                if (table.Rows.Count == 1 && table.Columns.Count == 1)
                    value = TypeCast.Cast(table.Rows[0][0], fallbackValue);
            }

            return value;
        }

        /// <summary>
        ///     Gets the current node that the <see cref="IMMPxApplication" /> is referencing.
        /// </summary>
        /// <param name="source">The process framework application.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxNode" /> representing the current node.
        /// </returns>
        public static IMMPxNode GetCurrentNode(this IMMPxApplication source)
        {
            if (source == null) return null;

            return ((IMMPxApplicationEx) source).CurrentNode;
        }

        /// <summary>
        ///     Returns the <see cref="Design" /> implementation of the current open design.
        /// </summary>
        /// <typeparam name="TDesign">The type of the design.</typeparam>
        /// <param name="source">The process framework application.</param>
        /// <param name="action">The function that will initialize the design (should be used for custom implementations).</param>
        /// <returns>
        ///     Returns a <see cref="Design" /> representing the design; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static TDesign GetDesign<TDesign>(this IMMPxApplication source, Func<int, IMMPxApplication, TDesign> action)
            where TDesign : Design
        {
            if (source == null) return null;
            if (action == null) throw new ArgumentNullException("action");

            var wm = source.GetWorkflowManager() as IMMWorkflowManager3;
            if (wm == null) return null;

            if (wm.CurrentOpenDesign == null) return null;

            return action(wm.CurrentOpenDesign.ID, source);
        }

        /// <summary>
        ///     Returns the <see cref="Design" /> implementation of the current open design.
        /// </summary>
        /// <param name="source">The process framework application reference.</param>
        /// <returns>
        ///     Returns a <see cref="Design" /> representing the design; otherwise <c>null</c>.
        /// </returns>
        public static Design GetDesign(this IMMPxApplication source)
        {
            if (source == null) return null;

            var wm = source.GetWorkflowManager() as IMMWorkflowManager3;
            if (wm == null) return null;

            if (wm.CurrentOpenDesign == null) return null;

            return new Design(source, wm.CurrentOpenDesign);
        }

        /// <summary>
        ///     Finds the filter that matches the specified <paramref name="filterName" />.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxFilter" /> representing the filter that matches the name; otherwise <c>null</c>.
        /// </returns>
        public static IMMPxFilter GetFilter(this IMMPxApplication source, string filterName)
        {
            if (source == null) return null;
            if (filterName == null) throw new ArgumentNullException("filterName");

            foreach (IMMPxFilter filter in source.Filters.AsEnumerable())
            {
                if (string.Equals(filter.Name, filterName, StringComparison.OrdinalIgnoreCase))
                {
                    return filter;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the collection of history for the specified node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxNodeHistory" /> representing the history collection.
        /// </returns>
        public static IMMPxNodeHistory GetHistory(this IMMPxApplication source, IMMPxNode node)
        {
            return source.GetHistory(node.Id, node.NodeType);
        }

        /// <summary>
        ///     Gets the collection of history for the specified node information.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxNodeHistory" /> representing the history collection.
        /// </returns>
        public static IMMPxNodeHistory GetHistory(this IMMPxApplication source, int nodeId, int nodeType)
        {
            return source.GetHistory(nodeId, nodeType, string.Format("NODE_ID = {0} AND NODE_TYPE_ID = {1}", nodeId, nodeType));
        }

        /// <summary>
        ///     Gets the collection of history for the specified node information.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>Returns a <see cref="IMMPxNodeHistory" /> representing the history collection.</returns>
        public static IMMPxNodeHistory GetHistory(this IMMPxApplication source, int nodeId, int nodeType, string whereClause)
        {
            var history = new PxNodeHistoryClass();
            if (history.Init(source.Connection, source.Login.SchemaName, nodeId, nodeType, whereClause))
                return history;

            return null;
        }

        /// <summary>
        ///     Gets the name of the node type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="node">The node.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the node type.</returns>
        public static string GetNodeTypeName(this IMMPxApplication source, IMMPxNode node)
        {
            return ((IMMPxHelper2) source.Helper).GetNodeTypeName(node.NodeType);
        }

        /// <summary>
        ///     Gets the fully qualified name of the table that will included (or exclude) the schema name depending on the
        ///     underlying database connection.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        ///     Returns a <see cref="String" /> representing the fully qualified table with schema name.
        /// </returns>
        /// <exception cref="ArgumentNullException">tableName</exception>
        public static string GetQualifiedTableName(this IMMPxApplication source, string tableName)
        {
            if (source == null) return null;
            if (tableName == null) throw new ArgumentNullException("tableName");

            if (source.Login == null)
                return tableName;

            if (string.IsNullOrEmpty(tableName))
                return tableName;

            if (string.IsNullOrEmpty(source.Login.SchemaName))
                return tableName;

            if (tableName.Contains("."))
                return tableName;

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", source.Login.SchemaName, tableName);
        }

        /// <summary>
        ///     Returns the <see cref="Session" /> implementation of the current open session.
        /// </summary>
        /// <typeparam name="TSession">The type of the session.</typeparam>
        /// <param name="source">The process framework application.</param>
        /// <param name="action">The function that will initialize the session (should be used for custom implementations).</param>
        /// <returns>
        ///     Returns a <see cref="Session" /> representing the session; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static TSession GetSession<TSession>(this IMMPxApplication source, Func<int, IMMPxApplication, TSession> action)
            where TSession : Session
        {
            if (source == null) return null;
            if (action == null) throw new ArgumentNullException("action");

            var sm = source.GetSessionManager() as IMMSessionManager3;
            if (sm == null) return null;

            if (sm.CurrentOpenSession == null) return null;

            return action(sm.CurrentOpenSession.get_ID(), source);
        }

        /// <summary>
        ///     Returns the <see cref="Session" /> implementation of the current open session.
        /// </summary>
        /// <param name="source">The process framework application.</param>
        /// <returns>
        ///     Returns a <see cref="Session" /> representing the session; otherwise <c>null</c>.
        /// </returns>
        public static Session GetSession(this IMMPxApplication source)
        {
            if (source == null) return null;

            var sm = source.GetSessionManager() as IMMSessionManager3;
            if (sm == null) return null;

            if (sm.CurrentOpenSession == null) return null;

            return new Session(source, sm.CurrentOpenSession);
        }

        /// <summary>
        ///     Gets the session manager extension using the application reference.
        /// </summary>
        /// <param name="source">The process framework application reference.</param>
        /// <returns>Returns a <see cref="IMMSessionManager" /> representing the Session Manager extension.</returns>
        public static IMMSessionManager GetSessionManager(this IMMPxApplication source)
        {
            if (source == null) return null;

            return source.FindPxExtensionByName(ArcFM.Process.SessionManager.Name) as IMMSessionManager;
        }

        /// <summary>
        ///     Finds the state with the specified <paramref name="stateID" />.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="stateID">The state ID.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxState" /> representing the state that matches the identifier; otherwise <c>null</c>.
        /// </returns>
        public static IMMPxState GetState(this IMMPxApplication source, int stateID)
        {
            if (source == null) return null;

            foreach (IMMPxState state in source.States.AsEnumerable())
            {
                if (state.StateID == stateID)
                    return state;
            }

            return null;
        }

        /// <summary>
        ///     Finds the state that matches the specified <paramref name="stateName" />.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxState" /> representing the state that matches the specified state name; otherwise
        ///     <c>null</c>.
        /// </returns>
        public static IMMPxState GetState(this IMMPxApplication source, string stateName)
        {
            if (source == null) return null;

            foreach (IMMPxState state in source.States.AsEnumerable())
            {
                if (string.Equals(state.Name, stateName, StringComparison.OrdinalIgnoreCase))
                    return state;
            }

            return null;
        }

        /// <summary>
        ///     Finds the user that matches the specified <paramref name="userID" />.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxUser" /> representing the user that matches the specified identifier; otherwise
        ///     <c>null</c>.
        /// </returns>
        public static IMMPxUser GetUser(this IMMPxApplication source, int userID)
        {
            if (source == null) return null;

            foreach (IMMPxUser user in source.Users.AsEnumerable())
            {
                if (user.Id == userID)
                    return user;
            }

            return null;
        }

        /// <summary>
        ///     Finds the user that matches the specified <paramref name="userName" /> in the name or display name.
        /// </summary>
        /// <param name="source">The process application reference.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxUser" /> representing the user that matches the specified user; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">userName</exception>
        public static IMMPxUser GetUser(this IMMPxApplication source, string userName)
        {
            if (source == null) return null;
            if (userName == null) throw new ArgumentNullException("userName");

            foreach (IMMPxUser user in source.Users.AsEnumerable())
            {
                if (string.Equals(user.Name, userName, StringComparison.OrdinalIgnoreCase))
                    return user;

                var user2 = user as IMMPxUser2;
                if (user2 != null)
                {
                    if (string.Equals(user2.DisplayName, userName, StringComparison.OrdinalIgnoreCase))
                        return user;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the name of the version for the specified <paramref name="node" />.
        /// </summary>
        /// <param name="source">The process framework application reference.</param>
        /// <param name="node">The node.</param>
        /// <returns>
        ///     Returns a <see cref="String" /> representing the name of the version; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">node</exception>
        /// <remarks>
        ///     The node needs to be refer to a session or design, otherwise the
        ///     version name will be <c>null</c>.
        /// </remarks>
        public static string GetVersionName(this IMMPxApplication source, IMMPxNode node)
        {
            if (source == null) return null;
            if (node == null) throw new ArgumentNullException("node");

            var version = ((IMMPxApplicationEx2) source).GetVersionNamer(node.NodeType);
            return version.GetVersionName(node);
        }

        /// <summary>
        ///     Returns the <see cref="WorkRequest" /> implementation of the current open work request.
        /// </summary>
        /// <param name="source">The process framework application.</param>
        /// <returns>
        ///     Returns a <see cref="WorkRequest" /> representing the work request; otherwise <c>null</c>.
        /// </returns>
        public static WorkRequest GetWorkRequest(this IMMPxApplication source)
        {
            if (source == null) return null;

            return source.GetWorkRequest((nodeID, sender) =>
            {
                var o = new WorkRequest(sender, nodeID);
                return o.Valid ? o : null;
            });
        }

        /// <summary>
        ///     Returns the <see cref="WorkRequest" /> implementation of the current open work request.
        /// </summary>
        /// <typeparam name="TWorkRequest">The type of the work request.</typeparam>
        /// <param name="source">The process framework application.</param>
        /// <param name="action">The function that will initialize the work request (should be used for custom implementations).</param>
        /// <returns>
        ///     Returns a <see cref="WorkRequest" /> representing the work request; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public static TWorkRequest GetWorkRequest<TWorkRequest>(this IMMPxApplication source, Func<int, IMMPxApplication, TWorkRequest> action)
            where TWorkRequest : WorkRequest
        {
            if (source == null) return null;
            if (action == null) throw new ArgumentNullException("action");

            Design design = source.GetDesign();
            if (design == null) return null;

            return action(design.WorkRequestID, source);
        }

        /// <summary>
        ///     Gets the workflow manager extension using the application reference.
        /// </summary>
        /// <param name="source">The process framework application reference.</param>
        /// <returns>Returns a <see cref="IMMWorkflowManager" /> representing the Session Manager extension.</returns>
        public static IMMWorkflowManager GetWorkflowManager(this IMMPxApplication source)
        {
            if (source == null) return null;

            return source.FindPxExtensionByName(ArcFM.Process.WorkflowManager.Name) as IMMWorkflowManager;
        }

        /// <summary>
        ///     Sets the <see cref="IMMPxApplication" /> to reference the specified <paramref name="node" />.
        /// </summary>
        /// <param name="source">The process framework application.</param>
        /// <param name="node">The node.</param>
        /// <exception cref="ArgumentNullException">node</exception>
        public static void SetCurrentNode(this IMMPxApplication source, IMMPxNode node)
        {
            if (source == null) return;
            if (node == null) throw new ArgumentNullException("node");

            ((IMMPxApplicationEx) source).CurrentNode = node;
        }

        #endregion
    }
}