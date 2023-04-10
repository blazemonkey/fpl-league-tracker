INSERT INTO [dbo].[stats] VALUES (1, 'Test', 'Test', '1_Test', 2, 1)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 101 AS Version
END
GO