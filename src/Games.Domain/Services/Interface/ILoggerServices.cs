namespace Games.Domain.Services.Interface;

public interface ILoggerServices
{
    Task LogInformation(string message);
    Task LogError(string message);
}
