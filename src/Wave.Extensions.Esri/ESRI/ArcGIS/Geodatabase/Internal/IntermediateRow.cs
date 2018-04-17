using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ESRI.ArcGIS.Geodatabase.Internal
{
    /// <summary>
    ///     A lightweight structure for the intermediate row.
    /// </summary>
    [ComVisible(false)]
    internal class IntermediateRow : IIntermediateRow, IEquatable<IIntermediateRow>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntermediateRow" /> struct.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="relClass">The relationship class.</param>
        public IntermediateRow(IRow row, IRelationshipClass relClass)
        {
            this.Row = row;
            this.Items = new Dictionary<string, object>();

            ITable table = (ITable) relClass;
            this.OriginForeignKey = TypeCast.Cast(row.get_Value(table.FindField(relClass.OriginForeignKey)), string.Empty);
            this.DestinationForeignKey = TypeCast.Cast(row.get_Value(table.FindField(relClass.DestinationForeignKey)), string.Empty);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the destination foreign key.
        /// </summary>
        public string DestinationForeignKey { get; private set; }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        ///     Gets the origin foreign key.
        /// </summary>
        public string OriginForeignKey { get; private set; }

        /// <summary>
        ///     Gets the physical intermediate row.
        /// </summary>
        public IRow Row { get; private set; }

        #endregion

        #region IEquatable<IIntermediateRow> Members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(IIntermediateRow other)
        {
            if (other == null)
                return false;

            return other.DestinationForeignKey.Equals(this.DestinationForeignKey, StringComparison.InvariantCultureIgnoreCase) && other.OriginForeignKey.Equals(this.OriginForeignKey, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region IIntermediateRow Members

        /// <summary>
        ///     Updates the field with specified <paramref name="fieldName" /> with the <paramref name="value" />.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        public void Update(string fieldName, object value)
        {
            if (this.Items.ContainsKey(fieldName))
            {
                int index = this.Row.Fields.FindField(fieldName);

                this.Row.set_Value(index, value);
                this.Row.Store();

                this.Items[fieldName] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            IntermediateRow other = obj as IntermediateRow;
            return this.Equals(other);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new {A = DestinationForeignKey, B = OriginForeignKey}.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", this.Row.OID);
        }

        #endregion
    }
}