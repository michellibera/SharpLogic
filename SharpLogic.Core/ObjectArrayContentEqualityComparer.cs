namespace SharpLogic.Core;

using System;

public class ObjectArrayContentEqualityComparer : IEqualityComparer<object[]>
{
    public static readonly ObjectArrayContentEqualityComparer Instance = new();

    private ObjectArrayContentEqualityComparer() { }
    public bool Equals(object[]? x, object[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Length != y.Length) return false;

        for(int i = 0; i< x.Length; i++)
        {
            if (!Equals(x[i], y[i])) return false;
        }

        return true;
    }

    public int GetHashCode(object[] obj)
    {
        if (obj is null) return 0;

        var hash = new HashCode();
        foreach (var item in obj)
        {
            hash.Add(item);
        }
        return hash.ToHashCode();
    }
}
