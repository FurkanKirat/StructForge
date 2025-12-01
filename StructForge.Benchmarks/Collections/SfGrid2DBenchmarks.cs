using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
public class SfGrid2DAdvancedBenchmarks
{
    [Params(32, 1000, 2000, 4096)]
    public int Size;

    private int[,] _nativeArray;
    private SfGrid2D<int> _sfGrid;
    
    private int[] _randomXs;
    private int[] _randomYs;

    [GlobalSetup]
    public void Setup()
    {
        _nativeArray = new int[Size, Size];
        _sfGrid = new SfGrid2D<int>(Size, Size);
        
        var rnd = new Random(42);
        _randomXs = new int[Size];
        _randomYs = new int[Size];
        
        for (int i = 0; i < _randomXs.Length; i++)
        {
            _randomXs[i] = rnd.Next(0, Size);
            _randomYs[i] = rnd.Next(0, Size);
        }
    }
    
    
    [Benchmark]
    public long Native_RandomRead()
    {
        long sum = 0;
        for (int i = 0; i < _randomXs.Length; i++)
        {
            sum += _nativeArray[_randomYs[i], _randomXs[i]];
        }
        return sum;
    }

    [Benchmark]
    public long SfGrid_RandomRead()
    {
        long sum = 0;
        for (int i = 0; i < _randomXs.Length; i++)
        {
            sum += _sfGrid.GetUnsafeRef(_randomXs[i], _randomYs[i]); 
        }
        return sum;
    }
    
    
    [Benchmark]
    public long Native_ColumnMajor()
    {
        long sum = 0;
        int h = _nativeArray.GetLength(0);
        int w = _nativeArray.GetLength(1);
        
        for (int x = 0; x < w; x++) 
        {
            for (int y = 0; y < h; y++)
            {
                sum += _nativeArray[y, x];
            }
        }
        return sum;
    }

    [Benchmark]
    public long SfGrid_ColumnMajor()
    {
        long sum = 0;
        int h = _sfGrid.Height;
        int w = _sfGrid.Width;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                sum += _sfGrid.GetUnsafeRef(x, y);
            }
        }
        return sum;
    }
    
    [Benchmark]
    public int[,] Native_Clone()
    {
        return (int[,])_nativeArray.Clone(); 
    }

    [Benchmark]
    public int[] SfGrid_SpanCopy()
    {
        var destination = new int[Size * Size];
        _sfGrid.AsSpan().CopyTo(destination);
        return destination;
    }
}