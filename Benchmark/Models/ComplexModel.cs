using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class ComplexModel
{
    public int IntProperty { get; set; }
    public double DoubleProperty { get; set; }
    public string StringProperty { get; set; } = string.Empty;
    public DateTime DateProperty { get; set; }
    public bool BoolProperty { get; set; }
    public List<int> IntList { get; set; } = new List<int>();
    public string[] StringArray { get; set; } = Array.Empty<string>();
    public double[] DoubleEnumerable { get; set; } = Array.Empty<double>();
    public CustomSubclass1 Nested1 { get; set; } = new();
    public List<CustomSubclass2> NestedList2 { get; set; } = new List<CustomSubclass2>();
    public CustomEnum1 EnumProperty1 { get; set; }
    public CustomEnum2 EnumProperty2 { get; set; }
}