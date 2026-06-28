using CompanyName.ProjectName.Domain.Users;
using FluentValidation;

namespace CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;

public sealed class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
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
