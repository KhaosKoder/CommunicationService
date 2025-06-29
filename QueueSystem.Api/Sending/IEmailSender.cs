using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Sending
{
    public interface IEmailSender
    {
        Task<SendResult> SendAsync(EmailMessage message);
    }

    public class SendResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
