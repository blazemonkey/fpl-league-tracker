CREATE OR ALTER PROCEDURE [dbo].[1_mostpointsinagameweek] @SeasonId INT, @LeagueId INT
AS
BEGIN
	SELECT TOP 10 TeamName AS 'Team Name', Gameweek, GameweekPoints AS Points
	FROM points po
	JOIN players pl
	ON po.PlayerId = pl.Id
	JOIN leagues l
	ON l.Id = pl.LeagueId 
	WHERE l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	ORDER BY GameweekPoints DESC, Gameweek
END
GO

CREATE OR ALTER PROCEDURE [dbo].[1_mostbenchpointsinagameweek] @SeasonId INT, @LeagueId INT
AS
BEGIN
	SELECT TOP 10 TeamName AS 'Team Name', Gameweek, GameweekPointsOnBench AS Points
	FROM points po
	JOIN players pl
	ON po.PlayerId = pl.Id
	JOIN leagues l
	ON l.Id = pl.LeagueId 
	WHERE l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	ORDER BY GameweekPointsOnBench DESC, Gameweek
END
GO