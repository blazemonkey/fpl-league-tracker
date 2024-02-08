IF ((SELECT COUNT(1) FROM [dbo].[charts] WHERE Name = 'Standings History') = 0)
BEGIN
	INSERT INTO [dbo].[charts] VALUES (1, 1, 'Standings History', '', 5, 1)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 103 AS Version
END
GO