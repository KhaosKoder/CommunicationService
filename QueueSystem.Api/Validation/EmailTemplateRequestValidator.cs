using FluentValidation;

namespace QueueSystem.Api.Validation
{
    public class EmailTemplateRequestValidator : AbstractValidator<QueueSystem.Api.Controllers.EmailTemplateController.EmailTemplateRequest>
    {
        public EmailTemplateRequestValidator()
        {
            RuleFor(x => x.To).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.Subject).NotEmpty().MaximumLength(256);
            RuleFor(x => x.TemplateName).NotEmpty();
            RuleFor(x => x.TemplateModel).NotNull();
        }
    }
}
