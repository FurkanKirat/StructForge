using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace StructForge.Benchmarks;

public static class SfProgram
{
    public static void Main(string[] args)
    {
        bool isFullMode = args.Any(a => a.Equals("--full", StringComparison.OrdinalIgnoreCase));

        var job = isFullMode 
            ? Job.MediumRun.WithId("FullRun") 
            : Job.ShortRun.WithId("QuickRun")
                 .WithWarmupCount(1)
                 .WithIterationCount(3)
                 .WithInvocationCount(16);

        var config = new ManualConfig()
            .AddJob(job)
            .AddLogger(ConsoleLogger.Default) 
            
            .AddColumnProvider(DefaultColumnProviders.Instance) 
            
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(HtmlExporter.Default)
            .AddExporter(CsvExporter.Default)
            
            .WithOptions(ConfigOptions.JoinSummary) 
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        Console.WriteLine("Starting benchmarks... (Results will be written to /bin/Release/.../Artifacts)");
        
        BenchmarkSwitcher
            .FromAssembly(typeof(SfProgram).Assembly)
            .Run(args, config);
    }
}