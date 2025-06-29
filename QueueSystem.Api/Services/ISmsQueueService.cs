using System.Collections.Generic;
using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Services
{
    public interface ISmsQueueService
    {
        Task<SmsMessage?> GetByIdAsync(long id);
        Task<List<SmsMessage>> GetPendingAsync(int maxCount = 100);
        Task<long> EnqueueAsync(SmsMessage message);
        Task UpdateAsync(SmsMessage message);
    }
}
