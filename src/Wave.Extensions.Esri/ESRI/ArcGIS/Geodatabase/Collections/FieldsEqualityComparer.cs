using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     An equaility comparer that validates that all of the field values are equal.
    /// </summary>
    public class FieldsEqualityComparer : IEqualityComparer<IRow>
    {
        #region IEqualityComparer<IRow> Members

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IRow x, IRow y)
        {
            return this.Equals(x, y, o => o.Editable);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(IRow obj)
        {
            return new {A = ((IDataset) obj.Table).Name, B = obj.OID}.GetHashCode();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <param name="action">The action that evaluates the <see cref="IField" /> value.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IRow x, IRow y, Func<IField, bool> action)
        {
            if ((x == null) & (y == null))
                return true;

            if ((x == null) ^ (y == null))
                return false;

            // Iterate through all of the fields.
            for (int i = 0; i < x.Fields.FieldCount; i++)
            {
                // Compare only those fields that are editable by ESRI.
                IField field = x.Fields.Field[i];
                if (action(field))
                {
                    if (!this.Equals(x, y, i))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type to compare.</param>
        /// <param name="y">The second object of type to compare.</param>
        /// <param name="index">The index of the field that will be compare.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(IRow x, IRow y, int index)
        {
            if ((x == null) & (y == null))
                return true;

            if ((x == null) ^ (y == null))
                return false;

            IAnnotationFeature xAnnoFeature = x as IAnnotationFeature;
            IAnnotationFeature yAnnoFeature = y as IAnnotationFeature;
            if (xAnnoFeature != null && yAnnoFeature != null)
            {
                // We have to handle Annotation a little different because the ELEMENT field is controlled by an interface called
                // IAnnotationFeature that is responsible for setting the element and the Shape.
                IFeatureClass oclass = x.Table as IFeatureClass;
                if (oclass != null)
                {
                    IAnnotationClassExtension annoClass = oclass.Extension as IAnnotationClassExtension;
                    if (annoClass != null)
                    {
                        int elementFieldIndex = annoClass.ElementFieldIndex;
                        if (index == elementFieldIndex)
                        {
                            // Since it's Annotation, pass in the Element for comparison instead
                            // of the ELEMENT value itself.
                            return this.AreEqual(xAnnoFeature.Annotation, yAnnoFeature.Annotation);
                        }
                    }
                }
            }

            return this.AreEqual(x.Value[index], y.Value[index]);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object  to compare.</param>
        /// <param name="y">The second object  to compare.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool AreEqual(object x, object y)
        {
            //if (x is IRelationalOperator && y is IRelationalOperator)
            //{
            //    // If you add this next part, it will account for the case where two versions
            //    // tapped into the same complex edge.  The reconcile will stitch up the
            //    // connectivity and geometry in these cases.  However, if the 2 lines just
            //    // had vertices added to them, only one version of the vertices will be in the final feature.
            //    return ((IRelationalOperator)x).Equals(y);
            //}

            IClone xClone = x as IClone;
            IClone yClone = y as IClone;
            if (xClone != null && yClone != null)
            {
                return xClone.IsEqual(yClone);
            }

            return Equals(x, y);
        }

        #endregion
    }
}