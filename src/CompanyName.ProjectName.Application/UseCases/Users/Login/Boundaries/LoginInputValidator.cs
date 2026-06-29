using CompanyName.ProjectName.Domain.Users;
using FluentValidation;

namespace CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;

public sealed class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .EmailAddress();
    }
}
