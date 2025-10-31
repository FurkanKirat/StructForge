using StructForge.Collections;
using StructForge.Comparers;
using StructForge.Searching;

namespace StructForge.Tests.Searching;

public class SfSearchingTests
{
    [Fact]
    public void DoesNotExist()
    {
        var list = new SfList<int> { 1, 2, 3, 4, 7, 98 };
        int search = list.BinarySearch(5);
        Assert.Equal(-1, search);
    }


    [Fact]
    public void DoesExist()
    {
        var list = new SfList<int> { 1, 2, 3, 4, 7, 98 };
        int search = list.BinarySearch(7);
        Assert.Equal(4, search);
    }
    
    [Fact]
    public void EmptyListReturnsMinusOne()
    {
        var list = new SfList<int>();
        int search = list.BinarySearch(10);
        Assert.Equal(-1, search);
    }
    
    [Fact]
    public void SingleElementExists()
    {
        var list = new SfList<int> { 42 };
        int search = list.BinarySearch(42);
        Assert.Equal(0, search);
    }

    [Fact]
    public void SingleElementDoesNotExist()
    {
        var list = new SfList<int> { 42 };
        int search = list.BinarySearch(100);
        Assert.Equal(-1, search);
    }

    
    [Fact]
    public void FirstAndLastElements()
    {
        var list = new SfList<int> { 1, 3, 5, 7, 9 };
        Assert.Equal(0, list.BinarySearch(1)); // first
        Assert.Equal(4, list.BinarySearch(9)); // last
    }

    [Fact]
    public void AllElementsExist()
    {
        var list = new SfList<int> { 2, 4, 6, 8 };
        for (int i = 0; i < list.Count; i++)
            Assert.Equal(i, list.BinarySearch(list[i]));
    }

    [Fact]
    public void CustomComparerDescending()
    {
        var list = new SfList<int> { 9, 7, 5, 3, 1 };
        var comparer = SfComparers<int>.ReverseComparer; // descending
        Assert.Equal(2, list.BinarySearch(5, comparer));
    }


}