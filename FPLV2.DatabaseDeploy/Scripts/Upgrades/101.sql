IF ((SELECT COUNT(1) FROM [dbo].[stats] WHERE Name = 'Player With Most Points') = 0)
BEGIN
	INSERT INTO [dbo].[stats] VALUES (2, 'Player With Most Points', 'Your MVP', '2_playerwithmostpoints', 1, 1)
END
GO

IF ((SELECT COUNT(1) FROM [dbo].[stats] WHERE Name = 'Player With Most Bench Points') = 0)
BEGIN
	INSERT INTO [dbo].[stats] VALUES (2, 'Player With Most Bench Points', 'Your Ignored MVP', '2_playerwithmostbenchpoints', 2, 1)
END
GO

IF ((SELECT COUNT(1) FROM [dbo].[charts] WHERE Name = 'Gameweek Total Bench Points History') = 0)
BEGIN
	INSERT INTO [dbo].[charts] VALUES (1, 1, 'Gameweek Total Bench Points History', '', 3, 0)
END
GO

IF ((SELECT COUNT(1) FROM [dbo].[charts] WHERE Name = 'Gameweek Bench Points History') = 0)
BEGIN
	INSERT INTO [dbo].[charts] VALUES (1, 1, 'Gameweek Bench Points History', '', 4, 1)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 101 AS Version
END
GO