using Games.Domain.Queue;
using Games.Domain.Services.Class;
using Games.Domain.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Domain.Dependency;

public static class DomainDependency
{
    public static IServiceCollection AddServices(this IServiceCollection services) {
        return services
            .AddScoped<ILoggerServices, LoggerServices>()
            .AddScoped<IBibliotecaServices, BibliotecaServices>()
            .AddScoped<IPromocoesServices, PromocoesServices>()
            .AddScoped<IWalletBusPublisher, WalletBusPublisher>()
            .AddScoped<IJogosServices, JogosServices>();


    }
}

