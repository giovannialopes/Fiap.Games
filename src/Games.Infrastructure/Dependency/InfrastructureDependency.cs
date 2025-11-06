using Games.Domain.Repositories;
using Games.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Infrastructure.Dependency;

public static class InfrastructureDependency
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) {
        return services
            .AddScoped<ILoggerRepository, LoggerRepository>()
            .AddScoped<IBibliotecaRepository, BibliotecaRepository>()
            .AddScoped<IJogosRepository, JogosRepository>()
            .AddScoped<IPromocoesRepository, PromocoesRepository>();


    }

}