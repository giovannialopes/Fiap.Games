using Games.Domain.Services.Interface;

namespace Games.Domain.Services.Class;

/// <summary>
/// Formatador de métricas Prometheus (SRP - Single Responsibility: apenas formata)
/// </summary>
public class PrometheusFormatter : IPrometheusFormatter
{
    public string FormatMetrics(IMetricsService metricsService)
    {
        var metrics = new System.Text.StringBuilder();
        
        // DRY - Método auxiliar para evitar duplicação
        AppendCounter(metrics, "games_created_total", "Total games created", metricsService.GetGamesCreated());
        AppendCounter(metrics, "games_purchased_total", "Total games purchased", metricsService.GetGamesPurchased());
        AppendCounter(metrics, "games_queried_total", "Total games queried", metricsService.GetGamesQueried());
        AppendGauge(metrics, "active_games", "Number of active games", metricsService.GetActiveGames());
        AppendGauge(metrics, "total_users", "Total number of users", metricsService.GetTotalUsers());
        
        // Métricas do sistema
        AppendCounter(metrics, "process_cpu_seconds_total", "Total CPU time used", metricsService.GetCpuUsage());
        AppendGauge(metrics, "process_memory_bytes", "Memory usage in bytes", metricsService.GetMemoryUsage());

        return metrics.ToString();
    }

    // DRY - Evita duplicação de código
    private static void AppendCounter(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} counter");
        sb.AppendLine($"{name} {value}");
    }

    private static void AppendGauge(System.Text.StringBuilder sb, string name, string help, double value)
    {
        sb.AppendLine($"# HELP {name} {help}");
        sb.AppendLine($"# TYPE {name} gauge");
        sb.AppendLine($"{name} {value}");
    }
}

