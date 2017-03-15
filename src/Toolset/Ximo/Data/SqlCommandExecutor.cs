using System;
using System.Data;
using System.Data.SqlClient;
using Ximo.Data.TransientStrategies;
using Ximo.TransientFaultHandling;

namespace Ximo.Data
{
    /// <summary>
    ///     Class with utilities and helpers for executing <see cref="SqlCommand" />(s).
    /// </summary>
    internal class SqlCommandExecutor : DisposableObject
    {
        private readonly CommandType _commandType;
        private readonly bool _connectionInternallyOwned;
        private readonly SqlParameterList _parameters;
        private readonly int _retryCount;
        private readonly int _retryWaitTimeInSeconds;
        private readonly SqlConnection _sqlConnection;
        private readonly SqlTransaction _sqlTransaction;
        private string _commandText;
        private bool _connectionInternallyOpened;
        private string _connectionString;
        private bool _readerMode;
        private RetryPolicy _retryPolicy;
        private SqlCommand _sqlCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlCommandExecutor" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        public SqlCommandExecutor(string connectionString, string commandText,
            CommandType commandType = CommandType.StoredProcedure, SqlParameterList parameters = null,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
            : this(
                new SqlConnection(connectionString), commandText, commandType, parameters, retryCount,
                retryWaitTimeInSeconds)
        {
            _connectionInternallyOwned = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlCommandExecutor" /> class.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        public SqlCommandExecutor(SqlConnection sqlConnection, string commandText,
            CommandType commandType = CommandType.StoredProcedure, SqlParameterList parameters = null,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
        {
            _sqlConnection = sqlConnection;
            _connectionString = _sqlConnection.ConnectionString;
            _retryCount = retryCount;
            _retryWaitTimeInSeconds = retryWaitTimeInSeconds;
            _commandText = commandText;
            _commandType = commandType;
            _parameters = parameters;
            _connectionInternallyOwned = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlCommandExecutor" /> class.
        /// </summary>
        /// <param name="sqlTransaction">The SQL transaction.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        public SqlCommandExecutor(SqlTransaction sqlTransaction, string commandText,
            CommandType commandType = CommandType.StoredProcedure, SqlParameterList parameters = null,
            int retryCount = 6, int retryWaitTimeInSeconds = 1)
            : this(sqlTransaction.Connection, commandText, commandType, parameters, retryCount, retryWaitTimeInSeconds)
        {
            _sqlTransaction = sqlTransaction;
        }

        private SqlCommand GetCommand
        {
            get
            {
                if (_sqlCommand == null)
                {
                    _sqlCommand = _sqlConnection.CreateSqlCommand(_commandText, _commandType, _parameters,
                        _sqlTransaction);
                    _sqlCommand.CommandTimeout = TimeSpan.FromHours(3).Seconds;
                }
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    _sqlConnection.OpenWithRetry(_retryCount, _retryWaitTimeInSeconds);
                    _connectionInternallyOpened = true;
                }
                return _sqlCommand;
            }
        }

        private RetryPolicy GetRetryPolicy => _retryPolicy ??
                                              (_retryPolicy =
                                                  RetryPolicy.CreateRetryPolicy(_retryCount, _retryWaitTimeInSeconds));

        /// <summary>
        ///     Executes the command and returns a scalar value.
        /// </summary>
        /// <typeparam name="T">The type of the scalar value.</typeparam>
        /// <returns>The scalar value of type <typeparamref name="T" />.</returns>
        public T ExecuteScalar<T>()
        {
            return GetRetryPolicy.ExecuteAction<T, SqlTransientExceptionStrategy>(() => (T) GetCommand.ExecuteScalar());
        }

        /// <summary>
        ///     Executes the command.
        /// </summary>
        /// <returns>The number of records affected.</returns>
        public int ExecuteNonQuery()
        {
            return GetRetryPolicy.ExecuteAction<int, SqlTransientExceptionStrategy>(() => GetCommand.ExecuteNonQuery());
        }

        /// <summary>
        ///     Executes the command and returns a <see cref="SqlDataReader" />.
        /// </summary>
        /// <returns>The <see cref="SqlDataReader" />.</returns>
        public SqlDataReader ExecuteReader()
        {
            _readerMode = true;
            Func<SqlDataReader> readerAction;
            if (_connectionInternallyOwned)
            {
                readerAction = () => GetCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            else
            {
                readerAction = () => GetCommand.ExecuteReader();
            }
            return GetRetryPolicy.ExecuteAction<SqlDataReader, SqlTransientExceptionStrategy>(readerAction);
        }

        protected override void Disposing()
        {
            if (!_readerMode)
            {
                if (_connectionInternallyOwned)
                {
                    if (_sqlConnection != null)
                    {
                        if (_sqlConnection.State != ConnectionState.Closed)
                        {
                            _sqlConnection.Close();
                        }
                        _sqlConnection.Dispose();
                    }
                }
                else if (_connectionInternallyOpened)
                {
                    _sqlConnection?.Close();
                }
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = null;
                }
                if (!string.IsNullOrEmpty(_commandText))
                {
                    _commandText = null;
                }
            }
            base.Disposing();
        }
    }
}