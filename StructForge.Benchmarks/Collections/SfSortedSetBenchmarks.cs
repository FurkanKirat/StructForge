using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
public class SfSortedSetBenchmarks
{
    [Params(10_000, 100_000)] 
    public int N;

    private SortedSet<int> _sysSet;
    private SfSortedSet<int> _sfSet;
    
    private int[] _data;
    private int[] _sortedData;
    private int[] _missData; 

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        
        _data = new int[N];
        _missData = new int[N];
        
        _sysSet = new SortedSet<int>();
        _sfSet = new SfSortedSet<int>();

        for (int i = 0; i < N; i++)
        {
            _data[i] = rnd.Next(); 
        }
        
        for (int i = 0; i < N; i++)
        {
            _missData[i] = rnd.Next(); 
        }

        foreach (var item in _data)
        {
            _sysSet.Add(item);
            _sfSet.TryAdd(item);
        }
    }
    
    
    [Benchmark(Baseline = true)]
    public void System_Add_Random()
    {
        _sysSet.Clear();
        foreach (var item in _data)
        {
            _sysSet.Add(item);
        }
    }

    [Benchmark]
    public void Sf_Add_Random()
    {
        _sfSet.Clear();
        foreach (var item in _data)
        {
            _sfSet.TryAdd(item);
        }
    }
    
    [Benchmark]
    public int System_Contains()
    {
        int found = 0;
        foreach (var item in _data)
        {
            if (_sysSet.Contains(item)) found++;
        }
        return found;
    }

    [Benchmark]
    public int Sf_Contains()
    {
        int found = 0;
        foreach (var item in _data)
        {
            if (_sfSet.Contains(item)) found++;
        }
        return found;
    }
    
    [Benchmark]
    public int System_Min()
    {
        return _sysSet.Min;
    }

    [Benchmark]
    public int Sf_Min()
    {
        return _sfSet.Min; 
    }
    
    
    [Benchmark]
    public int System_Foreach()
    {
        int sum = 0;
        foreach (var item in _sysSet) sum += item;
        return sum;
    }

    [Benchmark]
    public int Sf_Foreach()
    {
        int sum = 0;
        foreach (var item in _sfSet) sum += item;
        return sum;
    }
}