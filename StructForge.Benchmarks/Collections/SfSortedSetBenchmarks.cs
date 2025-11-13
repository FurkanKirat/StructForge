using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class SfSortedSetBenchmarks
    {
        private SortedSet<int> _systemSet;
        private SfSortedSet<int> _sfSet;
        private int[] _testData;
        private int[] _removeData;

        [Params(100, 1_000, 10_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _testData = new int[N];
            _removeData = new int[N / 2];

            for (int i = 0; i < N; i++)
                _testData[i] = random.Next();

            Array.Copy(_testData, _removeData, N / 2);

            _systemSet = new SortedSet<int>();
            _sfSet = new SfSortedSet<int>();
        }

        // --------------------------------------------------
        // ADD BENCHMARKS
        // --------------------------------------------------
        [Benchmark(Baseline = true)]
        public void SystemSortedSet_Add()
        {
            var set = new SortedSet<int>();
            for (int i = 0; i < N; i++)
                set.Add(_testData[i]);
        }

        [Benchmark]
        public void SfSortedSet_Add()
        {
            var set = new SfSortedSet<int>();
            for (int i = 0; i < N; i++)
                set.Add(_testData[i]);
        }

        // --------------------------------------------------
        // CONTAINS BENCHMARKS
        // --------------------------------------------------
        [Benchmark]
        public void SystemSortedSet_Contains()
        {
            for (int i = 0; i < N; i++)
                _systemSet.Contains(_testData[i]);
        }

        [Benchmark]
        public void SfSortedSet_Contains()
        {
            for (int i = 0; i < N; i++)
                _sfSet.Contains(_testData[i]);
        }

        // --------------------------------------------------
        // REMOVE BENCHMARKS
        // --------------------------------------------------
        [Benchmark]
        public void SystemSortedSet_Remove()
        {
            var set = new SortedSet<int>(_testData);
            for (int i = 0; i < _removeData.Length; i++)
                set.Remove(_removeData[i]);
        }

        [Benchmark]
        public void SfSortedSet_Remove()
        {
            var set = new SfSortedSet<int>(_testData);
            for (int i = 0; i < _removeData.Length; i++)
                set.Remove(_removeData[i]);
        }
    }
}
