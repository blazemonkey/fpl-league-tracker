using Dapper;
using FPLV2.Database.Models;
using FPLV2.Database.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace FPLV2.Database.Repositories;

public class ElementStatRepository : BaseRepository, IElementStatRepository
{
    public ElementStatRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<ElementStat[]> GetAllBySeasonId(int seasonId)
    {
        var sql = "select es.* from element_stats es join elements e on es.ApiElementId = e.Id join teams t on t.Id = e.TeamId join seasons s on s.Id = t.SeasonId where s.Id = @SeasonId";
        using var conn = await OpenConnection();
        var result = await conn.QueryAsync<ElementStat>(sql, new { SeasonId = seasonId });
        return result.ToArray();
    }

    public async Task<int> Insert(ElementStat elementStat)
    {
        var sql = "insert into element_stats (elementid, gameweek, minutes, goalsscored, assists, cleansheets, goalsconceded, owngoals, penaltiessaved, penaltiesmissed, yellowcards, redcards, saves, bonus, bps, influence, creativity, threat, ictindex, totalpoints, indreamteam, apielementid) output inserted.id values (@elementid, @gameweek, @minutes, @goalsscored, @assists, @cleansheets, @goalsconceded, @owngoals, @penaltiessaved, @penaltiesmissed, @yellowcards, @redcards, @saves, @bonus, @bps, @influence, @creativity, @threat, @ictindex, @totalpoints, @indreamteam, @apielementid)";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteScalarAsync<int>(sql, elementStat);
        return result;
    }

    public async Task<bool> Update(ElementStat elementStat)
    {
        var sql = "update element_stats set elementid = @ElementId, gameweek = @Gameweek, minutes = @Minutes, goalsscored = @GoalsScored, assists = @Assists, cleansheets = @CleanSheets, goalsconceded = @GoalsConceded, owngoals = @OwnGoals, penaltiessaved = @PenaltiesSaved, penaltiesmissed = @PenaltiesMissed, yellowcards = @YellowCards, redcards = @RedCards, saves = @Saves, bonus = @Bonus, bps = @Bps, influence = @Influence, creativity = @Creativity, threat = @Threat, ictindex = @IctIndex, totalpoints = @TotalPoints, indreamteam = @InDreamteam, apielementid = @ApiElementId where id = @Id";
        using var conn = await OpenConnection();
        var result = await conn.ExecuteAsync(sql, elementStat);
        var success = result > 0;
        return success;
    }
}
