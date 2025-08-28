namespace SharpLogic.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public class RuleType
{
    private readonly RuleDefinition _ruleDefinition;
    
    public string Name => _ruleDefinition.Name;
    public int Arity => _ruleDefinition.Arity;
    
    internal RuleType(RuleDefinition ruleDefinition)
    {
        _ruleDefinition = ruleDefinition;
    }
    
    public dynamic Invoke(params object[] args)
    {
        if (args.Length != Arity)
        {
            throw new ArgumentException($"Rule '{Name}' expects {Arity} arguments, got {args.Length}");
        }
        
        bool hasVariables = ContainsVariables(args);
        
        if (hasVariables)
        {
            return new RuleQuery(this, args);
        }
        else
        {
            var solutions = EvaluateWithArgs(args).ToList();
            return solutions.Count > 0;
        }
    }
    
    public LogicExpression Match(params object[] args)
    {
        if (args.Length != Arity)
        {
            throw new ArgumentException($"Rule '{Name}' expects {Arity} arguments, got {args.Length}");
        }
        return new RuleExpression(this, args);
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
    
    internal IEnumerable<Solution> EvaluateWithArgs(object[] args)
    {
        var initialSolution = new Solution();
        var ruleArgs = _ruleDefinition.CreateVariables();
        
        for (int i = 0; i < args.Length; i++)
        {
            if (!(args[i] is Anonymous) && !IsVariableType(args[i]))
            {
                initialSolution = initialSolution.Bind(ruleArgs[i], args[i]);
                if (initialSolution == null) yield break;
            }
        }
        
        var expression = _ruleDefinition.Expression(ruleArgs);
        foreach (var solution in expression.Evaluate(initialSolution))
        {
            yield return solution;
        }
    }
    
    public class RuleQuery : IEnumerable<object[]>
    {
        private readonly RuleType _ruleType;
        private readonly object[] _pattern;
        
        internal RuleQuery(RuleType ruleType, object[] pattern)
        {
            _ruleType = ruleType;
            _pattern = pattern;
        }
        
        public IEnumerator<object[]> GetEnumerator()
        {
            return GetResults().GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerable<object[]> GetResults()
        {
            var ruleArgs = _ruleType._ruleDefinition.CreateVariables();
            
            var initialSolution = new Solution();
            for (int i = 0; i < _pattern.Length; i++)
            {
                if (!(_pattern[i] is Anonymous) && !IsVariableType(_pattern[i]))
                {
                    initialSolution = initialSolution.Bind(ruleArgs[i], _pattern[i]);
                    if (initialSolution == null) yield break;
                }
            }
            
            var expression = _ruleType._ruleDefinition.Expression(ruleArgs);
            var solutions = expression.Evaluate(initialSolution);
            
            foreach (var solution in solutions)
            {
                var result = new object[_pattern.Length];
                for (int i = 0; i < _pattern.Length; i++)
                {
                    if (_pattern[i] is Anonymous)
                    {
                        result[i] = solution.GetBinding(ruleArgs[i]) ?? _pattern[i];
                    }
                    else if (IsVariableType(_pattern[i]))
                    {
                        result[i] = solution.GetBinding(_pattern[i]) ?? _pattern[i];
                    }
                    else
                    {
                        result[i] = _pattern[i];
                    }
                }
                yield return result;
            }
        }
        
        private static bool IsVariableType(object obj)
        {
            if (obj is null) return false;
            var type = obj.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Variable<>);
        }
        
        public override string ToString()
        {
            var args = string.Join(", ", _pattern.Select(arg =>
                arg is Anonymous ? "_" : arg?.ToString() ?? "null"));
            return $"{_ruleType.Name}({args})";
        }
    }
}