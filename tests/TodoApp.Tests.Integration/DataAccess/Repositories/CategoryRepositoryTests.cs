using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Npgsql;
using TodoApp.Tests.Integration.Infrastructure.DatabaseFixtures;
using TodoApp.WinForms.DataAccess.Repositories.Implementations;
using TodoApp.WinForms.Models;
using Xunit;

namespace TodoApp.Tests.Integration.DataAccess.Repositories
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public sealed class CategoryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new CategoryRepository();
        }

        // Called automatically by xUnit before each test
        public async Task InitializeAsync()
            => await _fixture.ClearTablesAsync().ConfigureAwait(false);

        // Called automatically by xUnit after each test
        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task CreateAsync_ShouldInsertCategoryIntoDatabase()
        {
            // Arrange
            CategoryEntity category = new CategoryEntity { Name = "Integration Test Category" };

            // Act
            await _repository.CreateAsync(category).ConfigureAwait(false);

            // Assert
            List<CategoryEntity> allCategories = (await _repository.GetAllAsync().ConfigureAwait(false)).ToList();
            allCategories.Should().ContainSingle();
            allCategories.First().Name.Should().Be("Integration Test Category");
            allCategories.First().Id.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("no category")]
        [InlineData("NO CATEGORY")]
        [InlineData("No Category")]
        [InlineData("no Category")]
        [InlineData("No category")]
        public async Task CreateAsync_ShouldThrowPostgresException_WhenCategoryNameIsForbidden(string forbiddenName)
        {
            // Arrange
            CategoryEntity category = new CategoryEntity { Name = forbiddenName };

            // Act
            Func<Task> act = async () => await _repository.CreateAsync(category).ConfigureAwait(false);

            // Assert
            var exception = await act.Should().ThrowAsync<PostgresException>().ConfigureAwait(false);
            exception.Which.SqlState.Should().Be("23514", because: "PostgreSQL code for check_violation");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectCategory_WhenIdExists()
        {
            // Arrange
            CategoryEntity category = new CategoryEntity { Name = "FindMe" };
            await _repository.CreateAsync(category).ConfigureAwait(false);

            // Since we reset identity, the first inserted record will get Id = 1
            int expectedId = 1;

            // Act
            CategoryEntity result = await _repository.GetByIdAsync(expectedId).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedId);
            result.Name.Should().Be("FindMe");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            int nonExistingId = 999999;

            // Act
            CategoryEntity result = await _repository.GetByIdAsync(nonExistingId).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingCategoryInDatabase()
        {
            // Arrange
            CategoryEntity category = new CategoryEntity { Name = "Old Name" };
            await _repository.CreateAsync(category).ConfigureAwait(false);

            CategoryEntity existingCategory = (await _repository.GetAllAsync().ConfigureAwait(false)).First();
            existingCategory.Name = "New Super Name";

            // Act
            await _repository.UpdateAsync(existingCategory).ConfigureAwait(false);

            // Assert
            CategoryEntity updatedCategory = await _repository.GetByIdAsync(existingCategory.Id).ConfigureAwait(false);
            updatedCategory.Should().NotBeNull();
            updatedCategory.Name.Should().Be("New Super Name");
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotThrowAndNotModifyAnything_WhenCategoryDoesNotExist()
        {
            // Arrange
            CategoryEntity nonExistingCategory = new CategoryEntity
            {
                Id = 999999,
                Name = "Ghost Category"
            };

            // Act
            Func<Task> act = async () => await _repository.UpdateAsync(nonExistingCategory).ConfigureAwait(false);

            // Assert
            await act.Should().NotThrowAsync();

            // Verify database remains empty
            IEnumerable<CategoryEntity> allCategories = await _repository.GetAllAsync().ConfigureAwait(false);
            allCategories.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCategoryFromDatabase()
        {
            // Arrange
            CategoryEntity category = new CategoryEntity { Name = "To Be Deleted" };
            await _repository.CreateAsync(category).ConfigureAwait(false);
            CategoryEntity existingCategory = (await _repository.GetAllAsync().ConfigureAwait(false)).First();

            // Act
            await _repository.DeleteAsync(existingCategory.Id).ConfigureAwait(false);

            // Assert
            CategoryEntity result = await _repository.GetByIdAsync(existingCategory.Id).ConfigureAwait(false);
            result.Should().BeNull();

            IEnumerable<CategoryEntity> all = await _repository.GetAllAsync().ConfigureAwait(false);
            all.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotThrowAndNotRemoveAnything_WhenCategoryDoesNotExist()
        {
            // Arrange
            int nonExistingId = 999999;

            // Act
            Func<Task> act = async () => await _repository.DeleteAsync(nonExistingId).ConfigureAwait(false);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
