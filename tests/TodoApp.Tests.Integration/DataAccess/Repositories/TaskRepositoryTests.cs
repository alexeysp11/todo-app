using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using TodoApp.Tests.Integration.Infrastructure.DatabaseFixtures;
using TodoApp.WinForms.DataAccess.Repositories;
using TodoApp.WinForms.DataAccess.Repositories.Implementations;
using TodoApp.WinForms.Models;
using Xunit;

namespace TodoApp.Tests.Integration.DataAccess.Repositories
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public sealed class TaskRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskRepository _taskRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly PriorityRepository _priorityRepository;
        private readonly TaskStatusRepository _statusRepository;
        private readonly Fixture _autoFixture;

        public TaskRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;

            _taskRepository = new TaskRepository();
            _categoryRepository = new CategoryRepository();
            _priorityRepository = new PriorityRepository();
            _statusRepository = new TaskStatusRepository();

            _autoFixture = new Fixture();
        }

        public async Task InitializeAsync()
            => await _fixture.ClearTablesAsync().ConfigureAwait(false);

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTaskWithJoinedData_WhenIdExists()
        {
            // Arrange
            PriorityEntity priority = (await _priorityRepository.GetAllAsync().ConfigureAwait(false)).First(p => p.Name == "High");
            TaskStatusEntity status = (await _statusRepository.GetAllAsync().ConfigureAwait(false)).First(s => s.Name == "New");

            CategoryEntity category = new CategoryEntity { Name = "Work" };
            await _categoryRepository.CreateAsync(category).ConfigureAwait(false);
            CategoryEntity savedCategory = (await _categoryRepository.GetAllAsync().ConfigureAwait(false)).First();

            TodoTaskEntity taskToCreate = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, savedCategory.Id)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .With(t => t.DueDate, DateTime.Today.AddDays(5))
                .Create();

            await _taskRepository.CreateAsync(taskToCreate).ConfigureAwait(false);
            TodoTaskEntity savedTask = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).First();

            // Prepare expected object with fields populated by DB and JOINs
            TodoTaskEntity expectedTask = taskToCreate;
            expectedTask.Id = savedTask.Id;
            expectedTask.CategoryName = "Work";
            expectedTask.PriorityName = "High";
            expectedTask.StatusName = "New";

            // Act
            TodoTaskEntity result = await _taskRepository.GetByIdAsync(savedTask.Id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTask, options => options
                .Excluding(t => t.CreatedAt) // Handled separately due to database generation
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
                .WhenTypeIs<DateTime>());

            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow.ToLocalTime(), TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTasksOrderedByCreatedAtDescending()
        {
            // Arrange
            PriorityEntity priority = (await _priorityRepository.GetAllAsync().ConfigureAwait(false)).First(p => p.Name == "Medium");
            TaskStatusEntity status = (await _statusRepository.GetAllAsync().ConfigureAwait(false)).First(s => s.Name == "In progress");

            TodoTaskEntity olderTask = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, (int?)null)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .Create();

            TodoTaskEntity newerTask = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, (int?)null)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .Create();

            // Insert older task first
            await _taskRepository.CreateAsync(olderTask).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false); // Ensure distinct timestamps in DB

            // Insert newer task second
            await _taskRepository.CreateAsync(newerTask).ConfigureAwait(false);

            // Act
            List<TodoTaskEntity> result = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).ToList();

            // Assert
            result.Should().NotBeNull()
                .And.HaveCount(2);

            // Verify sorting order: index 0 must be the newer task, index 1 must be the older task
            result[0].Name.Should().Be(newerTask.Name);
            result[1].Name.Should().Be(olderTask.Name);
        }

        [Fact]
        public async Task GetByCategoryIdAsync_ShouldReturnOnlyTasksBelongingToSpecificCategory()
        {
            // Arrange
            PriorityEntity priority = (await _priorityRepository.GetAllAsync().ConfigureAwait(false)).First(p => p.Name == "Low");
            TaskStatusEntity status = (await _statusRepository.GetAllAsync().ConfigureAwait(false)).First(s => s.Name == "New");

            // Create target category
            CategoryEntity targetCategory = new CategoryEntity { Name = "Target Category" };
            await _categoryRepository.CreateAsync(targetCategory).ConfigureAwait(false);
            int targetCategoryId = (await _categoryRepository.GetAllAsync().ConfigureAwait(false))
                .First(c => c.Name == "Target Category").Id;

            // Create other category
            CategoryEntity otherCategory = new CategoryEntity { Name = "Other Category" };
            await _categoryRepository.CreateAsync(otherCategory).ConfigureAwait(false);
            int otherCategoryId = (await _categoryRepository.GetAllAsync().ConfigureAwait(false))
                .First(c => c.Name == "Other Category").Id;

            // Create tasks matching requirements
            TodoTaskEntity task1 = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, targetCategoryId)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .Create();

            TodoTaskEntity task2 = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, targetCategoryId)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .Create();

            TodoTaskEntity alternativeTask = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.CategoryId, otherCategoryId)
                .With(t => t.PriorityId, priority.Id)
                .With(t => t.StatusId, status.Id)
                .Create();

            await _taskRepository.CreateAsync(task1).ConfigureAwait(false);
            await _taskRepository.CreateAsync(task2).ConfigureAwait(false);
            await _taskRepository.CreateAsync(alternativeTask).ConfigureAwait(false);

            // Act
            List<TodoTaskEntity> result = (await _taskRepository.GetByCategoryIdAsync(targetCategoryId).ConfigureAwait(false)).ToList();

            // Assert
            result.Should().NotBeNull()
                .And.HaveCount(2)
                .And.OnlyContain(t => t.CategoryId == targetCategoryId);

            string[] expectedNames = new[] { task1.Name, task2.Name };
            result.Select(t => t.Name).Should().BeEquivalentTo(expectedNames)
                .And.NotContain(alternativeTask.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999999;

            // Act
            TodoTaskEntity result = await _taskRepository.GetByIdAsync(nonExistingTaskId).ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByCategoryIdAsync_ShouldReturnEmpty_WhenCategoryHasNoTasksOrDoesNotExist()
        {
            // Arrange
            int emptyCategoryId = 999999;

            // Act
            IEnumerable<TodoTaskEntity> result = await _taskRepository.GetByCategoryIdAsync(emptyCategoryId).ConfigureAwait(false);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertTaskIntoDatabase()
        {
            // Arrange
            IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync().ConfigureAwait(false);
            IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync().ConfigureAwait(false);

            TodoTaskEntity task = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.PriorityId, priorities.First().Id)
                .With(t => t.StatusId, statuses.First().Id)
                .With(t => t.CategoryId, (int?)null)
                .With(t => t.DueDate, DateTime.Today.AddDays(1))
                .Create();

            // Act
            await _taskRepository.CreateAsync(task).ConfigureAwait(false);

            // Assert
            List<TodoTaskEntity> allTasks = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).ToList();
            allTasks.Should().ContainSingle();

            TodoTaskEntity insertedTask = allTasks.First();
            insertedTask.Name.Should().Be(task.Name);
            insertedTask.Description.Should().Be(task.Description);
            insertedTask.PriorityId.Should().Be(task.PriorityId);
            insertedTask.StatusId.Should().Be(task.StatusId);
            insertedTask.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingTaskInDatabase()
        {
            // Arrange
            List<PriorityEntity> priorities = (await _priorityRepository.GetAllAsync().ConfigureAwait(false)).ToList();
            List<TaskStatusEntity> statuses = (await _statusRepository.GetAllAsync().ConfigureAwait(false)).ToList();

            TodoTaskEntity task = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.PriorityId, priorities[0].Id)
                .With(t => t.StatusId, statuses[0].Id)
                .With(t => t.CategoryId, (int?)null)
                .With(t => t.DueDate, DateTime.Today.AddDays(1))
                .Create();

            await _taskRepository.CreateAsync(task).ConfigureAwait(false);
            TodoTaskEntity existingTask = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).First();

            // Modify properties using properties from lookups and AutoFixture
            existingTask.Name = "Updated Task Name";
            existingTask.Description = "Updated Description";
            existingTask.PriorityId = priorities[1].Id;
            existingTask.StatusId = statuses[1].Id;

            // Act
            await _taskRepository.UpdateAsync(existingTask).ConfigureAwait(false);

            // Assert
            TodoTaskEntity updatedTask = await _taskRepository.GetByIdAsync(existingTask.Id).ConfigureAwait(false);
            updatedTask.Should().NotBeNull();
            updatedTask.Name.Should().Be("Updated Task Name");
            updatedTask.Description.Should().Be("Updated Description");
            updatedTask.PriorityId.Should().Be(priorities[1].Id);
            updatedTask.StatusId.Should().Be(statuses[1].Id);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldModifyOnlyStatusOfTask()
        {
            // Arrange
            IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync().ConfigureAwait(false);
            List<TaskStatusEntity> statuses = (await _statusRepository.GetAllAsync().ConfigureAwait(false)).ToList();

            TodoTaskEntity task = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.PriorityId, priorities.First().Id)
                .With(t => t.StatusId, statuses[0].Id) // e.g., "New"
                .With(t => t.CategoryId, (int?)null)
                .Create();

            await _taskRepository.CreateAsync(task).ConfigureAwait(false);
            TodoTaskEntity existingTask = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).First();
            int newStatusId = statuses[2].Id; // e.g., "Done"

            // Act
            await _taskRepository.UpdateStatusAsync(existingTask.Id, newStatusId).ConfigureAwait(false);

            // Assert
            TodoTaskEntity updatedTask = await _taskRepository.GetByIdAsync(existingTask.Id).ConfigureAwait(false);
            updatedTask.Should().NotBeNull();
            updatedTask.StatusId.Should().Be(newStatusId);

            // Other fields must remain unchanged
            updatedTask.Name.Should().Be(existingTask.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTaskFromDatabase()
        {
            // Arrange
            IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync().ConfigureAwait(false);
            IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync().ConfigureAwait(false);

            TodoTaskEntity task = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.PriorityId, priorities.First().Id)
                .With(t => t.StatusId, statuses.First().Id)
                .With(t => t.CategoryId, (int?)null)
                .Create();

            await _taskRepository.CreateAsync(task).ConfigureAwait(false);
            TodoTaskEntity existingTask = (await _taskRepository.GetAllAsync().ConfigureAwait(false)).First();

            // Act
            await _taskRepository.DeleteAsync(existingTask.Id).ConfigureAwait(false);

            // Assert
            TodoTaskEntity result = await _taskRepository.GetByIdAsync(existingTask.Id).ConfigureAwait(false);
            result.Should().BeNull();

            IEnumerable<TodoTaskEntity> allTasks = await _taskRepository.GetAllAsync().ConfigureAwait(false);
            allTasks.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotThrow_WhenTaskDoesNotExist()
        {
            // Arrange
            IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync().ConfigureAwait(false);
            IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync().ConfigureAwait(false);

            TodoTaskEntity nonExistingTask = _autoFixture.Build<TodoTaskEntity>()
                .With(t => t.Id, 999999)
                .With(t => t.PriorityId, priorities.First().Id)
                .With(t => t.StatusId, statuses.First().Id)
                .With(t => t.CategoryId, (int?)null)
                .Create();

            // Act
            Func<Task> act = async () => await _taskRepository.UpdateAsync(nonExistingTask).ConfigureAwait(false);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldNotThrow_WhenTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999999;
            IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync().ConfigureAwait(false);

            // Act
            Func<Task> act = async () => await _taskRepository.UpdateStatusAsync(nonExistingTaskId, statuses.First().Id).ConfigureAwait(false);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotThrow_WhenTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999999;

            // Act
            Func<Task> act = async () => await _taskRepository.DeleteAsync(nonExistingTaskId).ConfigureAwait(false);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
