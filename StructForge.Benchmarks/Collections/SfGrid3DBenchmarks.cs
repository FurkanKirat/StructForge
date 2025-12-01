using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
public class SfGrid3DBenchmarks
{
    // 32^3 = 32,768 eleman (L1 Cache sığar)
    // 128^3 = 2,097,152 eleman (RAM'e taşar - Cache Miss başlar)
    [Params(32, 128)] 
    public int Size;

    private int[,,] _native3D;
    private SfGrid3D<int> _sfGrid3D;

    [GlobalSetup]
    public void Setup()
    {
        _native3D = new int[Size, Size, Size];
        _sfGrid3D = new SfGrid3D<int>(Size, Size, Size);
    }
    
    
    [Benchmark(Baseline = true)]
    public long Native3D_Iterate_XYZ()
    {
        long sum = 0;
        int s = Size;
        for (int z = 0; z < s; z++)
        for (int y = 0; y < s; y++)
        for (int x = 0; x < s; x++)
        {
            sum += _native3D[x, y, z];
        }
        return sum;
    }

    [Benchmark]
    public long SfGrid3D_Iterate_XYZ()
    {
        long sum = 0;
        int s = Size;
        for (int z = 0; z < s; z++)
        for (int y = 0; y < s; y++)
        for (int x = 0; x < s; x++)
        {
            sum += _sfGrid3D.GetUnsafeRef(x, y, z); 
        }
        return sum;
    }
    
    
    [Benchmark]
    public long Native3D_Iterate_ZYX()
    {
        long sum = 0;
        int s = Size;
        for (int x = 0; x < s; x++) 
        for (int y = 0; y < s; y++)
        for (int z = 0; z < s; z++)
        {
            sum += _native3D[x, y, z];
        }
        return sum;
    }

    [Benchmark]
    public long SfGrid3D_Iterate_ZYX()
    {
        long sum = 0;
        int s = Size;
        for (int x = 0; x < s; x++)
        for (int y = 0; y < s; y++)
        for (int z = 0; z < s; z++)
        {
            sum += _sfGrid3D.GetUnsafeRef(x, y, z);
        }
        return sum;
    }
    
    
    [Benchmark]
    public long SfGrid3D_AsSpan()
    {
        long sum = 0;
        foreach (var item in _sfGrid3D.AsSpan())
        {
            sum += item;
        }
        return sum;
    }
}