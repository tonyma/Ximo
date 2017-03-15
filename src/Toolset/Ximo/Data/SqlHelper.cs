using System.Data;
using System.Data.SqlClient;
using Ximo.Validation;

namespace Ximo.Data
{
    public static class SqlHelper
    {
        /// <summary>
        ///     Executes the command and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The scalar value.</returns>
        public static T ExecuteScalar<T>(string connectionString, string commandText, SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNullOrEmpty(connectionString, nameof(connectionString));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(connectionString, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteScalar<T>();
            }
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The number of affected records.</returns>
        public static int ExecuteNonQuery(string connectionString, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNullOrEmpty(connectionString, nameof(connectionString));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(connectionString, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the command and returns a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The <see cref="SqlDataReader" />qlDataReader.</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNullOrEmpty(connectionString, nameof(connectionString));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            var executor = new SqlCommandExecutor(connectionString, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds);
            return executor.ExecuteReader();
        }

        /// <summary>
        ///     Executes the command and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection">The SQL connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The scalar value.</returns>
        public static T ExecuteScalar<T>(SqlConnection sqlConnection, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlConnection, nameof(sqlConnection));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(sqlConnection, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteScalar<T>();
            }
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The number of affected records.</returns>
        public static int ExecuteNonQuery(SqlConnection sqlConnection, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlConnection, nameof(sqlConnection));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(sqlConnection, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the command and returns a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The <see cref="SqlDataReader" />.</returns>
        public static SqlDataReader ExecuteReader(SqlConnection sqlConnection, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlConnection, nameof(sqlConnection));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            var executor = new SqlCommandExecutor(sqlConnection, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds);
            return executor.ExecuteReader();
        }

        /// <summary>
        ///     Executes the command and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTransaction">The SQL transaction.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The scalar value.</returns>
        public static T ExecuteScalar<T>(SqlTransaction sqlTransaction, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlTransaction, nameof(sqlTransaction));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(sqlTransaction, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteScalar<T>();
            }
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <param name="sqlTransaction">The SQL transaction.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>The number of affected records.</returns>
        public static int ExecuteNonQuery(SqlTransaction sqlTransaction, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlTransaction, nameof(sqlTransaction));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            using (var executor = new SqlCommandExecutor(sqlTransaction, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds))
            {
                return executor.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Executes the command and returns a <see cref="SqlDataReader" />.
        /// </summary>
        /// <param name="sqlTransaction">The SQL transaction.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>A <see cref="SqlDataReader" />.</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction sqlTransaction, string commandText,
            SqlParameterList parameters = null,
            CommandType commandType = CommandType.StoredProcedure,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            Check.NotNull(sqlTransaction, nameof(sqlTransaction));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            var executor = new SqlCommandExecutor(sqlTransaction, commandText, commandType,
                parameters, retryCount, retryWaitTimeInSeconds);
            return executor.ExecuteReader();
        }
    }
}