using FluentValidation;
using KudosApp.Domain.DTOs.Ai;

namespace KudosApp.Domain.Validators;

public class SuggestMessageRequestValidator : AbstractValidator<SuggestMessageRequest>
{
    public SuggestMessageRequestValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty();
        RuleFor(x => x.Intent).NotEmpty().MinimumLength(3).MaximumLength(200);
    }
}
