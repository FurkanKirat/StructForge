using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfGraphTests
    {
        [Fact]
        public void AddVertex_ShouldIncreaseCount()
        {
            var graph = new SfGraph<int>();
            graph.AddVertex(1);
            graph.AddVertex(2);

            Assert.Equal(2, graph.VertexCount);
            Assert.True(graph.HasVertex(1));
            Assert.True(graph.HasVertex(2));
        }

        [Fact]
        public void AddEdge_ShouldAddMissingVerticesImplicitly()
        {
            var graph = new SfGraph<string>();
            graph.AddEdge("A", "B", 10.0f);

            Assert.Equal(2, graph.VertexCount);
            Assert.True(graph.HasVertex("A"));
            Assert.True(graph.HasVertex("B"));
            Assert.Equal(1, graph.EdgeCount);
        }

        [Fact]
        public void AddEdge_ShouldStoreWeightCorrectly()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2, 5.5f);

            float weight = graph.GetEdgeWeight(1, 2);
            Assert.Equal(5.5f, weight);
        }

        [Fact]
        public void RemoveEdge_ShouldDecreaseEdgeCount()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2, 1);
            graph.AddEdge(1, 3, 1);

            bool removed = graph.RemoveEdge(1, 2);

            Assert.True(removed);
            Assert.Equal(1, graph.EdgeCount);
            Assert.False(graph.HasEdge(1, 2));
            Assert.True(graph.HasEdge(1, 3));
        }

        [Fact]
        public void RemoveVertex_ShouldRemoveIncomingEdges()
        {
            var graph = new SfGraph<string>();
            
            graph.AddEdge("A", "B", 1); // A -> B
            graph.AddEdge("C", "B", 1); // C -> B
            graph.AddEdge("B", "D", 1); // B -> D

            graph.RemoveVertex("B");

            Assert.False(graph.HasVertex("B"));
            
            Assert.False(graph.HasEdge("A", "B")); 
            Assert.False(graph.HasEdge("C", "B"));
            
            Assert.True(graph.HasVertex("D"));
            Assert.Equal(0, graph.EdgeCount);
        }

        [Fact]
        public void GetNeighbors_ShouldReturnCorrectList()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2, 10);
            graph.AddEdge(1, 3, 20);

            var neighbors = graph.GetNeighbors(1);

            Assert.Equal(2, graph.GetNeighborCount(1));
            
            bool found2 = false, found3 = false;
            foreach (var edge in neighbors)
            {
                if (edge.Target == 2) found2 = true;
                if (edge.Target == 3) found3 = true;
            }
            Assert.True(found2 && found3);
        }
        
        [Fact]
        public void GetNeighbors_Returns_Only_Direct_Outgoing_Edges()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 4);

            var neighbors = new List<int>();
            foreach (var edge in graph.GetNeighbors(1))
            {
                neighbors.Add(edge.Target);
            }

            Assert.Equal(2, neighbors.Count);
            Assert.Contains(2, neighbors);
            Assert.Contains(3, neighbors);
            Assert.DoesNotContain(4, neighbors);
        }

        [Fact]
        public void GetNeighbors_Returns_Empty_For_Node_With_No_Outgoing_Edges()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2);
            
            var count = 0;
            foreach (var _ in graph.GetNeighbors(2))
            {
                count++;
            }

            Assert.Equal(0, count);
        }

        [Fact]
        public void GetNeighbors_Respects_Directionality()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2); // 1 -> 2

            var neighborsOf1 = new List<int>();
            foreach(var e in graph.GetNeighbors(1)) neighborsOf1.Add(e.Target);

            var neighborsOf2 = new List<int>();
            foreach(var e in graph.GetNeighbors(2)) neighborsOf2.Add(e.Target);

            Assert.Single(neighborsOf1);
            Assert.Contains(2, neighborsOf1);

            Assert.Empty(neighborsOf2);
        }

        [Fact]
        public void GetNeighbors_Handles_Self_Loops()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 1);

            var neighbors = new List<int>();
            foreach (var edge in graph.GetNeighbors(1))
            {
                neighbors.Add(edge.Target);
            }

            Assert.Single(neighbors);
            Assert.Equal(1, neighbors[0]);
        }

        [Fact]
        public void GetNeighbors_Throws_If_Node_Does_Not_Exist()
        {
            var graph = new SfGraph<int>();
            graph.AddVertex(1);

            Assert.Throws<KeyNotFoundException>(() =>
            {
                foreach (var _ in graph.GetNeighbors(99)) { }
            });
        }

        [Fact]
        public void GetNeighbors_Enumerates_Without_Allocation()
        {
            var graph = new SfGraph<int>();
            graph.AddEdge(1, 2);

            var enumerator = graph.GetNeighbors(1);
            
            Assert.True(enumerator.GetType().IsValueType, "GetNeighbors should return a struct enumerator to avoid GC");
        }
    }
}