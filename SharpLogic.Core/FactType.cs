namespace SharpLogic.Core;

using System;

public class FactType
{
    public string Name { get; }
    public int? Arity { get; private set; }
    private readonly HashSet<object[]> _facts = new(ObjectArrayContentEqualityComparer.Instance);

    public FactType(string name) => Name = name;
    public dynamic Invoke(params object[] args)
    {
        ValidateArity(args.Length);

        bool hasVariables = ContainsVariables(args);

        if(hasVariables)
        {
            return new FactQuery(this, args);
        }
        else
        {
            // HashSet automatically prevents duplicates
            _facts.Add(args);
            
            // Return this FactType for method chaining
            return this;
        }
    }

    private void ValidateArity(int arity)
    {
        if (Arity is null)
        {
            Arity = arity;
        }
        else if (Arity.Value != arity)
        {
            throw new ArgumentException($"Fact type '{Name}' expects {Arity} arguments, but got {arity}. " +
              $"All facts of type '{Name}' must have the same number of arguments.");
        }
    }

    private bool ContainsVariables(object[] args)
    {
        foreach(var arg in args)
        {
            if (arg is Anonymous || IsVariableType(arg))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsVariableType(object obj)
    {
        if (obj is null) return false;

        var type = obj.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Variable<>);
    }

    internal IEnumerable<object[]> QueryFacts(object[] pattern)
    {
        foreach(var fact in _facts)
        {
            if(Matches(fact, pattern))
            {
                yield return fact;
            }
        }
    }

    private bool Matches(object[] fact, object[] pattern)
    {
        if (fact.Length != pattern.Length) return false;

        for(int i = 0; i < fact.Length; i++)
        {
            if (pattern[i] is Anonymous)
            {
                continue;
            }

            if (IsVariableType(pattern[i]))
            {
                continue;
            }

            if (!Equals(fact[i], pattern[i]))
            {
                return false;
            }
        }

        return true;
    }

    public class FactQuery : IEnumerable<object[]>
    {
        private readonly FactType _factType;
        private readonly object[] _pattern;

        internal FactQuery(FactType factType, object[] pattern)
        {
            _factType = factType;
            _pattern = pattern;
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return _factType.QueryFacts(_pattern).GetEnumerator();
        }

        System.Collections.IEnumerator
        System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<object[]> GetResults()
        {
            return _factType.QueryFacts(_pattern);
        }

        public override string ToString()
        {
            var args = string.Join(", ", _pattern.Select(arg =>
                arg is Anonymous ? "_" : arg?.ToString() ?? "null"));
            return $"{_factType.Name}({args})";
        }
    }

}
