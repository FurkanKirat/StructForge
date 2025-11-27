using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfGrid2DTests
{
    [Fact]
    public void Constructor_Valid_CreatesCorrectDimensions()
    {
        var g = new SfGrid2D<int>(4, 3);
        Assert.Equal(4, g.Width);
        Assert.Equal(3, g.Height);
        Assert.Equal(12, g.Count);
    }

    [Fact]
    public void Constructor_InvalidDimensions_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid2D<int>(0, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid2D<int>(5, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfGrid2D<int>(-1, 2));
    }

    [Fact]
    public void Constructor_WithData_Valid()
    {
        int[] data = { 1, 2, 3, 4 };
        var grid = new SfGrid2D<int>(2, 2, data);
        Assert.Equal(4, grid.Count);
        Assert.Equal(4, grid[1, 1]);
    }

    [Fact]
    public void Constructor_WithDataWrongSize_Throws()
    {
        int[] data = { 1, 2 };
        Assert.Throws<ArgumentException>(() => new SfGrid2D<int>(2, 2, data));
    }

    [Fact]
    public void CopyConstructor_CopiesValues()
    {
        var a = new SfGrid2D<int>(2, 2);
        a[1, 1] = 10;

        var b = new SfGrid2D<int>(a);
        Assert.Equal(10, b[1, 1]);
    }

    [Fact]
    public void Indexer_SetGet_Works()
    {
        var g = new SfGrid2D<int>(3, 3);
        g[1, 2] = 99;

        Assert.Equal(99, g[1, 2]);
    }

    [Fact]
    public void Indexer_OutOfBounds_Throws()
    {
        var g = new SfGrid2D<int>(3, 3);
        Assert.Throws<ArgumentOutOfRangeException>(() => g[3, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => g[-1, 0]);
    }

    [Fact]
    public void ToIndex_ProducesCorrectIndex()
    {
        var g = new SfGrid2D<int>(4, 5);
        Assert.Equal(0, g.ToIndex(0, 0));
        Assert.Equal(3, g.ToIndex(3, 0));
        Assert.Equal(4, g.ToIndex(0, 1));
        Assert.Equal(19, g.ToIndex(3, 5 - 1));
    }

    [Fact]
    public void ToCoords_ReturnsCorrectCoordinates()
    {
        var g = new SfGrid2D<int>(4, 5);
        var (x, y) = g.ToCoords(7);
        Assert.Equal(3, x);
        Assert.Equal(1, y);
    }

    [Fact]
    public void TryGet_Works()
    {
        var g = new SfGrid2D<int>(3, 3);
        g[1, 1] = 50;

        Assert.True(g.TryGet(1, 1, out var v));
        Assert.Equal(50, v);

        Assert.False(g.TryGet(3, 3, out var _));
    }

    [Fact]
    public void Fill_FillsCorrectly()
    {
        var g = new SfGrid2D<int>(3, 3);
        g.Fill(7);
        for (int i = 0; i < 9; i++)
            Assert.Equal(7, g[i]);
    }

    [Fact]
    public void Clear_Works()
    {
        var g = new SfGrid2D<int>(3, 3);
        g.Fill(5);
        g.Clear();

        for (int i = 0; i < 9; i++)
            Assert.Equal(0, g[i]);
    }

    [Fact]
    public void Contains_Works()
    {
        var g = new SfGrid2D<int>(2, 2);
        g[1, 1] = 10;

        Assert.True(g.Contains(10));
        Assert.False(g.Contains(5));
    }

    [Fact]
    public void GetUnsafeRef_Works()
    {
        var g = new SfGrid2D<int>(2, 2);
        ref int r = ref g.GetUnsafeRef(1, 1);
        r = 42;

        Assert.Equal(42, g[1, 1]);
    }

    [Fact]
    public void CopyTo_Works()
    {
        var g = new SfGrid2D<int>(2, 2);
        g[0] = 1;
        g[1] = 2;
        g[2] = 3;
        g[3] = 4;

        int[] arr = new int[4];
        g.CopyTo(arr, 0);

        Assert.Equal(new[] { 1, 2, 3, 4 }, arr);
    }

    [Fact]
    public void Enumerator_Works()
    {
        var g = new SfGrid2D<int>(2, 2);
        g.Fill(3);

        int count = 0;
        foreach (var v in g)
        {
            Assert.Equal(3, v);
            count++;
        }

        Assert.Equal(4, count);
    }
}