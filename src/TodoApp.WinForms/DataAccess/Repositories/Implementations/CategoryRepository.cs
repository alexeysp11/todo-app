using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.DataAccess.Repositories.Implementations
{
    public sealed class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public async Task<CategoryEntity> GetByIdAsync(int id)
        {
            const string sql = "SELECT id, name FROM categories WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return new CategoryEntity
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<CategoryEntity>> GetAllAsync()
        {
            var categories = new List<CategoryEntity>();
            const string sql = "SELECT id, name FROM categories ORDER BY name;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    categories.Add(new CategoryEntity
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            return categories;
        }

        public async Task CreateAsync(CategoryEntity category)
        {
            const string sql = "INSERT INTO categories (name) VALUES (@name);";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@name", category.Name);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(CategoryEntity category)
        {
            const string sql = "UPDATE categories SET name = @name WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@name", category.Name);
                cmd.Parameters.AddWithValue("@id", category.Id);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM categories WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}
