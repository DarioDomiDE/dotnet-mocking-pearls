using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MockingPearls._06_Logging;

public class MyController : ControllerBase
{
    private readonly ILogger<MyController> _logger;

    public MyController(ILogger<MyController> logger)
    {
        _logger = logger;
    }

    [HttpPut]
    public ActionResult UpdateUser(int userId)
    {
        _logger.LogInformation($"create user {userId} failed");
        // ...
        return Ok("successful");
    }
}



public class HowToTestLoggingTheHardWay
{
    [Fact]
    public void RunUpdateUser_ShouldLogFailureLikeExpected()
    {
        // Arrange
        var logger = new Mock<ILogger<MyController>>();
        var controller = new MyController(logger.Object);
        var expectedLogMessage = "create user 42 failed";
        var expectedLogLevel = LogLevel.Information;

        // Act 
        controller.UpdateUser(42);

        // Assert
        Func<object, Type, bool> state = (v, t) => v.ToString() == expectedLogMessage;
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!));
    }
}

/*** Problem so far: WTF-code and hard to test ***/

// Possibilities - across Concern
//    fire Events for Logging
//    put Wrapper to Logger


public interface ILogWrapper<T>
{
    void LogInformation(string message);
    void LogCritical(string message);
    void Log(LogLevel level, string message);
    // ...
    void LogCreateUserFailed(int userId);
}

public class LogWrapper<T> : ILogWrapper<T> where T : ControllerBase
{
    private readonly ILogger<T> _logger;

    public LogWrapper(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    public void LogCritical(string message)
    {
        _logger.LogCritical(message);
    }

    public void Log(LogLevel level, string message)
    {
        _logger.Log(level, message);
    }

    // ...

    public void LogCreateUserFailed(int userId)
    {
        _logger.LogInformation($"create user {userId} failed");
    }
}

public class MyController2 : ControllerBase
{
    private readonly ILogWrapper<MyController2> _logger;

    public MyController2(ILogWrapper<MyController2> logger)
    {
        _logger = logger;
    }

    [HttpPut]
    public ActionResult UpdateUser(int userId)
    {
        _logger.LogInformation($"create user {userId} failed");
        _logger.LogCreateUserFailed(userId);
        // ...
        return Ok("successful");
    }
}

public class HowToTestLoggingTheEasyWay
{
    [Fact]
    public void RunUpdateUser_ShouldLogFailureLikeExpected()
    {
        // Arrange
        var logWrapper = new Mock<ILogWrapper<MyController2>>();
        var controller = new MyController2(logWrapper.Object);
        var expectedLogMessage = "create user 42 failed";
        var userId = 42;

        // Act 
        controller.UpdateUser(userId);

        // Assert
        logWrapper.Verify(x =>
            x.LogInformation(
                It.Is<string>(x => x == expectedLogMessage)
            ), Times.Once);

        logWrapper.Verify(x =>
            x.LogCreateUserFailed(
                It.Is<int>(x => x == userId)
            ), Times.Once);
    }
}
