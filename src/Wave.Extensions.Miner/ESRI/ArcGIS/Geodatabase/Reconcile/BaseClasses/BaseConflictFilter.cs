using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Carto;

using Miner.Interop;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An abstract class for resolving conflicts during reconcile.
    /// </summary>
    public abstract class BaseConflictFilter : IConflictFilter
    {
        #region Fields

        private readonly Dictionary<int, IMMFieldManager> _FieldManagersByClassID = new Dictionary<int, IMMFieldManager>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConflictFilter" /> class.
        /// </summary>
        /// <param name="conflictFilterName">Name of the conflict filter.</param>
        /// <param name="conflictFilterPriority">The conflict filter priority.</param>
        protected BaseConflictFilter(string conflictFilterName, int conflictFilterPriority)
        {
            this.Name = conflictFilterName;
            this.Priority = conflictFilterPriority;
        }

        #endregion

        #region IConflictFilter Members

        /// <summary>
        ///     Gets the name of the conflict filter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        public int Priority { get; protected set; }

        /// <summary>
        ///     Attempts to the resolve the <paramref name="conflictRow" /> that is in conflict using the different states of the
        ///     same row.
        /// </summary>
        /// <param name="conflictRow">The conflict row.</param>
        /// <param name="conflictClass">The conflict class.</param>
        /// <param name="currentRow">The row in the current version that is being edited.</param>
        /// <param name="preReconcileRow">The row prior to reconciliation or edit (child) version (these are edits that you made).</param>
        /// <param name="reconcileRow">The row that the current version is reconciling against or target (parent) version.</param>
        /// <param name="commonAncestorRow">
        ///     The common ancestor row of this version and the reconcile version (as they are in the
        ///     database; this is what the feature and attributes were before any edits were made).
        /// </param>
        /// <param name="childWins">if set to <c>true</c> indicating whether the child conflicts should over rule the targets.</param>
        /// <param name="columnLevel">if set to <c>true</c> indicating whether conflicts will be defined at the column level.</param>
        /// <param name="rowConflictType">Type of the row conflict at a granular level.</param>
        /// <remarks>
        ///     This method should be overridden by derived classes to provide
        ///     specific implementation for filtering row conflicts.
        /// </remarks>
        public abstract void ResolveConflict(IConflictRow conflictRow, IConflictClass conflictClass, IRow currentRow, IRow preReconcileRow, IRow reconcileRow, IRow commonAncestorRow, bool childWins, bool columnLevel, RowConflictType rowConflictType);

        /// <summary>
        ///     Determines whether this instance can resolve the specified conflict type.
        /// </summary>
        /// <param name="conflictType">Type of the conflict.</param>
        /// <param name="conflictClass">The conflict class.</param>
        /// <returns>
        ///     <c>true</c> if this instance can resolve the specified conflict type; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanResolve(TableConflictType conflictType, IConflictClass conflictClass)
        {
            return (conflictClass != null);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Clears the internal caches.
        /// </summary>
        protected void ClearCaches()
        {
            _FieldManagersByClassID.Clear();
        }

        /// <summary>
        ///     Returns the field value at the specified <paramref name="index" /> for the given <paramref name="row" />.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="index">The index.</param>
        /// <returns>Returns the <see cref="System.Object" /> representing the value.</returns>
        protected virtual object GetValue(IRow row, int index)
        {
            IAnnotationFeature annoFeature = row as IAnnotationFeature;
            if (annoFeature != null)
            {
                IFeatureClass oclass = (IFeatureClass) row.Table;
                IAnnotationClassExtension annoClass = oclass.Extension as IAnnotationClassExtension;
                if (annoClass != null)
                {
                    // We have to handle Annotation a little different because the ELEMENT field is controlled by an interface called
                    // IAnnotationFeature that is responsible for setting the element and the Shape.
                    int elementFieldIndex = annoClass.ElementFieldIndex;
                    if (index == elementFieldIndex)
                    {
                        return annoFeature.Annotation;
                    }
                }
            }

            return row.Value[index];
        }

        /// <summary>
        ///     Determines whether the specified field <paramref name="index" /> on the <paramref name="oclass" /> is editable by
        ///     both ESRI and ArcFM.
        /// </summary>
        /// <param name="oclass">The object class.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     <c>true</c> if the specified field index on the class is editable by both ESRI and ArcFM; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsEditable(IObjectClass oclass, int index)
        {
            IField field = oclass.Fields.Field[index];
            if (field.Editable)
            {
                ISubtypes subtypes = (ISubtypes) oclass;

                if (!_FieldManagersByClassID.ContainsKey(oclass.ObjectClassID))
                    _FieldManagersByClassID.Add(oclass.ObjectClassID, oclass.GetFieldManager(subtypes.DefaultSubtypeCode));

                IMMFieldManager fieldManager = _FieldManagersByClassID[oclass.ObjectClassID];
                IMMFieldAdapter fieldAdapter = fieldManager.FieldByIndex(index);
                return fieldAdapter.Editable;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified field is an ArcFM metadata field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///     <c>true</c> if the specified field is metadata; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsMetadata(IField field)
        {
            string[] fieldNames = {"CREATEDATE", "CREATEUSER", "DATEMODIFIED", "LASTUSER"};
            return fieldNames.Contains(field.Name);
        }

        /// <summary>
        ///     Sets the value from the <paramref name="sourceRow" /> on the <paramref name="targetRow" /> for the field with the
        ///     specific <paramref name="index" />.
        /// </summary>
        /// <param name="targetRow">The target row.</param>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="index">The index.</param>
        protected virtual void SetValue(IRow targetRow, IRow sourceRow, int index)
        {
            object o = this.GetValue(sourceRow, index);
            this.SetValue(targetRow, index, o);
        }

        /// <summary>
        ///     Updates the field with the specified field <paramref name="index" /> with the <paramref name="value" /> for the
        ///     row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetValue(IRow row, int index, object value)
        {
            IAnnotationFeature annoFeature = row as IAnnotationFeature;
            IElement element = value as IElement;
            if (annoFeature != null && element != null)
            {
                // We have to handle Annotation a little different because the ELEMENT field is controlled by an interface called
                // IAnnotationFeature that is responsible for setting the element and the Shape.
                IFeatureClass oclass = (IFeatureClass) row.Table;
                IAnnotationClassExtension annoClass = oclass.Extension as IAnnotationClassExtension;
                if (annoClass != null)
                {
                    int elementFieldIndex = annoClass.ElementFieldIndex;
                    if (index == elementFieldIndex)
                    {
                        annoFeature.Annotation = element;
                        return;
                    }
                }
            }

            row.Value[index] = value;
        }

        #endregion
    }
}