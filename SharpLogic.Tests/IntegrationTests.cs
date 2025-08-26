using Shouldly;
using SharpLogic.Core;
using static SharpLogic.Core.Globals;
using Xunit;

namespace SharpLogic.Tests;

public class IntegrationTests
{
    [Fact]
    public void ComplexLogicScenario_ShouldWorkEndToEnd()
    {
        // Arrange - Family tree scenario
        var parent = new FactType("parent");
        var age = new FactType("age");
        var likes = new FactType("likes");
        
        // Assert facts
        parent.AddMany(
            new object[] { "john", "mary" },
            new object[] { "mary", "susan" },
            new object[] { "bob", "alice" },
            new object[] { "alice", "tom" }
        );
        
        age.AddMany(
            new object[] { "john", 45 },
            new object[] { "mary", 20 },
            new object[] { "bob", 40 },
            new object[] { "alice", 18 },
            new object[] { "susan", 5 },
            new object[] { "tom", 2 }
        );
        
        likes.AddMany(
            new object[] { "mary", "chocolate" },
            new object[] { "susan", "toys" },
            new object[] { "alice", "books" }
        );
        
        // Act & Assert - Various queries
        
        // Who are the parents?
        var allParents = parent.Query(_, _).Select(f => f[0]).Distinct().ToList();
        allParents.Count.ShouldBe(4);
        allParents.ShouldContain("john");
        allParents.ShouldContain("mary");
        allParents.ShouldContain("bob");
        allParents.ShouldContain("alice");
        
        // Who are Mary's children?
        var marysChildren = parent.Query("mary", _).Select(f => f[1]).ToList();
        marysChildren.Count.ShouldBe(1);
        marysChildren.ShouldContain("susan");
        
        // Find all adults (age >= 18)
        var adults = age.Query(_, _)
                       .Where(f => (int)f[1] >= 18)
                       .Select(f => f[0])
                       .ToList();
        adults.Count.ShouldBe(4);
        adults.ShouldContain("john");
        adults.ShouldContain("mary");
        adults.ShouldContain("bob");
        adults.ShouldContain("alice");
        
        // Complex query: Find parents who have children and are adults
        var adultParents = parent.Query(_, _)
                                .Select(f => f[0]) // Get parent names
                                .Distinct()
                                .Where(parentName => 
                                    age.Exists(parentName, _) && 
                                    age.Query(parentName, _).Any(ageF => (int)ageF[1] >= 18))
                                .ToList();
        
        adultParents.Count.ShouldBe(4);
        
        // Check specific relationships
        parent.Exists("john", "mary").ShouldBeTrue();
        parent.Exists("mary", "john").ShouldBeFalse(); // Wrong order
        
        age.Exists("susan", 5).ShouldBeTrue();
        likes.Exists("mary", "chocolate").ShouldBeTrue();
    }
    
    [Fact]
    public void ArityValidation_ShouldWorkAcrossOperations()
    {
        // Arrange
        var relation = new FactType("relation");
        
        // Set arity to 3
        relation.Add("john", "friend", "mary");
        
        // Assert - All operations should respect arity
        relation.Arity.ShouldBe(3);
        
        // Valid operations with correct arity
        relation.Exists("john", "friend", "mary").ShouldBeTrue();
        relation.Count(_, "friend", _).ShouldBe(1);
        relation.Any(_, _, "mary").ShouldBeTrue();
        
        // Invalid operations with wrong arity should throw
        Should.Throw<ArgumentException>(() => relation.Add("john", "mary")); // 2 args
        Should.Throw<ArgumentException>(() => relation.Exists("john", "mary")); // 2 args  
        Should.Throw<ArgumentException>(() => relation.Query(_, _)); // 2 args
    }
    
    [Fact]
    public void LazyEvaluation_ShouldOnlyProcessWhenNeeded()
    {
        // Arrange
        var numbers = new FactType("number");
        
        // Add many facts
        for (int i = 0; i < 1000; i++)
        {
            numbers.Add(i, i * 2);
        }
        
        // Act - Create query but don't enumerate
        var query = numbers.Query(_, _);
        
        // The query should be created instantly (lazy)
        query.ShouldNotBeNull();
        
        // Now enumerate only first 5
        var first5 = query.Take(5).ToList();
        
        // Assert
        first5.Count.ShouldBe(5);
        // Verify we got expected results
        first5[0].ShouldBe(new object[] { 0, 0 });
        first5[1].ShouldBe(new object[] { 1, 2 });
    }
    
    [Fact]
    public void MixedTypes_ShouldWorkCorrectly()
    {
        // Arrange
        var mixed = new FactType("mixed");
        
        // Act - Add facts with different types
        mixed.Add("string", 42);
        mixed.Add("hello", 100);
        mixed.Add("test", -5);
        
        // Assert - Query with mixed types
        var stringFacts = mixed.Query("string", _).ToList();
        stringFacts.Count.ShouldBe(1);
        stringFacts[0].ShouldBe(new object[] { "string", 42 });
        
        var numberMatches = mixed.Query(_, 100).ToList();
        numberMatches.Count.ShouldBe(1);
        numberMatches[0].ShouldBe(new object[] { "hello", 100 });
    }
}