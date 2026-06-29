using CompanyName.ProjectName.Api.Endpoints.Auth;
using CompanyName.ProjectName.Api.Endpoints.Users;

namespace CompanyName.ProjectName.Api.Endpoints;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapUsers();
        app.MapAuth();

        return app;
    }
}
