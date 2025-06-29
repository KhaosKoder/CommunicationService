using Microsoft.AspNetCore.Mvc;
using QueueSystem.Api.Models;
using QueueSystem.Api.Services;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/sms")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsQueueService _service;
        public SmsController(ISmsQueueService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> SubmitSms([FromBody] SmsMessage message)
        {
            var id = await _service.EnqueueAsync(message);
            return CreatedAtAction(nameof(GetSmsById), new { id }, message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSmsById(long id)
        {
            var msg = await _service.GetByIdAsync(id);
            if (msg == null) return NotFound();
            return Ok(msg);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingSms()
        {
            var pending = await _service.GetPendingAsync();
            return Ok(pending);
        }
    }
}
