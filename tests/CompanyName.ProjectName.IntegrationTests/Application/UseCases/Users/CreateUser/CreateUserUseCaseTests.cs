using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CompanyName.ProjectName.IntegrationTests.Application.UseCases.Users.CreateUser;

[Collection("Integration")]
public sealed class CreateUserUseCaseTests(DatabaseFixture fixture)
{
    [Fact(DisplayName = "ExecuteAsync >> Should Create User >> When Input Is Valid")]
    public async Task ExecuteAsync_ShouldCreateUser_WhenInputIsValid()
    {
        // Arrange
        using var scope = fixture.Services.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ICreateUserUseCase>();
        var input = new CreateUserInput($"{Guid.NewGuid():N}@example.com", "John Doe");

        // Act
        var output = await useCase.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeTrue();
        var response = output.Result as UserResponse;
        response.Should().NotBeNull();
        response!.Email.Should().Be(input.Email.ToLowerInvariant());
        response.Name.Should().Be(input.Name);
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Error >> When Email Already Exists")]
    public async Task ExecuteAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        using var scope = fixture.Services.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ICreateUserUseCase>();
        var email = $"{Guid.NewGuid():N}@example.com";

        await useCase.ExecuteAsync(new CreateUserInput(email, "First User"));

        // Act
        var output = await useCase.ExecuteAsync(new CreateUserInput(email, "Second User"));

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Error >> When Input Is Invalid")]
    public async Task ExecuteAsync_ShouldReturnError_WhenInputIsInvalid()
    {
        // Arrange
        using var scope = fixture.Services.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ICreateUserUseCase>();
        var input = new CreateUserInput("not-an-email", string.Empty);

        // Act
        var output = await useCase.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().NotBeEmpty();
    }
}
