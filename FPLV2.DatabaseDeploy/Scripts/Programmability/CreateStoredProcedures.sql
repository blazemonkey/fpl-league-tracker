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

CREATE OR ALTER PROCEDURE [dbo].[1_mostcaptainpoints] @SeasonId INT, @LeagueId INT
AS
BEGIN
	CREATE TABLE #TempTable 
	(
		PlayerId INT,
		Points INT,
	)

	INSERT INTO #TempTable
	SELECT p.Id AS PlayerId, SUM(es.TotalPoints) AS Points
	FROM seasons s
	JOIN leagues l
	ON s.Id = l.SeasonId
	JOIN players_in_leagues pil
	ON l.Id = pil.LeagueId
	JOIN players p
	ON p.Id = pil.PlayerId
	JOIN picks pk
	ON pk.PlayerId = p.Id
	JOIN elements_stats es
	ON es.ElementId = pk.ElementId	
	AND es.Gameweek = pk.Gameweek
	AND l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	AND Multiplier = 2
	GROUP BY p.Id

	SELECT p.TeamName AS 'Team', Points 
	FROM #TempTable tt
	JOIN players p
	ON tt.PlayerId = p.Id
	ORDER BY Points DESC

	DROP TABLE #TempTable
END
GO

CREATE OR ALTER PROCEDURE [dbo].[2_playerwithmostpoints] @SeasonId INT, @LeagueId INT, @PlayerId INT

AS
BEGIN
	CREATE TABLE #TempTable 
	(
		ElementId INT,
		Points INT,
	)

	INSERT INTO #TempTable
	SELECT pk.ElementId, SUM(Multiplier * TotalPoints) AS Points 
	FROM seasons s
	JOIN leagues l
	ON s.Id = l.SeasonId
	JOIN players_in_leagues pil
	ON l.Id = pil.LeagueId
	JOIN players p
	ON p.Id = pil.PlayerId
	JOIN picks pk
	ON pk.PlayerId = p.Id
	JOIN elements_stats es
	ON es.ElementId = pk.ElementId	
	AND es.Gameweek = pk.Gameweek
	WHERE p.Id = @PlayerId
	AND l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	GROUP BY pk.ElementId

	SELECT (CONCAT(FirstName, ' ', SecondName)) AS 'Player Name' , t.Name AS 'Team', Points 
	FROM #TempTable tt
	JOIN elements e
	ON tt.ElementId = e.Id
	JOIN teams t
	ON e.TeamId = t.Id
	ORDER BY Points DESC, WebName

	DROP TABLE #TempTable
END
GO

CREATE OR ALTER PROCEDURE [dbo].[2_playerwithmostbenchpoints] @SeasonId INT, @LeagueId INT, @PlayerId INT

AS
BEGIN
	CREATE TABLE #TempTable 
	(
		ElementId INT,
		Points INT,
	)

	INSERT INTO #TempTable
	SELECT pk.ElementId, SUM(TotalPoints) AS Points 
	FROM seasons s
	JOIN leagues l
	ON s.Id = l.SeasonId
	JOIN players_in_leagues pil
	ON l.Id = pil.LeagueId
	JOIN players p
	ON p.Id = pil.PlayerId
	JOIN picks pk
	ON pk.PlayerId = p.Id
	JOIN elements_stats es
	ON es.ElementId = pk.ElementId	
	AND es.Gameweek = pk.Gameweek
	WHERE p.Id = @PlayerId
	AND l.LeagueId = @LeagueId
	AND l.SeasonId = @SeasonId
	AND Multiplier = 0
	GROUP BY pk.ElementId

	SELECT (CONCAT(FirstName, ' ', SecondName)) AS 'Player Name' , t.Name AS 'Team', Points 
	FROM #TempTable tt
	JOIN elements e
	ON tt.ElementId = e.Id
	JOIN teams t
	ON e.TeamId = t.Id
	ORDER BY Points DESC, WebName

	DROP TABLE #TempTable
END