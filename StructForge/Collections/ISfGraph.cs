namespace StructForge.Collections
{
    /// <summary>
    /// Represents a generic graph data structure with vertices of type <typeparamref name="TVertex"/>.
    /// Supports adding/removing vertices and edges, checking their existence, and retrieving edge weights.
    /// Pathfinding algorithms are coming in v1.6.0
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertices in the graph.</typeparam>
    public interface ISfGraph<in TVertex>
    {
        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="v">The vertex to add.</param>
        void AddVertex(TVertex v);

        /// <summary>
        /// Adds a directed edge from <paramref name="from"/> to <paramref name="to"/> with an optional weight.
        /// </summary>
        /// <param name="from">The starting vertex of the edge.</param>
        /// <param name="to">The ending vertex of the edge.</param>
        /// <param name="weight">The weight of the edge. Default is 1.0f.</param>
        void AddEdge(TVertex from, TVertex to, float weight = 1.0f);

        /// <summary>
        /// Removes a vertex from the graph, along with all connected edges.
        /// </summary>
        /// <param name="v">The vertex to remove.</param>
        /// <returns>True if the vertex was successfully removed; otherwise, false.</returns>
        bool RemoveVertex(TVertex v);

        /// <summary>
        /// Removes the edge from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The starting vertex of the edge.</param>
        /// <param name="to">The ending vertex of the edge.</param>
        /// <returns>True if the edge was successfully removed; otherwise, false.</returns>
        bool RemoveEdge(TVertex from, TVertex to);

        /// <summary>
        /// Determines whether the specified vertex exists in the graph.
        /// </summary>
        /// <param name="v">The vertex to check.</param>
        /// <returns>True if the vertex exists; otherwise, false.</returns>
        bool HasVertex(TVertex v);

        /// <summary>
        /// Determines whether an edge exists from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The starting vertex of the edge.</param>
        /// <param name="to">The ending vertex of the edge.</param>
        /// <returns>True if the edge exists; otherwise, false.</returns>
        bool HasEdge(TVertex from, TVertex to);

        /// <summary>
        /// Gets the weight of the edge from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        /// <param name="from">The starting vertex of the edge.</param>
        /// <param name="to">The ending vertex of the edge.</param>
        /// <returns>The weight of the edge. Throws an exception if the edge does not exist.</returns>
        float GetEdgeWeight(TVertex from, TVertex to);

        /// <summary>
        /// Gets the total number of vertices in the graph.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Gets the total number of edges in the graph.
        /// </summary>
        int EdgeCount { get; }
    }
}
