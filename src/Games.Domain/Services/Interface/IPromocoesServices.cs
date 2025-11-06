using Games.Domain.DTO;
using Games.Domain.Results;

namespace Games.Domain.Services.Interface;

public interface IPromocoesServices
{
    Task<Result<PromocoesDto.PromocoesDtoResponse>> InserePromocoes(PromocoesDto.PromocoesDtoRequest request);
    Task<Result<PromocoesDto.PromocoesDtoResponse>> ConsultaPromocoesAtivas();
    Task<Result<PromocoesDto.PromocaoDesativada>> DesativarPromocoes(long Id);
}
