using QueueSystem.Api.Models;
using QueueSystem.Api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueSystem.Api.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IEmailMessageRepository _repo;
        public EmailQueueService(IEmailMessageRepository repo) => _repo = repo;

        public Task<EmailMessage?> GetByIdAsync(long id) => _repo.GetByIdAsync(id);
        public Task<List<EmailMessage>> GetPendingAsync(int maxCount = 100) => _repo.GetPendingAsync(maxCount);
        public async Task<long> EnqueueAsync(EmailMessage message)
        {
            message.Status = 0; // Pending
            message.CreatedAt = System.DateTime.UtcNow;
            await _repo.AddAsync(message);
            return message.Id;
        }
        public Task UpdateAsync(EmailMessage message) => _repo.UpdateAsync(message);
    }
}
