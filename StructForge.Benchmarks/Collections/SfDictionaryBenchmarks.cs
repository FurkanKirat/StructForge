using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
[RankColumn]
[MemoryRandomization]
public class SfDictionaryBenchmarks
{
    [Params(10_000, 100_000)] 
    public int Count;
    
    // Test Class (class so object)
    public class UserID : IEquatable<UserID>
    {
        public int Id { get; }
        public string Region { get; }

        public UserID(int id, string region)
        {
            Id = id;
            Region = region;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Region);
        public bool Equals(UserID other) => other != null && Id == other.Id && Region == other.Region;
        public override bool Equals(object obj) => Equals(obj as UserID);
    }
    
    private UserID[] _allKeys;
    private UserID[] _lookupKeys;
    
    private Dictionary<UserID, int> _sysDict;
    private SfDictionary<UserID, int> _sfDict;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        
        _allKeys = Enumerable.Range(0, Count)
                             .Select(i => new UserID(i, "TR-" + i))
                             .ToArray();

        var existing = _allKeys.Take(Count / 2);
        var missing = Enumerable.Range(Count, Count / 2)
                                .Select(i => new UserID(i, "TR-" + i));
                                
        _lookupKeys = existing.Concat(missing).OrderBy(_ => random.Next()).ToArray();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _sysDict = new Dictionary<UserID, int>(Count);
        _sfDict = new SfDictionary<UserID, int>(Count);

        foreach (var key in _allKeys)
        {
            _sysDict.Add(key, 1);
            _sfDict.TryAdd(key, 1);
        }
    }

    // --- CONTAINS KEY ---
    
    [Benchmark]
    public bool System_ContainsKey()
    {
        bool found = false;
        foreach (var key in _lookupKeys)
            found ^= _sysDict.ContainsKey(key);
        return found;
    }

    [Benchmark]
    public bool StructForge_ContainsKey()
    {
        bool found = false;
        foreach (var key in _lookupKeys)
            found ^= _sfDict.ContainsKey(key);
        return found;
    }

    // --- TRY GET VALUE ---

    [Benchmark]
    public int System_TryGetValue()
    {
        int total = 0;
        foreach (var key in _lookupKeys)
        {
            if (_sysDict.TryGetValue(key, out int val))
                total += val;
        }
        return total;
    }

    [Benchmark]
    public int StructForge_TryGetValue()
    {
        int total = 0;
        foreach (var key in _lookupKeys)
        {
            if (_sfDict.TryGetValue(key, out int val))
                total += val;
        }
        return total;
    }

    // --- REMOVE ---

    [Benchmark]
    public int System_Remove()
    {
        int removed = 0;
        for (int i = 0; i < Count / 2; i++) 
        {
            if (_sysDict.Remove(_allKeys[i]))
                removed++;
        }
        return removed;
    }

    [Benchmark]
    public int StructForge_Remove()
    {
        int removed = 0;
        for (int i = 0; i < Count / 2; i++)
        {
            if (_sfDict.Remove(_allKeys[i]))
                removed++;
        }
        return removed;
    }
}