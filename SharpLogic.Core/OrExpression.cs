namespace SharpLogic.Core;

using System.Collections.Generic;

public class OrExpression : LogicExpression
{
    private readonly LogicExpression _left;
    private readonly LogicExpression _right;

    public OrExpression(LogicExpression left, LogicExpression right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<Solution> Evaluate(Solution? currentSolution = null)
    {
        var startSolution = currentSolution ?? new Solution();

        foreach(var solution in _left.Evaluate(startSolution))
        {
            yield return solution;
        }

        foreach(var solution in _right.Evaluate(startSolution))
        {
            yield return solution;
        }
    }
}
