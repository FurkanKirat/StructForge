using StructForge.Collections;
using StructForge.Sorting;

namespace StructForge.Tests.Sorting;

public class SortingTests
{
    private readonly Random _random = new Random();
    private (SfList<int> testList, SfList<int> expected) GetIntList(Action<SfList<int>> sort)
    {
        var list = new SfList<int>();
        int count = 100;
        var arr = new int[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = i;
        }
        list.AddRange(arr.OrderBy(_ => _random.Next()));
        sort(list);
        return (list, new SfList<int>(arr));
    }

    private (SfList<char> testList, SfList<char> expected) GetCharList(Action<SfList<char>> sort)
    {
        var str = Guid.NewGuid().ToString().ToCharArray().Distinct().ToArray();
        var list = new SfList<char>();
        list.AddRange(str);
        Array.Sort(str);
        sort(list);
        return (list, new SfList<char>(str));
    }
    
    [Fact]
    public void TreeSort_IntTest()
    {
        var intTest = GetIntList(
            list => SfSorting.TreeSort(list, Comparer<int>.Create((x, y) => x > y ? 1 : -1))
        );
        Assert.Equal(intTest.expected, intTest.testList);
    }

    [Fact]
    public void TreeSort_CharTest()
    {
        var charTest = GetCharList(
            list => SfSorting.TreeSort(list, Comparer<char>.Default)
            );
        Assert.Equal(charTest.expected, charTest.testList);
    }

    [Fact]
    public void QuickSort_IntTest()
    {
        var intTest = GetIntList(
            list => SfSorting.QuickSort(list, Comparer<int>.Create((x, y) => x > y ? 1 : -1))
        );
        Assert.Equal(intTest.expected, intTest.testList);
    }

    [Fact]
    public void QuickSort_CharTest()
    {
        var charTest = GetCharList(
            list => SfSorting.QuickSort(list, Comparer<char>.Default)
        );
        Assert.Equal(charTest.expected, charTest.testList);
    }
}