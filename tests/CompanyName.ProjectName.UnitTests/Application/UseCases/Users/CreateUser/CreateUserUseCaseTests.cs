using CompanyName.ProjectName.Application.DTOs.Users;
using CompanyName.ProjectName.Application.Interfaces;
using CompanyName.ProjectName.Application.Interfaces.Repositories;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Domain.Users;
using FluentAssertions;
using Moq;
using Xunit;

namespace CompanyName.ProjectName.UnitTests.Application.UseCases.Users.CreateUser;

public sealed class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateUserInputValidator _validator;
    private readonly CreateUserUseCase _sut;

    public CreateUserUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _validator = new CreateUserInputValidator();
        _sut = new CreateUserUseCase(_repositoryMock.Object, _validator);
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Success Output With User Response >> When Input Is Valid")]
    public async Task ExecuteAsync_ValidInput_ReturnsSuccessOutputWithUserResponse()
    {
        // Arrange
        var input = new CreateUserInput("John Doe", "john@example.com");
        _repositoryMock
            .Setup(r => r.ExistsWithEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var output = await _sut.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeTrue();
        output.Result.Should().BeOfType<UserResponse>();
        var response = (UserResponse)output.Result!;
        response.Name.Should().Be("John Doe");
        response.Email.Should().Be("john@example.com");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = "ExecuteAsync >> Should Return Invalid Output With Error Messages >> When Input Is Invalid")]
    [InlineData("", "john@example.com")]
    [InlineData("John Doe", "")]
    [InlineData("John Doe", "not-an-email")]
    public async Task ExecuteAsync_InvalidInput_ReturnsInvalidOutputWithMessages(string name, string email)
    {
        // Arrange
        var input = new CreateUserInput(name, email);

        // Act
        var output = await _sut.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Invalid Output With Email In Use Error >> When Email Is Already Registered")]
    public async Task ExecuteAsync_DuplicateEmail_ReturnsInvalidOutputWithEmailInUseError()
    {
        // Arrange
        var input = new CreateUserInput("John Doe", "john@example.com");
        _repositoryMock
            .Setup(r => r.ExistsWithEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var output = await _sut.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().Contain(UserErrors.EmailAlreadyInUse);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "ExecuteAsync >> Should Return Invalid Output With Commit Error >> When Commit Fails")]
    public async Task ExecuteAsync_CommitFails_ReturnsInvalidOutput()
    {
        // Arrange
        var input = new CreateUserInput("John Doe", "john@example.com");
        _repositoryMock
            .Setup(r => r.ExistsWithEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var output = await _sut.ExecuteAsync(input);

        // Assert
        output.IsValid.Should().BeFalse();
        output.ErrorMessages.Should().NotBeEmpty();
    }
}
