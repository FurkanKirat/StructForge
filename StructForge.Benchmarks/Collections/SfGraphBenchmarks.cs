using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections;

[MemoryDiagnoser]
public class SfGraphBenchmarks
{
    [Params(10_000)]
    public int N;

    private SfGraph<int> _sfGraph;
    private Dictionary<int, List<(int, float)>> _systemGraph;

    [GlobalSetup]
    public void Setup()
    {
        _sfGraph = new SfGraph<int>(N);
        _systemGraph = new Dictionary<int, List<(int, float)>>(N);

        for (int i = 0; i < N; i++)
        {
            if (!_systemGraph.ContainsKey(i)) _systemGraph[i] = new List<(int, float)>();
            int target = (i + 1) % N;
            if (!_systemGraph.ContainsKey(target)) _systemGraph[target] = new List<(int, float)>();
            _systemGraph[i].Add((target, 1.0f));

            _sfGraph.AddEdge(i, target, 1.0f);
        }
    }

    [Benchmark(Baseline = true)]
    public void Naive_AddEdges()
    {
        var graph = new Dictionary<int, List<(int, float)>>();
        
        for (int i = 0; i < N; i++)
        {
            if (!graph.ContainsKey(i)) 
                graph[i] = new List<(int, float)>();
            
            int target = i + 1;

            if (!graph.ContainsKey(target)) 
                graph[target] = new List<(int, float)>();
            
            var list = graph[i];
            bool exists = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].Item1 == target) 
                {
                    exists = true; 
                    break;
                }
            }

            if (!exists)
            {
                list.Add((target, 1.0f));
            }
        }
    }

    [Benchmark]
    public void StructForge_AddEdges()
    {
        var graph = new SfGraph<int>();
        for (int i = 0; i < N; i++)
        {
            graph.AddEdge(i, i + 1, 1.0f);
        }
    }

    [Benchmark]
    public float Naive_Traverse()
    {
        float totalWeight = 0;
        foreach (var kvp in _systemGraph)
        {
            foreach (var edge in kvp.Value)
            {
                totalWeight += edge.Item2;
            }
        }
        return totalWeight;
    }

    [Benchmark]
    public float StructForge_Traverse()
    {
        float totalWeight = 0;
        foreach (ref var vertex in _sfGraph.Vertices)
        {
            var neighbors = _sfGraph.GetNeighbors(vertex);
            foreach (ref var edge in neighbors)
            {
                totalWeight += edge.Weight;
            }
        }
        return totalWeight;
    }
    
[MemoryDiagnoser]
public class SfGraphHeavyBenchmarks
{
    [Params(5_000)]
    public int N;

    // --- SCENARIO 1: LARGE STRUCTURE (64 Bytes) ---
    public struct BigStruct : IEquatable<BigStruct>
    {
        public long P1, P2, P3, P4, P5, P6, P7, P8;

        public BigStruct(int id)
        {
            P1 = id; P2 = id; P3 = id; P4 = id; 
            P5 = id; P6 = id; P7 = id; P8 = id;
        }

        public bool Equals(BigStruct other) => P1 == other.P1;
        public override int GetHashCode() => P1.GetHashCode();
    }

    // --- SCENARIO 2: REFERENCE CLASS ---
    public class EntityClass
    {
        public int Id;
        public byte[] Payload;

        public EntityClass(int id)
        {
            Id = id;
            Payload = new byte[32];
        }
    }
    
    
    // Comparer for EntityClass (comparison by ID)
    public class EntityComparer : IEqualityComparer<EntityClass>
    {
        public bool Equals(EntityClass x, EntityClass y) => x?.Id == y?.Id;
        public int GetHashCode(EntityClass obj) => obj.Id.GetHashCode();
    }

    private SfGraph<BigStruct> _sfStructGraph;
    private Dictionary<BigStruct, List<(BigStruct, float)>> _sysStructGraph;

    private SfGraph<EntityClass> _sfClassGraph;
    private Dictionary<EntityClass, List<(EntityClass, float)>> _sysClassGraph;
    private EntityComparer _comparer;
    
    private BigStruct[] _structData;
    private EntityClass[] _classData;

    [GlobalSetup]
    public void Setup()
    {
        _comparer = new EntityComparer();
        _structData = new BigStruct[N];
        _classData = new EntityClass[N];
        
        for(int i=0; i<N; i++)
        {
            _structData[i] = new BigStruct(i);
            _classData[i] = new EntityClass(i);
        }
        
        _sfStructGraph = new SfGraph<BigStruct>(N);
        _sysStructGraph = new Dictionary<BigStruct, List<(BigStruct, float)>>(N);
        
        for (int i = 0; i < N; i++)
        {
            var from = _structData[i];
            var to = _structData[(i + 1) % N];

            _sfStructGraph.AddEdge(from, to, 1f);
            
            if (!_sysStructGraph.ContainsKey(from)) _sysStructGraph[from] = new List<(BigStruct, float)>();
            if (!_sysStructGraph.ContainsKey(to)) _sysStructGraph[to] = new List<(BigStruct, float)>();
            _sysStructGraph[from].Add((to, 1f));
        }

        _sfClassGraph = new SfGraph<EntityClass>(N, _comparer);
        _sysClassGraph = new Dictionary<EntityClass, List<(EntityClass, float)>>(N, _comparer);
        
        for (int i = 0; i < N; i++)
        {
            var from = _classData[i];
            var to = _classData[(i + 1) % N];

            _sfClassGraph.AddEdge(from, to, 1f);
            
            if (!_sysClassGraph.ContainsKey(from)) _sysClassGraph[from] = new List<(EntityClass, float)>();
            if (!_sysClassGraph.ContainsKey(to)) _sysClassGraph[to] = new List<(EntityClass, float)>();
            _sysClassGraph[from].Add((to, 1f));
        }
    }

    // ==========================================
    // STRUCT TESTS
    // ==========================================

    [Benchmark(Baseline = true)]
    public void Naive_Struct_Add()
    {
        var graph = new Dictionary<BigStruct, List<(BigStruct, float)>>();
        for (int i = 0; i < N; i++)
        {
            var from = _structData[i];
            var to = _structData[(i + 1) % N];

            if (!graph.ContainsKey(from)) graph[from] = new List<(BigStruct, float)>();
            if (!graph.ContainsKey(to)) graph[to] = new List<(BigStruct, float)>();
            
            var list = graph[from];
            bool exists = false;
            for(int k=0; k<list.Count; k++) if(list[k].Item1.Equals(to)) { exists=true; break; }
            
            if(!exists) list.Add((to, 1f));
        }
    }

    [Benchmark]
    public void Sf_Struct_Add()
    {
        var graph = new SfGraph<BigStruct>();
        for (int i = 0; i < N; i++)
        {
            var from = _structData[i];
            var to = _structData[(i + 1) % N];
            graph.AddEdge(from, to, 1f);
        }
    }

    [Benchmark]
    public float Sf_Struct_Traverse()
    {
        float sum = 0;
        foreach (ref var v in _sfStructGraph.Vertices)
        {
            var neighbors = _sfStructGraph.GetNeighbors(v);
            foreach (ref var edge in neighbors)
                sum += edge.Weight;
        }
        return sum;
    }

    // ==========================================
    // CLASS TESTS
    // ==========================================

    [Benchmark]
    public void Naive_Class_Add()
    {
        var graph = new Dictionary<EntityClass, List<(EntityClass, float)>>(_comparer);
        for (int i = 0; i < N; i++)
        {
            var from = _classData[i];
            var to = _classData[(i + 1) % N];

            if (!graph.ContainsKey(from)) graph[from] = new List<(EntityClass, float)>();
            if (!graph.ContainsKey(to)) graph[to] = new List<(EntityClass, float)>();
            
            var list = graph[from];
            bool exists = false;
            for(int k=0; k<list.Count; k++) if(_comparer.Equals(list[k].Item1, to)) { exists=true; break; }
            
            if(!exists) list.Add((to, 1f));
        }
    }

    [Benchmark]
    public void Sf_Class_Add()
    {
        var graph = new SfGraph<EntityClass>(16, _comparer);
        for (int i = 0; i < N; i++)
        {
            var from = _classData[i];
            var to = _classData[(i + 1) % N];
            graph.AddEdge(from, to, 1f);
        }
    }
}
}