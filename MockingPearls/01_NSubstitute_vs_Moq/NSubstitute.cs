using NSubstitute;
using Xunit;

namespace MockingPearls._01_NSubstitute_vs_Moq;

public class HowToNSubstitute
{
    // Slogan:
    // Mock, stub, fake, spy, test double? Strict or loose? Nah, just substitute for the type you need!
    [Fact]
    public void RunAnyMethod_ShouldReturnExpectedResult()
    {
        // Arrange
        var myService = Substitute.For<IUserService>();
        myService.GetUsername(42).Returns("Yoda");
        var service = new MainService(myService);

        // Act 
        var result = service.BuildUser(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Yoda is super", result);
        myService.Received().GetUsername(Arg.Any<int>());
        myService.Received().GetUsername(42);
        myService.DidNotReceive().GetUsername(43);
    }
}