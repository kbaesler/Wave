using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMMPageTemplateManager" /> interface.
    /// </summary>
    public static class PageTemplateManagerExtensions
    {
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
        /// <param name="pageTemplateUserNames">The page template user names.</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IMMPageTemplateName, IMemoryBlobStream>> GetBlobPageTemplates(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, params string[] pageTemplateUserNames)
        {
            var filter = new QueryFilterClass();
            filter.WhereClause = "TEMPLATE IS NOT NULL";

            if (pageTemplateUserNames != null && pageTemplateUserNames.Any())
            {
                filter.WhereClause = string.Format("{0} AND USERNAME IN ('{1}')", filter.WhereClause, string.Join("','", pageTemplateUserNames));
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
        ///     Gets the unopened page template with the specified type nad name.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageTemplateType">Type of the page template.</param>
        /// <param name="pageTemplateName">Name of the page template.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPageTemplate" /> representing the page template; otherwise <c>null</c>
        /// </returns>
        public static IMMPageTemplate GetUnopenedPageTemplate(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, string pageTemplateName)
        {
            var items = source.GetPageTemplateNames(pageTemplateType);

            foreach (var i in items.AsEnumerable())
            {
                if (i.Name.Equals(pageTemplateName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return source.GetUnopenedPageTemplate(i);
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the unopened page template with the specified type and users (optional)
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageTemplateType">Type of the page template.</param>
        /// <param name="pageTemplateUserNames">Names of the page template user.</param>
        /// <returns>
        ///     Returns a <see cref="IMMPageTemplate" /> representing the page template; otherwise <c>null</c>
        /// </returns>
        public static IEnumerable<KeyValuePair<IMMPageTemplateName, IMMPageTemplate>> GetUnopenedPageTemplates(this IMMPageTemplateManager source, mmPageTemplateType pageTemplateType, params string[] pageTemplateUserNames)
        {
            var filter = new QueryFilterClass();
            filter.WhereClause = "TEMPLATE IS NOT NULL";

            if (pageTemplateUserNames != null && pageTemplateUserNames.Any())
            {
                filter.WhereClause = string.Format("{0} AND USERNAME IN ('{1}')", filter.WhereClause, string.Join("','", pageTemplateUserNames));
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
            var table = source.Workspace.GetTable("SDE", system ? "MM_SYSTEM_PAGE_TEMPLATES" : "MM_PAGE_TEMPLATES");
            foreach (var row in table.Fetch(filter))
            {
                yield return func(row);
            }
        }

        #endregion
    }
}