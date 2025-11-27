using BenchmarkDotNet.Attributes;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections
{
    [MemoryDiagnoser]
    public class SfHashSetBenchmarks
    {
        private const int N = 1000000;
        private HashSet<int> _systemSet;
        private SfHashSet<int> _sfSet;

        [GlobalSetup]
        public void Setup()
        {
            _systemSet = new HashSet<int>(N);
            _sfSet = new SfHashSet<int>(N);
        }

        [Benchmark(Baseline = true)]
        public void SystemHashSet_AddContainsRemove()
        {
            var set = _systemSet;
            set.Clear();

            bool dummy = false;
            for (int i = 0; i < N; i++)
            {
                set.Add(i);
                dummy |= set.Contains(i);
                set.Remove(i);
            }

        }

        [Benchmark]
        public void SfHashSet_AddContainsRemove()
        {
            var set = _sfSet;
            set.Clear();

            bool dummy = false;
            for (int i = 0; i < N; i++)
            {
                set.Add(i);
                dummy |= set.Contains(i);
                set.Remove(i);
            }
        }
    }
}