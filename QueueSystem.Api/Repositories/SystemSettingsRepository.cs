using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Data;
using QueueSystem.Api.Models;
using System.Threading.Tasks;

namespace QueueSystem.Api.Repositories
{
    public class SystemSettingsRepository : ISystemSettingsRepository
    {
        private readonly QueueSystemDbContext _db;
        public SystemSettingsRepository(QueueSystemDbContext db) => _db = db;

        public async Task<SystemSetting?> GetByKeyAsync(string key) =>
            await _db.SystemSettings.FindAsync(key);

        public async Task SetAsync(string key, string value)
        {
            var setting = await _db.SystemSettings.FindAsync(key);
            if (setting == null)
            {
                setting = new SystemSetting { Key = key, Value = value };
                _db.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
                _db.SystemSettings.Update(setting);
            }
            await _db.SaveChangesAsync();
        }
    }
}
