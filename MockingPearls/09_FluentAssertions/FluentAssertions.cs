using FluentAssertions;
using Xunit;

namespace MockingPearls._09_FluentAssertions;

/*** How to Test Exceptions ***/

public class MyService
{
    public void Run()
    {
        throw new ArgumentException("failed to run");
    }
}

public class ExceptionTest
{
    [Fact]
    public void RunService_ShouldThrowException()
    {
        // Arrange
        var service = new MyService();

        // Act
        var act = () => service.Run();

        // Assert
        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("failed to run", ex.Message);
    }
}

public class ExceptionTestWithWithFluentAssertions
{
    [Fact]
    public void RunService_ShouldThrowException()
    {
        // Arrange
        var service = new MyService();

        // Act
        var act = () => service.Run();

        // Assert
        act
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("failed to run");
    }
}




/*** How to Test a String ***/



public class MyService2
{

    public string GetData()
    {
        return "ABCDEFGHI";
    }
}

public class GetDataTest
{
    [Fact]
    public void RunService_ShouldReturnExpectedString()
    {
        // Arrange
        var service = new MyService2();

        // Act
        var result = service.GetData();

        // Assert
        Assert.Equal("ABCDEFGHI", result);
        Assert.NotEqual("ABC", result);
        Assert.StartsWith("AB", result);
        Assert.EndsWith("HI", result);
        Assert.Contains("EF", result);
        Assert.Equal(9, result.Length);
    }
}

public class GetDataTest_WithFluentAssertions
{
    [Fact]
    public void RunService_ShouldThrowException()
    {
        // Arrange
        var service = new MyService2();

        // Act
        var result = service.GetData();

        // Assert
        result.Should().Be("ABCDEFGHI", result);
        result.Should().NotBe("ABC", result);
        result.Should()
              .StartWith("AB")
              .And
              .EndWith("HI")
              .And
              .Contain("EF")
              .And
              .HaveLength(9);
    }
}