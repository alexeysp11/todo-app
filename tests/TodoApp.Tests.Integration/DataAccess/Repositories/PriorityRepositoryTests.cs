using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TodoApp.Tests.Integration.Infrastructure.DatabaseFixtures;
using TodoApp.WinForms.DataAccess.Repositories.Implementations;
using TodoApp.WinForms.Models;
using Xunit;

namespace TodoApp.Tests.Integration.DataAccess.Repositories
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public sealed class PriorityRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly PriorityRepository _repository;

        public PriorityRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new PriorityRepository();
        }

        // Called automatically by xUnit before each test
        public async Task InitializeAsync()
            => await _fixture.ClearTablesAsync().ConfigureAwait(false);

        // Called automatically by xUnit after each test
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllAsync_ShouldReturnPredefinedPriorities()
        {
            // Arrange
            string[] expectedNames = new[] { "Low", "Medium", "High" };

            // Act
            List<PriorityEntity> result = (await _repository.GetAllAsync().ConfigureAwait(false)).ToList();

            // Assert
            result.Should().NotBeNull()
                .And.HaveCount(3)
                .And.OnlyContain(s => s.Id > 0);

            result.Select(s => s.Name).Should().BeEquivalentTo(expectedNames);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectPriority_WhenIdExists()
        {
            // Arrange
            List<PriorityEntity> allPriorities = (await _repository.GetAllAsync().ConfigureAwait(false)).ToList();
            PriorityEntity targetPriority = allPriorities.First();

            // Act
            PriorityEntity result = await _repository.GetByIdAsync(targetPriority.Id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(targetPriority.Id);
            result.Name.Should().Be(targetPriority.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPriorityDoesNotExist()
        {
            // Arrange
            int nonExistingId = 999999;

            // Act
            PriorityEntity result = await _repository.GetByIdAsync(nonExistingId).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }
    }
}
