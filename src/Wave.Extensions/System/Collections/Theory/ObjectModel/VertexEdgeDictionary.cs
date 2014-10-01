using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
    /// <summary>
    ///     An observable dictionary of vertices and it's collection of edges.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [ComVisible(false)]
    public sealed class VertexEdgeDictionary<TVertex, TEdge> : ObservableDictionary<TVertex, EdgeCollection<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
    }
}