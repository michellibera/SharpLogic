namespace SharpLogic.Core;

using System.Collections.Generic;

public class RuleExpression : LogicExpression
{
    private readonly RuleType _ruleType;
    private readonly object[] _pattern;

    public RuleExpression(RuleType ruleType, object[] pattern)
    {
        _ruleType = ruleType;
        _pattern = pattern;
    }

    public override IEnumerable<Solution> Evaluate(Solution? currentSolution = null)
    {
        var startSolution = currentSolution ?? new Solution();
        return _ruleType.EvaluateWithArgs(_pattern);
    }
}