using CompanyName.ProjectName.Api.Requests.Auth;
using CompanyName.ProjectName.Domain.Users;
using FluentValidation;

namespace CompanyName.ProjectName.Api.Validators.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .EmailAddress();
    }
}
