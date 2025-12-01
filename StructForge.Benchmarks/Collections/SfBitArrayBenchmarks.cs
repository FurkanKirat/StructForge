using System.Collections;
using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfBitArrayOperationsBenchmarks
{
    [Params(100_000, 1_000_000)]
    public int Size;

    private bool[] _boolA, _boolB;
    private BitArray _sysA, _sysB;
    private SfBitArray _sfA, _sfB;

    [GlobalSetup]
    public void Setup()
    {
        _boolA = new bool[Size]; _boolB = new bool[Size];
        _sysA = new BitArray(Size); _sysB = new BitArray(Size);
        _sfA = new SfBitArray(Size); _sfB = new SfBitArray(Size);
        
        for (int i = 0; i < Size; i++)
        {
            bool valA = i % 2 == 0;
            bool valB = i % 3 == 0;

            _boolA[i] = valA; _boolB[i] = valB;
            _sysA[i] = valA; _sysB[i] = valB;
            
            _sfA.SetUnchecked(i, valA); 
            _sfB.SetUnchecked(i, valB);
        }
    }

    // --- TEST 1: BITWISE AND  ---
    [Benchmark(Baseline = true)]
    public void BoolArray_And()
    {
        for (int i = 0; i < Size; i++)
        {
            _boolA[i] &= _boolB[i];
        }
    }

    [Benchmark]
    public void SystemBitArray_And()
    {
        _sysA.And(_sysB);
    }

    [Benchmark]
    public void SfBitArray_And()
    {
        _sfA.And(_sfB); 
    }

    // --- TEST 2: BULK SET (FILL) ---
    [Benchmark]
    public void BoolArray_Fill()
    {
        Array.Fill(_boolA, true);
    }

    [Benchmark]
    public void SystemBitArray_SetAll()
    {
        _sysA.SetAll(true);
    }

    [Benchmark]
    public void SfBitArray_Fill()
    {
        _sfA.SetAll(true); 
    }
    
    // --- TEST 3: RANDOM ACCESS (Get) ---

    [Benchmark]
    public int BoolArray_RandomAccess()
    {
        int count = 0;
        for (int i = 0; i < Size; i += 3) 
        {
            if (_boolA[i]) count++;
        }
        return count;
    }

    [Benchmark]
    public int SfBitArray_RandomAccess()
    {
        int count = 0;
        for (int i = 0; i < Size; i += 3)
        {
            if (_sfA.GetUnchecked(i)) count++;
        }
        return count;
    }
}