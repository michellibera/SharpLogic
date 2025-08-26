namespace SharpLogic.Core;

using System;
using static SharpLogic.Core.FactType;

public static class Extensions
{
    public static Variable<T> Var<T>(string? name = null) => new(name);

    public static bool Exists(this FactType factType, params object[] args)
    {
        if (factType.Arity.HasValue && args.Length != factType.Arity.Value)
        {
            throw new ArgumentException($"Fact type '{factType.Name}' expects {factType.Arity.Value} arguments, but got {args.Length}");
        }

        return factType.QueryFacts(args).Any();
    }

    public static int Count(this FactType factType, params object[] pattern)
    {
        return factType.QueryFacts(pattern).Count();
    }

    public static void AddMany(this FactType factType, params object[][] facts)
    {
        foreach (var fact in facts)
        {
            factType.Invoke(fact);
        }
    }
}

public static class Facts
{
    public static FactType FactType(string name) => new(name);

    public static Dictionary<string, FactType> FactTypes(params string[] names)
    {
        return names.ToDictionary(name => name, name => new FactType(name));
    }
}

public static class Globals
{
    public static readonly Anonymous _ = Anonymous._;
}

public static class QueryExtensions
{
    public static FactQuery Query(this FactType factType, params object[] args)
    {
        return (FactQuery)factType.Invoke(args);
    }

    public static FactType Add(this FactType factType, params object[] args)
    {
        factType.Invoke(args);
        return factType;
    }

    public static bool Any(this FactType factType, params object[] pattern)
    {
        return factType.QueryFacts(pattern).Any();
    }
}
