using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfBitArray2DBenchmarks
{
    [Params(1024)]
    public int Size;

    private bool[,] _nativeBool;
    private SfBitArray2D _sfBit2D;

    [GlobalSetup]
    public void Setup()
    {
        _nativeBool = new bool[Size, Size];
        _sfBit2D = new SfBitArray2D(Size, Size);
        
        var rnd = new Random(42);
        for(int y=0; y<Size; y++)
        for(int x=0; x<Size; x++)
        {
            bool val = rnd.Next(0, 2) == 1;
            _nativeBool[y, x] = val;
            _sfBit2D[x, y] = val;
        }
    }

    
    [Benchmark(Baseline = true)]
    public int Native_Iterate()
    {
        int count = 0;
        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            if (_nativeBool[y, x]) count++;
        }
        return count;
    }

    [Benchmark]
    public int SfBit2D_Iterate()
    {
        int count = 0;
        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            if (_sfBit2D.GetUnchecked(x, y)) count++;
        }
        return count;
    }
    
    [Benchmark]
    public int Native_CountTrue()
    {
        int count = 0;
        int s = Size;
        for (int y = 0; y < s; y++)
        for (int x = 0; x < s; x++)
        {
            if (_nativeBool[x, y]) count++;
        }
        return count;
    }
    
    
    [Benchmark]
    public int SfBit3D_PopCount()
    {
        return _sfBit2D.CountTrue(); 
    }
}