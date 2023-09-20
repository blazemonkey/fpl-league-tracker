IF ((SELECT COUNT(1) FROM [dbo].[stats]) = 0)
BEGIN
	INSERT INTO [dbo].[stats] VALUES (1, 'Most Points In A Gameweek', 'Gameweek Dominator', '1_mostpointsinagameweek', 1, 1)
	INSERT INTO [dbo].[stats] VALUES (1, 'Most Bench Points In A Gameweek', 'No Faith', '1_mostbenchpointsinagameweek', 2, 1)
END
GO

IF ((SELECT COUNT(1) FROM [dbo].[charts]) = 0)
BEGIN
	INSERT INTO [dbo].[charts] VALUES (1, 1, 'Gameweek Total Points History', '', 1, 1)
	INSERT INTO [dbo].[charts] VALUES (1, 1, 'Gameweek Points History', '', 2, 1)
END
GO

