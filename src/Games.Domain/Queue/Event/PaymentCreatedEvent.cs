namespace Games.Domain.Queue.Event;

public record PaymentCreatedEvent(
    Guid JogoId,
    Guid PerfilId,
    decimal saldo
);
