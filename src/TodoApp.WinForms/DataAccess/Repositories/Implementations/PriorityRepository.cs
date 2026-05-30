using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;

namespace TodoApp.WinForms.DataAccess.Repositories.Implementations
{
    public sealed class PriorityRepository : BaseRepository, IPriorityRepository
    {
        public async Task<IEnumerable<PriorityEntity>> GetAllAsync()
        {
            var list = new List<PriorityEntity>();
            const string sql = "SELECT id, name FROM priorities ORDER BY id;";

            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    list.Add(new PriorityEntity
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            return list;
        }

        public async Task<PriorityEntity> GetByIdAsync(int id)
        {
            const string sql = "SELECT id, name FROM priorities WHERE id = @id;";
            using (var conn = GetOpenedConnection())
            using (var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return new PriorityEntity
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }
            return null;
        }

        public Task CreateAsync(PriorityEntity entity) => throw new NotImplementedException();
        public Task UpdateAsync(PriorityEntity entity) => throw new NotImplementedException();
        public Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}
