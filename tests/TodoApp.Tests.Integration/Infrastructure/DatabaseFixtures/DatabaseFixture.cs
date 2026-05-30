using System.Threading.Tasks;
using System;
using TodoApp.WinForms.DataAccess;
using Npgsql;

namespace TodoApp.Tests.Integration.Infrastructure.DatabaseFixtures
{
    public sealed class DatabaseFixture : IDisposable
    {
        // Before each test class run, we completely clear the category table
        // And we reset the SERIAL (id) counter so that tests start with predictable IDs.
        public async Task ClearTablesAsync()
        {
            const string sql = "TRUNCATE TABLE tasks, categories RESTART IDENTITY CASCADE;";

            using (var conn = (NpgsqlConnection)DbConnectionFactory.CreateConnection())
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                await conn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
        }
    }
}
