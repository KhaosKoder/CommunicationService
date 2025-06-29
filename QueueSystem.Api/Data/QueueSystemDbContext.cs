using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Models;

namespace QueueSystem.Api.Data
{
    public class QueueSystemDbContext : DbContext
    {
        public QueueSystemDbContext(DbContextOptions<QueueSystemDbContext> options) : base(options) { }

        public DbSet<EmailMessage> EmailMessages { get; set; } = null!;
        public DbSet<SmsMessage> SmsMessages { get; set; } = null!;
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailMessage>()
                .HasIndex(e => e.Status)
                .HasDatabaseName("IX_EmailMessages_Status");

            modelBuilder.Entity<SmsMessage>()
                .HasIndex(s => s.Status)
                .HasDatabaseName("IX_SmsMessages_Status");
        }
    }
}
