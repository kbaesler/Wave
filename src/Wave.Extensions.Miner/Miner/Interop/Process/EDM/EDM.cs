using System;
using System.Data;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A light weight structure used to hold extended data management data.
    /// </summary>
    public struct EDM
    {
        #region Fields

        /// <summary>
        ///     Extended Data property name.
        /// </summary>
        public string Name;

        /// <summary>
        ///     Extended Data property type.
        /// </summary>
        public string Type;

        /// <summary>
        ///     Extended Data property value.
        /// </summary>
        public string Value;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EDM" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public EDM(string name, string value, string type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EDM" /> struct.
        /// </summary>
        /// <param name="row">The row </param>
        internal EDM(DataRow row)
            : this((row[Fields.Name] != DBNull.Value) ? row[Fields.Name].ToString().Trim() : "",
                (row[Fields.Value] != DBNull.Value) ? row[Fields.Value].ToString().Trim() : "",
                (row[Fields.Type] != DBNull.Value) ? row[Fields.Type].ToString().Trim() : "")
        {
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
            if (!(obj is EDM))
                return false;

            EDM other = (EDM) obj;
            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(this.Type, other.Type, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        /// <summary>
        ///     The inequality operator (!=) returns false if its operands are equal, true otherwise.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///     <c>true</c> if the specified <paramref name="x" /> is not equal to the <paramref name="y" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool operator !=(EDM x, EDM y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        ///     The equality operator (=) returns true if the values of its operands are equal, false otherwise.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///     <c>true</c> if the specified <paramref name="x" /> is equal to the <paramref name="y" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(EDM x, EDM y)
        {
            return x.Equals(y);
        }

        #region Nested Type: Fields

        /// <summary>
        ///     The field names that are required by the <see cref="DataRow" /> for the <see cref="EDM" /> struct.
        /// </summary>
        internal struct Fields
        {
            #region Constants

            /// <summary>
            ///     The EDM_NAME field name.
            /// </summary>
            public const string Name = "EDM_NAME";

            /// <summary>
            ///     The EDM_TYPE field name.
            /// </summary>
            public const string Type = "EDM_TYPE";

            /// <summary>
            ///     The EDM_VALUE field name.
            /// </summary>
            public const string Value = "EDM_VALUE";

            #endregion
        }

        #endregion
    }
}