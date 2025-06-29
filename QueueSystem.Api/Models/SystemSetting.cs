using System.ComponentModel.DataAnnotations;

namespace QueueSystem.Api.Models
{
    public class SystemSetting
    {
        [Key]
        [MaxLength(128)]
        public string Key { get; set; } = null!;

        [MaxLength(2048)]
        public string Value { get; set; } = null!;
    }
}
