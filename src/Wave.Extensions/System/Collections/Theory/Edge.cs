using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     The default <see cref="IEdge&lt;TVertex&gt;" /> implementation.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    [DebuggerDisplay("{Source}->{Target}")]
    [ComVisible(false)]
    public class Edge<TVertex> : IEdge<TVertex>
    {
        #region Fields

        private readonly TVertex _Source;
        private readonly TVertex _Target;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Edge&lt;TVertex&gt;" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public Edge(TVertex source, TVertex target)
        {
            _Source = source;
            _Target = target;
        }

        #endregion

        #region IEdge<TVertex> Members

        /// <summary>
        ///     Gets the source vertex
        /// </summary>
        /// <value></value>
        public TVertex Source
        {
            get { return _Source; }
        }

        /// <summary>
        ///     Gets the target vertex
        /// </summary>
        /// <value></value>
        public TVertex Target
        {
            get { return _Target; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return this.Source + "->" + this.Target;
        }

        #endregion
    }
}