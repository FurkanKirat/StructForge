using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
[MemoryRandomization]
public class SfGridIterationBenchmarks
{
    [Params(32, 1000, 2000, 4096)]
    public int Size;

    private SfGrid2D<int> _sfGrid;
    
    [IterationSetup]
    public void IterationSetup()
    {
        _sfGrid = new SfGrid2D<int>(Size, Size);
        
        var rnd = new Random(42);
   
        for (int i = 0; i < Size * Size; i++)
            _sfGrid[i] = rnd.Next();
    }
    
    [Benchmark]
    public int Nested2DIndexing() {
        int sum = 0;
        for (int y = 0; y < Size; y++) {
            for (int x = 0; x < Size; x++) {
                sum += _sfGrid.GetUnsafeRef(y * Size + x);
            }
        }
        return sum;
    }

    [Benchmark(Baseline = true)]
    public int NestedWithIndexIncrement() {
        int sum = 0;
        int idx = 0;
        for (int y = 0; y < Size; y++) {
            for (int x = 0; x < Size; x++) {
                sum += _sfGrid.GetUnsafeRef(idx++);
            }
        }
        return sum;
    }
    
    [Benchmark]
    public int Using2DAccessor() {
        int sum = 0;
        for (int y = 0; y < Size; y++) {
            for (int x = 0; x < Size; x++) {
                sum += _sfGrid.GetUnsafeRef(x, y);
            }
        }
        return sum;
    }

    [Benchmark]
    public int ForeachSpan() {
        int sum = 0;
        foreach (var item in _sfGrid.AsSpan()) {
            sum += item;
        }
        return sum;
    }
    
    [Benchmark]
    public int ForeachAsReadonlySpan() {
        int sum = 0;
        foreach (var item in _sfGrid.AsReadOnlySpan()) {
            sum += item;
        }
        return sum;
    }
    
    [Benchmark]
    public int ForeachEnumerable() {
        int sum = 0;
        foreach (var item in _sfGrid) {
            sum += item;
        }
        return sum;
    }
    
}