using CompanyName.ProjectName.Domain.Users;
using CompanyName.ProjectName.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyName.ProjectName.Infrastructure.Persistence.Configurations;

internal sealed class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}
