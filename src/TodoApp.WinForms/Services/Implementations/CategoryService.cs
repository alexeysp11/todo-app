using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Abstractions;

namespace TodoApp.WinForms.Services.Implementations
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync()
        {
            return _categoryRepository.GetAllAsync();
        }

        public Task<CategoryEntity> GetCategoryByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid category ID for update.");
            return _categoryRepository.GetByIdAsync(id);
        }

        public Task CreateCategoryAsync(CategoryEntity category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be empty.");
            if (category.Name.Length > 50)
                throw new ArgumentException("The category name must not exceed 50 characters.");

            // Prevent system-reserved keyword collision.
            if (category.Name.Equals("No Category", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The name 'No Category' is reserved by the system.");

            return _categoryRepository.CreateAsync(category);
        }

        public Task UpdateCategoryAsync(CategoryEntity category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (category.Id <= 0) throw new ArgumentException("Invalid category ID for update.");
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be empty.");
            if (category.Name.Length > 50)
                throw new ArgumentException("The category name must not exceed 50 characters.");

            return _categoryRepository.UpdateAsync(category);
        }

        public Task DeleteCategoryAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid category ID for delete.");
            return _categoryRepository.DeleteAsync(id);
        }
    }
}
