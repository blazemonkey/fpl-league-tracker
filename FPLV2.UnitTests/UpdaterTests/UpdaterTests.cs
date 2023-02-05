using FPLV2.UnitTests.Models;
using FPLV2.Updater;
using FPLV2.Updater.Functions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;

namespace FPLV2.UnitTests.UpdaterTests;

public class UpdaterTests : UnitTests
{
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
            var result = new HttpResponseMessage()
            {
                StatusCode = mhp.StatusCode,
                Content = new StringContent(mhp.ResponseContent)
            };

            var protectedMock = handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.OriginalString.StartsWith($"{baseUrl}{mhp.RequestUrl}")), ItExpr.IsAny<CancellationToken>());
            protectedMock.ReturnsAsync(result).Verifiable();
        }

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri(baseUrl)
        };
        mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return mockHttpClientFactory;
    }

    protected void AssertTeam(Database.Models.Team dbTeam, Updater.Models.Team team, int seasonId)
    {
        Assert.AreEqual(team.Id, dbTeam.TeamId);
        Assert.AreEqual(team.Name, dbTeam.Name);
        Assert.AreEqual(team.ShortName, dbTeam.ShortName);
        Assert.AreEqual(team.Code, dbTeam.Code);
        Assert.AreEqual(seasonId, dbTeam.SeasonId);
    }

    protected void AssertElement(Database.Models.Element dbElement, Updater.Models.Element element, int teamId)
    {
        Assert.AreEqual(element.FirstName, dbElement.FirstName);
        Assert.AreEqual(element.SecondName, dbElement.SecondName);
        Assert.AreEqual(element.WebName, dbElement.WebName);
        Assert.AreEqual(element.Code, dbElement.Code);
        Assert.AreEqual(element.ElementType, dbElement.ElementType);
        Assert.AreEqual(teamId, dbElement.TeamId);
        Assert.AreEqual(element.Team, dbElement.ElementTeamId);
    }

    protected void AssertPlayer(Database.Models.Player dbPlayer, Updater.Models.Player player, int leagueId)
    {
        Assert.AreEqual(player.Entry, dbPlayer.EntryId);
        Assert.AreEqual(player.PlayerName, dbPlayer.PlayerName);
        Assert.AreEqual(player.EntryName, dbPlayer.TeamName);
        Assert.AreEqual(leagueId, dbPlayer.LeagueId);
    }
}
