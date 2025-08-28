namespace SharpLogic.Core;

using System;
using System.Collections.Generic;
using System.Linq;



public class FactExpression : LogicExpression
{
    private readonly FactType _factType;
    private readonly object[] _pattern;

    public FactExpression(FactType factType, object[] pattern)
    {
        _factType = factType;
        _pattern = pattern;
    }

    public override IEnumerable<Solution> Evaluate(Solution? currentSolution = null)
    {
        var startSolution = currentSolution ?? new Solution();

        var boundPattern = ApplyBindings(_pattern, startSolution);
        var matchingFacts = _factType.QueryFacts(boundPattern);

        foreach (var fact in matchingFacts)
        {
            var solution = UnifyFactWithPattern(fact, _pattern, startSolution);
            if (solution != null) yield return solution;
        }
    }

    private object[] ApplyBindings(object[] pattern, Solution solution)
    {
        return pattern.Select(arg =>
        {
            if (IsVariable(arg) && solution.IsBound(arg)) return solution.GetBinding(arg)!;

            return arg;
        }).ToArray();
    }

    private Solution? UnifyFactWithPattern(object[] fact, object[] pattern, Solution currentSolution)
    {
        var result = currentSolution;

        for(int i = 0; i < fact.Length; i++)
        {
            if (IsVariable(pattern[i]))
            {
                result = result.Bind(pattern[i], fact[i]);
                if (result == null) return null;
            }
            else if (pattern[i] is not Anonymous && !Equals(pattern[i], fact[i]))
            {
                return null;
            }
        }
        return result;
    }

    private static bool IsVariable(object obj)
    {
        if (obj is null) return false;
        var type = obj.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Variable<>);
    }
}
