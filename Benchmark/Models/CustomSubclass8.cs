using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass8
{
    public long Property1 { get; set; }
    public CustomSubclass2 Property2 { get; set; } = new();
}