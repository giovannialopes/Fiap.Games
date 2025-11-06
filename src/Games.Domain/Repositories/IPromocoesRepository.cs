using Games.Domain.Entities;

namespace Games.Domain.Repositories;

public interface IPromocoesRepository : ICommit
{
    Task AdicionarPromocoes(PromotionEnt promotion);
    Task<PromotionEnt> ConsultaPromocoesAtivas();
    Task DesativarPromocoes(long promocaoId);
}
