using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
public class SfHashSetBenchmarks
{
    [Params(10_000, 100_000)] 
    public int Size;

    private HashSet<int> _sysSet;
    private SfHashSet<int> _sfSet;
    
    private int[] _data;
    private int[] _missData;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        
        _data = new int[Size];
        _missData = new int[Size];
        
        _sysSet = new HashSet<int>();
        _sfSet = new SfHashSet<int>();

        for (int i = 0; i < Size; i++)
        {
            _data[i] = rnd.Next(); 
        }

        for (int i = 0; i < Size; i++)
        {
            int val = rnd.Next();
            while (_data.Contains(val)) val = rnd.Next();
            _missData[i] = val;
        }

        foreach (var item in _data)
        {
            _sysSet.Add(item);
            _sfSet.TryAdd(item);
        }
    }
    
    
    [Benchmark(Baseline = true)]
    public void System_Add()
    {
        _sysSet.Clear(); 
        foreach (var item in _data)
        {
            _sysSet.Add(item);
        }
    }

    [Benchmark]
    public void Sf_Add()
    {
        _sfSet.Clear();
        foreach (var item in _data)
        {
            _sfSet.TryAdd(item);
        }
    }
    

    [Benchmark]
    public int System_Contains_Hit()
    {
        int found = 0;
        foreach (var item in _data)
        {
            if (_sysSet.Contains(item)) found++;
        }
        return found;
    }

    [Benchmark]
    public int Sf_Contains_Hit()
    {
        int found = 0;
        foreach (var item in _data)
        {
            if (_sfSet.Contains(item)) found++;
        }
        return found;
    }

    [Benchmark]
    public int System_Contains_Miss()
    {
        int found = 0;
        foreach (var item in _missData)
        {
            if (_sysSet.Contains(item)) found++;
        }
        return found;
    }

    [Benchmark]
    public int Sf_Contains_Miss()
    {
        int found = 0;
        foreach (var item in _missData)
        {
            if (_sfSet.Contains(item)) found++;
        }
        return found;
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