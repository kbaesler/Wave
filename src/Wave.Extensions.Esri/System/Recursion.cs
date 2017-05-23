using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    ///     Represents an item in a recursive projection.
    /// </summary>
    /// <typeparam name="TValue">The type of the item</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Recursion<TValue> : IRecursion<TValue>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Recursion&lt;TValue&gt;" /> class.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <param name="item">The item.</param>
        public Recursion(int depth, TValue item)
        {
            this.Depth = depth;
            this.Value = item;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Used to indicate infinite recursion depth.
        /// </summary>
        public static int Infinity
        {
            get { return -1; }
        }

        /// <summary>
        ///     Gets or sets the recursive depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth { get; set; }

        /// <summary>
        ///     Gets or sets the item.
        /// </summary>
        public TValue Value { get; set; }

        #endregion
    }
}