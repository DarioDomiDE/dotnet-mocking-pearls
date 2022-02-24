using Moq;
using Xunit;

namespace MockingPearls._01_NSubstitute_vs_Moq;

public interface ISomeService
{
    string AnyMethod();
}

public class MainService
{
    private readonly ISomeService _service;

    public MainService(ISomeService service)
    {
        _service = service;
    }

    public string AnyMethod(int userId)
    {
        var result = _service.AnyMethod();
        result += " is super";
        return result;
    }
}

public class HowToMoq
{
    [Fact]
    public void RunMyController_ShouldWhatever()
    {
        // Arrange
        var myService = new Mock<ISomeService>();
        var message = "Hello Team";
        myService.Setup(x => x.AnyMethod()).Returns(message);
        var service = new MainService(myService.Object);

        // Act 
        var result = service.AnyMethod(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(message + " is super", result);
    }
}
