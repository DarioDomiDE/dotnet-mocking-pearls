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
    private HttpClient BuildHttpClient(HttpContent content)
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
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        httpClient.BaseAddress = new Uri("https://www.wharever-it-wont_be.called/");
        return httpClient;
    }

    [Fact]
    public async Task ThenClientGetAsync_ShouldUseHttpClient()
    {
        // Arrange
        var message = "this is the response";
        var httpContent = new StringContent(message);
        var httpClient = BuildHttpClient(httpContent);
        var serviceInfoClient = new MyService(httpClient);

        // Act
        var response = await serviceInfoClient.RunRequest().ConfigureAwait(false);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(message, response);
    }
}