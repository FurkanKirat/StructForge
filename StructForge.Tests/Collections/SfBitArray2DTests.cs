using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfBitArray2DTests
{
    [Fact]
    public void Constructor_ValidDimensions_CreatesCorrectSize()
    {
        var arr = new SfBitArray2D(4, 3);
        Assert.Equal(4, arr.Width);
        Assert.Equal(3, arr.Height);
        Assert.Equal(12, arr.Count);
    }

    [Fact]
    public void Constructor_InvalidDimensions_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfBitArray2D(0, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfBitArray2D(-1, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SfBitArray2D(5, 0));
    }

    [Fact]
    public void Constructor_WithBits_ValidArray_Works()
    {
        int w = 4, h = 4;
        int totalBits = w * h;
        int neededUlongs = (totalBits + 63) / 64;

        ulong[] bits = new ulong[neededUlongs];
        bits[0] = 0b1011;

        var arr = new SfBitArray2D(w, h, bits);

        Assert.True(arr[0, 0]);  // LSB
        Assert.True(arr[1, 0]);
        Assert.False(arr[2, 0]);
        Assert.True(arr[3, 0]);
    }

    [Fact]
    public void Constructor_WithBits_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => new SfBitArray2D(8, 8, new ulong[0]));
    }

    [Fact]
    public void Index_ComputesCorrectly()
    {
        var arr = new SfBitArray2D(10, 10);

        Assert.Equal(0, arr.IndexSafe(0, 0));
        Assert.Equal(5, arr.IndexSafe(5, 0));
        Assert.Equal(10, arr.IndexSafe(0, 1));
        Assert.Equal(55, arr.IndexSafe(5, 5));
    }

    [Fact]
    public void Index_OutOfRange_Throws()
    {
        var arr = new SfBitArray2D(5, 5);
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.IndexSafe(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.IndexSafe(5, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.IndexSafe(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => arr.IndexSafe(0, 5));
    }

    [Fact]
    public void SetAndGet_WorksCorrectly()
    {
        var arr = new SfBitArray2D(3, 3);
        arr[1, 1] = true;
        Assert.True(arr[1, 1]);
    }

    [Fact]
    public void Toggle_Works()
    {
        var arr = new SfBitArray2D(3, 3);
        Assert.False(arr[1, 1]);

        arr.Toggle(1, 1);
        Assert.True(arr[1, 1]);

        arr.Toggle(1, 1);
        Assert.False(arr[1, 1]);
    }

    [Fact]
    public void SetAll_Works()
    {
        var arr = new SfBitArray2D(4, 4);
        arr.SetAll(true);

        for (int y = 0; y < 4; y++)
        for (int x = 0; x < 4; x++)
            Assert.True(arr[x, y]);
    }

    [Fact]
    public void Clear_Works()
    {
        var arr = new SfBitArray2D(4, 4);
        arr.SetAll(true);
        arr.Clear();

        for (int y = 0; y < 4; y++)
        for (int x = 0; x < 4; x++)
            Assert.False(arr[x, y]);
    }

    [Fact]
    public void And_Or_Xor_Works()
    {
        var a = new SfBitArray2D(2, 2);
        var b = new SfBitArray2D(2, 2);

        a[0, 0] = true;
        b[0, 0] = true;
        b[1, 1] = true;

        a.And(b);
        Assert.True(a[0, 0]);
        Assert.False(a[1, 1]);

        a = new SfBitArray2D(b);
        a.Or(b);
        Assert.True(a[0, 0]);
        Assert.True(a[1, 1]);

        a = new SfBitArray2D(b);
        a.Xor(b);
        Assert.False(a[0, 0]);
        Assert.False(a[1, 1]);
    }

    [Fact]
    public void CopyConstructor_Works()
    {
        var a = new SfBitArray2D(3, 3);
        a[1, 1] = true;

        var b = new SfBitArray2D(a);
        Assert.True(b[1, 1]);
    }

    [Fact]
    public void ToFromULongArray_RoundTrip_Works()
    {
        var a = new SfBitArray2D(5, 5);
        a[2, 3] = true;

        var span = a.AsReadOnlySpan();
        var b = new SfBitArray2D(5, 5, span.ToArray());

        Assert.True(b[2, 3]);
    }
}