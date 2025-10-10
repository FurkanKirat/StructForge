namespace StructForge.Tests.Collections;

using Xunit;
using StructForge.Collections;

using System;
using System.Linq;

public class SfBinaryHeapTests
{
    [Fact]
    public void MaxHeap_AddAndPop_ShouldMaintainMaxHeapProperty()
    {
        var heap = new SfMaxHeap<int>();

        int[] values = { 20, 5, 30, 15, 40, 10 };
        foreach (var v in values)
            heap.Add(v);

        int previous = int.MaxValue;
        while (!heap.IsEmpty)
        {
            int current = heap.Pop();
            Assert.True(current <= previous, "Heap property violated in MaxHeap");
            previous = current;
        }
    }

    [Fact]
    public void MinHeap_AddAndPop_ShouldMaintainMinHeapProperty()
    {
        var heap = new SfMinHeap<int>();

        int[] values = { 20, 5, 30, 15, 40, 10 };
        foreach (var v in values)
            heap.Add(v);

        int previous = int.MinValue;
        while (!heap.IsEmpty)
        {
            int current = heap.Pop();
            Assert.True(current >= previous, "Heap property violated in MinHeap");
            previous = current;
        }
    }

    [Fact]
    public void Heap_ShouldHandleDuplicateValuesCorrectly()
    {
        var heap = new SfMaxHeap<int>();
        int[] values = { 5, 10, 10, 20, 20, 20 };
        foreach (var v in values)
            heap.Add(v);

        int last = int.MaxValue;
        while (!heap.IsEmpty)
        {
            int current = heap.Pop();
            Assert.True(current <= last, "Heap property violated with duplicates");
            last = current;
        }
    }

    [Fact]
    public void Heap_ConstructorWithEnumerable_ShouldBuildCorrectHeap()
    {
        int[] initial = { 15, 5, 20, 10, 30 };
        var heap = new SfMaxHeap<int>(initial);

        int[] popped = new int[initial.Length];
        for (int i = 0; i < initial.Length; i++)
            popped[i] = heap.Pop();

        var sorted = initial.OrderByDescending(x => x).ToArray();
        Assert.Equal(sorted, popped);
    }

    [Fact]
    public void Peek_ShouldAlwaysReturnRootWithoutRemoving()
    {
        var heap = new SfMaxHeap<int>();
        heap.Add(10);
        heap.Add(20);
        heap.Add(5);

        int peeked = heap.Peek();
        Assert.Equal(20, peeked);
        Assert.Equal(3, heap.Count);

        heap.Pop();
        peeked = heap.Peek();
        Assert.Equal(10, peeked);
    }

    [Fact]
    public void Contains_ShouldReturnCorrectValues()
    {
        var heap = new SfMinHeap<int>();
        heap.Add(10);
        heap.Add(20);
        heap.Add(5);

        Assert.True(heap.Contains(10));
        Assert.True(heap.Contains(5));
        Assert.False(heap.Contains(100));
    }

    [Fact]
    public void Pop_OnEmptyHeap_ShouldThrow()
    {
        var heap = new SfMaxHeap<int>();
        Assert.Throws<InvalidOperationException>(() => heap.Pop());
    }

    [Fact]
    public void BulkRandomInsertions_ShouldMaintainHeapProperty()
    {
        var rnd = new Random();
        var heap = new SfMaxHeap<int>();
        int[] nums = Enumerable.Range(1, 1000).Select(_ => rnd.Next(1, 5000)).ToArray();

        foreach (var n in nums)
            heap.Add(n);

        int prev = int.MaxValue;
        while (!heap.IsEmpty)
        {
            int current = heap.Pop();
            Assert.True(current <= prev, "MaxHeap property violated during bulk random insertion");
            prev = current;
        }
    }
}
