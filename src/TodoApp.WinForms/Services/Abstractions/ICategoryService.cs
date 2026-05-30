using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.Services.Abstractions
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CategoryEntity category);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync();
        Task<CategoryEntity> GetCategoryByIdAsync(int id);
        Task UpdateCategoryAsync(CategoryEntity category);
    }
}