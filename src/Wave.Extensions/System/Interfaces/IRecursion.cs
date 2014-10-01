using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    ///     Provides the properties to represent an item in a recursive projection.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    public interface IRecursion<TValue>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the recursive depth.
        /// </summary>
        /// <value>The depth.</value>
        int Depth { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        TValue Value { get; set; }

        #endregion
    }
}