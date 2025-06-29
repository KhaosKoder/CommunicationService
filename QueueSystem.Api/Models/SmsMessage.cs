using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueSystem.Api.Models
{
    public class SmsMessage
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(32)]
        public string ToNumber { get; set; } = null!;

        [Required, MaxLength(512)]
        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public int Attempts { get; set; }
        public int Status { get; set; }
        [MaxLength(1024)]
        public string? ErrorMessage { get; set; }
    }
}
