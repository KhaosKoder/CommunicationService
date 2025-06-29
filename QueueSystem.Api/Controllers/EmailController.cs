using Microsoft.AspNetCore.Mvc;
using QueueSystem.Api.Models;
using QueueSystem.Api.Services;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailQueueService _service;
        public EmailController(IEmailQueueService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> SubmitEmail([FromBody] EmailMessage message)
        {
            var id = await _service.EnqueueAsync(message);
            return CreatedAtAction(nameof(GetEmailById), new { id }, message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailById(long id)
        {
            var msg = await _service.GetByIdAsync(id);
            if (msg == null) return NotFound();
            return Ok(msg);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingEmails()
        {
            var pending = await _service.GetPendingAsync();
            return Ok(pending);
        }
    }
}
