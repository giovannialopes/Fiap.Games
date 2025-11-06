using Games.Domain.Entities;

namespace Games.Domain.Repositories;

public interface IJogosRepository : ICommit
{
    Task AdicionarJogos(GamesEnt jogo);
    Task AtualizarJogos(GamesEnt jogo);
    Task<GamesEnt> ObterJogoPorNome(string jogo);
    Task<GamesEnt> ObterJogoPorId(Guid jogoId);
    Task<LibraryEnt> VerificaSeUsuarioPossuiJogo(string jogoId, string usuarioId);
    Task<List<GamesEnt>> ObterJogos();
    Task<List<GamesEnt>> ObterJogosPorUsuario(string Usuario);
}
