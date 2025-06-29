using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Data;
using QueueSystem.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueSystem.Api.Repositories
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        private readonly QueueSystemDbContext _db;
        public EmailMessageRepository(QueueSystemDbContext db) => _db = db;

        public async Task<EmailMessage?> GetByIdAsync(long id) =>
            await _db.EmailMessages.FindAsync(id);

        public async Task<List<EmailMessage>> GetPendingAsync(int maxCount = 100) =>
            await _db.EmailMessages
                .Where(e => e.Status == 0)
                .OrderBy(e => e.CreatedAt)
                .Take(maxCount)
                .ToListAsync();

        public async Task AddAsync(EmailMessage message)
        {
            await _db.EmailMessages.AddAsync(message);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailMessage message)
        {
            _db.EmailMessages.Update(message);
            await _db.SaveChangesAsync();
        }

        public async Task<List<EmailMessage>> GetByStatusAsync(int status, int maxCount = 100) =>
            await _db.EmailMessages
                .Where(e => e.Status == status)
                .OrderBy(e => e.CreatedAt)
                .Take(maxCount)
                .ToListAsync();
    }
}
