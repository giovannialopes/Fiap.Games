using Games.Domain.DTO;
using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Games.Domain.DTO.ErrorDto;

namespace Games.API.Controllers;

/// <summary>
/// Controller para estatísticas e relatórios do sistema
/// </summary>
[Route("api/v1/statistics")]
[ApiController]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IJogosServices _jogosServices;
    private readonly IBibliotecaServices _bibliotecaServices;
    private readonly ILoggerServices _logger;

    public StatisticsController(
        IJogosServices jogosServices,
        IBibliotecaServices bibliotecaServices,
        ILoggerServices logger)
    {
        _jogosServices = jogosServices;
        _bibliotecaServices = bibliotecaServices;
        _logger = logger;
    }

    /// <summary>
    /// Obtém estatísticas gerais da plataforma
    /// </summary>
    /// <returns>Estatísticas da plataforma</returns>
    [HttpGet("platform")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlatformStatistics()
    {
        await _logger.LogInformation("Consultando estatísticas da plataforma");
        
        var jogosResult = await _jogosServices.ConsultarJogos();
        
        if (!jogosResult.IsSuccess)
        {
            return BadRequest(jogosResult.Error);
        }

        var jogos = jogosResult.Value ?? new List<JogosDto.JogosDtoResponse>();
        
        var stats = new
        {
            timestamp = DateTime.UtcNow,
            platform = new
            {
                totalGames = jogos.Count,
                activeGames = jogos.Count,
                totalRevenue = jogos.Sum(j => j.Preco), // Estimativa
                averagePrice = jogos.Any() ? jogos.Average(j => j.Preco) : 0,
                gamesByType = jogos.GroupBy(j => j.Tipo)
                    .Select(g => new { type = g.Key, count = g.Count() })
                    .ToList()
            }
        };

        return Ok(stats);
    }

    /// <summary>
    /// Obtém estatísticas de um usuário específico
    /// </summary>
    /// <param name="perfilId">ID do perfil do usuário</param>
    /// <returns>Estatísticas do usuário</returns>
    [HttpGet("user/{perfilId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserStatistics(string perfilId)
    {
        if (string.IsNullOrWhiteSpace(perfilId))
        {
            return BadRequest(new ErrorResponse { Message = "PerfilId é obrigatório", Code = "400" });
        }

        await _logger.LogInformation($"Consultando estatísticas do usuário {perfilId}");
        
        var bibliotecaResult = await _bibliotecaServices.ConsultarBibliotecaPorUsuario(perfilId);
        
        var stats = new
        {
            timestamp = DateTime.UtcNow,
            userId = perfilId,
            library = new
            {
                totalGames = bibliotecaResult.IsSuccess ? (bibliotecaResult.Value?.Count() ?? 0) : 0,
                games = bibliotecaResult.IsSuccess ? bibliotecaResult.Value?.ToList() ?? new List<JogosDto.JogosDtoResponse>() : new List<JogosDto.JogosDtoResponse>()
            }
        };

        return Ok(stats);
    }
}

