using System.Threading.Tasks;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Repositories
{
    public interface ISystemSettingsRepository
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task SetAsync(string key, string value);
    }
}
