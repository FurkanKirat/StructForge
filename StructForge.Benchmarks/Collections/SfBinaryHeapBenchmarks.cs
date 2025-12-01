using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfBinaryHeapBenchmarks
{
    [Params(10_000, 100_000)]
    public int N;

    private int[] _data;
    
    private PriorityQueue<int, int> _sysPQ; 
    private SfBinaryHeap<int> _sfHeap;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _data = new int[N];
        for(int i=0; i<N; i++) _data[i] = rnd.Next();

        _sysPQ = new PriorityQueue<int, int>(N);
        _sfHeap = new SfBinaryHeap<int>(N);
    }

    [Benchmark(Baseline = true)]
    public void SystemPQ_PushPop()
    {
        _sysPQ.Clear();
        foreach (var item in _data)
        {
            _sysPQ.Enqueue(item, item);
        }
        while (_sysPQ.Count > 0)
        {
            _sysPQ.Dequeue();
        }
    }

    [Benchmark]
    public void SfHeap_PushPop()
    {
        _sfHeap.Clear();
        foreach (var item in _data)
        {
            _sfHeap.Add(item);
        }
        while (_sfHeap.Count > 0)
        {
            _sfHeap.Pop();
        }
    }
}