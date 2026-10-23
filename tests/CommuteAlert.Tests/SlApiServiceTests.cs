using CommuteAlert.Web.Sl;
using System.Net;

namespace CommuteAlert.Tests;

public class SlApiServiceTests
{
    [Fact] //T-14
    public async Task GetDeparturesAsync_calls_correct_url_and_reads_response()
    {
        var json = File.ReadAllText("TestData/departures.json");
        HttpRequestMessage? sent = null;
        var handler = new FakeHttpMessageHandler(request =>
        {
            sent = request;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
        });
        var service = new SlApiService(new HttpClient(handler));

        var result = await service.GetDeparturesAsync(1234);

        Assert.Equal("https://transport.integration.sl.se/v1/sites/1234/departures",
            sent!.RequestUri!.ToString());
        Assert.NotEmpty(result);
    }
}
