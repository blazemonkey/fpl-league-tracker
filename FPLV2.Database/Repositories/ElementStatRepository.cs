using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Xml.Linq;

namespace FPLV2.Database.Repositories;

public class ElementStatRepository : BaseRepository, IElementStatRepository
{
    public ElementStatRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<ElementStat[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select es.* from elements_stats es join elements e on es.ElementId = e.Id join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<ElementStat>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task<ElementStat[]> GetAllBySeasonId(int seasonId, int gameweek)
    {
        var sql = "select es.* from elements_stats es join elements e on es.ElementId = e.Id join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId and es.gameweek = @Gameweek";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<ElementStat>(sql, new { SeasonId = seasonId, Gameweek = gameweek });
        return result.ToArray();
    }

    public async Task<int> GetLatestGameweekBySeasonId(int seasonId)
    {
        var sql = "select max(es.gameweek) from elements_stats es join elements e on es.ElementId = e.Id join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QuerySingleOrDefaultAsync<int?>(sql, new { SeasonId = seasonId }) ?? 1; // gameweek 1 is by default the first one we want
        return result;
    }
    public async Task<bool> DeleteById(int id, SqlConnection conn = null)
    {
        var sql = "delete from elements_stats where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, new { Id = id });
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result > 0;
    }
    public async Task DeleteAll()
    {
        var sql = "delete from elements_stats";
        using var conn = await OpenConnection();
        await conn.ExecuteAsync(sql);
    }

    public async Task<int> Insert(ElementStat elementStat, SqlConnection conn = null)
    {
        var sql = "insert into elements_stats (elementid, gameweek, minutes, goalsscored, assists, cleansheets, goalsconceded, owngoals, penaltiessaved, penaltiesmissed, yellowcards, redcards, saves, bonus, bps, influence, creativity, threat, ictindex, totalpoints, indreamteam, apielementid) output inserted.id values (@elementid, @gameweek, @minutes, @goalsscored, @assists, @cleansheets, @goalsconceded, @owngoals, @penaltiessaved, @penaltiesmissed, @yellowcards, @redcards, @saves, @bonus, @bps, @influence, @creativity, @threat, @ictindex, @totalpoints, @indreamteam, @apielementid)";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteScalarAsync<int>(sql, elementStat);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        return result;
    }

    public async Task<bool> Update(ElementStat elementStat, SqlConnection conn = null)
    {
        var sql = "update elements_stats set elementid = @ElementId, gameweek = @Gameweek, minutes = @Minutes, goalsscored = @GoalsScored, assists = @Assists, cleansheets = @CleanSheets, goalsconceded = @GoalsConceded, owngoals = @OwnGoals, penaltiessaved = @PenaltiesSaved, penaltiesmissed = @PenaltiesMissed, yellowcards = @YellowCards, redcards = @RedCards, saves = @Saves, bonus = @Bonus, bps = @Bps, influence = @Influence, creativity = @Creativity, threat = @Threat, ictindex = @IctIndex, totalpoints = @TotalPoints, indreamteam = @InDreamteam, apielementid = @ApiElementId where id = @Id";
        var sqlConnection = conn ?? await OpenConnection();
        var result = await sqlConnection.ExecuteAsync(sql, elementStat);
        if (conn == null)
            await sqlConnection.DisposeAsync();
        var success = result > 0;
        return success;
    }

    public async Task<bool> ReplaceElementStatsBySeasonId(ElementStat[] elementStats, int seasonId, int gameweek)
    {
        var existItems = await GetAllBySeasonId(seasonId, gameweek);
        var newItems = elementStats.Select(x => new { x.ElementId, x.Gameweek }).Except(existItems.Select(x => new { x.ElementId, x.Gameweek }));
        var oldItems = existItems.Select(x => new { x.ElementId, x.Gameweek }).Except(elementStats.Select(x => new { x.ElementId, x.Gameweek }));
        var sameItems = existItems.Select(x => new { x.ElementId, x.Gameweek }).Intersect(elementStats.Select(x => new { x.ElementId, x.Gameweek }));

        using var conn = await OpenConnection();

        foreach (var i in newItems)
        {
            var pick = elementStats.FirstOrDefault(x => x.ElementId == i.ElementId && x.Gameweek == i.Gameweek);
            if (pick == null) continue;

            var result = await Insert(pick, conn);
            if (result <= 0)
                return false;
        }

        foreach (var i in oldItems)
        {
            var pick = existItems.FirstOrDefault(x => x.ElementId == i.ElementId && x.Gameweek == i.Gameweek);
            if (pick == null) continue;

            var result = await DeleteById(pick.Id, conn);
            if (result == false)
                return false;
        }

        foreach (var i in sameItems)
        {
            var newElementStats = elementStats.FirstOrDefault(x => x.ElementId == i.ElementId && x.Gameweek == i.Gameweek);
            if (newElementStats == null) continue;
            var oldElementStats = existItems.FirstOrDefault(x => x.ElementId == i.ElementId && x.Gameweek == i.Gameweek);
            if (oldElementStats == null) continue;

            newElementStats.Id = oldElementStats.Id;
            var hasChanged = JsonSerializer.Serialize(oldElementStats) != JsonSerializer.Serialize(newElementStats);
            if (hasChanged == false) continue;

            var result = await Update(newElementStats, conn);
            if (result == false)
                return false;
        }

        return true;
    }
}
