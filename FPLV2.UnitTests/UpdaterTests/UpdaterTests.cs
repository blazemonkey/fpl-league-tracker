using FPLV2.UnitTests.Models;
using FPLV2.Updater;
using FPLV2.Updater.Api;
using FPLV2.Updater.Functions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;

namespace FPLV2.UnitTests.UpdaterTests;

/// <summary>
/// Tests for the Updater Azure Function
/// </summary>
public abstract class UpdaterTests : UnitTests
{
    /// <summary>
    /// Gets the request url to be matched against
    /// </summary>
    protected abstract string RequestUrl { get; }

    /// <summary>
    /// Gets the Sample Data json file name
    /// </summary>
    protected abstract string ResourceName { get; }

    /// <summary>
    /// Executes the API of the Updater Function
    /// </summary>
    /// <typeparam name="T">The generic type which is a subclass of BaseApi</typeparam>
    /// <param name="parameters">Parameters that are used to mock the HttpMessageHandler responses</param>
    /// <returns>A task</returns>
    protected async Task<bool> ExecuteApi<T>(params MockHttpParameter[] parameters) where T : BaseApi
    {
        var mockHttpClientFactory = GetMockHttpClientFactory(parameters);
        var httpClient = mockHttpClientFactory.Object.CreateClient();
        var fplClient = new FplClient(httpClient);

        var api = (T)Activator.CreateInstance(typeof(T), fplClient, UnitOfWork);
        var success = await api.Execute();
        return success;
    }

    /// <summary>
    /// Get the JSON used for the live data tests
    /// </summary>
    /// <returns></returns>
    protected string GetLiveDataJson()
    {
        var json = EmbeddedResourceHelper.GetResourceFromJson(ResourceName);
        return json;
    }

    protected async Task ExecUpdaterFunction(params MockHttpParameter[] parameters)
    {
        var mockHttpClientFactory = GetMockHttpClientFactory(parameters);

        var httpClient = mockHttpClientFactory.Object.CreateClient();
        var fplClient = new FplClient(httpClient);
        var func = new UpdateFunction(new NullLoggerFactory(), Configuration, fplClient, UnitOfWork);

        await func.Run(new Function.FunctionInfo());
    }

    private Mock<IHttpClientFactory> GetMockHttpClientFactory(MockHttpParameter[] parameters)
    {
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var baseUrl = Configuration["FplBaseUrl"];
        foreach (var mhp in parameters ?? new MockHttpParameter[] { })
        {
            var result = Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = mhp.StatusCode,
                Content = new StringContent(mhp.ResponseContent)
            });

            if (string.IsNullOrEmpty(mhp.RequestUrl))
                mhp.RequestUrl = RequestUrl; // there must always be a request url, if it isn't set explicity, use the abstract one for the calling api test class

            var protectedMock = handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.OriginalString.StartsWith($"{baseUrl}{mhp.RequestUrl}")), ItExpr.IsAny<CancellationToken>());
            protectedMock.Returns(() => result).Verifiable();
        }

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return mockHttpClientFactory;
    }

    /// <summary>
    /// This is required because mocking multiple http response messages with the same request url means the second call always returns an empty string, due to the stream pointer being at the end
    /// </summary>
    /// <param name="count">Number of times to mock</param>
    /// <returns>Array of MockHttpParameter</returns>
    protected MockHttpParameter[] GetMockEmptyElementStats(int count)
    {
        var mocks = new List<MockHttpParameter>();
        for (var i = 1; i <= count; i++)
        {
            var mock = new Models.MockHttpParameter() { RequestUrl = $"event/{i}", StatusCode = System.Net.HttpStatusCode.OK, ResponseContent = "{}" }; // empty because we don't care about the result here
            mocks.Add(mock);
        }

        return mocks.ToArray();
    }

    /// <summary>
    /// Assert the Player model and the API model for the Player is correct
    /// </summary>
    /// <param name="dbPlayer">Player model stored in Database</param>
    /// <param name="player">Player model returned from API</param>
    /// <param name="leagueId">Id of the league</param>
    protected static void AssertPlayer(Database.Models.Player dbPlayer, Updater.Models.Player player, int leagueId)
    {
        Assert.AreEqual(player.Entry, dbPlayer.EntryId);
        Assert.AreEqual(player.PlayerName, dbPlayer.PlayerName);
        Assert.AreEqual(player.EntryName, dbPlayer.TeamName);
        Assert.AreEqual(leagueId, dbPlayer.LeagueId);
    }
}
