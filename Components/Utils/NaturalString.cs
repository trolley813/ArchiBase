using NaturalSort.Extension;

public struct NaturalString : IComparable<NaturalString>
{
    public string Data { get; set; }

    public readonly int CompareTo(NaturalString other) =>
        comparer.Compare(Data, other.Data);

    private static readonly NaturalSortComparer comparer =
        StringComparison.OrdinalIgnoreCase.WithNaturalSort();

    public static implicit operator NaturalString(string data) => new() { Data = data };
    public static implicit operator string(NaturalString other) => other.Data;
}