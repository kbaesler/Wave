using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for ArcGIS COM collections
    /// </summary>
    public static class SystemExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IStringArray" />
        /// </summary>
        /// <param name="source">An <see cref="IStringArray" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<string> AsEnumerable(this IStringArray source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return source.Element[i];
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumWorkspaceStatus" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumWorkspaceStatus" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<IWorkspaceStatus> AsEnumerable(this IEnumWorkspaceStatus source)
        {
            if (source != null)
            {
                source.Reset();
                IWorkspaceStatus status;

                while ((status = source.Next()) != null)
                {
                    yield return status;
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumDatasetName" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumDatasetName" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<IDatasetName> AsEnumerable(this IEnumDatasetName source)
        {
            if (source != null)
            {
                source.Reset();
                IDatasetName datasetName;
                while ((datasetName = source.Next()) != null)
                {
                    yield return datasetName;
                }
            }
        }


        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ILongArray" />
        /// </summary>
        /// <param name="source">An <see cref="IArray" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the values from the input source.
        /// </returns>
        public static IEnumerable<int> AsEnumerable(this ILongArray source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return source.Element[i];
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IArray" />
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">An <see cref="IArray" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the values from the input source.
        /// </returns>
        public static IEnumerable<TValue> AsEnumerable<TValue>(this IArray source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return (TValue) source.Element[i];
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumDomain" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumDomain" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the values from the input source.</returns>
        public static IEnumerable<IDomain> AsEnumerable(this IEnumDomain source)
        {
            if (source != null)
            {
                source.Reset();
                IDomain domain = source.Next();
                while (domain != null)
                {
                    yield return domain;
                    domain = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumBSTR" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumBSTR" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the values from the input source.</returns>
        public static IEnumerable<string> AsEnumerable(this IEnumBSTR source)
        {
            if (source != null)
            {
                source.Reset();
                string name = source.Next();
                while (name != null)
                {
                    yield return name;
                    name = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumIDs" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumIDs" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the values from the input source.</returns>
        public static IEnumerable<int> AsEnumerable(this IEnumIDs source)
        {
            if (source != null)
            {
                source.Reset();
                int oid = source.Next();
                while (oid >= 0)
                {
                    yield return oid;
                    oid = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IFields" />
        /// </summary>
        /// <param name="source">An <see cref="IFields" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the values from the input source.</returns>
        public static IEnumerable<IField> AsEnumerable(this IFields source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.FieldCount; i++)
                {
                    IField field = source.Field[i];
                    lock (field)
                    {
                        yield return field;
                    }
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumRelationshipClass" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumRelationshipClass" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the relationship classes from the input source.</returns>
        public static IEnumerable<IRelationshipClass> AsEnumerable(this IEnumRelationshipClass source)
        {
            if (source != null)
            {
                source.Reset();
                IRelationshipClass relationshipClass = source.Next();
                while (relationshipClass != null)
                {
                    yield return relationshipClass;
                    relationshipClass = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ICursor" />
        /// </summary>
        /// <param name="source">An <see cref="ICursor" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the rows from the input source.</returns>
        /// <remarks>This method does not dispose of the cursor. Care should be taken to properly dispoose of all cursors</remarks>
        public static IEnumerable<IRow> AsEnumerable(this ICursor source)
        {
            if (source != null)
            {
                IRow row = source.NextRow();
                while (row != null)
                {
                    yield return row;
                    row = source.NextRow();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IFeatureCursor" />
        /// </summary>
        /// <param name="source">An <see cref="IFeatureCursor" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the features from the input source.</returns>
        public static IEnumerable<IFeature> AsEnumerable(this IFeatureCursor source)
        {
            if (source != null)
            {
                IFeature feature = source.NextFeature();
                while (feature != null)
                {
                    yield return feature;
                    feature = source.NextFeature();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IFeatureClassContainer" />
        /// </summary>
        /// <param name="source">An <see cref="IFeatureClassContainer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature class from the input source.</returns>
        public static IEnumerable<IFeatureClass> AsEnumerable(this IFeatureClassContainer source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.ClassCount; i++)
                {
                    yield return source.Class[i];
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumFeatureClass" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumFeatureClass" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.</returns>
        public static IEnumerable<IFeatureClass> AsEnumerable(this IEnumFeatureClass source)
        {
            if (source != null)
            {
                source.Reset();
                IFeatureClass featureclass = source.Next();
                while (featureclass != null)
                {
                    yield return featureclass;
                    featureclass = source.Next();
                }
            }
        }


        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumWorkspaceEx" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumWorkspaceEx" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the feature classes from the input source.</returns>
        public static IEnumerable<IWorkspace> AsEnumerable(this IEnumWorkspaceEx source)
        {
            if (source != null)
            {
                source.Reset();
                IWorkspace workspace = source.Next();
                while (workspace != null)
                {
                    yield return workspace;
                    workspace = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IFIDSet" />
        /// </summary>
        /// <param name="source">An <see cref="IFIDSet" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the IDs from the input source.</returns>
        public static IEnumerable<int> AsEnumerable(this IFIDSet source)
        {
            if (source != null)
            {
                source.Reset();
                int objectId;

                source.Next(out objectId);
                while (objectId >= 0)
                {
                    yield return objectId;
                    source.Next(out objectId);
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ISet" />
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">An <see cref="ISet" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the IDs from the input source.
        /// </returns>
        public static IEnumerable<TValue> AsEnumerable<TValue>(this ISet source)
        {
            if (source != null)
            {
                source.Reset();
                object o = source.Next();
                while (o != null)
                {
                    yield return (TValue) o;
                    o = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumFeature" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumFeature" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the features from the input source.</returns>
        public static IEnumerable<IFeature> AsEnumerable(this IEnumFeature source)
        {
            if (source != null)
            {
                source.Reset();
                IFeature feature = source.Next();
                while (feature != null)
                {
                    yield return feature;
                    feature = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumRelationship" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumRelationship" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the relationships from the input source.</returns>
        public static IEnumerable<IRelationship> AsEnumerable(this IEnumRelationship source)
        {
            if (source != null)
            {
                source.Reset();
                IRelationship relationship = source.Next();
                while (relationship != null)
                {
                    yield return relationship;
                    relationship = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumDataset" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumDataset" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<IDataset> AsEnumerable(this IEnumDataset source)
        {
            if (source != null)
            {
                source.Reset();
                IDataset dataset = source.Next();
                while (dataset != null)
                {
                    yield return dataset;
                    dataset = source.Next();
                }
            }
        }


        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumNetEID" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumNetEID" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.</returns>
        public static IEnumerable<int> AsEnumerable(this IEnumNetEID source)
        {
            if (source != null)
            {
                source.Reset();
                int eid = source.Next();
                while (eid != 0)
                {
                    yield return eid;
                    eid = source.Next();
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IEnumSubtype" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumSubtype" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> that contains the datasets from the input source.
        /// </returns>
        public static IEnumerable<KeyValuePair<int, string>> AsEnumerable(this IEnumSubtype source)
        {
            if (source != null)
            {
                source.Reset();
                int subtypeCode;
                string subtypeName = source.Next(out subtypeCode);
                while (string.IsNullOrEmpty(subtypeName) == false)
                {
                    yield return new KeyValuePair<int, string>(subtypeCode, subtypeName);
                    subtypeName = source.Next(out subtypeCode);
                }
            }
        }

        /// <summary>
        ///     Decrements the reference count of the supplied runtime callable wrapper.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Release(this ICursor source)
        {
            while (Marshal.ReleaseComObject(source) > 0)
            {
            }
        }

        /// <summary>
        ///     Decrements the reference count of the supplied runtime callable wrapper.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Release(this IFeatureCursor source)
        {
            while (Marshal.ReleaseComObject(source) > 0)
            {
            }
        }

        #endregion
    }
}