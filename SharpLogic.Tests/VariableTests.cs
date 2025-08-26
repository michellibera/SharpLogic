using Shouldly;
using SharpLogic.Core;
using Xunit;

namespace SharpLogic.Tests;

public class VariableTests
{
    [Fact]
    public void Variable_WithName_ShouldStoreNameAndType()
    {
        // Act
        var stringVar = new Variable<string>("Name");
        var intVar = new Variable<int>("Age");
        
        // Assert
        stringVar.Name.ShouldBe("Name");
        stringVar.Type.ShouldBe(typeof(string));
        
        intVar.Name.ShouldBe("Age");  
        intVar.Type.ShouldBe(typeof(int));
    }
    
    [Fact]
    public void Variable_WithoutName_ShouldGenerateUniqueName()
    {
        // Act
        var var1 = new Variable<string>();
        var var2 = new Variable<string>();
        
        // Assert
        var1.Name.ShouldNotBeNull();
        var2.Name.ShouldNotBeNull();
        var1.Name.ShouldNotBe(var2.Name); // Should be unique
        var1.Name.ShouldStartWith("_");    // Generated names start with _
    }
    
    [Fact]
    public void Variable_Equality_ShouldBeBasedOnIdentity()
    {
        // Arrange
        var var1 = new Variable<string>("X");
        var var2 = new Variable<string>("X"); // Same name, different instance
        var var3 = var1; // Same instance
        
        // Assert
        var1.ShouldNotBe(var2);  // Different instances, even with same name
        var1.ShouldBe(var3);     // Same instance
        
        (var1 == var2).ShouldBeFalse();
        (var1 == var3).ShouldBeTrue();
        (var1 != var2).ShouldBeTrue();
    }
    
    [Fact]
    public void Variable_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var var1 = new Variable<string>("X");
        var var2 = var1;
        
        // Act
        var hash1 = var1.GetHashCode();
        var hash2 = var2.GetHashCode();
        
        // Assert
        hash1.ShouldBe(hash2); // Same instance should have same hash code
    }
    
    [Fact]
    public void Variable_ToString_ShouldShowNameAndType()
    {
        // Arrange
        var stringVar = new Variable<string>("Name");
        var intVar = new Variable<int>("Age");
        
        // Act & Assert
        stringVar.ToString().ShouldBe("var Name: String");
        intVar.ToString().ShouldBe("var Age: Int32");
    }
}