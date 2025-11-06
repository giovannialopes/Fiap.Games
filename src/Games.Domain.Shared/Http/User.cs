using Games.Domain.Shared.DTO;
using System.Net;
using System.Reflection;

namespace Games.Domain.Shared.Http;

public static class User
{
    private const string _HOST = "https://users-api-fiap-f5eraydvfqatejb8.brazilsouth-01.azurewebsites.net/api/v1";

    public static CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId) {
        Console.WriteLine($"[DEBUG] Chamando endpoint: {_HOST}/consulta/saldos | perfilId:{perfilId}");

        var headers = new WebHeaderCollection { { "UsuarioId", perfilId.ToString() } };
        return Request.Get<CarteiraDto.CarteiraDtoResponse>($"{_HOST}/consulta/saldos", headers: headers);
    }
}

