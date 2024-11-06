using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass6
{
    public Guid Property1 { get; set; }
    public CustomSubclass3 Property2 { get; set; } = new();
}