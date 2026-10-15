USE [master];
GO

ALTER AUTHORIZATION ON DATABASE::[ApplicationDbContext] TO [sa];
GO

SELECT
	name AS ApplicationDbContext,
	suser_sname(owner_sid) AS OwnerName
FROM sys.databases
WHERE name = 'ApplicationDbContext';