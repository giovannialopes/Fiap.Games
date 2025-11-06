using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;

public class JogosRepository : IJogosRepository
{
    private readonly DbGames _dbGames;

    public JogosRepository(DbGames dbFcg) {
        _dbGames = dbFcg;
    }

    public async Task Commit() =>
        await _dbGames.
        SaveChangesAsync();

    public async Task AdicionarJogos(GamesEnt jogo) {
        await _dbGames.JOGOS.AddAsync(jogo);
        await Commit();
    }


    public async Task AtualizarJogos(GamesEnt jogo) {
        _dbGames.JOGOS.Update(jogo);
        await Commit();
    }

    public async Task<GamesEnt> ObterJogoPorNome(string jogo) =>
        await _dbGames.JOGOS.AsNoTracking().FirstOrDefaultAsync(x => x.Nome.Equals(jogo) && x.Ativo == true);


    public async Task<GamesEnt> ObterJogoPorId(Guid jogoId) =>
        await _dbGames.JOGOS.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(jogoId) && x.Ativo == true);

    public async Task<List<GamesEnt>> ObterJogos() =>
        await _dbGames.JOGOS.AsNoTracking().Where(x => x.Ativo).ToListAsync();

    public async Task<List<GamesEnt>> ObterJogosPorUsuario(string Usuario) {
        var jogos = await (
        from b in _dbGames.BIBLIOTECA_JOGOS
        join j in _dbGames.JOGOS on b.JogoId equals j.Id
        where b.PerfilId == Guid.Parse(Usuario)
        select j).ToListAsync();


        return jogos;
    }

    public async Task<LibraryEnt> VerificaSeUsuarioPossuiJogo(string jogoId, string usuarioId) =>
        await _dbGames.BIBLIOTECA_JOGOS
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.JogoId.Equals(Guid.Parse(jogoId)) &&
                x.PerfilId.Equals(Guid.Parse(usuarioId))
            );


}
