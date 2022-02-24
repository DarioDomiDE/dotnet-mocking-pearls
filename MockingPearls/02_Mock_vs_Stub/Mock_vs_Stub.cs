using Moq;
using Xunit;

namespace MockingPearls._02_Mock_vs_Stub;

public interface IAnotherService
{
    string AnyMethod();
}

public class AnotherService : IAnotherService
{
    public string AnyMethod()
    {
        return "Hello Universe";
    }
}

public class MainService
{
    private readonly IAnotherService _service;

    public MainService(IAnotherService service)
    {
        _service = service;
    }

    public string AnyMethod()
    {
        return _service.AnyMethod();
    }
}

public class HowToDummy
{
    [Fact]
    public void RunMyController_ShouldWhatever()
    {
        // Arrange
        var message = "Hello Team";
        // -----
        var mock = new Mock<IAnotherService>();
        // -----
        var stub = new Mock<IAnotherService>();
        stub.Setup(x => x.AnyMethod()).Returns(message);
        // -----
        //var dummy = null;
        // -----
        //var fake = null;
        // -----
        //var realClass = new AnotherService();
        var controller = new MainService(mock.Object);

        // Act 
        var result = controller.AnyMethod();

        // Assert
        //Assert.NotNull(result);
    }
}