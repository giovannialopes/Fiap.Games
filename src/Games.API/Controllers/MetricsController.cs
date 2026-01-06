using Games.Domain.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Games.API.Controllers;

/// <summary>
/// Controller para expor métricas e estatísticas do sistema
/// SRP - Responsabilidade única: apenas expor endpoints de métricas
/// </summary>
[Route("api/v1/metrics")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly IPrometheusFormatter _formatter;

    public MetricsController(IMetricsService metricsService, IPrometheusFormatter formatter)
    {
        _metricsService = metricsService;
        _formatter = formatter;
    }

    /// <summary>
    /// Obtém métricas do sistema em formato Prometheus
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetMetrics()
    {
        var metrics = _formatter.FormatMetrics(_metricsService);
        return Content(metrics, "text/plain");
    }

    /// <summary>
    /// Obtém estatísticas do sistema em formato JSON
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            metrics = new
            {
                games = new
                {
                    created = _metricsService.GetGamesCreated(),
                    purchased = _metricsService.GetGamesPurchased(),
                    queried = _metricsService.GetGamesQueried(),
                    active = _metricsService.GetActiveGames()
                },
                users = new
                {
                    total = _metricsService.GetTotalUsers()
                },
                system = new
                {
                    cpuUsage = _metricsService.GetCpuUsage(),
                    memoryUsage = _metricsService.GetMemoryUsage(),
                    uptime = _metricsService.GetUptime()
                }
            }
        });
    }
}

