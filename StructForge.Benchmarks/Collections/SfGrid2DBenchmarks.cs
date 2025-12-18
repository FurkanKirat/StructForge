using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
[MemoryRandomization]
public class SfGrid2DBenchmarks
{
    [Params(32, 1000, 2000, 4096)]
    public int Size;

    private int[,] _nativeArray;
    private SfGrid2D<int> _sfGrid;
    
    private int[] _randomXs;
    private int[] _randomYs;

    [IterationSetup]
    public void IterationSetup()
    {
        _nativeArray = new int[Size, Size];
        _sfGrid = new SfGrid2D<int>(Size, Size);

        var rnd = new Random();
        _randomXs = new int[Size];
        _randomYs = new int[Size];

        for (int i = 0; i < _randomXs.Length; i++)
        {
            _randomXs[i] = rnd.Next(0, Size);
            _randomYs[i] = rnd.Next(0, Size);
        }
        
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                int val = rnd.Next();
                _nativeArray[y, x] = val;
                _sfGrid[x, y] = val;
            }
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
    public long Native_RowMajor()
    {
        long sum = 0;
        int h = _nativeArray.GetLength(0);
        int w = _nativeArray.GetLength(1);
    
        for (int y = 0; y < h; y++) 
        {
            for (int x = 0; x < w; x++)
            {
                sum += _nativeArray[y, x];
            }
        }
        return sum;
    }

    [Benchmark]
    public long SfGrid_RowMajor()
    {
        long sum = 0;
        int h = _sfGrid.Height;
        int w = _sfGrid.Width;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
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
    public int[] SfGrid_ToArray()
    {
        return _sfGrid.ToArray();
    }
    
    [Benchmark]
    public long SfGrid_ForeachSpan()
    {
        long sum = 0;
        foreach (var item in _sfGrid.AsSpan())
        {
            sum += item;
        }
        return sum;
    }
    
    [Benchmark]
    public long Native_Foreach()
    {
        long sum = 0;
        foreach (var item in _nativeArray)
        {
            sum += item;
        }
        return sum;
    }
}