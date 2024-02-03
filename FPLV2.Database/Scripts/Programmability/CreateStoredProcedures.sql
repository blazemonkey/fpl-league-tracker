CREATE OR ALTER PROCEDURE [dbo].[1_mostpointsinagameweek] @SeasonId INT, @LeagueId INT
AS
BEGIN
	SELECT TeamName AS 'Team Name', Gameweek, GameweekPoints AS Points
	FROM points po
	JOIN players pl
	ON po.PlayerId = pl.Id
	JOIN players_in_leagues pil
	ON pil.Playerid = pl.Id
	JOIN leagues l
	ON l.Id = pil.LeagueId 
	WHERE l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	ORDER BY GameweekPoints DESC, Gameweek
END
GO

CREATE OR ALTER PROCEDURE [dbo].[1_mostbenchpointsinagameweek] @SeasonId INT, @LeagueId INT
AS
BEGIN
	SELECT TeamName AS 'Team Name', Gameweek, GameweekPointsOnBench AS Points
	FROM points po
	JOIN players pl
	ON po.PlayerId = pl.Id
	JOIN players_in_leagues pil
	ON pil.Playerid = pl.Id
	JOIN leagues l
	ON l.Id = pil.LeagueId 
	WHERE l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	ORDER BY GameweekPointsOnBench DESC, Gameweek
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 100 AS Version
END
GO