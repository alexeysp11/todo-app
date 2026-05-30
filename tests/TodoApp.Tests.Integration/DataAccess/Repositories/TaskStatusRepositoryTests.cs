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
    public sealed class TaskStatusRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskStatusRepository _repository;

        public TaskStatusRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TaskStatusRepository();
        }

        // Called automatically by xUnit before each test
        public async Task InitializeAsync()
            => await _fixture.ClearTablesAsync().ConfigureAwait(false);

        // Called automatically by xUnit after each test
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllAsync_ShouldReturnPredefinedStatuses()
        {
            // Arrange
            string[] expectedStatuses = new[] { "New", "In progress", "Done" };

            // Act
            List<TaskStatusEntity> result = (await _repository.GetAllAsync().ConfigureAwait(false)).ToList();

            // Assert
            result.Should().NotBeNull()
                .And.HaveCount(3)
                .And.OnlyContain(s => s.Id > 0);

            result.Select(s => s.Name).Should().BeEquivalentTo(expectedStatuses);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectStatus_WhenIdExists()
        {
            // Arrange
            List<TaskStatusEntity> allStatuses = (await _repository.GetAllAsync().ConfigureAwait(false)).ToList();
            TaskStatusEntity targetStatus = allStatuses.First();

            // Act
            TaskStatusEntity result = await _repository.GetByIdAsync(targetStatus.Id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(targetStatus.Id);
            result.Name.Should().Be(targetStatus.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenStatusDoesNotExist()
        {
            // Arrange
            int nonExistingId = 999999;

            // Act
            TaskStatusEntity result = await _repository.GetByIdAsync(nonExistingId).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }
    }
}
