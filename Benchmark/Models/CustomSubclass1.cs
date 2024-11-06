using System.Diagnostics.CodeAnalysis;

namespace Benchmark.Models;

[ExcludeFromCodeCoverage]
public class CustomSubclass1
{
    public Guid GuidProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public char CharProperty { get; set; }
    public bool[] BoolArray { get; set; } = Array.Empty<bool>();
}