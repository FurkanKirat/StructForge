using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfAvlTreeBenchmarks
{
    [Params(10_000)]
    public int N;
    
    private SortedSet<int> _sysSet;
    private SfAvlTree<int> _sfTree;
    private int[] _randomData;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _randomData = Enumerable.Range(0, N).Select(_ => rnd.Next()).ToArray();
        
        _sysSet = new SortedSet<int>();
        _sfTree = new SfAvlTree<int>();
        
        foreach(var i in _randomData)
        {
            _sysSet.Add(i);
            _sfTree.TryAdd(i);
        }
    }

    [Benchmark]
    public void System_Add()
    {
        var set = new SortedSet<int>();
        foreach (var item in _randomData) set.Add(item);
    }

    [Benchmark]
    public void Sf_Add()
    {
        var tree = new SfAvlTree<int>();
        foreach (var item in _randomData) tree.Add(item);
    }

    [Benchmark(Baseline = true)]
    public int System_Contains()
    {
        int found = 0;
        foreach (var item in _randomData) 
            if (_sysSet.Contains(item)) found++;
        return found;
    }

    [Benchmark]
    public int Sf_Contains()
    {
        int found = 0;
        foreach (var item in _randomData) 
            if (_sfTree.Contains(item)) found++;
        return found;
    }
}