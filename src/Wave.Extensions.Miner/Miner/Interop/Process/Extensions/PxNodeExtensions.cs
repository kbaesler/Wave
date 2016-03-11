using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Miner.Interop.Process.IMMPxNode" /> interface.
    /// </summary>
    public static class PxNodeExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds the node as a child to the specified <paramref name="parent" />.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException">parent</exception>
        public static void Add(this IMMPxNode source, IMMPxNode parent)
        {
            if (source == null) return;
            if (parent == null) throw new ArgumentNullException("parent");

            ID8List list = (ID8List) parent;
            ((ID8ListEx) parent).BuildChildren = true;

            list.Add((ID8ListItem) source);
        }

        /// <summary>
        ///     Executes the task with the given name, if it exists, on the given node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when successfully executed the task.
        /// </returns>
        public static bool ExecuteTask(this IMMPxNode source, string taskName)
        {
            var task = source.GetTask(taskName);
            if (task == null) return false;

            var transition = ((IMMPxTask2) task).Transition;
            if (!transition.FromStates.Contains(source.State))
                return false;

            return task.Execute(source);
        }

        /// <summary>
        ///     Finds the task using the specified <paramref name="source" /> and <paramref name="taskName" />.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxTask" /> representing the tasks that matches specified task name for the given node;
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">taskName</exception>
        public static IMMPxTask GetTask(this IMMPxNode source, string taskName)
        {
            return source.GetTask(taskName, false);
        }

        /// <summary>
        ///     Finds the task using the specified <paramref name="source" /> and <paramref name="taskName" />.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="enabledTask">if set to <c>true</c> if the task must be enabled.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxTask" /> representing the tasks that matches specified task name for the given node;
        ///     otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">taskName</exception>
        public static IMMPxTask GetTask(this IMMPxNode source, string taskName, bool enabledTask)
        {
            if (source == null) return null;
            if (taskName == null) throw new ArgumentNullException("taskName");

            IEnumerable<IMMPxTask> tasks;
            if (enabledTask)
            {
                IMMPxNode3 node3 = source as IMMPxNode3;
                if (node3 == null) return null;

                tasks = node3.EnabledTasks.AsEnumerable();
            }
            else
            {
                IMMPxNode4 node4 = source as IMMPxNode4;
                if (node4 == null) return null;

                tasks = node4.AllTasks.AsEnumerable();
            }

            return tasks.FirstOrDefault(o => o.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Finds the task using the specified <paramref name="source" /> and <paramref name="taskID" />.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="taskID">The task ID.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxTask" /> representing the tasks that matches specified task name for the given node;
        ///     otherwise <c>null</c>.
        /// </returns>
        public static IMMPxTask GetTask(this IMMPxNode source, int taskID)
        {
            IMMPxNode3 node3 = source as IMMPxNode3;
            if (node3 == null) return null;

            IMMEnumPxTasks tasks = node3.EnabledTasks;
            foreach (var task in tasks.AsEnumerable())
            {
                if (taskID == task.TaskID)
                {
                    return task;
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the current top level node of the list that the specified node belongs to.
        /// </summary>
        /// <param name="source">The starting node.</param>
        /// <returns>
        ///     Returns the <see cref="Miner.Interop.Process.IMMPxNode" /> representing the top level node; otherwise <c>null</c>.
        /// </returns>
        public static IMMPxNode GetTopLevelNode(this IMMPxNode source)
        {
            if (source == null) return null;

            foreach (var node in source.AsEnumerable())
            {
                if (((IMMPxNode2) node).IsPxTopLevel) return node;
            }

            return null;
        }

        /// <summary>
        ///     Finds the transition that matches the specified <paramref name="transitionName" /> in the name or display name for
        ///     the available transitions.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="transitionName">Name of the transition.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPxTransition" /> representing the state that matches the identifier; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">transitionName</exception>
        public static IMMPxTransition GetTransition(this IMMPxNode source, string transitionName)
        {
            if (source == null) return null;
            if (transitionName == null) throw new ArgumentNullException("transitionName");

            foreach (var transition in source.Transitions.AsEnumerable())
            {
                if (string.Equals(transition.Name, transitionName, StringComparison.CurrentCultureIgnoreCase))
                    return transition;

                if (string.Equals(transition.DisplayName, transitionName, StringComparison.CurrentCultureIgnoreCase))
                    return transition;
            }

            return null;
        }

        /// <summary>
        ///     Finds the transition using the specified <paramref name="transitionID" />.
        /// </summary>
        /// <param name="source">The node.</param>
        /// <param name="transitionID">The transition ID.</param>
        /// <returns>
        ///     The <see cref="IMMPxTransition" /> matching the specified transition name; otherwise <c>null</c>.
        /// </returns>
        public static IMMPxTransition GetTransition(this IMMPxNode source, int transitionID)
        {
            if (source == null) return null;

            foreach (var transition in source.Transitions.AsEnumerable())
            {
                if (transition.TransitionID == transitionID)
                    return transition;
            }

            return null;
        }

        #endregion
    }
}