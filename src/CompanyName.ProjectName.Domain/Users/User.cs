using CompanyName.ProjectName.Domain.Abstractions;
using CompanyName.ProjectName.Domain.Users.Events;

namespace CompanyName.ProjectName.Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    private User(Guid id, Email email, string name) : base(id)
    {
        Email = email;
        Name = name;
    }

    public Email Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    public static User Create(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(UserErrors.NameEmpty);

        var userEmail = Email.Create(email);
        var user = new User(Guid.NewGuid(), userEmail, name);
        user.RaiseDomainEvent(new UserCreatedEvent(user.Id));

        return user;
    }

    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
    }

    public static User Reconstitute(Guid id, Email email, string name) =>
        new(id, email, name);
}
