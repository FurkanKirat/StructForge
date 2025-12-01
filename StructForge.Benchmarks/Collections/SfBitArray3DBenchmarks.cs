using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
public class SfBitArray3DBenchmarks
{

    [Params(128)] 
    public int Size;

    private bool[,,] _nativeBool;
    private SfBitArray3D _sfBit3D;

    [GlobalSetup]
    public void Setup()
    {
        _nativeBool = new bool[Size, Size, Size];
        _sfBit3D = new SfBitArray3D(Size, Size, Size);
        
        var rnd = new Random(42);
        for (int z = 0; z < Size; z++)
        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            bool val = rnd.Next(0, 2) == 1;
            
            _nativeBool[x, y, z] = val;
            if (val) _sfBit3D.SetUnchecked(x, y, z, true);
        }
    }
    
    [Benchmark(Baseline = true)]
    public int Native_CountTrue()
    {
        int count = 0;
        int s = Size;
        for (int z = 0; z < s; z++)
        for (int y = 0; y < s; y++)
        for (int x = 0; x < s; x++)
        {
            if (_nativeBool[x, y, z]) count++;
        }
        return count;
    }
    
    [Benchmark]
    public int SfBit3D_Iterate()
    {
        int count = 0;
        int s = Size;
        for (int z = 0; z < s; z++)
        for (int y = 0; y < s; y++)
        for (int x = 0; x < s; x++)
        {
            if (_sfBit3D.GetUnchecked(x, y, z)) count++;
        }
        return count;
    }
    
    
    [Benchmark]
    public int SfBit3D_PopCount()
    {
        return _sfBit3D.CountTrue(); 
    }
}