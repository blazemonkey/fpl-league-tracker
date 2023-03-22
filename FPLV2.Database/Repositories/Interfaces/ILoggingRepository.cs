namespace FPLV2.Database.Repositories.Interfaces;

public interface ILoggingRepository
{
    Task LogDebug(string message);
    Task LogInformation(string message);
    Task LogWarning(string message);
    Task LogError(string message);
}
