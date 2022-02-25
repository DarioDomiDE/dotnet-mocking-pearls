using Moq;
using Xunit;

namespace MockingPearls._02_Mock_vs_Stub;

/*** Mock vs Stub (vs Fake vs Spy vs Dummy) ***/


public interface IUserService
{
    string GetUser(string value);
}

public class UserService : IUserService
{
    public string GetUser(string value)
    {
        return value + "Hello Universe";
    }
}

public class MainService
{
    private readonly IUserService _service;

    public MainService(IUserService service)
    {
        _service = service;
    }

    public string GetUser(string value)
    {
        return _service.GetUser(value);
    }
}

public class HowToDummy
{
    private class FakeUserService : IUserService
    {
        public string GetUser(string value)
        {
            return "Custom Implementation";
        }
    }

    [Fact]
    public void RunMyController_ShouldNotBeNull()
    {
        // Arrange
        var message = "Hello Team";
        // -----
        // 1) Mock
        // It defines an expectation of how it will be used
        // Main task is to validate/assert correct output based on input
        // It will cause a failure if the expectation isn’t met.
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetUser(It.IsAny<string>())).Returns(message);
        // -----
        // 2) Stub
        // Is an object that provides fake data.
        // A stub is an implementation that behaves "unnaturally"
        // It can be an own class as well.
        var stub = new Mock<IUserService>();
        stub.Setup(x => x.GetUser(It.IsAny<string>())).Throws<Exception>();
        // -----
        // 3) Spy
        // It records information about how the class is being used.
        // It can be an own class as well.
        var spy = new Mock<IUserService>();
        var data = new List<string>();
        spy.Setup(h => h.GetUser(It.IsAny<string>()))
            .Callback<string>(x => data.Add(x))
            .Returns(message);
        // -----
        // 4) Dummy
        // It is used as a placeholder for a parameter.
        var dummy = new Mock<IUserService>();
        // -----
        // 5) Fake
        // an actual object with limited capabilities
        // e.g. a fake web service
        var fake = new FakeUserService();
        // -----
        // 6) Real Implementation
        var realClass = new UserService();
        // -----
        var controller = new MainService(spy.Object);

        // Act 
        var result = controller.GetUser("test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(message, result);
        Assert.Equal(1, data.Count);
        Assert.Equal("test", data[0]);
    }
}