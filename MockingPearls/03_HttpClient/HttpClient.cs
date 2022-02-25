using System.Net;
using Moq;
using Moq.Protected;
using Xunit;

namespace MockingPearls._03_HttpClient;

class MyService
{
    private readonly HttpClient _httpClient;

    public MyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RunRequest()
    {
        var response = await _httpClient.GetAsync("http://google.de/");
        return await response.Content.ReadAsStringAsync();
    }
}


/*** Problem: How to test? ***/

public class HttpClientTest
{
    private Mock<HttpMessageHandler> BuildHttpClient(HttpContent content)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = content
        };
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return mockHttpMessageHandler;
    }

    [Fact]
    public async Task ThenClientGetAsync_ShouldUseHttpClient()
    {
        // Arrange
        var message = "this is the response";
        var httpContent = new StringContent(message);
        var handler = BuildHttpClient(httpContent);
        using var httpClient = new HttpClient(handler.Object);
        httpClient.BaseAddress = new Uri("https://www.wharever-it-wont_be.called/");

        var serviceInfoClient = new MyService(httpClient);

        // Act
        var response = await serviceInfoClient.RunRequest().ConfigureAwait(false);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(message, response);

        // still Disadvantage: that HttpClient will not be disposed correctly
    }
}