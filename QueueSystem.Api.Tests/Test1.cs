using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using QueueSystem.Api.Data;
using QueueSystem.Api.Models;
using QueueSystem.Api.Repositories;
using QueueSystem.Api.Validation;
using QueueSystem.Api.Controllers;
using QueueSystem.Api.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Moq;

namespace QueueSystem.Api.Tests;

[TestClass]
public class EmailMessageRepositoryTests
{
    private QueueSystemDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<QueueSystemDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new QueueSystemDbContext(options);
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddEmailMessage()
    {
        var db = GetDbContext(nameof(AddAsync_ShouldAddEmailMessage));
        var repo = new EmailMessageRepository(db);
        var message = new EmailMessage
        {
            ToAddress = "test@example.com",
            Subject = "Test Subject",
            Body = "Test Body",
            CreatedAt = System.DateTime.UtcNow,
            Status = 0
        };
        await repo.AddAsync(message);
        Assert.AreEqual(1, db.EmailMessages.Count());
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnCorrectMessage()
    {
        var db = GetDbContext(nameof(GetByIdAsync_ShouldReturnCorrectMessage));
        var repo = new EmailMessageRepository(db);
        var message = new EmailMessage
        {
            ToAddress = "test2@example.com",
            Subject = "Test2",
            Body = "Body2",
            CreatedAt = System.DateTime.UtcNow,
            Status = 0
        };
        db.EmailMessages.Add(message);
        db.SaveChanges();
        var result = await repo.GetByIdAsync(message.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("test2@example.com", result!.ToAddress);
    }

    [TestMethod]
    public async Task GetPendingAsync_ShouldReturnPendingMessages()
    {
        var db = GetDbContext(nameof(GetPendingAsync_ShouldReturnPendingMessages));
        var repo = new EmailMessageRepository(db);
        db.EmailMessages.Add(new EmailMessage { ToAddress = "a@a.com", Subject = "A", Body = "A", CreatedAt = System.DateTime.UtcNow, Status = 0 });
        db.EmailMessages.Add(new EmailMessage { ToAddress = "b@b.com", Subject = "B", Body = "B", CreatedAt = System.DateTime.UtcNow, Status = 1 });
        db.SaveChanges();
        var pending = await repo.GetPendingAsync();
        Assert.AreEqual(1, pending.Count);
        Assert.AreEqual("a@a.com", pending[0].ToAddress);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateMessage()
    {
        var db = GetDbContext(nameof(UpdateAsync_ShouldUpdateMessage));
        var repo = new EmailMessageRepository(db);
        var message = new EmailMessage { ToAddress = "c@c.com", Subject = "C", Body = "C", CreatedAt = System.DateTime.UtcNow, Status = 0 };
        db.EmailMessages.Add(message);
        db.SaveChanges();
        message.Status = 2;
        await repo.UpdateAsync(message);
        var updated = db.EmailMessages.First();
        Assert.AreEqual(2, updated.Status);
    }

    [TestMethod]
    public async Task GetByStatusAsync_ShouldReturnCorrectStatus()
    {
        var db = GetDbContext(nameof(GetByStatusAsync_ShouldReturnCorrectStatus));
        var repo = new EmailMessageRepository(db);
        db.EmailMessages.Add(new EmailMessage { ToAddress = "d@d.com", Subject = "D", Body = "D", CreatedAt = System.DateTime.UtcNow, Status = 3 });
        db.EmailMessages.Add(new EmailMessage { ToAddress = "e@e.com", Subject = "E", Body = "E", CreatedAt = System.DateTime.UtcNow, Status = 0 });
        db.SaveChanges();
        var result = await repo.GetByStatusAsync(3);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("d@d.com", result[0].ToAddress);
    }
}

[TestClass]
public class SmsMessageRepositoryTests
{
    private QueueSystemDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<QueueSystemDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new QueueSystemDbContext(options);
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddSmsMessage()
    {
        var db = GetDbContext(nameof(AddAsync_ShouldAddSmsMessage));
        var repo = new SmsMessageRepository(db);
        var message = new SmsMessage
        {
            ToNumber = "1234567890",
            Message = "Test Message",
            CreatedAt = System.DateTime.UtcNow,
            Status = 0
        };
        await repo.AddAsync(message);
        Assert.AreEqual(1, db.SmsMessages.Count());
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnCorrectSmsMessage()
    {
        var db = GetDbContext(nameof(GetByIdAsync_ShouldReturnCorrectSmsMessage));
        var repo = new SmsMessageRepository(db);
        var message = new SmsMessage
        {
            ToNumber = "5555555555",
            Message = "Hello",
            CreatedAt = System.DateTime.UtcNow,
            Status = 0
        };
        db.SmsMessages.Add(message);
        db.SaveChanges();
        var result = await repo.GetByIdAsync(message.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("5555555555", result!.ToNumber);
    }

    [TestMethod]
    public async Task GetPendingAsync_ShouldReturnPendingSmsMessages()
    {
        var db = GetDbContext(nameof(GetPendingAsync_ShouldReturnPendingSmsMessages));
        var repo = new SmsMessageRepository(db);
        db.SmsMessages.Add(new SmsMessage { ToNumber = "1", Message = "A", CreatedAt = System.DateTime.UtcNow, Status = 0 });
        db.SmsMessages.Add(new SmsMessage { ToNumber = "2", Message = "B", CreatedAt = System.DateTime.UtcNow, Status = 1 });
        db.SaveChanges();
        var pending = await repo.GetPendingAsync();
        Assert.AreEqual(1, pending.Count);
        Assert.AreEqual("1", pending[0].ToNumber);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateSmsMessage()
    {
        var db = GetDbContext(nameof(UpdateAsync_ShouldUpdateSmsMessage));
        var repo = new SmsMessageRepository(db);
        var message = new SmsMessage { ToNumber = "3", Message = "C", CreatedAt = System.DateTime.UtcNow, Status = 0 };
        db.SmsMessages.Add(message);
        db.SaveChanges();
        message.Status = 2;
        await repo.UpdateAsync(message);
        var updated = db.SmsMessages.First();
        Assert.AreEqual(2, updated.Status);
    }

    [TestMethod]
    public async Task GetByStatusAsync_ShouldReturnCorrectStatusSmsMessages()
    {
        var db = GetDbContext(nameof(GetByStatusAsync_ShouldReturnCorrectStatusSmsMessages));
        var repo = new SmsMessageRepository(db);
        db.SmsMessages.Add(new SmsMessage { ToNumber = "4", Message = "D", CreatedAt = System.DateTime.UtcNow, Status = 3 });
        db.SmsMessages.Add(new SmsMessage { ToNumber = "5", Message = "E", CreatedAt = System.DateTime.UtcNow, Status = 0 });
        db.SaveChanges();
        var result = await repo.GetByStatusAsync(3);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("4", result[0].ToNumber);
    }
}

[TestClass]
public class EmailMessageValidatorTests
{
    [TestMethod]
    public void ValidEmailMessage_PassesValidation()
    {
        var validator = new EmailMessageValidator();
        var message = new EmailMessage
        {
            ToAddress = "valid@email.com",
            Subject = "Subject",
            Body = "Body"
        };
        var result = validator.Validate(message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void InvalidEmailMessage_FailsValidation()
    {
        var validator = new EmailMessageValidator();
        var message = new EmailMessage
        {
            ToAddress = "not-an-email",
            Subject = "",
            Body = ""
        };
        var result = validator.Validate(message);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ToAddress"));
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Subject"));
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Body"));
    }
}

[TestClass]
public class SmsMessageValidatorTests
{
    [TestMethod]
    public void ValidSmsMessage_PassesValidation()
    {
        var validator = new SmsMessageValidator();
        var message = new SmsMessage
        {
            ToNumber = "1234567890",
            Message = "Hello!"
        };
        var result = validator.Validate(message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void InvalidSmsMessage_FailsValidation()
    {
        var validator = new SmsMessageValidator();
        var message = new SmsMessage
        {
            ToNumber = "",
            Message = ""
        };
        var result = validator.Validate(message);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ToNumber"));
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Message"));
    }
}

[TestClass]
public class SmsTemplateRequestValidatorTests
{
    [TestMethod]
    public void ValidSmsTemplateRequest_PassesValidation()
    {
        var validator = new SmsTemplateRequestValidator();
        var request = new SmsTemplateController.SmsTemplateRequest { To = "123", TemplateName = "Test", TemplateModel = new { Name = "A" } };
        var result = validator.Validate(request);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void InvalidSmsTemplateRequest_FailsValidation()
    {
        var validator = new SmsTemplateRequestValidator();
        var request = new SmsTemplateController.SmsTemplateRequest { To = "", TemplateName = "", TemplateModel = null! };
        var result = validator.Validate(request);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count > 0);
    }
}

[TestClass]
public class SystemSettingsRepositoryTests
{
    private QueueSystemDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<QueueSystemDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new QueueSystemDbContext(options);
    }

    [TestMethod]
    public async Task SetAsync_ShouldAddNewSetting()
    {
        var db = GetDbContext(nameof(SetAsync_ShouldAddNewSetting));
        var repo = new SystemSettingsRepository(db);
        await repo.SetAsync("TestKey", "TestValue");
        var setting = db.SystemSettings.FirstOrDefault(s => s.Key == "TestKey");
        Assert.IsNotNull(setting);
        Assert.AreEqual("TestValue", setting!.Value);
    }

    [TestMethod]
    public async Task SetAsync_ShouldUpdateExistingSetting()
    {
        var db = GetDbContext(nameof(SetAsync_ShouldUpdateExistingSetting));
        db.SystemSettings.Add(new SystemSetting { Key = "TestKey", Value = "OldValue" });
        db.SaveChanges();
        var repo = new SystemSettingsRepository(db);
        await repo.SetAsync("TestKey", "NewValue");
        var setting = db.SystemSettings.FirstOrDefault(s => s.Key == "TestKey");
        Assert.IsNotNull(setting);
        Assert.AreEqual("NewValue", setting!.Value);
    }

    [TestMethod]
    public async Task GetByKeyAsync_ShouldReturnCorrectSetting()
    {
        var db = GetDbContext(nameof(GetByKeyAsync_ShouldReturnCorrectSetting));
        db.SystemSettings.Add(new SystemSetting { Key = "TestKey", Value = "TestValue" });
        db.SaveChanges();
        var repo = new SystemSettingsRepository(db);
        var setting = await repo.GetByKeyAsync("TestKey");
        Assert.IsNotNull(setting);
        Assert.AreEqual("TestValue", setting!.Value);
    }

    [TestMethod]
    public async Task GetByKeyAsync_ShouldReturnNullForMissingKey()
    {
        var db = GetDbContext(nameof(GetByKeyAsync_ShouldReturnNullForMissingKey));
        var repo = new SystemSettingsRepository(db);
        var setting = await repo.GetByKeyAsync("MissingKey");
        Assert.IsNull(setting);
    }
}

[TestClass]
public class EmailQueueServiceTests
{
    [TestMethod]
    public async Task EnqueueAsync_ShouldSetStatusAndCreatedAt_AndReturnId()
    {
        var repo = new Mock<IEmailMessageRepository>();
        EmailMessage? addedMessage = null;
        repo.Setup(r => r.AddAsync(It.IsAny<EmailMessage>())).Callback<EmailMessage>(m => addedMessage = m).Returns(Task.CompletedTask);
        var service = new EmailQueueService(repo.Object);
        var message = new EmailMessage { ToAddress = "a@b.com", Subject = "S", Body = "B" };
        var id = await service.EnqueueAsync(message);
        Assert.AreEqual(0, message.Status);
        Assert.IsTrue((System.DateTime.UtcNow - message.CreatedAt).TotalSeconds < 5);
        Assert.AreEqual(message.Id, id);
        Assert.AreSame(message, addedMessage);
    }

    [TestMethod]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var repo = new Mock<IEmailMessageRepository>();
        var expected = new EmailMessage { Id = 42 };
        repo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(expected);
        var service = new EmailQueueService(repo.Object);
        var result = await service.GetByIdAsync(42);
        Assert.AreSame(expected, result);
    }

    [TestMethod]
    public async Task GetPendingAsync_DelegatesToRepository()
    {
        var repo = new Mock<IEmailMessageRepository>();
        var expected = new List<EmailMessage> { new EmailMessage() };
        repo.Setup(r => r.GetPendingAsync(10)).ReturnsAsync(expected);
        var service = new EmailQueueService(repo.Object);
        var result = await service.GetPendingAsync(10);
        Assert.AreSame(expected, result);
    }

    [TestMethod]
    public async Task UpdateAsync_DelegatesToRepository()
    {
        var repo = new Mock<IEmailMessageRepository>();
        var message = new EmailMessage();
        repo.Setup(r => r.UpdateAsync(message)).Returns(Task.CompletedTask).Verifiable();
        var service = new EmailQueueService(repo.Object);
        await service.UpdateAsync(message);
        repo.Verify();
    }
}

[TestClass]
public class SmsQueueServiceTests
{
    [TestMethod]
    public async Task EnqueueAsync_ShouldSetStatusAndCreatedAt_AndReturnId()
    {
        var repo = new Mock<ISmsMessageRepository>();
        SmsMessage? addedMessage = null;
        repo.Setup(r => r.AddAsync(It.IsAny<SmsMessage>())).Callback<SmsMessage>(m => addedMessage = m).Returns(Task.CompletedTask);
        var service = new SmsQueueService(repo.Object);
        var message = new SmsMessage { ToNumber = "123", Message = "Body" };
        var id = await service.EnqueueAsync(message);
        Assert.AreEqual(0, message.Status);
        Assert.IsTrue((System.DateTime.UtcNow - message.CreatedAt).TotalSeconds < 5);
        Assert.AreEqual(message.Id, id);
        Assert.AreSame(message, addedMessage);
    }

    [TestMethod]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var repo = new Mock<ISmsMessageRepository>();
        var expected = new SmsMessage { Id = 42 };
        repo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(expected);
        var service = new SmsQueueService(repo.Object);
        var result = await service.GetByIdAsync(42);
        Assert.AreSame(expected, result);
    }

    [TestMethod]
    public async Task GetPendingAsync_DelegatesToRepository()
    {
        var repo = new Mock<ISmsMessageRepository>();
        var expected = new List<SmsMessage> { new SmsMessage() };
        repo.Setup(r => r.GetPendingAsync(10)).ReturnsAsync(expected);
        var service = new SmsQueueService(repo.Object);
        var result = await service.GetPendingAsync(10);
        Assert.AreSame(expected, result);
    }

    [TestMethod]
    public async Task UpdateAsync_DelegatesToRepository()
    {
        var repo = new Mock<ISmsMessageRepository>();
        var message = new SmsMessage();
        repo.Setup(r => r.UpdateAsync(message)).Returns(Task.CompletedTask).Verifiable();
        var service = new SmsQueueService(repo.Object);
        await service.UpdateAsync(message);
        repo.Verify();
    }
}
