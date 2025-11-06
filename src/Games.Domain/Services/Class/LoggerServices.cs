using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Domain.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Games.Domain.Services.Class;

public class LoggerServices(ILogger<LoggerServices> logger,
        IServiceProvider serviceProvider) : ILoggerServices
{
    /// <summary>
    /// Registra uma mensagem de erro no sistema de logs e persiste no repositório de logs.
    /// </summary>
    /// <param name="message">Mensagem de erro a ser registrada.</param>
    /// <returns>Task assíncrona representando a operação de log.</returns>
    public async Task LogError(string message) {
        logger.LogError("Erro", message);

        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService(typeof(ILoggerRepository)) as ILoggerRepository;

        var log = ILoggerEnt.Criar("Erro", message, 0);

        await repository!.AddILogger(log);
        await repository.Commit();
    }

    /// <summary>
    /// Registra uma mensagem de informação no sistema de logs e persiste no repositório de logs.
    /// </summary>
    /// <param name="message">Mensagem informativa a ser registrada.</param>
    /// <returns>Task assíncrona representando a operação de log.</returns>
    public async Task LogInformation(string message) {
        logger.LogInformation(message);

        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetService(typeof(ILoggerRepository)) as ILoggerRepository;

        var log = ILoggerEnt.Criar(message, string.Empty, 0);

        await repository!.AddILogger(log);
        await repository.Commit();
    }
}
