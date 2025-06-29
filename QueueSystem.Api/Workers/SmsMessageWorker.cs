using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QueueSystem.Api.Models;
using QueueSystem.Api.Repositories;
using QueueSystem.Api.Sending;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueSystem.Api.Workers
{
    public class SmsMessageWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SmsMessageWorker> _logger;
        private readonly int _retryIntervalSeconds;
        private readonly int _maxRetries;
        private readonly int _maxDurationHours;

        public SmsMessageWorker(IServiceProvider serviceProvider, ILogger<SmsMessageWorker> logger, IOptions<SmsSenderSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _retryIntervalSeconds = options.Value.RetryIntervalSeconds;
            _maxRetries = options.Value.MaxRetries;
            _maxDurationHours = options.Value.MaxDurationHours;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ISmsMessageRepository>();
                    var settingsRepo = scope.ServiceProvider.GetRequiredService<ISystemSettingsRepository>();
                    var sender = scope.ServiceProvider.GetRequiredService<ISmsSender>();

                    var status = await settingsRepo.GetByKeyAsync("SmsSendingStatus");
                    if (status?.Value == "Paused")
                    {
                        _logger.LogInformation("SMS sending is paused.");
                        await Task.Delay(TimeSpan.FromSeconds(_retryIntervalSeconds), stoppingToken);
                        continue;
                    }

                    var pending = await repo.GetPendingAsync();
                    foreach (var msg in pending)
                    {
                        var now = DateTime.UtcNow;
                        var ageHours = (now - msg.CreatedAt).TotalHours;
                        if (msg.Attempts >= _maxRetries || ageHours >= _maxDurationHours)
                        {
                            msg.Status = 3; // Expired
                            msg.ErrorMessage = msg.ErrorMessage ?? "Expired by worker (max attempts or age)";
                            await repo.UpdateAsync(msg);
                            continue;
                        }
                        var result = await sender.SendAsync(msg);
                        if (result.Success)
                        {
                            msg.Status = 1; // Sent
                            msg.SentAt = now;
                            msg.ErrorMessage = null;
                        }
                        else
                        {
                            msg.Attempts++;
                            msg.LastAttemptAt = now;
                            msg.ErrorMessage = result.ErrorMessage;
                        }
                        await repo.UpdateAsync(msg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SmsMessageWorker loop");
                }
                await Task.Delay(TimeSpan.FromSeconds(_retryIntervalSeconds), stoppingToken);
            }
        }
    }

    public class SmsSenderSettings
    {
        public int MaxRetries { get; set; }
        public int RetryIntervalSeconds { get; set; }
        public int MaxDurationHours { get; set; }
    }
}
