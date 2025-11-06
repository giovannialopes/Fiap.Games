using Games.Domain.Queue.Event;

namespace Games.Domain.Queue;

public interface IWalletBusPublisher
{
    Task Publish(PaymentCreatedEvent cmd, CancellationToken cancellationToken);
}
