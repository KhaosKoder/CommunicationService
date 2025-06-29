using Microsoft.AspNetCore.Mvc;
using QueueSystem.Api.Models;
using QueueSystem.Api.Services;
using QueueSystem.Api.Templating;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/sms/template")]
    public class SmsTemplateController : ControllerBase
    {
        private readonly ISmsQueueService _queueService;
        private readonly ITemplateRenderer _renderer;
        public SmsTemplateController(ISmsQueueService queueService, ITemplateRenderer renderer)
        {
            _queueService = queueService;
            _renderer = renderer;
        }

        public class SmsTemplateRequest
        {
            public string To { get; set; } = null!;
            public string TemplateName { get; set; } = null!;
            public object TemplateModel { get; set; } = null!;
        }

        [HttpPost]
        public async Task<IActionResult> SendTemplatedSms([FromBody] SmsTemplateRequest req)
        {
            var message = await _renderer.RenderAsync(req.TemplateName, req.TemplateModel);
            var msg = new SmsMessage
            {
                ToNumber = req.To,
                Message = message
            };
            var id = await _queueService.EnqueueAsync(msg);
            return CreatedAtAction("GetSmsById", "Sms", new { id }, msg);
        }
    }
}
