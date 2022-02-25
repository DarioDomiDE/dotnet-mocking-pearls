using Moq;
using Xunit;

namespace MockingPearls._07_Delegate_vs_Mocking;

public interface IProcessStatusSender
{
    void SendAsync(string url, string connectionName, object data, int timeout);
}

public class SendService
{
    private readonly IProcessStatusSender _sender;

    public SendService(IProcessStatusSender sender)
    {
        _sender = sender;
    }

    public void SendMessage(object data)
    {
        // do any business logic
        // ...
        var url = "http..";
        var connectionName = "updates";
        var timeout = 1000;
        _sender.SendAsync(url, connectionName, data, timeout);
    }
}

public class SignalRTestWithMocking {

    [Fact]
    public void SendMessages_ShouldSendExpectedData()
    {
        // Arrange
        var sender = new Mock<IProcessStatusSender>();
        var service = new SendService(sender.Object);
        object data = 42;

        // Act
        service.SendMessage(data);

        // Assert
        sender.Verify(x =>
            x.SendAsync(
                It.Is<string>(x => x == "http.."),
                It.Is<string>(x => x == "updates"),
                It.Is<object>(x => x == data),
                It.IsAny<int>()
            ));
    }
}


/*** Problem so far: A lot of mocking ***/

public delegate void StatusSender(object data);

public class SendService2
{
    public SendService2()
    {
    }

    public void SendMessage(object data, StatusSender sender)
    {
        // do any business logic
        // ...
        sender.Invoke(data);
    }
}

public class AnyControllerOrService
{
    private readonly SendService2 _sendService2;
    private readonly IProcessStatusSender _sender;

    public AnyControllerOrService(
        SendService2 sendService2,
        IProcessStatusSender sender)
    {
        _sendService2 = sendService2;
        _sender = sender;
    }

    public void AnyMethod()
    {
        var data = 42;
        var url = "http..";
        var connectionName = "updates";
        var timeout = 1000;
        StatusSender sender = data => _sender.SendAsync(url, connectionName, data, timeout);
        _sendService2.SendMessage(data, sender);
    }
}

public class SignalRTestWithDelegate
{

    [Fact]
    public void SendMessages_ShouldSendExpectedData()
    {
        // Arrange
        var service = new SendService2();
        object result = null;
        StatusSender sender = data => result = data;

        // Act
        service.SendMessage(42, sender);

        // Assert
        Assert.Equal(42, result);
    }
}


