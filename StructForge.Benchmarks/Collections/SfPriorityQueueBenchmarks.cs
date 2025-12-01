using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfPriorityQueueBenchmarks
{
    [Params(10_000)]
    public int N;

    private int[] _priorities;
    
    private PriorityQueue<int, int> _sysPQ;
    private SfPriorityQueue<int, int> _sfPQ;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        _priorities = new int[N];
        for (int i = 0; i < N; i++) _priorities[i] = rnd.Next();

        _sysPQ = new PriorityQueue<int, int>(N);
        _sfPQ = new SfPriorityQueue<int, int>(N);
    }

    [Benchmark(Baseline = true)]
    public void SystemPQ_Sort()
    {
        for (int i = 0; i < N; i++)
            _sysPQ.Enqueue(i, _priorities[i]);
            
        while (_sysPQ.Count > 0)
            _sysPQ.Dequeue();
    }

    [Benchmark]
    public void SfPQ_Sort()
    {
        for (int i = 0; i < N; i++)
            _sfPQ.Enqueue(i, _priorities[i]);

        while (!_sfPQ.IsEmpty)
            _sfPQ.Dequeue();
    }
}