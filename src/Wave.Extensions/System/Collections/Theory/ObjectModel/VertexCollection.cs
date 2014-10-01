using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
    /// <summary>
    ///     An observable collection of vertices.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    [ComVisible(false)]
    public sealed class VertexCollection<TVertex> : Collection<TVertex>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VertexCollection&lt;TVertex&gt;" /> class.
        /// </summary>
        public VertexCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VertexCollection&lt;TVertex&gt;" /> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public VertexCollection(VertexCollection<TVertex> other)
            : base(other)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns></returns>
        public VertexCollection<TVertex> Clone()
        {
            return new VertexCollection<TVertex>(this);
        }

        #endregion
    }
}