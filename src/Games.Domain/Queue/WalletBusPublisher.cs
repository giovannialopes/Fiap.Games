using Games.Domain.Queue.Event;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Games.Domain.Queue;

public class WalletBusPublisher : IWalletBusPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<WalletBusPublisher> _logger;
    
    public WalletBusPublisher(IPublishEndpoint publishEndpoint, ILogger<WalletBusPublisher> logger) 
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Publish(PaymentCreatedEvent cmd, CancellationToken ct = default) 
    {
        try
        {
            _logger.LogInformation("Publicando evento PaymentCreatedEvent: JogoId={JogoId}, PerfilId={PerfilId}, Saldo={Saldo}", 
                cmd.JogoId, cmd.PerfilId, cmd.saldo);
            
            await _publishEndpoint.Publish(cmd, ct);
            
            _logger.LogInformation("Evento PaymentCreatedEvent publicado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento PaymentCreatedEvent: JogoId={JogoId}, PerfilId={PerfilId}", 
                cmd.JogoId, cmd.PerfilId);
            throw;
        }
    }
}
