namespace SharpLogic.Core;

using System;
public struct Anonymous
{
    public static readonly Anonymous _ = new();

    public override string ToString() => "_";
    public override bool Equals(object? obj) => obj is Anonymous;
    public override int GetHashCode() => typeof(Anonymous).GetHashCode();
    public static bool operator ==(Anonymous left, Anonymous right) => true;
    public static bool operator !=(Anonymous left, Anonymous right) => false;
}
