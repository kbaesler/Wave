using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMPageTemplateManager" /> interface.
    /// </summary>
    public static class PageTemplateManagerExtensions
    {
        #region Constants

        /// <summary>
        ///     The system page templates
        /// </summary>
        private const string SystemPageTemplates = "MM_SYSTEM_PAGE_TEMPLATES";

        /// <summary>
        ///     The user page templates
        /// </summary>
        private const string UserPageTemplates = "MM_PAGE_TEMPLATES";

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMMPageTemplateName" />
        /// </summary>
        /// <param name="source">An <see cref="IMMEnumPageTemplateName" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the layers from the input source.
        /// </returns>
        public static IEnumerable<IMMPageTemplateName> AsEnumerable(this IMMEnumPageTemplateName source)
        {
            source.Reset();

            IMMPageTemplateName name;
            while ((name = source.Next()) != null)
            {
                yield return name;
            }
        }

        /// <summary>
        ///     Gets the page template blobs for the specific type and users (optional)
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageTemplateType">Type of the page template.</param>
        /// <param name="userName">The page template user names.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{KeyValuePair{TKey, TValue}}" /> representing the page template and blob
        ///     value.
        /// </returns>
        public static IEnumerable<KeyValuePair<IMMPageTemplateName, IMemoryBlobStream>> GetBlobPageTemplates(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, params string[] userName)
        {
            var filter = new QueryFilterClass();
            filter.WhereClause = "TEMPLATE IS NOT NULL";

            if (userName != null && userName.Any())
            {
                filter.WhereClause = string.Format("{0} AND USERNAME IN ('{1}')", filter.WhereClause, string.Join("','", userName));
            }

            ((IQueryFilterDefinition2) filter).PostfixClause = "ORDER BY USERNAME";

            var system = pageTemplateType == mmPageTemplateType.mmPTTSystem;

            return GetPageTemplatesImpl(source, system, filter, row =>
            {
                var name = (string) row.Value[row.Fields.FindField("NAME")];
                var blob = (IMemoryBlobStream) row.Value[row.Fields.FindField("TEMPLATE")];

                var i = new MMPageTemplateNameClass();
                i.Initialize(row.OID, name, pageTemplateType);

                return new KeyValuePair<IMMPageTemplateName, IMemoryBlobStream>(i, blob);
            });
        }

        /// <summary>
        ///     Gets the type of the page template.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Returns a <see cref="mmPageTemplateType" /> representing the type for the user.</returns>
        public static mmPageTemplateType GetPageTemplateType(this IMMPageTemplateManager source, string userName)
        {
            switch (userName)
            {
                case "__DESIGNER__":
                    return mmPageTemplateType.mmPTTDesigner;

                case "__MAPSHEET__":
                    return mmPageTemplateType.mmPTTMapSheet;

                case "SYSTEM":
                    return mmPageTemplateType.mmPTTSystem;

                default:
                    return mmPageTemplateType.mmPTTUser;
            }
        }

        /// <summary>
        ///     Gets the unopened page templates with the specified type and name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageTemplateType">Type of the page template.</param>
        /// <param name="userName">Name of the page template.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IMMPageTemplate}" /> representing the page templates.
        /// </returns>
        public static IEnumerable<IMMPageTemplate> GetUnopenedPageTemplates(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, string userName)
        {
            var items = source.GetPageTemplateNames(pageTemplateType);

            foreach (var i in items.AsEnumerable())
            {
                yield return source.GetUnopenedPageTemplate(i);
            }
        }

        /// <summary>
        ///     Gets the unopened page template with the specified type and users (optional)
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageTemplateType">Type of the page template.</param>
        /// <param name="userNames">Names of the page template user.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPageTemplate" /> representing the page template; otherwise <c>null</c>
        /// </returns>
        public static IEnumerable<KeyValuePair<IMMPageTemplateName, IMMPageTemplate>> GetUnopenedPageTemplates(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, params string[] userNames)
        {
            var filter = new QueryFilterClass();
            filter.WhereClause = "TEMPLATE IS NOT NULL";

            if (userNames != null && userNames.Any())
            {
                filter.WhereClause = string.Format("{0} AND USERNAME IN ('{1}')", filter.WhereClause, string.Join("','", userNames));
            }

            ((IQueryFilterDefinition2) filter).PostfixClause = "ORDER BY USERNAME";

            var system = pageTemplateType == mmPageTemplateType.mmPTTSystem;

            return GetPageTemplatesImpl(source, system, filter, row =>
            {
                var name = (string) row.Value[row.Fields.FindField("NAME")];

                var i = new MMPageTemplateNameClass();
                i.Initialize(row.OID, name, pageTemplateType);

                var t = source.GetUnopenedPageTemplate(i);
                return new KeyValuePair<IMMPageTemplateName, IMMPageTemplate>(i, t);
            });
        }

        /// <summary>
        ///     Gets the unopened page templates with the specified type and name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="userName">Name of the page template.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IMMPageTemplate}" /> representing the page templates.
        /// </returns>
        public static IEnumerable<IMMPageTemplate> GetUnopenedPageTemplates(this IMMPageTemplateManager source, string userName)
        {
            var pageTemplateType = source.GetPageTemplateType(userName);
            return source.GetUnopenedPageTemplates(pageTemplateType, userName);
        }

        /// <summary>
        ///     Gets the user names that are associated with the user page templates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{String}" /> representing the unique user names.
        /// </returns>
        public static IEnumerable<string> GetUserNames(this IMMPageTemplateManager source)
        {
            var reader = source.Workspace.ExecuteReader(string.Format("SELECT DISTINCT(USERNAME) FROM {0}", UserPageTemplates));
            return reader.AsEnumerable().Select(o => o.GetValue(0, string.Empty)).ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the page templates based on the filter and function delegate.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="system">if set to <c>true</c> if the page template manager references the system tables.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="func">The function delegate that returns the key/value pair of the page template information.</param>
        /// <returns>
        ///     Returns a <see cref="KeyValuePair{TKey, TValue}" /> representing the page templates
        /// </returns>
        private static IEnumerable<KeyValuePair<TKey, TValue>> GetPageTemplatesImpl<TKey, TValue>(IMMPageTemplateManager source, bool system, IQueryFilter filter, Func<IRow, KeyValuePair<TKey, TValue>> func)
        {
            var table = source.Workspace.GetTable("SDE", system ? SystemPageTemplates : UserPageTemplates);

            var cursor = table.Search(filter, true);

            var cr = new ComReleaser();
            cr.ManageLifetime(cursor);

            return cursor.AsEnumerable().Select(func);
        }

        #endregion
    }
}