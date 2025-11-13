using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using StructForge.Benchmarks.Collections;

namespace StructForge.Benchmarks;

public static class SfProgram
{
    public static void Main(string[] args)
    {
        // Quick mod: hızlı test için (geliştirme sırasında)
        var quickConfig = DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(1)        // sadece 1 ısınma turu
                .WithIterationCount(5)      // 5 ölçüm turu
                .WithInvocationCount(16)     // her turda 1 çağrı
                .WithId("Quick"))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        // Full mod: yayın öncesi, daha doğru sonuçlar için
        var fullConfig = DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(5)         // daha fazla ısınma
                .WithIterationCount(15)     // daha fazla tekrar
                .WithId("Full"))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        // ✅ hangi mod çalışacak
        bool quickMode = true; // false yaparsan Full mode çalışır

        var config = quickMode ? quickConfig : fullConfig;

        // Benchmarks'ı burada seçiyorsun:
        BenchmarkRunner.Run<SfSortedSetBenchmarks>(config);
        // BenchmarkRunner.Run<SfListBenchmarks>(config);
        // BenchmarkRunner.Run<SfAvlTreeBenchmarks>(config);
    }
}