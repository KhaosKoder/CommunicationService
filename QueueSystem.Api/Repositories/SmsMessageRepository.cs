using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Data;
using QueueSystem.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueSystem.Api.Repositories
{
    public class SmsMessageRepository : ISmsMessageRepository
    {
        private readonly QueueSystemDbContext _db;
        public SmsMessageRepository(QueueSystemDbContext db) => _db = db;

        public async Task<SmsMessage?> GetByIdAsync(long id) =>
            await _db.SmsMessages.FindAsync(id);

        public async Task<List<SmsMessage>> GetPendingAsync(int maxCount = 100) =>
            await _db.SmsMessages
                .Where(s => s.Status == 0)
                .OrderBy(s => s.CreatedAt)
                .Take(maxCount)
                .ToListAsync();

        public async Task AddAsync(SmsMessage message)
        {
            await _db.SmsMessages.AddAsync(message);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(SmsMessage message)
        {
            _db.SmsMessages.Update(message);
            await _db.SaveChangesAsync();
        }

        public async Task<List<SmsMessage>> GetByStatusAsync(int status, int maxCount = 100) =>
            await _db.SmsMessages
                .Where(s => s.Status == status)
                .OrderBy(s => s.CreatedAt)
                .Take(maxCount)
                .ToListAsync();
    }
}
