namespace CompanyName.ProjectName.Infrastructure.Persistence.Models;

public sealed class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
