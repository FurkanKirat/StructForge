using StructForge.Algorithms;
using StructForge.Collections;

namespace StructForge.Tests.Algorithms;

public class SfGraphAlgorithmTests
{
    [Fact]
    public void Dfs_Visits_All_Reachable_Nodes()
    {
        var graph = new SfGraph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(1, 4);
        // 1 -> 2 -> 3
        // 1 -> 4

        var visited = new HashSet<int>();
        
        foreach (var node in graph.Dfs(1))
        {
            visited.Add(node);
        }

        Assert.Equal(4, visited.Count);
        Assert.Contains(1, visited);
        Assert.Contains(2, visited);
        Assert.Contains(3, visited);
        Assert.Contains(4, visited);
    }
    
    [Fact]
    public void Dfs_With_Cycle_Visits_Each_Node_Once()
    {
        var graph = new SfGraph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 1); // cycle

        var visited = new List<int>();

        foreach (var node in graph.Dfs(1))
        {
            visited.Add(node);
        }

        Assert.Equal(3, visited.Count);
        Assert.Contains(1, visited);
        Assert.Contains(2, visited);
        Assert.Contains(3, visited);
    }

    [Fact]
    public void Dfs_Does_Not_Visit_Disconnected_Nodes()
    {
        var graph = new SfGraph<int>();
        graph.AddEdge(1, 2);
        graph.AddVertex(99);

        var visited = new HashSet<int>();

        foreach (var node in graph.Dfs(1))
        {
            visited.Add(node);
        }

        Assert.DoesNotContain(99, visited);
        Assert.Contains(1, visited);
        Assert.Contains(2, visited);
        Assert.Equal(2, visited.Count);
    }
    
    [Fact]
    public void Dfs_Enumerable_Returns_All_Reachable_Nodes()
    {
        var graph = new SfGraph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 4);
        
        var result = new HashSet<int>(graph.Dfs(1).ToList());

        Assert.Equal(4, result.Count);
        Assert.True(result.SetEquals(new[] { 1, 2, 3, 4 }));
    }

    [Fact]
    public void Dfs_Throws_If_Start_Node_Not_In_Graph()
    {
        var graph = new SfGraph<int>();
        
        Assert.Throws<ArgumentException>(() =>
        {
            _ = graph.Dfs(42); 
        });
    }

    [Fact]
    public void Dfs_Visits_Each_Node_Only_Once_Duplicate_Edges()
    {
        var graph = new SfGraph<int>();
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 2); // duplicate edge
        graph.AddEdge(2, 1); // cycle

        int counter = 0;
        
        foreach (var _ in graph.Dfs(1))
        {
            counter++;
        }
        
        Assert.Equal(2, counter);
    }
    
    [Fact]
    public void Bfs_ShouldVisitNodesLevelByLevel()
    {
        var graph = new SfGraph<int>();

        // 1 -> 2 -> 4
        // 1 -> 3
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(2, 4);

        var result = graph.Bfs(1).ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal(1, result[0]);
        
        int index2 = result.IndexOf(2);
        int index3 = result.IndexOf(3);
        int index4 = result.IndexOf(4);

        Assert.True(index2 < index4, "Node 2 should come before Node 4");
        Assert.True(index3 < index4, "Node 3 should come before Node 4");
    }

    [Fact]
    public void Bfs_ShouldVisitEachNodeOnce()
    {
        var graph = new SfGraph<int>();

        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 1); // cycle

        var visited = graph.Bfs(1).ToList();

        Assert.Equal(3, visited.Count);
        Assert.Equal(3, visited.Distinct().Count());
    }
    
    [Fact]
    public void Dijkstra_FindsShortestPathsCorrectly()
    {
        var graph = new SfGraph<string>();

        graph.AddEdge("A", "B", 1);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("B", "D", 2);
        graph.AddEdge("B", "C", 5);
        graph.AddEdge("C", "D", 1);

        var (dist, prev) = graph.Dijkstra("A");

        Assert.Equal(0f, dist["A"]);
        Assert.Equal(1f, dist["B"]);
        Assert.Equal(4f, dist["C"]);
        Assert.Equal(3f, dist["D"]);

        Assert.Equal("A", prev["B"]);
        Assert.Equal("B", prev["D"]);
    }
    
    [Fact]
    public void Dijkstra_WithTarget_FindsShortestPathAndStopsEarly()
    {
        // Arrange
        var graph = new SfGraph<string>();

        graph.AddEdge("A", "B", 1);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("B", "D", 2);
        graph.AddEdge("B", "C", 5);
        graph.AddEdge("C", "D", 1);

        // Act
        var (dist, prev) = graph.Dijkstra("A", "D");

        // Assert distance to target
        Assert.Equal(3f, dist["D"]);

        // Assert path via prev
        Assert.Equal("B", prev["D"]);
        Assert.Equal("A", prev["B"]);

        // Start node has zero distance
        Assert.Equal(0f, dist["A"]);
    }


}