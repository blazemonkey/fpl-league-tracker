namespace FPLV2.Database.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IElementRepository Elements { get; }
        public IElementStatRepository ElementStats { get; }
        public ILeagueRepository Leagues { get; }
        public IPickRepository Picks { get; }
        public IPlayerRepository Players { get; }
        public IPointsRepository Points { get; }
        public ISeasonRepository Seasons { get; }
        public ITeamRepository Teams { get; }
    }
}
