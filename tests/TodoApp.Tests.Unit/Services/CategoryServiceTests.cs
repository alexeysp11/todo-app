using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Implementations;
using Xunit;

namespace TodoApp.Tests.Unit.Services
{
    public sealed class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_categoryRepoMock.Object);
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var invalidCategory = new CategoryEntity { Name = "   " };

            // Act
            Func<Task> act = async () => await _categoryService.CreateCategoryAsync(invalidCategory).ConfigureAwait(false);

            // Assert
            // Asterisks indicate a substring search (similar to Contains).
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Category name cannot be empty*")
                .ConfigureAwait(false);

            // Check that the repository was not called
            _categoryRepoMock.Verify(r => r.CreateAsync(It.IsAny<CategoryEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateCategory_ShouldCallRepository_WhenCategoryIsValid()
        {
            // Arrange
            var validCategory = new CategoryEntity { Name = "Sport" };

            // Act
            await _categoryService.CreateCategoryAsync(validCategory).ConfigureAwait(false);

            // Assert
            _categoryRepoMock.Verify(r => r.CreateAsync(validCategory), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowException_WhenNameIsReserved()
        {
            // Arrange
            var invalidCategory = new CategoryEntity { Name = "no category" };

            // Act
            Func<Task> act = async () => await _categoryService.CreateCategoryAsync(invalidCategory).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*reserved*")
                .ConfigureAwait(false);

            _categoryRepoMock.Verify(r => r.CreateAsync(It.IsAny<CategoryEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowException_WhenNameExceeds50Characters()
        {
            // Arrange
            // Generating a string exactly 51 characters long
            var invalidCategory = new CategoryEntity { Name = new string('A', 51) };

            // Act
            Func<Task> act = async () => await _categoryService.CreateCategoryAsync(invalidCategory).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .ConfigureAwait(false);

            _categoryRepoMock.Verify(r => r.CreateAsync(It.IsAny<CategoryEntity>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetCategoryById_ShouldThrowException_WhenIdIsInvalid(int invalidId)
        {
            // Act
            Func<Task> act = async () => await _categoryService.GetCategoryByIdAsync(invalidId).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().ConfigureAwait(false);
        }
    }
}
