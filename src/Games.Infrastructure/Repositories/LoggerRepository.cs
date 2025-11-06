using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Infrastructure.Data;

namespace Games.Infrastructure.Repositories;

public class LoggerRepository : ILoggerRepository
{
    private readonly DbGames _dbGames;

    public LoggerRepository(DbGames dbGames) {
        _dbGames = dbGames;
    }

    public async Task Commit() => await _dbGames.SaveChangesAsync();

    public async Task AddILogger(ILoggerEnt loggerEnt) => await _dbGames.LOGS.AddAsync(loggerEnt);
}

