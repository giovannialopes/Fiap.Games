namespace Games.Domain.Services.Interface;

/// <summary>
/// Interface para serviço de métricas (DIP - Dependency Inversion Principle)
/// </summary>
public interface IMetricsService
{
    void IncrementGamesCreated();
    void IncrementGamesPurchased();
    void IncrementGamesQueried();
    void SetActiveGames(long value);
    void SetTotalUsers(long value);
    void RecordPurchaseDuration(double seconds);
    
    // Getters para exposição de métricas
    long GetGamesCreated();
    long GetGamesPurchased();
    long GetGamesQueried();
    long GetActiveGames();
    long GetTotalUsers();
    
    // Métricas do sistema
    double GetCpuUsage();
    long GetMemoryUsage();
    TimeSpan GetUptime();
}

