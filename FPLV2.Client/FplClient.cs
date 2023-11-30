using FPLV2.Client.Models;
using System.Text.Json;

namespace FPLV2.Client;

public class FplClient
{
    private readonly HttpClient _httpClient;

    public FplClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BootstrapStatic> GetBootstrapStatic()
    {
        var response = await _httpClient.GetAsync("bootstrap-static/");
        var result = await ReadResponse<BootstrapStatic>(response);
        return result;
    }

    public async Task<Standings> GetLeagueStandings(int leagueId, bool includePlayers)
    {
        var players = new List<Player>();
        var result = await GetLeagueStandings(leagueId, 1, includePlayers ? players : null);
        return result;
    }

    private async Task<Standings> GetLeagueStandings(int leagueId, int page, List<Player> players)
    {
        var response = await _httpClient.GetAsync($"leagues-classic/{leagueId}/standings/?page_standings={page}");
        var result = await ReadResponse<Standings>(response);
        if (players == null)
            return result;

        players.AddRange(result?.Results?.Players ?? new Player[] { });
        if (result?.Results?.HasNext == true)
            await GetLeagueStandings(leagueId, ++page, players);

        if (result.Results == null)
            result.Results = new PlayerResults();

        result.Results.Players = players?.ToArray();
        return result;
    }

    public async Task<Pick[]> GetPicks(int entryId, int gameweek)
    {
        var response = await _httpClient.GetAsync($"entry/{entryId}/event/{gameweek}/picks/");
        var result = await ReadResponse<PickRoot>(response);
        return result.Picks;

    }

    public async Task<Entry> GetEntry(int entryId)
    {
        var response = await _httpClient.GetAsync($"entry/{entryId}/");
        var result = await ReadResponse<Entry>(response);
        return result;
    }

    public async Task<Points[]> GetPointsHistory(int entryId)
    {
        var response = await _httpClient.GetAsync($"entry/{entryId}/history/");
        var result = await ReadResponse<PointsRoot>(response);
        return result.Current;
    }

    public async Task<ElementStat[]> GetElementStats(int gameweek)
    {
        var response = await _httpClient.GetAsync($"event/{gameweek}/live/");
        var result = await ReadResponse<ElementStatRoot>(response);
        return result.Elements;
    }

    private async Task<T> ReadResponse<T>(HttpResponseMessage message)
    {
        message.EnsureSuccessStatusCode();

        var content = await message.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<T>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        return result;
    }
}
