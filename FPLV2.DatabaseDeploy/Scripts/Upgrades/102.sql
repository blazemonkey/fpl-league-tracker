IF ((SELECT COUNT(1) FROM [dbo].[stats] WHERE Name = 'Most Captain Points') = 0)
BEGIN
	INSERT INTO [dbo].[stats] VALUES (1, 'Most Captain Points', 'Difference Maker', '1_mostcaptainpoints', 3, 1)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetVersion]
AS
BEGIN
	SELECT 102 AS Version
END
GO