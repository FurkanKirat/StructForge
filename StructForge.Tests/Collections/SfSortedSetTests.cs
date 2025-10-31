using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfSortedSetTests
{
    private record SfPlayer (int Id, string Name);
    [Fact]
    public void SfSortedSetTest()
    {
        var set = new SfSortedSet<SfPlayer>(Comparer<SfPlayer>.Create((p1, p2) => string.Compare(p1.Name, p2.Name, StringComparison.Ordinal)));
        var p1 = new SfPlayer(1, "John");
        var p2 = new SfPlayer(2, "John");
        
        set.Add(p1);
        
        Assert.True(set.TryGetValue(p2, out var actualValue));
        Assert.Equal(p1, actualValue);
    }
}