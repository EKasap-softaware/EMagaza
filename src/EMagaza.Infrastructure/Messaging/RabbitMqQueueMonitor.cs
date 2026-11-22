using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMagaza.Infrastructure.Messaging;

/// <summary>Kuyruk sayaçlarını RabbitMQ Management API'sinden okur (MassTransit sayaç sunmaz).</summary>
public sealed class RabbitMqQueueMonitor : IQueueMonitor
{
    private readonly HttpClient _http;
    private readonly ILogger<RabbitMqQueueMonitor> _logger;

    public RabbitMqQueueMonitor(IOptions<RabbitMqOptions> options, ILogger<RabbitMqQueueMonitor> logger)
    {
        var o = options.Value;
        _logger = logger;
        _http = new HttpClient { BaseAddress = new Uri($"http://{o.Host}:{o.ManagementPort}"), Timeout = TimeSpan.FromSeconds(3) };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{o.User}:{o.Password}")));
    }

    public async Task<QueueStatus?> GetStatusAsync()
    {
        try
        {
            using var response = await _http.GetAsync($"/api/queues/%2F/{OrderEvent.QueueName}");
            if (response.StatusCode == HttpStatusCode.NotFound) return new QueueStatus(0, 0);   // kuyruk henüz kurulmadı

            response.EnsureSuccessStatusCode();
            var info = await response.Content.ReadFromJsonAsync<QueueInfo>();
            return new QueueStatus(info?.Messages ?? 0, info?.Consumers ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("RabbitMQ durumu okunamadı: {Message}", ex.Message);
            return null;
        }
    }

    private sealed record QueueInfo(
        [property: JsonPropertyName("messages")] long Messages,
        [property: JsonPropertyName("consumers")] long Consumers);
}
