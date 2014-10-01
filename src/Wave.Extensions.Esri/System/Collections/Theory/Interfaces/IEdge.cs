using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     A directed edge
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    [ComVisible(false)]
    public interface IEdge<out TVertex>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the source vertex
        /// </summary>
        TVertex Source { get; }

        /// <summary>
        ///     Gets the target vertex
        /// </summary>
        TVertex Target { get; }

        #endregion
    }
}