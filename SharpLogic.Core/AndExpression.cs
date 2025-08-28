namespace SharpLogic.Core;

using System.Collections.Generic;

public class AndExpression : LogicExpression
{
    private readonly LogicExpression _left;
    private readonly LogicExpression _right;

    public AndExpression(LogicExpression left, LogicExpression right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<Solution> Evaluate(Solution? currentSolution = null)
    {
        var startSolution = currentSolution ?? new Solution();

        var leftSolutions = _left.Evaluate(startSolution);

        foreach(var leftSolution in leftSolutions)
        {
            var rightSolutions = _right.Evaluate(leftSolution);

            foreach(var rightSolution in rightSolutions)
            {
                yield return rightSolution;
            }
        }
    }
}
