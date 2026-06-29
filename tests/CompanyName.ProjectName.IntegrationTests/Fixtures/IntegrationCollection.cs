using Xunit;

namespace CompanyName.ProjectName.IntegrationTests.Fixtures;

[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<DatabaseFixture> { }
