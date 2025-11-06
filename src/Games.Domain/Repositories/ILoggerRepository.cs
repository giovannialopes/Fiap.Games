using Games.Domain.Entities;

namespace Games.Domain.Repositories;

public interface ILoggerRepository : ICommit
{
    Task AddILogger(ILoggerEnt loggerEnt);
}
