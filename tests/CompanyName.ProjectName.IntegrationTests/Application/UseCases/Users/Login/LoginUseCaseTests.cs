using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;
using CompanyName.ProjectName.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CompanyName.ProjectName.IntegrationTests.Application.UseCases.Users.Login;

[Collection("Integration")]
public sealed class LoginUseCaseTests(DatabaseFixture fixture)
{
    [Fact(DisplayName = "ExecuteAsync >> Should Return Token >> When User Exists")]
    public async Task ExecuteAsync_ShouldReturnToken_WhenUserExists()
    {
        // Arrange
        using var scope = fixture.Services.CreateScope();
        var createUseCase = scope.ServiceProvider.GetRequiredService<ICreateUserUseCase>();
        var loginUseCase = scope.ServiceProvider.GetRequiredService<ILoginUseCase>();

        var email = $"{Guid.NewGuid():N}@example.com";
        await createUseCase.ExecuteAsync(new CreateUserInput(email, "Alice"));

        // Act
        var output = await loginUseCase.ExecuteAsync(new LoginInput(email));

        // Assert
        output.IsValid.Should().BeTrue();
        var response = output.Result as LoginResponse;
        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Error >> When Email Does Not Match")]
    public async Task ExecuteAsync_ShouldReturnError_WhenEmailDoesNotMatch()
    {
        // Arrange
        using var scope = fixture.Services.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ILoginUseCase>();

        // Act
        var output = await useCase.ExecuteAsync(new LoginInput("nobody@example.com"));

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().NotBeEmpty();
    }
}
