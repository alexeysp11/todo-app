using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.DataAccess.Repositories
{
    public sealed class TaskRepository : BaseRepository, ITaskRepository
    {
        /// <summary>
        /// The common part of SELECT to avoid code duplication.
        /// </summary>
        private const string BaseSelectSql = @"
            SELECT t.id, t.name, t.description, t.due_date, t.created_at,
                   t.category_id, c.name as category_name,
                   t.priority_id, p.name as priority_name,
                   t.status_id, s.name as status_name
            FROM tasks t
            LEFT JOIN categories c ON t.category_id = c.id
            JOIN priorities p ON t.priority_id = p.id
            JOIN statuses s ON t.status_id = s.id";

        public async Task<TodoTaskEntity> GetByIdAsync(int id)
        {
            string sql = $"{BaseSelectSql} WHERE t.id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return MapToTodoTask(reader);
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<TodoTaskEntity>> GetAllAsync()
        {
            var tasks = new List<TodoTaskEntity>();
            string sql = $"{BaseSelectSql} ORDER BY t.created_at DESC;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    tasks.Add(MapToTodoTask(reader));
                }
            }
            return tasks;
        }

        public async Task<IEnumerable<TodoTaskEntity>> GetByCategoryIdAsync(int categoryId)
        {
            var tasks = new List<TodoTaskEntity>();
            string sql = $"{BaseSelectSql} WHERE t.category_id = @catId ORDER BY t.created_at DESC;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@catId", categoryId);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        tasks.Add(MapToTodoTask(reader));
                    }
                }
            }
            return tasks;
        }

        public async Task CreateAsync(TodoTaskEntity task)
        {
            const string sql = @"
                INSERT INTO tasks (name, description, category_id, priority_id, status_id, due_date)
                VALUES (@name, @desc, @catId, @priorityId, @statusId, @dueDate);";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@name", task.Name);
                cmd.Parameters.AddWithValue("@desc", (object)task.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@catId", (object)task.CategoryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@priorityId", task.PriorityId);
                cmd.Parameters.AddWithValue("@statusId", task.StatusId);
                cmd.Parameters.AddWithValue("@dueDate", (object)task.DueDate ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(TodoTaskEntity task)
        {
            const string sql = @"
                UPDATE tasks
                SET name = @name, description = @desc, category_id = @catId,
                    priority_id = @priorityId, status_id = @statusId, due_date = @dueDate
                WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", task.Id);
                cmd.Parameters.AddWithValue("@name", task.Name);
                cmd.Parameters.AddWithValue("@desc", (object)task.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@catId", (object)task.CategoryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@priorityId", task.PriorityId);
                cmd.Parameters.AddWithValue("@statusId", task.StatusId);
                cmd.Parameters.AddWithValue("@dueDate", (object)task.DueDate ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateStatusAsync(int id, int newStatusId)
        {
            const string sql = "UPDATE tasks SET status_id = @statusId WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@statusId", newStatusId);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM tasks WHERE id = @id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Helper method for mapping an <see cref="IDataReader"/> string to a <see cref="TodoTaskEntity"/> object.
        /// </summary>
        private TodoTaskEntity MapToTodoTask(IDataReader reader)
        {
            return new TodoTaskEntity
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString(),
                Description = DBNullValueCheck<string>(reader["description"]),
                DueDate = DBNullValueCheck<DateTime?>(reader["due_date"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),

                CategoryId = DBNullValueCheck<int?>(reader["category_id"]),
                CategoryName = DBNullValueCheck<string>(reader["category_name"]) ?? "No Category",

                PriorityId = Convert.ToInt32(reader["priority_id"]),
                PriorityName = reader["priority_name"].ToString(),

                StatusId = Convert.ToInt32(reader["status_id"]),
                StatusName = reader["status_name"].ToString()
            };
        }
    }
}
