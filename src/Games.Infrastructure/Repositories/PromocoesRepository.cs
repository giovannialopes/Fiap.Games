using Games.Domain.Entities;
using Games.Domain.Repositories;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories;

public class PromocoesRepository : IPromocoesRepository
{
    private readonly DbGames _dbGames;

    public PromocoesRepository(DbGames dbGames) {
        _dbGames = dbGames;
    }
    public async Task Commit() => await _dbGames.SaveChangesAsync();


    public async Task AdicionarPromocoes(PromotionEnt promocoes) {
        await _dbGames.PROMOCAO.AddAsync(promocoes);
        await Commit();
    }

    public async Task DesativarPromocoes(long promocaoId) {
        var promocao = await _dbGames.PROMOCAO.FindAsync(promocaoId);
        _dbGames.PROMOCAO.Remove(promocao);
        await Commit();
    }

    public async Task<PromotionEnt> ConsultaPromocoesAtivas() {
        var agora = DateTime.UtcNow;

        var promocao = await _dbGames.PROMOCAO
            .Where(p => p.DataInicio <= agora && p.DataFim >= agora)
            .OrderBy(p => p.DataInicio)
            .FirstOrDefaultAsync();

        if (promocao == null)
            return null;

        return new PromotionEnt {
            Id = promocao.Id,
            Nome = promocao.Nome,
            Valor = promocao.Valor,
            DataInicio = promocao.DataInicio,
            DataFim = promocao.DataFim,
            IdJogos = promocao.IdJogos
        };
    }
}
