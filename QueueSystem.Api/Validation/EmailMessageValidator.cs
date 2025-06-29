using FluentValidation;

namespace QueueSystem.Api.Validation
{
    public class EmailMessageValidator : AbstractValidator<QueueSystem.Api.Models.EmailMessage>
    {
        public EmailMessageValidator()
        {
            RuleFor(x => x.ToAddress).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.Subject).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Body).NotEmpty();
        }
    }
}
