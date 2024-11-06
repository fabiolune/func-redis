using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass10
{
    public CustomSubclass4 Property1 { get; set; } = new();
    public CustomSubclass5 Property2 { get; set; } = new();
}