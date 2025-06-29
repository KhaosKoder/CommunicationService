using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Configuration;
using QueueSystem.Api.Data;
using QueueSystem.Api.Repositories;
using QueueSystem.Api.Services;
using QueueSystem.Api.Workers;
using QueueSystem.Api.Sending;
using QueueSystem.Api.Templating;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Bind DatabaseSettings from configuration
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));

// Register sender settings from configuration
builder.Services.Configure<EmailSenderSettings>(builder.Configuration.GetSection("EmailSender"));
builder.Services.Configure<SmsSenderSettings>(builder.Configuration.GetSection("SmsSender"));

// Register Metrics settings
builder.Services.Configure<MetricsSettings>(builder.Configuration.GetSection("Metrics"));

// Register DotLiquid settings and template renderer
builder.Services.Configure<DotLiquidSettings>(builder.Configuration.GetSection("DotLiquid"));
builder.Services.AddSingleton<ITemplateRenderer, DotLiquidTemplateRenderer>();

// Register DbContext with connection string from configuration
builder.Services.AddDbContext<QueueSystemDbContext>((sp, options) =>
{
    var dbSettings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>().Value;
    options.UseSqlServer(dbSettings.ConnectionString);
});

// Register repositories
builder.Services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
builder.Services.AddScoped<ISmsMessageRepository, SmsMessageRepository>();
builder.Services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();

// Register queue services
builder.Services.AddScoped<IEmailQueueService, EmailQueueService>();
builder.Services.AddScoped<ISmsQueueService, SmsQueueService>();

// Register background workers
builder.Services.AddHostedService<EmailMessageWorker>();
builder.Services.AddHostedService<SmsMessageWorker>();

// Register sender stubs (replace with real implementations later)
builder.Services.AddSingleton<IEmailSender, StubEmailSender>();
builder.Services.AddSingleton<ISmsSender, StubSmsSender>();

// Register FluentValidation
builder.Services.AddControllers().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<QueueSystem.Api.Validation.EmailMessageValidator>();
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

