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

            Assert.Equal(2, neighbors.Count);
            
            bool found2 = false, found3 = false;
            foreach (var edge in neighbors)
            {
                if (edge.Target == 2) found2 = true;
                if (edge.Target == 3) found3 = true;
            }
            Assert.True(found2 && found3);
        }
    }
}