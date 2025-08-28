namespace SharpLogic.Core;

using System;
using static SharpLogic.Core.FactType;

public static class Extensions
{
    public static Variable<T> Var<T>(string? name = null) => new(name);
    
    private static int _varCounter = 0;
    public static Variable<object> Var(string? name = null) 
    {
        return name != null 
            ? new Variable<object>(name) 
            : new Variable<object>($"_G{_varCounter++}");
    }

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
    
    public static RuleType Rule(string name, Delegate ruleFunc)
    {
        Rules.Rule(name, ruleFunc);
        var ruleDefinition = Rules.GetRule(name)!;
        return new RuleType(ruleDefinition);
    }
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

    public static RuleType.RuleQuery Query(this RuleType ruleType, params object[] args)
    {
        return (RuleType.RuleQuery)ruleType.Invoke(args);
    }

    public static bool Exists(this RuleType ruleType, params object[] args)
    {
        bool hasVariables = args.Any(arg => arg is Anonymous || IsVariableType(arg));
        
        if (hasVariables)
        {
            return ruleType.Query(args).Any();
        }
        else
        {
            return (bool)ruleType.Invoke(args);
        }
    }

    public static bool Any(this RuleType ruleType, params object[] pattern)
    {
        return ruleType.Query(pattern).Any();
    }

    public static int Count(this RuleType ruleType, params object[] pattern)
    {
        return ruleType.Query(pattern).Count();
    }

    private static bool IsVariableType(object obj)
    {
        if (obj is null) return false;
        var type = obj.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Variable<>);
    }
}
