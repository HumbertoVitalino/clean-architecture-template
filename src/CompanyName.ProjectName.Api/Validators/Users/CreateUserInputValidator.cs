using CompanyName.ProjectName.Api.Requests.Users;
using CompanyName.ProjectName.Domain.Users;
using FluentValidation;

namespace CompanyName.ProjectName.Api.Validators.Users;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .EmailAddress();
    }
}
