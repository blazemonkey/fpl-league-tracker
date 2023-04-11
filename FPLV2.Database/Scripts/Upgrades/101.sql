INSERT INTO [dbo].[stats] VALUES (1, 'Most Bench Points In A Gameweek', 'No Faith', '1_mostbenchpointsinagameweek', 2, 1)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 101 AS Version
END
GO