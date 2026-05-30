using System.Data;
using System;

namespace TodoApp.WinForms.DataAccess.Abstractions
{
    public abstract class BaseRepository
    {
        /// <summary>
        /// A method that encapsulates the creation and opening of a connection.
        /// </summary>
        /// <returns></returns>
        protected IDbConnection GetOpenedConnection()
        {
            IDbConnection connection = DbConnectionFactory.CreateConnection();
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Helper method for safely handling DBNull from a database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected T DBNullValueCheck<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }
    }
}
