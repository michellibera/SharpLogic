using Shouldly;
using SharpLogic.Core;
using static SharpLogic.Core.Globals;
using Xunit;

namespace SharpLogic.Tests;

public class ExtensionTests
{
    [Fact]
    public void Add_ShouldReturnFactTypeForChaining()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        var result = parent.Add("john", "mary");
        
        // Assert
        result.ShouldBeSameAs(parent); // Same object reference
        parent.Exists("john", "mary").ShouldBeTrue();
    }
    
    [Fact]
    public void Add_WithChaining_ShouldAddMultipleFacts()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        parent.Add("john", "mary")
              .Add("mary", "susan")
              .Add("bob", "alice");
        
        // Assert
        parent.Count(_, _).ShouldBe(3);
        parent.Exists("john", "mary").ShouldBeTrue();
        parent.Exists("mary", "susan").ShouldBeTrue();
        parent.Exists("bob", "alice").ShouldBeTrue();
    }
    
    [Fact]
    public void Exists_WithExistingFact_ShouldReturnTrue()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Invoke("john", "mary");
        
        // Act & Assert
        parent.Exists("john", "mary").ShouldBeTrue();
        parent.Exists("mary", "john").ShouldBeFalse(); // Different order
    }
    
    [Fact]
    public void Count_WithPattern_ShouldReturnMatchCount()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Add("john", "mary")
              .Add("mary", "susan")
              .Add("bob", "mary");
        
        // Act & Assert
        parent.Count(_, _).ShouldBe(3);           // All facts
        parent.Count(_, "mary").ShouldBe(2);      // Children of mary
        parent.Count("john", _).ShouldBe(1);      // Parents of john
        parent.Count("nonexistent", _).ShouldBe(0); // No matches
    }
    
    [Fact]
    public void AddMany_ShouldAddMultipleFactsAtOnce()
    {
        // Arrange
        var parent = new FactType("parent");
        
        // Act
        parent.AddMany(
            new object[] { "john", "mary" },
            new object[] { "mary", "susan" },
            new object[] { "bob", "alice" }
        );
        
        // Assert
        parent.Count(_, _).ShouldBe(3);
        parent.Exists("john", "mary").ShouldBeTrue();
        parent.Exists("mary", "susan").ShouldBeTrue();
        parent.Exists("bob", "alice").ShouldBeTrue();
    }
    
    [Fact]
    public void Query_ExtensionMethod_ShouldReturnFactQuery()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Add("john", "mary");
        
        // Act
        var query = parent.Query(_, "mary");
        
        // Assert
        query.ShouldBeOfType<FactType.FactQuery>();
        var results = query.ToList();
        results.Count.ShouldBe(1);
        results[0].ShouldBe(new object[] { "john", "mary" });
    }
    
    [Fact]
    public void Any_WithMatches_ShouldReturnTrue()
    {
        // Arrange
        var parent = new FactType("parent");
        parent.Add("john", "mary");
        
        // Act & Assert
        parent.Any(_, "mary").ShouldBeTrue();      // Has parents of mary
        parent.Any("john", _).ShouldBeTrue();      // John has children
        parent.Any(_, "nonexistent").ShouldBeFalse(); // No parents of nonexistent
    }
    
    [Fact]
    public void Var_ShouldCreateVariableWithName()
    {
        // Act
        var nameVar = Extensions.Var<string>("Name");
        var ageVar = Extensions.Var<int>("Age");
        
        // Assert
        nameVar.Name.ShouldBe("Name");
        nameVar.Type.ShouldBe(typeof(string));
        
        ageVar.Name.ShouldBe("Age");
        ageVar.Type.ShouldBe(typeof(int));
    }
    
    [Fact]
    public void Var_WithoutName_ShouldGenerateUniqueName()
    {
        // Act
        var var1 = Extensions.Var<string>();
        var var2 = Extensions.Var<string>();
        
        // Assert
        var1.Name.ShouldNotBe(var2.Name); // Should be unique
        var1.Name.ShouldStartWith("_");    // Should start with underscore
        var1.Type.ShouldBe(typeof(string));
    }
}