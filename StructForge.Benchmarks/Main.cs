using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using StructForge.Benchmarks.Collections;

namespace StructForge.Benchmarks;

public static class SfProgram
{
    public static void Main(string[] args)
    {
        var quickConfig = DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(1) 
                .WithIterationCount(5)
                .WithInvocationCount(16)
                .WithId("Quick"))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        var fullConfig = DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(5)
                .WithIterationCount(15)
                .WithId("Full"))
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        bool quickMode = true;

        var config = quickMode ? quickConfig : fullConfig;
        
        BenchmarkRunner.Run<SfDictionaryBenchmarks>(config);
        
    }
}