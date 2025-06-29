using Microsoft.AspNetCore.Mvc;
using QueueSystem.Api.Repositories;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly ISystemSettingsRepository _settingsRepo;
        public StatusController(ISystemSettingsRepository settingsRepo) => _settingsRepo = settingsRepo;

        [HttpPost("email/pause")]
        public async Task<IActionResult> PauseEmailSending()
        {
            await _settingsRepo.SetAsync("EmailSendingStatus", "Paused");
            return Ok(new { status = "Paused" });
        }

        [HttpPost("email/resume")]
        public async Task<IActionResult> ResumeEmailSending()
        {
            await _settingsRepo.SetAsync("EmailSendingStatus", "Running");
            return Ok(new { status = "Running" });
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetEmailStatus()
        {
            var status = await _settingsRepo.GetByKeyAsync("EmailSendingStatus");
            return Ok(new { status = status?.Value ?? "Unknown" });
        }

        [HttpPost("sms/pause")]
        public async Task<IActionResult> PauseSmsSending()
        {
            await _settingsRepo.SetAsync("SmsSendingStatus", "Paused");
            return Ok(new { status = "Paused" });
        }

        [HttpPost("sms/resume")]
        public async Task<IActionResult> ResumeSmsSending()
        {
            await _settingsRepo.SetAsync("SmsSendingStatus", "Running");
            return Ok(new { status = "Running" });
        }

        [HttpGet("sms")]
        public async Task<IActionResult> GetSmsStatus()
        {
            var status = await _settingsRepo.GetByKeyAsync("SmsSendingStatus");
            return Ok(new { status = status?.Value ?? "Unknown" });
        }
    }
}
