using FluentValidation;

namespace QueueSystem.Api.Validation
{
    public class SmsTemplateRequestValidator : AbstractValidator<QueueSystem.Api.Controllers.SmsTemplateController.SmsTemplateRequest>
    {
        public SmsTemplateRequestValidator()
        {
            RuleFor(x => x.To).NotEmpty().MaximumLength(32);
            RuleFor(x => x.TemplateName).NotEmpty();
            RuleFor(x => x.TemplateModel).NotNull();
        }
    }
}
