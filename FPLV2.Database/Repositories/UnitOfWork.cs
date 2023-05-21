using FPLV2.Database.Repositories.Interfaces;

namespace FPLV2.Database.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IChartRepository charts, IElementRepository elements, IElementStatRepository elementStats, ILeagueRepository leagues, ILoggingRepository logs, IPickRepository picks, IPlayerRepository players, IPointsRepository points, ISeasonRepository seasons, IStatsRepository stats, ITeamRepository teams)
    {
        Charts = charts;
        Elements = elements;
        ElementStats = elementStats;
        Leagues = leagues;
        Logs = logs;
        Picks = picks;
        Players = players;
        Points = points;
        Seasons = seasons;
        Stats = stats;
        Teams = teams;
    }

    public IChartRepository Charts { get; }
    public IElementRepository Elements { get; }
    public IElementStatRepository ElementStats { get; }
    public ILeagueRepository Leagues { get; }
    public ILoggingRepository Logs { get; }
    public IPickRepository Picks { get; }
    public IPlayerRepository Players { get; }
    public IPointsRepository Points { get; }
    public ISeasonRepository Seasons { get; }
    public IStatsRepository Stats { get; }
    public ITeamRepository Teams { get; }
}
