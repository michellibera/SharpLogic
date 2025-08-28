using Shouldly;
using SharpLogic.Core;
using static SharpLogic.Core.Globals;
using Xunit;

namespace SharpLogic.Tests;

public class FactTypeTests
{
    [Fact]
    public void FactType_ShouldHaveCorrectName()
    {
        // Arrange & Act
        var parent = new FactType("parent");
        
        // Assert
        parent.Name.ShouldBe("parent");
        parent.Arity.ShouldBeNull();
    }
    
    [Fact]
    public void Invoke_WithConcreteValues_ShouldSetArityAndReturnFactType()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        var result = parent.Invoke("john", "mary");
        
        // Assert
        parent.Arity.ShouldBe(2);
        object resultObj = result;
        resultObj.ShouldBeOfType<FactType>();
        
        FactType resultFactType = result;
        resultFactType.ShouldBeSameAs(parent);
    }
    
    [Fact]
    public void Invoke_DuplicateFact_ShouldPreventDuplicates()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        parent.Invoke("john", "mary");
        parent.Invoke("john", "mary");
        parent.Invoke("mary", "susan");
        
        // Assert - Should have only 2 unique facts, not 3
        parent.Count(_, _).ShouldBe(2);
        parent.Exists("john", "mary").ShouldBeTrue();
        parent.Exists("mary", "susan").ShouldBeTrue();
    }
    
    [Fact]
    public void Invoke_ShouldSupportMethodChaining()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        var result = parent.Invoke("john", "mary")
                          .Invoke("mary", "susan")
                          .Invoke("bob", "alice");
        
        // Assert
        FactType resultFactType = result;
        resultFactType.ShouldBeSameAs(parent);
        parent.Count(_, _).ShouldBe(3);
    }
    
    [Fact]
    public void Invoke_WithDifferentArity_ShouldThrowException()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary"); // Sets arity to 2
        
        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => 
            parent.Invoke("john", "mary", "biological"));
        
        exception.Message.ShouldContain("expects 2 arguments, but got 3");
        exception.Message.ShouldContain("parent");
    }
    
    [Fact]
    public void Invoke_WithAnonymousWildcard_ShouldReturnQuery()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary");
        parent.Invoke("mary", "susan");
        parent.Invoke("bob", "alice");
        
        // Act
        var queryResult = parent.Invoke(_, "mary");
        
        // Assert
        object queryObj = queryResult;
        queryObj.ShouldBeOfType<FactType.FactQuery>();
        
        FactType.FactQuery query = queryResult;
        var results = query.ToList();
        results.Count.ShouldBe(1);
        results[0].ShouldBe(new object[] { "john", "mary" }, "Should match the expected fact array");
    }
    
    [Fact]
    public void Query_WithMultipleWildcards_ShouldReturnAllFacts()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary");
        parent.Invoke("mary", "susan");
        
        // Act
        var queryResult = parent.Invoke(_, _);
        
        // Assert
        FactType.FactQuery query = queryResult;
        var results = query.ToList();
        results.Count.ShouldBe(2);
        results.ShouldContain(result => result.SequenceEqual(new object[] { "john", "mary" }));
        results.ShouldContain(result => result.SequenceEqual(new object[] { "mary", "susan" }));
    }
    
    [Fact]  
    public void Query_WithVariable_ShouldReturnMatches()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary");
        parent.Invoke("bob", "mary");
        
        var parentVar = new Variable<string>("Parent");
        
        // Act
        var queryResult = parent.Invoke(parentVar, "mary");
        
        // Assert
        FactType.FactQuery query = queryResult;
        var results = query.ToList();
        results.Count.ShouldBe(2);
        results.ShouldContain(result => result.SequenceEqual(new object[] { "john", "mary" }));
        results.ShouldContain(result => result.SequenceEqual(new object[] { "bob", "mary" }));
    }
    
    [Fact]
    public void Query_WithNoMatches_ShouldReturnEmpty()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary");
        
        // Act
        var queryResult = parent.Invoke(_, "nonexistent");
        
        // Assert
        FactType.FactQuery query = queryResult;
        var results = query.ToList();
        results.ShouldBeEmpty();
    }
}