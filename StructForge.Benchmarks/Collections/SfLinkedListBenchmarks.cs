using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfLinkedListBenchmarks
{
    [Params(10_000)]
    public int N;

    private LinkedList<int> _sysList;
    private SfLinkedList<int> _sfList;

    [GlobalSetup]
    public void Setup()
    {
        _sysList = new LinkedList<int>();
        _sfList = new SfLinkedList<int>();
        
        for(int i=0; i<N; i++)
        {
            _sysList.AddLast(i);
            _sfList.AddLast(i);
        }
    }

    [Benchmark(Baseline = true)]
    public long System_Foreach()
    {
        long sum = 0;
        foreach (var item in _sysList) sum += item;
        return sum;
    }

    [Benchmark]
    public long Sf_Foreach()
    {
        long sum = 0;
        foreach (var item in _sfList) sum += item;
        return sum;
    }
}