using System.Diagnostics;
using Games.Domain.Services.Interface;

namespace Games.Domain.Services.Class;

/// <summary>
/// Serviço de métricas (SRP - Single Responsibility: apenas gerencia métricas)
/// </summary>
public class MetricsService : IMetricsService
{
    private long _gamesCreated = 0;
    private long _gamesPurchased = 0;
    private long _gamesQueried = 0;
    private long _activeGames = 0;
    private long _totalUsers = 0;
    private static readonly Process _process = Process.GetCurrentProcess();

    public void IncrementGamesCreated() => Interlocked.Increment(ref _gamesCreated);
    public void IncrementGamesPurchased() => Interlocked.Increment(ref _gamesPurchased);
    public void IncrementGamesQueried() => Interlocked.Increment(ref _gamesQueried);
    public void SetActiveGames(long value) => Interlocked.Exchange(ref _activeGames, value);
    public void SetTotalUsers(long value) => Interlocked.Exchange(ref _totalUsers, value);
    public void RecordPurchaseDuration(double seconds) { /* YAGNI - não implementado ainda */ }

    public long GetGamesCreated() => _gamesCreated;
    public long GetGamesPurchased() => _gamesPurchased;
    public long GetGamesQueried() => _gamesQueried;
    public long GetActiveGames() => _activeGames;
    public long GetTotalUsers() => _totalUsers;

    public double GetCpuUsage() => _process.TotalProcessorTime.TotalSeconds;
    public long GetMemoryUsage() => _process.WorkingSet64;
    public TimeSpan GetUptime() => DateTime.UtcNow - _process.StartTime.ToUniversalTime();
}

