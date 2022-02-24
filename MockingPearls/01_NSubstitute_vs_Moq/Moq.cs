using Moq;
using Xunit;

namespace MockingPearls._01_NSubstitute_vs_Moq;

public class HowToMoq
{
    [Fact]
    public void RunAnyMethod_ShouldReturnExpectedResult()
    {
        // Arrange
        var myService = new Mock<IUserService>();
        myService.Setup(x => x.GetUsername(It.Is<int>(x => x == 42))).Returns("Yoda");
        var service = new MainService(myService.Object);

        // Act 
        var result = service.BuildUser(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Yoda is super", result);
        myService.Verify(x => x.GetUsername(It.IsAny<int>()));
        myService.Verify(x => x.GetUsername(42));
        myService.Verify(x => x.GetUsername(It.Is<int>(x => x != 43)));
    }
}