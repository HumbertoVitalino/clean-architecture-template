namespace CompanyName.ProjectName.Infrastructure.Repositories.Models;

public sealed class UserModel
{
    public UserModel() { }

    public UserModel(Guid id, string email, string name)
    {
        Id = id;
        Email = email;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
}
