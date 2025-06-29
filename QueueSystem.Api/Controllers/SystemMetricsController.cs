using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Data;
using QueueSystem.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QueueSystem.Api.Controllers
{
    [ApiController]
    [Route("api/metrics")]
    public class SystemMetricsController : ControllerBase
    {
        private readonly QueueSystemDbContext _db;
        public SystemMetricsController(QueueSystemDbContext db) => _db = db;

        [HttpGet("email/summary")]
        public async Task<IActionResult> GetEmailSummary(int days = 5)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.EmailMessages
                .Where(e => e.CreatedAt >= fromDate)
                .GroupBy(e => e.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    sent = g.Count(x => x.Status == 1),
                    failed = g.Count(x => x.Status == 2),
                    expired = g.Count(x => x.Status == 3),
                    pending = g.Count(x => x.Status == 0),
                    failureRate = g.Count(x => x.Status == 2) / (double)Math.Max(1, g.Count())
                })
                .OrderBy(x => x.date)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("sms/summary")]
        public async Task<IActionResult> GetSmsSummary(int days = 5)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.SmsMessages
                .Where(e => e.CreatedAt >= fromDate)
                .GroupBy(e => e.CreatedAt.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    sent = g.Count(x => x.Status == 1),
                    failed = g.Count(x => x.Status == 2),
                    expired = g.Count(x => x.Status == 3),
                    pending = g.Count(x => x.Status == 0),
                    failureRate = g.Count(x => x.Status == 2) / (double)Math.Max(1, g.Count())
                })
                .OrderBy(x => x.date)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("email/current")]
        public async Task<IActionResult> GetEmailCurrent()
        {
            var utcNow = DateTime.UtcNow.Date;
            var sentToday = await _db.EmailMessages.CountAsync(e => e.Status == 1 && e.SentAt >= utcNow);
            var pending = await _db.EmailMessages.CountAsync(e => e.Status == 0);
            var failed = await _db.EmailMessages.CountAsync(e => e.Status == 2);
            var expired = await _db.EmailMessages.CountAsync(e => e.Status == 3);
            var total = sentToday + failed + expired + pending;
            var failureRate = total > 0 ? (failed / (double)total) : 0.0;
            return Ok(new { pending, failed, expired, sentToday, failureRate });
        }

        [HttpGet("sms/current")]
        public async Task<IActionResult> GetSmsCurrent()
        {
            var utcNow = DateTime.UtcNow.Date;
            var sentToday = await _db.SmsMessages.CountAsync(e => e.Status == 1 && e.SentAt >= utcNow);
            var pending = await _db.SmsMessages.CountAsync(e => e.Status == 0);
            var failed = await _db.SmsMessages.CountAsync(e => e.Status == 2);
            var expired = await _db.SmsMessages.CountAsync(e => e.Status == 3);
            var total = sentToday + failed + expired + pending;
            var failureRate = total > 0 ? (failed / (double)total) : 0.0;
            return Ok(new { pending, failed, expired, sentToday, failureRate });
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetSystemHealth()
        {
            // For demo: worker status and last run times are not tracked yet
            var emailCurrent = await GetEmailCurrent() as OkObjectResult;
            var smsCurrent = await GetSmsCurrent() as OkObjectResult;
            var workers = new
            {
                emailWorkerStatus = "Unknown",
                smsWorkerStatus = "Unknown",
                lastEmailWorkerRunAt = (DateTime?)null,
                lastSmsWorkerRunAt = (DateTime?)null
            };
            return Ok(new
            {
                email = emailCurrent?.Value,
                sms = smsCurrent?.Value,
                workers
            });
        }

        [HttpGet("email/volume")]
        public async Task<IActionResult> GetEmailVolume(string interval = "day", int days = 7)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.EmailMessages
                .Where(e => e.CreatedAt >= fromDate)
                .GroupBy(e => interval == "hour" ? e.CreatedAt.ToString("yyyy-MM-dd HH") : e.CreatedAt.Date.ToString("yyyy-MM-dd"))
                .Select(g => new
                {
                    interval = g.Key,
                    sent = g.Count(x => x.Status == 1),
                    failed = g.Count(x => x.Status == 2)
                })
                .OrderBy(x => x.interval)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("sms/volume")]
        public async Task<IActionResult> GetSmsVolume(string interval = "day", int days = 7)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.SmsMessages
                .Where(e => e.CreatedAt >= fromDate)
                .GroupBy(e => interval == "hour" ? e.CreatedAt.ToString("yyyy-MM-dd HH") : e.CreatedAt.Date.ToString("yyyy-MM-dd"))
                .Select(g => new
                {
                    interval = g.Key,
                    sent = g.Count(x => x.Status == 1),
                    failed = g.Count(x => x.Status == 2)
                })
                .OrderBy(x => x.interval)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("email/errors")]
        public async Task<IActionResult> GetEmailErrors(int days = 5, int top = 5)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.EmailMessages
                .Where(e => e.CreatedAt >= fromDate && e.ErrorMessage != null)
                .GroupBy(e => e.ErrorMessage)
                .Select(g => new { errorMessage = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(top)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("sms/errors")]
        public async Task<IActionResult> GetSmsErrors(int days = 5, int top = 5)
        {
            var utcNow = DateTime.UtcNow;
            var fromDate = utcNow.Date.AddDays(-days + 1);
            var data = await _db.SmsMessages
                .Where(e => e.CreatedAt >= fromDate && e.ErrorMessage != null)
                .GroupBy(e => e.ErrorMessage)
                .Select(g => new { errorMessage = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(top)
                .ToListAsync();
            return Ok(data);
        }
    }
}
