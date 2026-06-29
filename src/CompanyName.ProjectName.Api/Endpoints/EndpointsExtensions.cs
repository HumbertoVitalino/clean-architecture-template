using Asp.Versioning;
using Asp.Versioning.Builder;
using CompanyName.ProjectName.Api.Endpoints.Auth;
using CompanyName.ProjectName.Api.Endpoints.Users;

namespace CompanyName.ProjectName.Api.Endpoints;

public static class EndpointsExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        app.MapUsers(versionSet);
        app.MapAuth(versionSet);

        return app;
    }
}
