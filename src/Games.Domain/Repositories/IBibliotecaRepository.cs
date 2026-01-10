using Games.Domain.Entities;

namespace Games.Domain.Repositories;

public interface IBibliotecaRepository : ICommit
{
    Task AdicionaNaBiblioteca(LibraryEnt library);

    Task<LibraryEnt> ValidaSeJaPossuiJogo(Guid JogoId, Guid PerfilId);
}
