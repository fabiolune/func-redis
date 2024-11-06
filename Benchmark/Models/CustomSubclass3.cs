using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass3
{
    public int Property1 { get; set; }
    public string Property2 { get; set; } = string.Empty;
    public bool Property3 { get; set; }
}