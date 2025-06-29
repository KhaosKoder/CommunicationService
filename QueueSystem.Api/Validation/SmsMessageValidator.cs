using FluentValidation;

namespace QueueSystem.Api.Validation
{
    public class SmsMessageValidator : AbstractValidator<QueueSystem.Api.Models.SmsMessage>
    {
        public SmsMessageValidator()
        {
            RuleFor(x => x.ToNumber).NotEmpty().MaximumLength(32);
            RuleFor(x => x.Message).NotEmpty().MaximumLength(512);
        }
    }
}
