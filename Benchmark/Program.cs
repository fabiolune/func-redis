using Benchmark.Models;
using BenchmarkDotNet.Running;
using System.Diagnostics.CodeAnalysis;

namespace Benchmark;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main() => BenchmarkRunner.Run<SerDesBenchmark<ComplexModel>>();
}