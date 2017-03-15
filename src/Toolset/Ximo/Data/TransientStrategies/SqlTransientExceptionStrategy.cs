using System;
using System.Data.SqlClient;
using Ximo.TransientFaultHandling;

namespace Ximo.Data.TransientStrategies
{
    public class SqlTransientExceptionStrategy : ITransientExceptionStrategy
    {
        /// <summary>
        ///     Determines whether the specified <see cref="Exception" /> is transient.
        /// </summary>
        /// <remarks>
        ///     <para>Usually used with Sql Azure. Sql Error numbers checked are:</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Error_Number</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>11001</term>
        ///             <description>
        ///                 A network-related or instance-specific error occurred while establishing a connection to SQL Server.
        ///                 The server was not found or was not accessible. Verify that the instance name is correct and that SQL
        ///                 Server is configured to allow remote connections. (provider: TCP Provider, error: 0 - No such host is
        ///                 known.)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>40501</term>
        ///             <description>The service is currently busy. Retry the request after 10 seconds.</description>
        ///         </item>
        ///         <item>
        ///             <term>10928</term>
        ///             <description>The limit for the database has been reached.</description>
        ///         </item>
        ///         <item>
        ///             <term>10929</term>
        ///             <description>
        ///                 The server is currently too busy to support the minimum guarantee of requests for this
        ///                 database.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>10053</term>
        ///             <description>
        ///                 A transport-level error has occurred when receiving results from the server.
        ///                 An established connection was aborted by the software in your host machine.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>10054</term>
        ///             <description>
        ///                 A transport-level error has occurred when sending the request to the server.
        ///                 (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>10060</term>
        ///             <description>
        ///                 A network-related or instance-specific error occurred while establishing a connection to SQL Server.
        ///                 The server was not found or was not accessible. Verify that the instance name is correct and that SQL
        ///                 Server
        ///                 is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt
        ///                 failed
        ///                 because the connected party did not properly respond after a period of time, or established connection
        ///                 failed
        ///                 because connected host has failed to respond.)"
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>10060</term>
        ///             <description>The service has encountered an error processing your request. Please try again.</description>
        ///         </item>
        ///         <item>
        ///             <term>40540</term>
        ///             <description>The service has encountered an error processing your request. Please try again.</description>
        ///         </item>
        ///         <item>
        ///             <term>40613</term>
        ///             <description>
        ///                 Database is not currently available. Please retry the connection later. If the problem persists,
        ///                 contact customer
        ///                 support, and provide them the session tracing ID.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>40143</term>
        ///             <description>The service has encountered an error processing your request. Please try again.</description>
        ///         </item>
        ///         <item>
        ///             <term>233</term>
        ///             <description>
        ///                 The client was unable to establish a connection because of an error during connection initialization
        ///                 process before login.
        ///                 Possible causes include the following: the client tried to connect to an unsupported version of SQL
        ///                 Server; the server was too busy
        ///                 to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed
        ///                 connections) on the server.
        ///                 (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>64</term>
        ///             <description>
        ///                 A connection was successfully established with the server, but then an error occurred during
        ///                 the login process.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>20</term>
        ///             <description>The instance of SQL Server you attempted to connect to does not support encryption.</description>
        ///         </item>
        ///     </list>
        ///     <para></para>
        /// </remarks>
        /// <param name="exception">The <see cref="Exception" /> instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Exception" /> is transient; otherwise, <c>false</c>.
        /// </returns>
        /// <sqlException cref="ArgumentNullException">Thrown when the <paramref name="exception" /> is null. </sqlException>
        public bool IsTransient(Exception exception)
        {
            var sqlException = exception as SqlException;
            if (sqlException != null)
            {
                foreach (SqlError sqlError in sqlException.Errors)
                {
                    switch (sqlError.Number)
                    {
                        case ThrottlingCondition.ThrottlingErrorNumber:
                            // Decode the reason code from the error message to determine the grounds for throttling.
                            var condition = ThrottlingCondition.FromError(sqlError);

                            // Attach the decoded values as additional attributes to the original SQL exception.
                            sqlException.Data[condition.ThrottlingMode.GetType().Name] =
                                condition.ThrottlingMode.ToString();
                            sqlException.Data[condition.GetType().Name] = condition;
                            return true;
                        case 11001:
                        case 10928:
                        case 10929:
                        case 10053:
                        case 10054:
                        case 10060:
                        case 40197:
                        case 40540:
                        case 40613:
                        case 40143:
                        case 233:
                        case 64:
                        case 20:
                            return true;
                    }
                }
            }
            else if (exception is TimeoutException)
            {
                return true;
            }
            return false;
        }
    }
}