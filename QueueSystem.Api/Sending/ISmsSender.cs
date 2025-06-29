using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Sending
{
    public interface ISmsSender
    {
        Task<SendResult> SendAsync(SmsMessage message);
    }
}
