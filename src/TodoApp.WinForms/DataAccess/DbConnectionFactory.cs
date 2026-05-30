using System.Configuration;
using System.Data;
using Npgsql;

namespace TodoApp.WinForms.DataAccess
{
    public static class DbConnectionFactory
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["TodoDbConnection"]?.ConnectionString;

        public static IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
