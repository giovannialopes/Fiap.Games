using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;

public class BibliotecaRepository : IBibliotecaRepository
{
    private readonly DbGames _dbGames;

    public BibliotecaRepository(DbGames dbGames) {
        _dbGames = dbGames;
    }

    public async Task AdicionaNaBiblioteca(LibraryEnt library) {

        await _dbGames.BIBLIOTECA_JOGOS.AddAsync(library);
        await Commit();
    }

    public async Task<LibraryEnt> ValidaSeJaPossuiJogo(Guid jogoId) =>
         await _dbGames.BIBLIOTECA_JOGOS.AsNoTracking().FirstOrDefaultAsync(x => x.JogoId.Equals(jogoId));

    public Task Commit() => _dbGames.SaveChangesAsync();

}
