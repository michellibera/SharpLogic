namespace SharpLogic.Core;

using System;
public class Variable<T> : IEquatable<Variable<T>>
{
    public string Name { get; }
    public Type Type => typeof(T);
    private readonly Guid _id;

    public Variable(string? name = null)
    {
        _id = Guid.NewGuid();
        Name = name ?? $"_{_id:N}"[..8];
    }

    public bool Equals(Variable<T>? other)
    {
        if (other is null) return false;
        return _id.Equals(other._id);
    }

    public override bool Equals(object? obj)
        => obj is Variable<T> other && Equals(other);

    public override int GetHashCode() => _id.GetHashCode();

    public override string ToString() => $"var {Name}: {typeof(T).Name}";

    public static bool operator ==(Variable<T>? left, Variable<T>? right)
        => EqualityComparer<Variable<T>>.Default.Equals(left, right);

    public static bool operator !=(Variable<T>? left, Variable<T>? right)
        => !(left == right);
}
