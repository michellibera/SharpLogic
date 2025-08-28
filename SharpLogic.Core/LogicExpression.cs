namespace SharpLogic.Core;

using System;
using System.Collections.Generic;

public abstract class LogicExpression
{
    public abstract IEnumerable<Solution> Evaluate(Solution? currentSolution = null);

    public static AndExpression operator &(LogicExpression left, LogicExpression right)
    {
        return new AndExpression(left, right);
    }

    public static OrExpression operator |(LogicExpression left, LogicExpression right)
    {
        return new OrExpression(left, right);
    }
}
