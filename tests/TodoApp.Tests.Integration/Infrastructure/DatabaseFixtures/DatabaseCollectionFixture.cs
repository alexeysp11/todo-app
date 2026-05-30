using Xunit;

namespace TodoApp.Tests.Integration.Infrastructure.DatabaseFixtures
{
    [CollectionDefinition(nameof(DatabaseCollectionFixture))]
    public sealed class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture>
    {
    }
}
