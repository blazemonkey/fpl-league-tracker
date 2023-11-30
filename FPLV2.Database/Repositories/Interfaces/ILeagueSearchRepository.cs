﻿using FPLV2.Database.Models;

namespace FPLV2.Database.Repositories.Interfaces;

public interface ILeagueSearchRepository
{
    Task<LeagueSearch[]> GetAll();
    Task<LeagueSearch[]> GetAllBySeasonId(int seasonId);
    Task<int> GetMaxLeagueId(int seasonId);
    Task DeleteAll();
    Task<int> Insert(LeagueSearch league);
    Task<int> InsertAll(LeagueSearch[] leagues);
    Task<bool> Update(LeagueSearch league);
}
