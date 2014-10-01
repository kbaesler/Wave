using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An interface used to construct a process framework work request entity.
    /// </summary>
    [ComVisible(false)]
    public interface IPxWorkRequest : IPxNode
    {
        #region Public Properties

        /// <summary>
        ///     Get or set the Comments associated with this instance.
        /// </summary>
        /// <value>
        ///     The comments.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The comments cannot be larger then 255 characters.</exception>
        string Comments { get; set; }

        /// <summary>
        ///     Gets the customer.
        /// </summary>
        /// <value>
        ///     The customer.
        /// </value>
        Customer Customer { get; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The description cannot be larger then 128 characters.</exception>
        string Description { get; set; }

        /// <summary>
        ///     Gets all of the designs that are associated with the <see cref="IPxWorkRequest" />.
        /// </summary>
        IEnumerable<IPxDesign> Designs { get; }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        /// <value>
        ///     The end date.
        /// </value>
        DateTime? EndDate { get; set; }

        /// <summary>
        ///     Gets the location.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        Location Location { get; }

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        /// <value>
        ///     The owner.
        /// </value>
        IMMPxUser Owner { get; set; }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>
        ///     The start date.
        /// </value>
        DateTime? StartDate { get; set; }

        #endregion
    }
}