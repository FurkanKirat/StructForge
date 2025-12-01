using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfStackQueueBenchmarks
{
    [Params(10_000)]
    public int N;

    private Stack<int> _sysStack;
    private SfStack<int> _sfStack;
    
    private Queue<int> _sysQueue;
    private SfQueue<int> _sfQueue;

    [GlobalSetup]
    public void Setup()
    {
        _sysStack = new Stack<int>(N);
        _sfStack = new SfStack<int>(N);
        
        _sysQueue = new Queue<int>(N);
        _sfQueue = new SfQueue<int>(N);
    }

    
    [Benchmark(Baseline = true)]
    public int SystemStack_PushPop()
    {
        int last = 0;
        for(int i=0; i<N; i++) _sysStack.Push(i);
        for(int i=0; i<N; i++) last = _sysStack.Pop();
        return last;
    }

    [Benchmark]
    public int SfStack_PushPop()
    {
        int last = 0;
        for(int i=0; i<N; i++) _sfStack.Push(i);
        for(int i=0; i<N; i++) last = _sfStack.Pop();
        return last;
    }

    
    [Benchmark]
    public int SystemQueue_EnqDeq()
    {
        int last = 0;
        for(int i=0; i<N; i++) _sysQueue.Enqueue(i);
        for(int i=0; i<N; i++) last = _sysQueue.Dequeue();
        return last;
    }

    [Benchmark]
    public int SfQueue_EnqDeq()
    {
        int last = 0;
        for(int i=0; i<N; i++) _sfQueue.Enqueue(i);
        for(int i=0; i<N; i++) last = _sfQueue.Dequeue();
        return last;
    }
}