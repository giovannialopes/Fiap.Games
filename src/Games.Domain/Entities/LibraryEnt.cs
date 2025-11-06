namespace Games.Domain.Entities;

public class LibraryEnt
{
    public long Id { get; set; }
    public Guid JogoId { get; set; }
    public Guid PerfilId { get; set; }

    public static LibraryEnt Criar(Guid jogoId, Guid perfilId) {
        return new LibraryEnt {
            JogoId = jogoId,
            PerfilId = perfilId
        };
    }
}
