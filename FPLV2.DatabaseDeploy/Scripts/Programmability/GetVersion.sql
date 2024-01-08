IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'GetVersion')
BEGIN
	exec('CREATE PROCEDURE GetVersion AS BEGIN SELECT 100 AS Version END -- this should be override in the deploy code')
END
