using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetCare360.HealthChecks;

public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalServiceHealthCheck(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var url = _configuration["ExternalServices:HealthUrl"];

        if (string.IsNullOrWhiteSpace(url))
        {
            return HealthCheckResult.Degraded(
                "URL do serviço externo não configurada.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();

            client.Timeout = TimeSpan.FromSeconds(5);

            using var response = await client.GetAsync(
                url,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy(
                    "Serviço externo disponível.");
            }

            return HealthCheckResult.Unhealthy(
                $"Serviço externo retornou HTTP {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Não foi possível acessar o serviço externo.",
                ex);
        }
    }
}