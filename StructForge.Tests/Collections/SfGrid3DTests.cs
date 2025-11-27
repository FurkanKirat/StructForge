namespace StructForge.Tests.Collections;

using StructForge.Collections;
using Xunit;
using System;

public class SfGrid3DTests
{
    [Fact]
    public void Constructor_Valid()
    {
        var g = new SfGrid3D<int>(4, 3, 2);
        Assert.Equal(4, g.Width);
        Assert.Equal(3, g.Height);
        Assert.Equal(2, g.Depth);
        Assert.Equal(4 * 3 * 2, g.Count);
    }

    [Fact]
    public void Constructor_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid3D<int>(0, 2, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid3D<int>(2, 0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid3D<int>(2, 2, 0));
    }

    [Fact]
    public void Constructor_WithData_Valid()
    {
        int[] arr = new int[8];
        arr[3] = 99;

        var g = new SfGrid3D<int>(2, 2, 2, arr);
        Assert.Equal(99, g[1, 1, 0]);
    }

    [Fact]
    public void Constructor_WithWrongData_Throws()
    {
        Assert.Throws<ArgumentException>(() => new SfGrid3D<int>(2, 2, 2, new int[3]));
    }

    [Fact]
    public void CopyConstructor_Works()
    {
        var a = new SfGrid3D<int>(2, 2, 2);
        a[1, 1, 1] = 42;

        var b = new SfGrid3D<int>(a);
        Assert.Equal(42, b[1, 1, 1]);
    }

    [Fact]
    public void Indexer_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        g[1, 0, 1] = 55;
        Assert.Equal(55, g[1, 0, 1]);
    }

    [Fact]
    public void ToIndex_Works()
    {
        var g = new SfGrid3D<int>(4, 3, 2);
        Assert.Equal(0, g.ToIndex(0, 0, 0));
        Assert.Equal(3, g.ToIndex(3, 0, 0));
        Assert.Equal(4, g.ToIndex(0, 1, 0));
        Assert.Equal(12, g.ToIndex(0, 0, 1)); // next layer
    }

    [Fact]
    public void ToCoords_Works()
    {
        var g = new SfGrid3D<int>(4, 3, 2);
        var (x, y, z) = g.ToCoords(15);
        Assert.Equal(3, x);
        Assert.Equal(0, y);
        Assert.Equal(1, z);
    }

    [Fact]
    public void TryGet_Works()
    {
        var g = new SfGrid3D<int>(3, 3, 3);
        g[2, 1, 1] = 123;

        Assert.True(g.TryGet(2, 1, 1, out var v));
        Assert.Equal(123, v);

        Assert.False(g.TryGet(5, 1, 1, out _));
    }

    [Fact]
    public void Fill_Works()
    {
        var g = new SfGrid3D<int>(3, 3, 3);
        g.Fill(8);

        for (int i = 0; i < g.Count; i++)
            Assert.Equal(8, g[i]);
    }

    [Fact]
    public void Clear_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        g.Fill(5);
        g.Clear();

        for (int i = 0; i < g.Count; i++)
            Assert.Equal(0, g[i]);
    }

    [Fact]
    public void Contains_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        g[1, 1, 1] = 77;

        Assert.True(g.Contains(77));
        Assert.False(g.Contains(10));
    }

    [Fact]
    public void UnsafeRef_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        ref int r = ref g.GetUnsafeRef(1, 1, 1);
        r = 1234;

        Assert.Equal(1234, g[1, 1, 1]);
    }

    [Fact]
    public void CopyTo_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        g.Fill(3);

        int[] arr = new int[8];
        g.CopyTo(arr, 0);

        foreach (var v in arr)
            Assert.Equal(3, v);
    }

    [Fact]
    public void Enumerator_Works()
    {
        var g = new SfGrid3D<int>(2, 2, 2);
        g.Fill(9);

        int count = 0;
        foreach (int v in g)
        {
            Assert.Equal(9, v);
            count++;
        }

        Assert.Equal(8, count);
    }
}
