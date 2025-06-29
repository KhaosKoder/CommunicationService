using QueueSystem.Api.Models;
using QueueSystem.Api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueSystem.Api.Services
{
    public class SmsQueueService : ISmsQueueService
    {
        private readonly ISmsMessageRepository _repo;
        public SmsQueueService(ISmsMessageRepository repo) => _repo = repo;

        public Task<SmsMessage?> GetByIdAsync(long id) => _repo.GetByIdAsync(id);
        public Task<List<SmsMessage>> GetPendingAsync(int maxCount = 100) => _repo.GetPendingAsync(maxCount);
        public async Task<long> EnqueueAsync(SmsMessage message)
        {
            message.Status = 0; // Pending
            message.CreatedAt = System.DateTime.UtcNow;
            await _repo.AddAsync(message);
            return message.Id;
        }
        public Task UpdateAsync(SmsMessage message) => _repo.UpdateAsync(message);
    }
}
