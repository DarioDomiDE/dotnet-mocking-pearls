using Moq;
using Xunit;

namespace MockingPearls._04_ExtensionMethod;

public class MyService
{
    public void AnyMethod(string s)
    {
        s.LogData();
    }
}

public static class StringExtensions
{
    public static void LogData(this string s)
    {
        // do some logic
    }
}


/*** Problem so far: Not testable at all ***/




public class MyService2
{
    public void AnyMethod(string s)
    {
        s.LogData2();
    }
}

public interface ICustomLogger
{
    void LogString(string s);
}

public class CustomLogger : ICustomLogger
{
    public void LogString(string s)
    {
        // do some logic
    }
}

public static class IntExtensions
{
    private static readonly ICustomLogger _defaultImplementation = new CustomLogger();
    public static ICustomLogger Implementation = _defaultImplementation;

    public static void RevertToDefaultImplementation()
    {
        Implementation = _defaultImplementation;
    }

    public static void LogData2(this string s)
    {
        Implementation.LogString(s);
    }
}

public class TestPunctuation
{
    [Fact]
    public void Test()
    {
        // Arrage
        var input = "Test Any String";
        var logger = new Mock<ICustomLogger>();
        IntExtensions.Implementation = logger.Object;
        var service = new MyService2();

        // Act
        service.AnyMethod(input);

        // Assert
        logger.Verify(x => x.LogString(It.Is<string>(x => x == input)), Times.Once());
        IntExtensions.RevertToDefaultImplementation();
    }
}