using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass4
{
    public double Property1 { get; set; }
    public bool Property2 { get; set; }
    public CustomSubclass1 Property3 { get; set; } = new();
}