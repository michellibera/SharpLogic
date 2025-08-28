namespace SharpLogic.Core;

using System;
using System.Collections.Generic;

public static class Rules
{
    private static readonly Dictionary<string, RuleDefinition> _rules = new();

    public static void Rule(string name, Delegate ruleFunc)
    {
        var method = ruleFunc.Method;
        var parameters = method.GetParameters();
        int arity = parameters.Length;

        _rules[name] = new RuleDefinition(name, arity, args =>
        {
            if (args.Length != arity)
            {
                throw new ArgumentException($"Rule '{name}' expects {arity} arguments, got {args.Length}");
            }

            var result = ruleFunc.DynamicInvoke(args);
            return (LogicExpression)result;
        });
    }

    public static RuleDefinition? GetRule(string name)
    {
        _rules.TryGetValue(name, out var rule);
        return rule;
    }

    public static bool HasRule(string name) => _rules.ContainsKey(name);

    public static IEnumerable<string> GetRuleNames() => _rules.Keys;

}

public class RuleDefinition
{
    public string Name { get; }
    public int Arity { get; }
    public Func<object[], LogicExpression> Expression { get; }

    public RuleDefinition(string name, int arity, Func<object[], LogicExpression> expression)
    {
        Name = name;
        Arity = arity;
        Expression = expression;
    }

    public IEnumerable<Solution> Evaluate()
    {
        var args = CreateVariables();
        var expression = Expression(args);
        return expression.Evaluate(new Solution());
    }

    internal object[] CreateVariables()
    {
        var variables = new object[Arity];
        for(int i = 0; i < Arity; i++)
        {
            variables[i] = new Variable<object>($"Arg{i}");
        }
        return variables;
    }
}
