using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StructForge.Collections;

namespace StructForge.Benchmarks.Collections
{
    [MemoryDiagnoser]
    public class SfDictionaryBenchmarks
    {
        private const int N = 10000;
        private int[] keys;
        private int[] values;

        private Dictionary<int, int> systemDict;
        private SfDictionary<int, int> sfDict;

        [GlobalSetup]
        public void Setup()
        {
            keys = new int[N];
            values = new int[N];
            var rand = new Random(42);

            for (int i = 0; i < N; i++)
            {
                keys[i] = rand.Next();
                values[i] = rand.Next();
            }

            systemDict = new Dictionary<int, int>();
            sfDict = new SfDictionary<int, int>();
        }

        [Benchmark(Baseline = true)]
        public void SystemDictionary_AddContainsRemove()
        {
            systemDict.Clear();
            for (int i = 0; i < N; i++)
                systemDict.Add(keys[i], values[i]);

            for (int i = 0; i < N; i++)
                _ = systemDict.ContainsKey(keys[i]);

            for (int i = 0; i < N; i++)
                systemDict.Remove(keys[i]);
        }

        [Benchmark]
        public void SfDictionary_AddContainsRemove()
        {
            sfDict.Clear();
            for (int i = 0; i < N; i++)
                sfDict.Add(keys[i], values[i]);

            for (int i = 0; i < N; i++)
                _ = sfDict.ContainsKey(keys[i]);

            for (int i = 0; i < N; i++)
                sfDict.Remove(keys[i]);
        }
    }
}