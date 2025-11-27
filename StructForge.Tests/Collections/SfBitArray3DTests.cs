using StructForge.Collections;

namespace StructForge.Tests.Collections;

public class SfBitArray3DTests
{
    [Fact]
    public void Constructor_ValidDimensions_CreatesCorrectSize()
    {
        var arr = new SfBitArray3D(4, 3, 2);
        Assert.Equal(4, arr.Width);
        Assert.Equal(3, arr.Height);
        Assert.Equal(2, arr.Depth);
        Assert.Equal(4 * 3 * 2, arr.Count);
    }

    [Fact]
    public void Constructor_WithBits_Works()
    {
        int w = 4, h = 4, d = 2;
        int total = w * h * d;
        int ulongs = (total + 63) / 64;

        ulong[] bits = new ulong[ulongs];
        bits[0] = 0b1011;

        var arr = new SfBitArray3D(w, h, d, bits);

        Assert.True(arr[0, 0, 0]);
        Assert.True(arr[1, 0, 0]);
        Assert.False(arr[2, 0, 0]);
        Assert.True(arr[3, 0, 0]);
    }

    [Fact]
    public void Index3D_Works()
    {
        var arr = new SfBitArray3D(10, 10, 10);

        Assert.Equal(0, arr.Index(0, 0, 0));
        Assert.Equal(5, arr.Index(5, 0, 0));
        Assert.Equal(10, arr.Index(0, 1, 0));
        Assert.Equal(100, arr.Index(0, 0, 1)); // next layer
        Assert.Equal(555, arr.Index(5, 5, 5));
    }

    [Fact]
    public void SetAndGet_Works()
    {
        var arr = new SfBitArray3D(4, 4, 4);
        arr[2, 2, 2] = true;

        Assert.True(arr[2, 2, 2]);
    }

    [Fact]
    public void Toggle_Works()
    {
        var arr = new SfBitArray3D(3, 3, 3);

        arr.Toggle(1, 1, 1);
        Assert.True(arr[1, 1, 1]);

        arr.Toggle(1, 1, 1);
        Assert.False(arr[1, 1, 1]);
    }

    [Fact]
    public void And_Or_Xor_Works()
    {
        var a = new SfBitArray3D(2, 2, 2);
        var b = new SfBitArray3D(2, 2, 2);

        a[0, 0, 0] = true;
        b[0, 0, 0] = true;
        b[1, 1, 1] = true;

        a.And(b);
        Assert.True(a[0, 0, 0]);
        Assert.False(a[1, 1, 1]);

        a = new SfBitArray3D(b);
        a.Or(b);
        Assert.True(a[0, 0, 0]);
        Assert.True(a[1, 1, 1]);

        a = new SfBitArray3D(b);
        a.Xor(b);
        Assert.False(a[0, 0, 0]);
        Assert.False(a[1, 1, 1]);
    }

    [Fact]
    public void CopyConstructor_Works()
    {
        var a = new SfBitArray3D(3, 3, 3);
        a[2, 1, 0] = true;

        var b = new SfBitArray3D(a);

        Assert.True(b[2, 1, 0]);
    }

    [Fact]
    public void ToULongArray_RoundTrip_Works()
    {
        var a = new SfBitArray3D(5, 5, 5);
        a[2, 3, 1] = true;

        ulong[] ulongs = a.ToULongArray();
        var b = new SfBitArray3D(5, 5, 5, ulongs);

        Assert.True(b[2, 3, 1]);
    }
}