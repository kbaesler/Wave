using System;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An interface used to construct a process framework session entity.
    /// </summary>
    [ComVisible(false)]
    public interface IPxSession : IPxNode
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the create date.
        /// </summary>
        /// <value>The create date.</value>
        DateTime CreateDate { get; set; }

        /// <summary>
        ///     Gets or sets the create user.
        /// </summary>
        /// <value>
        ///     The create user.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The create user cannot be larger then 32 characters.</exception>
        string CreateUser { get; set; }

        /// <summary>
        ///     Gets or sets the database identifier.
        /// </summary>
        /// <value>
        ///     The database.
        /// </value>
        string Database { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 255 characters.</exception>
        string Description { get; set; }

        /// <summary>
        ///     Gets or sets the enterprise identifier.
        /// </summary>
        /// <value>
        ///     The enterprise.
        /// </value>
        int Enterprise { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxSession" /> is view only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="IPxSession" /> is view only; otherwise, <c>false</c>.
        /// </value>
        bool IsViewOnly { get; }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The owner cannot be larger then 32 characters.</exception>
        string Owner { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IPxSession" /> is redlining.
        /// </summary>
        /// <value>
        ///     <c>true</c> if redlining; otherwise, <c>false</c>.
        /// </value>
        bool Redlining { get; set; }

        /// <summary>
        ///     Gets or sets the type of the session.
        /// </summary>
        /// <value>
        ///     The type of the session.
        /// </value>
        mmSessionNodeType SessionType { get; set; }

        #endregion
    }
}