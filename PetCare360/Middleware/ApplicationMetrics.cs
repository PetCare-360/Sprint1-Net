using System.Diagnostics.Metrics;

namespace PetCare360.Middleware;

public class ApplicationMetrics
{
    public const string MeterName = "PetCare360";

    private readonly Meter _meter;

    public Counter<long> Requests { get; }
    public Counter<long> Errors { get; }
    public Histogram<double> RequestDuration { get; }

    public ApplicationMetrics()
    {
        _meter = new Meter(MeterName, "1.0.0");

        Requests = _meter.CreateCounter<long>(
            "petcare360_requests_total",
            description: "Total de requisições HTTP recebidas pela API.");

        Errors = _meter.CreateCounter<long>(
            "petcare360_errors_total",
            description: "Total de respostas HTTP com erro.");

        RequestDuration = _meter.CreateHistogram<double>(
            "petcare360_request_duration_ms",
            unit: "ms",
            description: "Tempo de processamento das requisições HTTP.");
    }
}