using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections
{
    [MemoryDiagnoser]
    public class SfListBenchmarks
    {
        private const int N = 10000;
        private List<int> _systemList;
        private SfList<int> _sfList;

        [GlobalSetup]
        public void Setup()
        {
            _systemList = new List<int>(N);
            _sfList = new SfList<int>(N);
        }

        [Benchmark(Baseline = true)]
        public void SystemList_AddRemove()
        {
            var list = _systemList;
            list.Clear();
            for (int i = 0; i < N; i++)
                list.Add(i);

            for (int i = N - 1; i >= 0; i--)
                list.RemoveAt(i);
        }

        [Benchmark]
        public void SfList_AddRemove()
        {
            var list = _sfList;
            list.Clear();
            for (int i = 0; i < N; i++)
                list.Add(i);

            for (int i = N - 1; i >= 0; i--)
                list.RemoveAt(i);
        }
    }
}