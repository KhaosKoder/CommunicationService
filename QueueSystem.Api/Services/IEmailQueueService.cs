using System.Collections.Generic;
using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Services
{
    public interface IEmailQueueService
    {
        Task<EmailMessage?> GetByIdAsync(long id);
        Task<List<EmailMessage>> GetPendingAsync(int maxCount = 100);
        Task<long> EnqueueAsync(EmailMessage message);
        Task UpdateAsync(EmailMessage message);
    }
}
