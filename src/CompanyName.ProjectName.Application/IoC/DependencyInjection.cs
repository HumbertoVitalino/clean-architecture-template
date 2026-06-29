using CompanyName.ProjectName.Application.Interfaces.UseCases;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser;
using CompanyName.ProjectName.Application.UseCases.Users.CreateUser.Boundaries;
using CompanyName.ProjectName.Application.UseCases.Users.GetUserById;
using CompanyName.ProjectName.Application.UseCases.Users.Login;
using CompanyName.ProjectName.Application.UseCases.Users.Login.Boundaries;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyName.ProjectName.Application.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        services.AddScoped<CreateUserInputValidator>();

        services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();

        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<LoginInputValidator>();

        return services;
    }
}
