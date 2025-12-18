using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfEnumSetBenchmarks
{
    public enum SfTestEnum
    {
        Value0 = 0, Value1, Value2, Value3, Value4, 
        Value99 = 99
    }

    private SfTestEnum[] _values;
    private HashSet<SfTestEnum> _sysSet;
    private SfEnumSet<SfTestEnum> _sfSet;

    [GlobalSetup]
    public void Setup()
    {
        _values = (SfTestEnum[])Enum.GetValues(typeof(SfTestEnum));
        _sysSet = new HashSet<SfTestEnum>();
        _sfSet = new SfEnumSet<SfTestEnum>();
    }

    [Benchmark(Baseline = true)]
    public void System_Add()
    {
        _sysSet.Clear();
        foreach (var val in _values)
            _sysSet.Add(val);
    }

    [Benchmark]
    public void Sf_Add()
    {
        _sfSet.Clear();
        foreach (var val in _values)
            _sfSet.Add(val);
    }

  
    [Benchmark]
    public int System_Contains()
    {
        int count = 0;
        foreach (var val in _values)
            if (_sysSet.Contains(val)) count++;
        return count;
    }

    [Benchmark]
    public int Sf_Contains()
    {
        int count = 0;
        foreach (var val in _values)
            if (_sfSet.Contains(val)) count++;
        return count;
    }
    
    
    [Benchmark]
    public void System_Union()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        setA.UnionWith(setB);
    }

    [Benchmark]
    public void Sf_Union()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        setA.UnionWith(setB);
    }
    
    // --- TEST 4: REMOVE---
    [Benchmark]
    public void System_Remove()
    {
        var set = new HashSet<SfTestEnum>(_values);
        foreach (var val in _values)
            set.Remove(val);
    }

    [Benchmark]
    public void Sf_Remove()
    {
        var set = new SfEnumSet<SfTestEnum>(_values);
        foreach (var val in _values)
            set.Remove(val);
    }

    // --- TEST 5: INTERSECT ---
    [Benchmark]
    public void System_Intersect()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        setA.IntersectWith(setB);
    }

    [Benchmark]
    public void Sf_Intersect()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        setA.IntersectWith(setB);
    }

    // --- TEST 6: EXCEPT ---
    [Benchmark]
    public void System_Except()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        setA.ExceptWith(setB);
    }

    [Benchmark]
    public void Sf_Except()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        setA.ExceptWith(setB);
    }

    // --- TEST 7: SYMMETRIC EXCEPT (XOR) ---
    [Benchmark]
    public void System_SymmetricExcept()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        setA.SymmetricExceptWith(setB);
    }

    [Benchmark]
    public void Sf_SymmetricExcept()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        setA.SymmetricExceptWith(setB); // Bitwise XOR
    }

    // --- TEST 8: SUBSET / SUPERSET ---
    [Benchmark]
    public bool System_IsSubset()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        return setA.IsSubsetOf(setB);
    }

    [Benchmark]
    public bool Sf_IsSubset()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        return setA.IsSubsetOf(setB);
    }

    // --- TEST 9: OVERLAPS ---
    [Benchmark]
    public bool System_Overlaps()
    {
        var setA = new HashSet<SfTestEnum>(_values);
        var setB = new HashSet<SfTestEnum>(_values);
        return setA.Overlaps(setB);
    }

    [Benchmark]
    public bool Sf_Overlaps()
    {
        var setA = new SfEnumSet<SfTestEnum>(_values);
        var setB = new SfEnumSet<SfTestEnum>(_values);
        return setA.Overlaps(setB);
    }
}