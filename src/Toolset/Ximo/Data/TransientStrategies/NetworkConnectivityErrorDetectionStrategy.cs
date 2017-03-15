using System;
using System.Data.SqlClient;
using Ximo.TransientFaultHandling;

namespace Ximo.Data.TransientStrategies
{
    public sealed class NetworkConnectivityErrorDetectionStrategy : ITransientExceptionStrategy
    {
        /// <summary>
        ///     Determines whether the specified exception is transient.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the specified exception is transient; otherwise, <c>false</c>.</returns>
        public bool IsTransient(Exception exception)
        {
            SqlException sqlException;

            if (exception != null && (sqlException = exception as SqlException) != null)
            {
                switch (sqlException.Number)
                {
                    // SQL Error Code: 11001
                    // A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
                    // The server was not found or was not accessible. Verify that the instance name is correct and that SQL 
                    // Server is configured to allow remote connections. (provider: TCP Provider, error: 0 - No such host is known.)
                    case 11001:
                        return true;
                }
            }

            return false;
        }
    }
}