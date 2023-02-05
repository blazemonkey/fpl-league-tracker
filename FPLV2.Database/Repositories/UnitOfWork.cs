﻿using FPLV2.Database.Repositories.Interfaces;

namespace FPLV2.Database.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IElementRepository elements, IElementStatRepository elementStats, ILeagueRepository leagues, IPickRepository picks, IPlayerRepository players, IPointsRepository points, ISeasonRepository seasons, ITeamRepository teams)
    {
        Elements = elements;
        ElementStats = elementStats;
        Leagues = leagues;
        Picks = picks;
        Players = players;
        Points = points;
        Seasons = seasons;
        Teams = teams;
    }

    public IElementRepository Elements { get; }
    public IElementStatRepository ElementStats { get; }
    public ILeagueRepository Leagues { get; }
    public IPickRepository Picks { get; }
    public IPlayerRepository Players { get; }
    public IPointsRepository Points { get; }
    public ISeasonRepository Seasons { get; }
    public ITeamRepository Teams { get; }
}