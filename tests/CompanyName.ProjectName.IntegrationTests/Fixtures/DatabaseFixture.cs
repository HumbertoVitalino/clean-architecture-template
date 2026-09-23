using CompanyName.ProjectName.Application.IoC;
using CompanyName.ProjectName.Infrastructure.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace CompanyName.ProjectName.IntegrationTests.Fixtures;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private const string DefaultTestConnectionString =
        "Host=localhost;Port=5432;Database=integration_tests_db;Username=postgres;Password=Integration@Test123";

    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var testConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? DefaultTestConnectionString;

        await ExecuteInitScriptAsync(testConnectionString);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = testConnectionString,
                ["Jwt:SecretKey"] = "integration-test-secret-key-min-32-chars!!",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:ExpirationInMinutes"] = "60"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddApplication();
        services.AddInfrastructure(configuration);

        Services = services.BuildServiceProvider();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static async Task ExecuteInitScriptAsync(string connectionString)
    {
        var sqlPath = Path.Combine(AppContext.BaseDirectory, "sql", "init.sql");
        var sql = await File.ReadAllTextAsync(sqlPath);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
