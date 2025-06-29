using System.Collections.Generic;
using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Repositories
{
    public interface ISmsMessageRepository
    {
        Task<SmsMessage?> GetByIdAsync(long id);
        Task<List<SmsMessage>> GetPendingAsync(int maxCount = 100);
        Task AddAsync(SmsMessage message);
        Task UpdateAsync(SmsMessage message);
        Task<List<SmsMessage>> GetByStatusAsync(int status, int maxCount = 100);
    }
}
