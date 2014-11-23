using System;
using System.Diagnostics;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A wrapper around the <see cref="IMMPxUser" /> inteface that can be used with WPF Data Binding.
    /// </summary>
    [DebuggerDisplay("DisplayName = {DisplayName}, ID = {ID}")]
    public class PxUser
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PxUser" /> class.
        /// </summary>
        /// <param name="user">The user.</param>
        public PxUser(IMMPxUser2 user)
        {
            this.ID = user.Id;
            this.Name = user.Name;
            this.DisplayName = user.DisplayName;
            this.Description = user.Description;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set; }

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; private set; }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; private set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        ///     The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            PxUser other = obj as PxUser;
            if (other == null) return false;

            return (string.Equals(other.Name, this.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new {A = Description, B = Name, C = ID, D = DisplayName}.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }

        #endregion
    }
}