using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass2
{
    public long LongProperty { get; set; }
    public float FloatProperty { get; set; }
    public byte ByteProperty { get; set; }
    public List<string> StringList { get; set; } = new List<string>();
}