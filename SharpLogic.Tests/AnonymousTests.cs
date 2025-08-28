using Shouldly;
using SharpLogic.Core;
using Xunit;

namespace SharpLogic.Tests;

public class AnonymousTests
{
    [Fact]
    public void Anonymous_ShouldHaveConsistentValue()
    {
        // Act
        var anonymous1 = Anonymous._;
        var anonymous2 = Anonymous._;
        
        // Assert
        anonymous1.ShouldBe(anonymous2);
    }
    
    [Fact]
    public void Anonymous_Equality_ShouldAlwaysBeTrue()
    {
        // Arrange
        var anon1 = Anonymous._;
        var anon2 = new Anonymous();
        
        // Assert  
        anon1.ShouldBe(anon2);
        (anon1 == anon2).ShouldBeTrue();
        (anon1 != anon2).ShouldBeFalse();
    }
    
    [Fact]
    public void Anonymous_ToString_ShouldReturnUnderscore()
    {
        // Act & Assert
        Anonymous._.ToString().ShouldBe("_");
    }
    
    [Fact]
    public void Anonymous_GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var anon1 = Anonymous._;
        var anon2 = new Anonymous();
        
        // Act
        var hash1 = anon1.GetHashCode();
        var hash2 = anon2.GetHashCode();
        
        // Assert
        hash1.ShouldBe(hash2);
    }
    
    [Fact]
    public void Globals_ShouldProvideAccessToAnonymous()
    {
        // Act
        var globalAnonymous = Globals._;
        
        // Assert
        globalAnonymous.ShouldBe(Anonymous._);
    }
}