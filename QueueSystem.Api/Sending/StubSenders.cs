using System.Threading.Tasks;
using QueueSystem.Api.Models;
using Microsoft.Extensions.Logging;

namespace QueueSystem.Api.Sending
{
    public class StubEmailSender : IEmailSender
    {
        private readonly ILogger<StubEmailSender> _logger;
        public StubEmailSender(ILogger<StubEmailSender> logger) => _logger = logger;
        public Task<SendResult> SendAsync(EmailMessage message)
        {
            _logger.LogInformation($"Stub: Pretending to send email to {message.ToAddress}");
            return Task.FromResult(new SendResult { Success = true });
        }
    }

    public class StubSmsSender : ISmsSender
    {
        private readonly ILogger<StubSmsSender> _logger;
        public StubSmsSender(ILogger<StubSmsSender> logger) => _logger = logger;
        public Task<SendResult> SendAsync(SmsMessage message)
        {
            _logger.LogInformation($"Stub: Pretending to send SMS to {message.ToNumber}");
            return Task.FromResult(new SendResult { Success = true });
        }
    }
}
