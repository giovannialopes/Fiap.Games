using Games.Domain.Shared.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Games.Domain.Shared.Http;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;
    private readonly string _baseUrl;

    public UserService(IConfiguration configuration, ILogger<UserService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Busca a URL do serviço de usuários da configuração
        var usersServiceUrl = _configuration["ExternalServices:Users"];
        if (string.IsNullOrWhiteSpace(usersServiceUrl))
        {
            _logger.LogWarning("ExternalServices:Users não configurado, usando URL padrão");
            usersServiceUrl = "https://users-api.azurewebsites.net";
        }
        
        _baseUrl = usersServiceUrl.TrimEnd('/');
        _logger.LogInformation("UserService configurado com URL: {BaseUrl}", _baseUrl);
    }

    public CarteiraDto.CarteiraDtoResponse PegaSaldo(Guid perfilId)
    {
        try
        {
            var url = $"{_baseUrl}/api/v1/consulta/saldos";
            _logger.LogInformation("Chamando endpoint: {Url} | perfilId: {PerfilId}", url, perfilId);

            var headers = new WebHeaderCollection { { "UsuarioId", perfilId.ToString() } };
            
            var result = Request.Get<CarteiraDto.CarteiraDtoResponse>(url, headers: headers);
            
            _logger.LogInformation("Saldo obtido com sucesso para perfilId: {PerfilId}, Saldo: {Saldo}", 
                perfilId, result?.Saldo);
            
            return result;
        }
        catch (WebException ex)
        {
            _logger.LogError(ex, "Erro ao obter saldo para perfilId: {PerfilId}. Status: {Status}", 
                perfilId, ex.Status);
            
            if (ex.Response is HttpWebResponse httpResponse)
            {
                _logger.LogError("Status HTTP: {StatusCode}, StatusDescription: {StatusDescription}", 
                    httpResponse.StatusCode, httpResponse.StatusDescription);
            }
            
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter saldo para perfilId: {PerfilId}", perfilId);
            throw;
        }
    }
}

