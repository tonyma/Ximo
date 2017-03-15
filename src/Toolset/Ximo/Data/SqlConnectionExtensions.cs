using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ximo.Data.TransientStrategies;
using Ximo.Extensions;
using Ximo.TransientFaultHandling;
using Ximo.Validation;

namespace Ximo.Data
{
    /// <summary>
    ///     Class with <see cref="SqlConnection" /> utilities.
    /// </summary>
    public static class SqlConnectionExtensions
    {
        /// <summary>
        ///     Checks if the <see cref="SqlConnection" /> object is within a specified set of states.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection" /> instance.</param>
        /// <param name="states">The states to be checked.</param>
        /// <returns><c>true</c> if the instance's state is one of the specified state, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connection" /> is null. </exception>
        public static bool StateIsWithin(this SqlConnection connection, params ConnectionState[] states)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return states != null && states.Length > 0 && states.Where(x => (connection.State & x) == x).Any();
        }

        /// <summary>
        ///     Determines whether the <see cref="SqlConnection" /> instance is in a specified state.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection" /> instance.</param>
        /// <param name="state">The <see cref="ConnectionState" /> to be checked.</param>
        /// <returns><c>true</c> if the sqlConnection is in the specified state; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connection" /> is null. </exception>
        public static bool IsInState(this SqlConnection connection, ConnectionState state)
        {
            return connection.StateIsWithin(state);
        }

        /// <summary>
        ///     Determines whether the <see cref="SqlConnection" /> instance is in not in a specified state.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection" /> instance.</param>
        /// <param name="state">The <see cref="ConnectionState" /> to be checked.</param>
        /// <returns><c>true</c> if the sqlConnection is not in the specified state; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connection" /> is null. </exception>
        public static bool IsNotInState(this SqlConnection connection, ConnectionState state)
        {
            return !connection.IsInState(state);
        }

        /// <summary>
        ///     If the <see cref="SqlConnection" /> instance's state is not <see cref="ConnectionState.Open" /> it attempts to open
        ///     it.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection" /> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connection" /> is null. </exception>
        public static void OpenIfNot(this SqlConnection connection)
        {
            if (connection.IsNotInState(ConnectionState.Open))
            {
                connection.Open();
            }
        }

        /// <summary>
        ///     Opens the specified sqlConnection.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection" /> instance.</param>
        /// <param name="numberOfRetries">The number of retries to open before failing.</param>
        /// <param name="timeBetweenRetriesInSeconds">The time between retries in seconds.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="connection" /> is null. </exception>
        public static void OpenWithRetry(this SqlConnection connection, int numberOfRetries = 6,
            int timeBetweenRetriesInSeconds = 1)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var policy = RetryPolicy.CreateRetryPolicy(numberOfRetries, timeBetweenRetriesInSeconds);
            policy.ExecuteAction<NetworkConnectivityErrorDetectionStrategy>(connection.Open);
        }

        /// <summary>
        ///     Creates a SQL command fot the <see cref="SqlConnection" /> instance.
        /// </summary>
        /// <param name="sqlConnection">The <see cref="SqlConnection" /> instance.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">{Optional} The parameters.</param>
        /// <param name="sqlTransaction">{Optional} The SQL transaction.</param>
        /// <returns>The constructed <see cref="SqlCommand" />.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     sqlConnection
        ///     or
        ///     commandText
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the <paramref name="sqlConnection" /> is null or
        ///     <paramref name="commandText" /> is null or empty.
        /// </exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static SqlCommand CreateSqlCommand(this SqlConnection sqlConnection, string commandText,
            CommandType commandType = CommandType.Text, IEnumerable<SqlParameter> parameters = null,
            SqlTransaction sqlTransaction = null)
        {
            Check.NotNull(sqlConnection, nameof(sqlConnection));
            Check.NotNullOrEmpty(commandText, nameof(commandText));

            var command = new SqlCommand(commandText, sqlConnection) {CommandType = commandType};
            if (sqlTransaction != null)
            {
                command.Transaction = sqlTransaction;
            }
            // ReSharper disable once PossibleMultipleEnumeration
            if (!parameters.IsNullOrEmpty())
            {
                // ReSharper disable once PossibleMultipleEnumeration
                // ReSharper disable once PossibleNullReferenceException
                foreach (var sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }
            }
            return command;
        }
    }
}