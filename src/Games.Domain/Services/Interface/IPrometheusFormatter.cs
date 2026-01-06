namespace Games.Domain.Services.Interface;

/// <summary>
/// Interface para formatação de métricas Prometheus (ISP - Interface Segregation)
/// </summary>
public interface IPrometheusFormatter
{
    string FormatMetrics(IMetricsService metricsService);
}

