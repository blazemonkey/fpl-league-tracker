namespace FPLV2.Database.Repositories.Interfaces;

public interface IUnitOfWork
{
    public IChartRepository Charts { get; }
    public IElementRepository Elements { get; }
    public IElementStatRepository ElementStats { get; }
    public ILeagueRepository Leagues { get; }
    public ILeagueSearchRepository LeagueSearch { get; }
    public ILoggingRepository Logs { get; }
    public IPickRepository Picks { get; }
    public IPlayerRepository Players { get; }
    public IPointsRepository Points { get; }
    public ISeasonRepository Seasons { get; }
    public IStatsRepository Stats { get; }
    public ITeamRepository Teams { get; }
}
