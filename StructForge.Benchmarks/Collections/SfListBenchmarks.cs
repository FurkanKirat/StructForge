using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections
{
    [MemoryDiagnoser]
    [RankColumn]
    public class SfListBenchmarks
    {
        [Params(100, 10_000)] 
        public int N;

        private List<int> _systemList;
        private SfList<int> _sfList;

        [GlobalSetup]
        public void Setup()
        {
            _systemList = new List<int>(N);
            _sfList = new SfList<int>(N);

            for (int i = 0; i < N; i++)
            {
                _systemList.Add(i);
                _sfList.Add(i);
            }
        }
        
        
        [Benchmark(Baseline = true)]
        public void SystemList_InsertAtZero()
        {
            var list = new List<int>();
            for (int i = 0; i < N; i++)
            {
                list.Insert(0, i);
            }
        }

        [Benchmark]
        public void SfList_InsertAtZero()
        {
            var list = new SfList<int>();
            for (int i = 0; i < N; i++)
            {
                list.Insert(0, i);
            }
        }
        
        
        [Benchmark]
        public long SystemList_Foreach()
        {
            long sum = 0;
            foreach (var item in _systemList)
            {
                sum += item;
            }
            return sum;
        }

        [Benchmark]
        public long SfList_Foreach()
        {
            long sum = 0;
            foreach (var item in _sfList)
            {
                sum += item;
            }
            return sum;
        }

        
        [Benchmark]
        public int SystemList_Indexer()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += _systemList[i];
            }
            return sum;
        }

        [Benchmark]
        public int SfList_Indexer()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += _sfList[i];
            }
            return sum;
        }
        
        [Benchmark]
        public void SystemList_RemoveAtSwap_Simulated()
        {
            var list = new List<int>(_systemList); 
            
            while (list.Count > 0)
            {
                int indexToRemove = 0;
                int lastIndex = list.Count - 1;
                
                var lastItem = list[lastIndex];
                list[indexToRemove] = lastItem;
                list.RemoveAt(lastIndex);
            }
        }

        [Benchmark]
        public void SfList_RemoveAtSwap()
        {
            var list = new SfList<int>(_sfList);
            
            while (list.Count > 0)
            {
                list.RemoveAtSwap(0); 
            }
        }
        
        [Benchmark]
        public int SystemList_AsSpan_Sum()
        {
            var span = CollectionsMarshal.AsSpan(_systemList);
            int sum = 0;
            for (int i = 0; i < span.Length; i++)
            {
                sum += span[i];
            }
            return sum;
        }

        [Benchmark]
        public int SfList_AsSpan_Sum()
        {
            var span = _sfList.AsSpan();
            int sum = 0;
            for (int i = 0; i < span.Length; i++)
            {
                sum += span[i];
            }
            return sum;
        }
    }
}