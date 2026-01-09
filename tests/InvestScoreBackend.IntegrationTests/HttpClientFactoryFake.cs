using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace InvestScoreBackend.IntegrationTests.Fakes
{
    public class HttpClientFactoryFake : IHttpClientFactory
    {
        public HttpClient CreateClient(string name = "")
        {
            var handler = new FakeHttpMessageHandler();
            return new HttpClient(handler);
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var fakeResponse = new
            {
                choices = new[]
                {
                    new {
                        message = new {
                            content = JsonSerializer.Serialize(new {
                                assets = new[]
                                {
                                    new { ticker = "AAPL", roe = 28.4, dy = 0.6, cagr = 12.5, liquidez = 75000000, risco = "Baixo", score = 14.5 },
                                    new { ticker = "MSFT", roe = 35.1, dy = 0.8, cagr = 13.2, liquidez = 68000000, risco = "Baixo", score = 17.95 },
                                    new { ticker = "TSLA", roe = 15.7, dy = 0.0, cagr = 25.4, liquidez = 62000000, risco = "Médio", score = 7.85 }
                                }
                            })
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(fakeResponse);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }
}
