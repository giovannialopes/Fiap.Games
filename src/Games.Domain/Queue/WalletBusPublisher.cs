using Azure.Messaging.ServiceBus;
using Games.Domain.Queue.Event;
using MassTransit;
using System.Text.Json;

namespace Games.Domain.Queue;

public class WalletBusPublisher : IWalletBusPublisher
{
    private readonly ServiceBusSender _sender;
    public WalletBusPublisher(ServiceBusSender sender) => _sender = sender;

    public async Task Publish(PaymentCreatedEvent cmd, CancellationToken ct = default) {
        var msg = new ServiceBusMessage(BinaryData.FromString(JsonSerializer.Serialize(cmd))) {
            ContentType = "application/json",
            Subject = "wallet.removerSaldo.requested",
        };

        await _sender.SendMessageAsync(msg, ct);
    }

}
