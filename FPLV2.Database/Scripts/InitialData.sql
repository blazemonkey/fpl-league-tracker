IF ((SELECT COUNT(1) FROM [dbo].[stats]) = 0)
BEGIN
	INSERT INTO [dbo].[stats] VALUES (1, 'Most Points In A Gameweek', 'Gameweek Dominator', '1_mostpointsinagameweek', 1, 1)
END
GO