using BenchmarkDotNet.Attributes;
using StructForge.Collections; 

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfRingBufferBenchmarks
{
    [Params(100, 10_000)]
    public int Capacity;

    private Queue<int> _systemQueue;
    private SfRingBuffer<int> _sfRingBuffer;

    [GlobalSetup]
    public void Setup()
    {
        _systemQueue = new Queue<int>(Capacity);
        _sfRingBuffer = new SfRingBuffer<int>(Capacity);
    }
    
    [Benchmark(Baseline = true)]
    public void SystemQueue_Churn()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _systemQueue.Enqueue(i);
        }

        for (int i = 0; i < Capacity; i++)
        {
            _systemQueue.Dequeue();
        }
    }

    [Benchmark]
    public void SfRingBuffer_Churn()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _sfRingBuffer.Enqueue(i);
        }

        for (int i = 0; i < Capacity; i++)
        {
            _sfRingBuffer.Dequeue();
        }
    }
    
    [Benchmark]
    public void SystemQueue_Overflow()
    {
        for (int i = 0; i < Capacity * 2; i++)
        {
            _systemQueue.Enqueue(i); 
        }
    }

    [Benchmark]
    public void SfRingBuffer_Overflow()
    {
        for (int i = 0; i < Capacity * 2; i++)
        {
            _sfRingBuffer.Enqueue(i);
        }
    }
}