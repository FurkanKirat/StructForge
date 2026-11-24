using StructForge.Collections;
using StructForge.Enumerators;
using StructForge.Helpers;

namespace StructForge.Algorithms
{
    /// <summary>
    /// Provides high-performance graph traversal algorithms.
    /// </summary>
    public static class SfGraphAlgorithms
    {
        /// <summary>
        /// Returns a strict struct-enumerator for Depth-First Search.
        /// No heap allocations, no delegate overhead.
        /// Usage: foreach(var node in graph.Dfs(start)) { ... }
        /// </summary>
        public static SfDfsEnumerable<T> Dfs<T>(this SfGraph<T> graph, T start)
        {
            if (!graph.Contains(start))
                SfThrowHelper.ThrowArgument("Start node not in graph");

            return new SfDfsEnumerable<T>(graph, start);
        }

        /// <summary>
        /// Returns a strict struct-enumerator for Breadth-First Search.
        /// No heap allocations, no delegate overhead.
        /// Usage: foreach(var node in graph.Bfs(start)) { ... }
        /// </summary>
        public static SfBfsEnumerable<T> Bfs<T>(this SfGraph<T> graph, T start)
        {
            if (!graph.Contains(start))
                SfThrowHelper.ThrowArgument("Start node not in graph");

            return new SfBfsEnumerable<T>(graph, start);
        }

        /// <summary>
        /// Computes shortest paths from the given start node using Dijkstra's algorithm.
        /// </summary>
        public static (SfDictionary<T, float> dist,
            SfDictionary<T, T> prev) Dijkstra<T>(this SfGraph<T> graph, T start)
        {
            if (!graph.Contains(start))
                SfThrowHelper.ThrowArgument("Start node not in graph");
            
            var distance = new SfDictionary<T, float>(graph.Count, graph.Comparer);
            var prev = new SfDictionary<T, T>(graph.Count, graph.Comparer);
            
            foreach (var node in graph)
                distance[node] = float.PositiveInfinity;
            distance[start] = 0;
            
            var priorityQueue = new SfPriorityQueue<T, float>();
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                float curDist = priorityQueue.PeekPriority();
                T cur = priorityQueue.Dequeue();
                
                if (curDist > distance[cur])
                    continue;

                foreach (var neighbor in graph.GetNeighbors(cur))
                {
                    float newDist = distance[cur] + neighbor.Weight;

                    if (newDist < distance[neighbor.Target])
                    {
                        distance[neighbor.Target] = newDist;
                        priorityQueue.Enqueue(neighbor.Target, newDist);
                        prev[neighbor.Target] = cur;
                    }
                }
            }

            return (distance, prev);
        }
        
        /// <summary>
        /// Computes shortest paths from the given start node using Dijkstra's algorithm.
        /// </summary>
        public static (SfDictionary<T, float> dist,
            SfDictionary<T, T> prev) Dijkstra<T>(this SfGraph<T> graph, T start, T target)
        {
            if (!graph.Contains(start))
                SfThrowHelper.ThrowArgument("Start node not in graph");
            
            var distance = new SfDictionary<T, float>(graph.Count, graph.Comparer);
            var prev = new SfDictionary<T, T>(graph.Count, graph.Comparer);
            
            foreach (var node in graph)
                distance[node] = float.PositiveInfinity;
            distance[start] = 0;
            
            var priorityQueue = new SfPriorityQueue<T, float>();
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count > 0)
            {
                float curDist = priorityQueue.PeekPriority();
                T cur = priorityQueue.Dequeue();
                
                if (curDist > distance[cur])
                    continue;
                
                if (graph.Comparer.Equals(cur, target))
                    break;

                foreach (var neighbor in graph.GetNeighbors(cur))
                {
                    float newDist = distance[cur] + neighbor.Weight;

                    if (newDist < distance[neighbor.Target])
                    {
                        distance[neighbor.Target] = newDist;
                        priorityQueue.Enqueue(neighbor.Target, newDist);
                        prev[neighbor.Target] = cur;
                    }
                }
            }

            return (distance, prev);
        }
    }
}