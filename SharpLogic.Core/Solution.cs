namespace SharpLogic.Core;

using System.Collections.Generic;
using System.Linq;

public class Solution
{
    private readonly Dictionary<object, object> _bindings;

    public Solution()
    {
        _bindings = new Dictionary<object, object>();
    }

    private Solution(Dictionary<object,object> bindings)
    {
        _bindings = new Dictionary<object, object>(bindings);
    }

    public Solution Bind(object variable, object value)
    {
        if(_bindings.TryGetValue(variable, out var existingValue))
        {
            if (Equals(existingValue, value)) return this;

            return null;
        }

        var newBindings = new Dictionary<object, object>(_bindings);
        newBindings[variable] = value;
        return new Solution(newBindings);
    }

    public object? GetBinding(object variable)
    {
        return _bindings.TryGetValue(variable, out var value) ? value : null;
    }

    public bool IsBound(object variable)
    {
        return _bindings.ContainsKey(variable);
    }

    public bool IsConsistent(Solution other)
    {
        foreach(var kvp in _bindings)
        {
            if (other._bindings.TryGetValue(kvp.Key, out var otherValue))
            {
                if (!Equals(kvp.Value, otherValue)) return false;
            }
        }
        return true;
    }

    public Solution? Merge(Solution other)
    {
        if (!IsConsistent(other)) return null;

        var merged = new Dictionary<object,object>(_bindings);
        foreach(var kvp in other._bindings)
        {
            merged[kvp.Key] = kvp.Value;
        }

        return new Solution(merged);
    }

    public IEnumerable<object> Variables => _bindings.Keys;
    public int Count => _bindings.Count;
    public bool IsEmpty => _bindings.Count == 0;

    public override string ToString()
    {
        if (_bindings.Count == 0) return "{}";

        var bindings = _bindings.Select(kvp => $"{kvp.Key}: { kvp.Value}");
        return "{" + string.Join(", ", bindings) + "}";
    }
}
