using Games.Domain.Shared.DTO;

namespace Games.Domain.Shared.Http;

public interface IUserService
{
    CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId);
}

