using Shouldly;
using SharpLogic.Core;
using static SharpLogic.Core.Globals;
using static SharpLogic.Core.Extensions;
using Xunit;

namespace SharpLogic.Tests;

public class RuleTypeTests
{
    [Fact]
    public void Rule_ShouldCreateRuleWithCorrectNameAndArity()
    {
        var parent = Facts.FactType("parent");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        grandparent.Name.ShouldBe("grandparent");
        grandparent.Arity.ShouldBe(2);
    }
    
    [Fact]
    public void Rule_WithConcreteValues_ShouldReturnBooleanResult()
    {
        var parent = Facts.FactType("parent");
        parent.Add("john", "mary");
        parent.Add("mary", "susan");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var result = grandparent.Exists("john", "susan");
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void Rule_WithConcreteValues_ShouldReturnFalseForNonMatching()
    {
        var parent = Facts.FactType("parent");
        parent.Add("john", "mary");
        parent.Add("mary", "susan");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var result = grandparent.Exists("john", "alice");
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void Rule_WithVariables_ShouldReturnRuleQuery()
    {
        var parent = Facts.FactType("parent");
        parent.Add("john", "mary");
        parent.Add("mary", "susan");
        parent.Add("bob", "alice");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var query = grandparent.Query(_, "susan");
        var results = query.ToList();
        results.Count.ShouldBe(1);
        results[0].ShouldBe(new object[] { "john", "susan" });
    }
    
    [Fact]
    public void Rule_WithMultipleVariables_ShouldReturnAllMatches()
    {
        var parent = Facts.FactType("parent");
        parent.Add("john", "mary");
        parent.Add("mary", "susan");
        parent.Add("bob", "alice");
        parent.Add("alice", "tom");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var query = grandparent.Query(_, _);
        var results = query.ToList();
        results.Count.ShouldBe(2);
        results.ShouldContain(result => result.SequenceEqual(new object[] { "john", "susan" }));
        results.ShouldContain(result => result.SequenceEqual(new object[] { "bob", "tom" }));
    }
    
    [Fact]
    public void Rule_WithWrongArity_ShouldThrowException()
    {
        var parent = Facts.FactType("parent");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var exception = Should.Throw<ArgumentException>(() => 
            grandparent.Invoke("john", "mary", "extra"));
        
        exception.Message.ShouldContain("expects 2 arguments, got 3");
        exception.Message.ShouldContain("grandparent");
    }
    
    [Fact]
    public void Rule_UsingAnotherRule_ShouldWorkCorrectly()
    {
        var parent = Facts.FactType("parent");
        parent.Add("john", "mary");
        parent.Add("mary", "susan");
        parent.Add("susan", "tom");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var greatGrandparent = Rule("great_grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return grandparent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var result = greatGrandparent.Exists("john", "tom");
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void Rule_Match_ShouldReturnRuleExpression()
    {
        var parent = Facts.FactType("parent");
        
        var grandparent = Rule("grandparent", (object X, object Z) =>
        {
            var Y = Var();
            return parent.Match(X, Y) & parent.Match(Y, Z);
        });
        
        var expression = grandparent.Match("john", "susan");
        
        expression.ShouldBeOfType<RuleExpression>();
    }
}