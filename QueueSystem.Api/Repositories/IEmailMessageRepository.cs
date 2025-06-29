using System.Collections.Generic;
using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Repositories
{
    public interface IEmailMessageRepository
    {
        Task<EmailMessage?> GetByIdAsync(long id);
        Task<List<EmailMessage>> GetPendingAsync(int maxCount = 100);
        Task AddAsync(EmailMessage message);
        Task UpdateAsync(EmailMessage message);
        Task<List<EmailMessage>> GetByStatusAsync(int status, int maxCount = 100);
    }
}
