using Microsoft.AspNetCore.Mvc;
using QueueSystem.Api.Models;
using QueueSystem.Api.Services;
using QueueSystem.Api.Templating;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/email/template")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailQueueService _queueService;
        private readonly ITemplateRenderer _renderer;
        public EmailTemplateController(IEmailQueueService queueService, ITemplateRenderer renderer)
        {
            _queueService = queueService;
            _renderer = renderer;
        }

        public class EmailTemplateRequest
        {
            public string To { get; set; } = null!;
            public string Subject { get; set; } = null!;
            public string TemplateName { get; set; } = null!;
            public object TemplateModel { get; set; } = null!;
        }

        [HttpPost]
        public async Task<IActionResult> SendTemplatedEmail([FromBody] EmailTemplateRequest req)
        {
            var body = await _renderer.RenderAsync(req.TemplateName, req.TemplateModel);
            var msg = new EmailMessage
            {
                ToAddress = req.To,
                Subject = req.Subject,
                Body = body
            };
            var id = await _queueService.EnqueueAsync(msg);
            return CreatedAtAction("GetEmailById", "Email", new { id }, msg);
        }
    }
}
