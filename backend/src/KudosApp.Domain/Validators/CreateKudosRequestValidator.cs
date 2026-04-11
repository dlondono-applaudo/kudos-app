using FluentValidation;
using KudosApp.Domain.DTOs.Kudos;

namespace KudosApp.Domain.Validators;

public class CreateKudosRequestValidator : AbstractValidator<CreateKudosRequest>
{
    public CreateKudosRequestValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty();
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Message).NotEmpty().MinimumLength(5).MaximumLength(500);
    }
}
