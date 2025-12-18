using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using StructForge.Comparers;
using StructForge.Helpers;

namespace StructForge.Collections
{
    /// <inheritdoc cref="ISfGraph{V}" />
    [DebuggerTypeProxy(typeof(SfGraphDebugView<>))]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SfGraph<TVertex> : ISfGraph<TVertex>, ISfDataStructure<TVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        public struct SfGraphEdge
        {
            /// <summary>
            /// Target Vertex for edge
            /// </summary>
            public readonly TVertex Target;
            /// <summary>
            /// Weight of the edge
            /// </summary>
            public readonly float Weight;

            internal SfGraphEdge(TVertex target, float weight)
            {
                Target = target;
                Weight = weight;
            }
        }
        private readonly SfDictionary<TVertex, SfList<SfGraphEdge>> _adjacencyList;
        private readonly IEqualityComparer<TVertex> _comparer;
        private int _edgeCount;

        /// <inheritdoc />
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _adjacencyList.Count;
        }

        /// <inheritdoc />
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _adjacencyList.Count == 0;
        }

        /// <inheritdoc />
        public int VertexCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _adjacencyList.Count;
        }

        /// <inheritdoc />
        public int EdgeCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _edgeCount;
        }

        /// <summary>
        /// Creates a new instance of SfGraph
        /// </summary>
        /// <param name="capacity">Starting Capacity of internal dictionary</param>
        /// <param name="comparer">Equality Comparer for internal dictionary</param>
        public SfGraph(int capacity = 16, IEqualityComparer<TVertex> comparer = null)
        {
            _comparer = comparer ?? SfEqualityComparers<TVertex>.Default;
            _adjacencyList = new SfDictionary<TVertex, SfList<SfGraphEdge>>(capacity, _comparer);
        }

        /// <inheritdoc />
        public void AddVertex(TVertex v)
        {
            _adjacencyList.TryAdd(v, new SfList<SfGraphEdge>(1));
        }

        /// <inheritdoc />
        public void AddEdge(TVertex from, TVertex to, float weight)
        {
            if (!_adjacencyList.TryGetValue(from, out var list))
            {
                list = new SfList<SfGraphEdge>();
                _adjacencyList.Add(from, list);
            }

            if (!_adjacencyList.ContainsKey(to))
            {
                _adjacencyList.Add(to, new SfList<SfGraphEdge>(1));
            }
            
            bool edgeExists = false;
            foreach(var edge in list)
            {
                if (_comparer.Equals(edge.Target, to))
                {
                    edgeExists = true; 
                    break;
                }
            }

            if (!edgeExists)
            {
                list.Add(new SfGraphEdge(to, weight));
                _edgeCount++;
            }
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveVertex(TVertex v)
        {
            if (_adjacencyList.TryGetValue(v, out var outgoingEdges))
            {
                _edgeCount -= outgoingEdges.Count;
                
                _adjacencyList.Remove(v);
            }
            else
            {
                return false;
            }
            
            foreach (var key in _adjacencyList.Keys)
            {
                RemoveEdge(key, v);
            }
            return true;
        }


        /// <inheritdoc />
        public bool RemoveEdge(TVertex from, TVertex to)
        {
            if (_adjacencyList.TryGetValue(from, out SfList<SfGraphEdge> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (_comparer.Equals(list[i].Target, to))
                    {
                        list.RemoveAt(i);
                        _edgeCount--;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasVertex(TVertex v) => _adjacencyList.ContainsKey(v);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasEdge(TVertex from, TVertex to)
        {
            if (_adjacencyList.TryGetValue(from, out SfList<SfGraphEdge> list))
            {
                foreach(var edge in list)
                    if (_comparer.Equals(edge.Target, to))
                        return true;
            }
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetEdgeWeight(TVertex from, TVertex to)
        {
            if (_adjacencyList.TryGetValue(from, out SfList<SfGraphEdge> list))
                foreach (SfGraphEdge edge in list)
                    if(_comparer.Equals(edge.Target, to))
                        return edge.Weight;
            
            return float.PositiveInfinity;
        }

        /// <summary>
        /// Returns the neighbors of given vertex
        /// </summary>
        /// <param name="v">Given vertex</param>
        /// <returns>Neighbors of given vertex</returns>
        /// <exception cref="KeyNotFoundException">If vertex does not exist throws KeyNotFoundException</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SfList<SfGraphEdge> GetNeighbors(TVertex v)
        {
            if (_adjacencyList.TryGetValue(v, out var list))
                return list;
            
            throw new KeyNotFoundException($"Vertex {v} not found in graph.");
        }
        
        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public SfDictionary<TVertex, SfList<SfGraphEdge>>.SfKeyCollection Vertices => _adjacencyList.Keys;
        
        /// <summary>
        /// Returns an enumerator for iterating over the collection.
        /// Can be used by <c>foreach</c> loops.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public SfDictionary<TVertex, SfList<SfGraphEdge>>.SfKeyCollection GetEnumerator() => _adjacencyList.Keys;
        /// <inheritdoc />
        IEnumerator<TVertex> IEnumerable<TVertex>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <inheritdoc />
        public void ForEach(Action<TVertex> action)
        {
            foreach (var v in _adjacencyList.Keys)
                action(v);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TVertex item) => HasVertex(item);

        /// <inheritdoc />
        public bool Contains(TVertex item, IEqualityComparer<TVertex> comparer)
        {
            if (Equals(_comparer, comparer))
                return _adjacencyList.ContainsKey(item);
            
            foreach (var v in _adjacencyList.Keys)
                if (comparer.Equals(v, item))
                    return true;
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _adjacencyList.Clear();
            _edgeCount = 0;
        }

        /// <inheritdoc />
        public void CopyTo(TVertex[] array, int arrayIndex)
        {
            if (array is null)
                SfThrowHelper.ThrowArgumentNull(nameof(array));
            if (arrayIndex < 0)
                SfThrowHelper.ThrowArgumentOutOfRange(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length) 
                SfThrowHelper.ThrowArgument("Destination array is not large enough.");

            foreach (TVertex v in _adjacencyList.Keys)
                array[arrayIndex++] = v;
        }

        /// <inheritdoc />
        public TVertex[] ToArray()
        {
            TVertex[] array = new TVertex[_adjacencyList.Count];
            CopyTo(array, 0);
            return array;
        }
        private string DebuggerDisplay => $"SfGraph<{typeof(TVertex).Name}> (Vertex Count {VertexCount}, Edge Count: {_edgeCount})";
    }
    
    internal sealed class SfGraphDebugView<T>
    {
        private readonly SfGraph<T> _graph;

        public SfGraphDebugView(SfGraph<T> graph)
        {
            _graph = graph;
        }

        public T[] Vertices
        {
            get
            {
                if (_graph?.Vertices == null) return Array.Empty<T>();
                try
                {
                    var vertices = new T[_graph.VertexCount];
                    int i = 0;
                    foreach (var v in _graph.Vertices)
                    {
                        vertices[i++] = v;
                    }

                    return vertices;
                }
                catch
                {
                    return Array.Empty<T>();
                }
                
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public SfGraphEdgeView[] Edges
        {
            get
            {
                if (_graph == null) return Array.Empty<SfGraphEdgeView>();

                var result = new List<SfGraphEdgeView>();

                try
                {
                    var safeVertices = _graph.ToArray();

                    for (int i = 0; i < safeVertices.Length; i++)
                    {
                        var from = safeVertices[i];
                
                        try
                        {
                            var neighbors = _graph.GetNeighbors(from);
                            
                            if (neighbors == null || neighbors.Count == 0) continue;
                            
                            for (int j = 0; j < neighbors.Count; j++)
                            {
                                var edge = neighbors[j]; 
                                result.Add(new SfGraphEdgeView(from, edge.Target, edge.Weight));
                            }
                        }
                        catch 
                        {
                            continue; 
                        }
                    }
                }
                catch
                {
                    return Array.Empty<SfGraphEdgeView>();
                }

                return result.ToArray();
            }
        }
        [DebuggerDisplay("{From} -> {To} (W: {Weight})")]
        public struct SfGraphEdgeView
        {
            public T From { get; }
            public T To { get; }
            public float Weight { get; }

            public SfGraphEdgeView(T from, T to, float weight)
            {
                From = from;
                To = to;
                Weight = weight;
            }
        }
    }
}