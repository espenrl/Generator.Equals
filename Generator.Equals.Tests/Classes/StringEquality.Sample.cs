using System;

namespace Generator.Equals.Tests.Classes;

public partial class StringEquality
{
    [Equatable]
    public partial class Sample
    {
        [StringEquality(StringComparison.Ordinal)] public required string StringOrdinal { get; init; }
        [StringEquality(StringComparison.OrdinalIgnoreCase)] public required string StringOrdinalIgnoreCase { get; init; }
    }
}

