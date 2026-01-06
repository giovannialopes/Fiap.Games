using Games.Domain.Shared.DTO;
using System.Net;

namespace Games.Domain.Shared.Http;

/// <summary>
/// Classe estática obsoleta. Use IUserService através de injeção de dependência.
/// </summary>
public static class User
{
    private const string _HOST = "https://localhost:7205/api/v1";

    public static CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId) {
        Console.WriteLine($"[DEBUG] Chamando endpoint: {_HOST}/consulta/saldos | perfilId:{perfilId}");

        var headers = new WebHeaderCollection { { "UsuarioId", perfilId.ToString() } };
        return Request.Get<CarteiraDto.CarteiraDtoResponse>($"{_HOST}/consulta/saldos", headers: headers);
    }
}

