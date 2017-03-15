using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ximo.Data.TransientStrategies
{
    /// <summary>
    ///     Implements an object that holds the decoded reason code returned from SQL Database when throttling conditions are
    ///     encountered.
    /// </summary>
    public class ThrottlingCondition
    {
        /// <summary>
        ///     Gets the error number that corresponds to the throttling conditions reported by SQL Database.
        /// </summary>
        public const int ThrottlingErrorNumber = 40501;

        /// <summary>
        ///     Provides a compiled regular expression used to extract the reason code from the error message.
        /// </summary>
        private static readonly Regex SqlErrorCodeRegEx = new Regex(@"Code:\s*(\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        ///     Maintains a collection of key/value pairs where a key is the resource type and a value is the type of throttling
        ///     applied to the given resource type.
        /// </summary>
        private readonly IList<Tuple<ThrottledResourceType, ThrottlingType>> _throttledResources =
            new List<Tuple<ThrottledResourceType, ThrottlingType>>(9);

        /// <summary>
        ///     Gets an unknown throttling condition in the event that the actual throttling condition cannot be determined.
        /// </summary>
        public static ThrottlingCondition Unknown
        {
            get
            {
                var unknownCondition = new ThrottlingCondition {ThrottlingMode = ThrottlingMode.Unknown};
                unknownCondition._throttledResources.Add(Tuple.Create(ThrottledResourceType.Unknown,
                    ThrottlingType.Unknown));

                return unknownCondition;
            }
        }

        /// <summary>
        ///     Gets the value that reflects the throttling mode in SQL Database.
        /// </summary>
        public ThrottlingMode ThrottlingMode { get; private set; }

        /// <summary>
        ///     Gets a list of the resources in the SQL Database that were subject to throttling conditions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "As designed")]
        public IEnumerable<Tuple<ThrottledResourceType, ThrottlingType>> ThrottledResources
        {
            get { return _throttledResources; }
        }

        /// <summary>
        ///     Gets a value that indicates whether physical data file space throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnDataSpace
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.PhysicalDatabaseSpace); }
        }

        /// <summary>
        ///     Gets a value that indicates whether physical log space throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnLogSpace
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.PhysicalLogSpace); }
        }

        /// <summary>
        ///     Gets a value that indicates whether transaction activity throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnLogWrite
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.LogWriteIoDelay); }
        }

        /// <summary>
        ///     Gets a value that indicates whether data read activity throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnDataRead
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.DataReadIoDelay); }
        }

        /// <summary>
        ///     Gets a value that indicates whether CPU throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnCpu
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.Cpu); }
        }

        /// <summary>
        ///     Gets a value that indicates whether database size throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnDatabaseSize
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.DatabaseSize); }
        }

        /// <summary>
        ///     Gets a value that indicates whether concurrent requests throttling was reported by SQL Database.
        /// </summary>
        public bool IsThrottledOnWorkerThreads
        {
            get { return _throttledResources.Any(x => x.Item1 == ThrottledResourceType.WorkerThreads); }
        }

        /// <summary>
        ///     Gets a value that indicates whether throttling conditions were not determined with certainty.
        /// </summary>
        public bool IsUnknown
        {
            get { return ThrottlingMode == ThrottlingMode.Unknown; }
        }

        /// <summary>
        ///     Determines throttling conditions from the specified SQL exception.
        /// </summary>
        /// <param name="ex">
        ///     The <see cref="SqlException" /> object that contains information relevant to an error returned by SQL
        ///     Server when throttling conditions were encountered.
        /// </param>
        /// <returns>
        ///     An instance of the object that holds the decoded reason codes returned from SQL Database when throttling
        ///     conditions were encountered.
        /// </returns>
        public static ThrottlingCondition FromException(SqlException ex)
        {
            if (ex != null)
            {
                foreach (SqlError error in ex.Errors)
                {
                    if (error.Number == ThrottlingErrorNumber)
                    {
                        return FromError(error);
                    }
                }
            }

            return Unknown;
        }

        /// <summary>
        ///     Determines the throttling conditions from the specified SQL error.
        /// </summary>
        /// <param name="error">
        ///     The <see cref="SqlError" /> object that contains information relevant to a warning or error
        ///     returned by SQL Server.
        /// </param>
        /// <returns>
        ///     An instance of the object that holds the decoded reason codes returned from SQL Database when throttling
        ///     conditions were encountered.
        /// </returns>
        public static ThrottlingCondition FromError(SqlError error)
        {
            if (error != null)
            {
                var match = SqlErrorCodeRegEx.Match(error.Message);
                int reasonCode;

                if (match.Success && int.TryParse(match.Groups[1].Value, out reasonCode))
                {
                    return FromReasonCode(reasonCode);
                }
            }

            return Unknown;
        }

        /// <summary>
        ///     Determines the throttling conditions from the specified reason code.
        /// </summary>
        /// <param name="reasonCode">
        ///     The reason code returned by SQL Database that contains the throttling mode and the exceeded
        ///     resource types.
        /// </param>
        /// <returns>
        ///     An instance of the object holding the decoded reason codes returned from SQL Database when encountering
        ///     throttling conditions.
        /// </returns>
        public static ThrottlingCondition FromReasonCode(int reasonCode)
        {
            if (reasonCode > 0)
            {
                // Decode throttling mode from the last 2 bits.
                var throttlingMode = (ThrottlingMode) (reasonCode & 3);

                var condition = new ThrottlingCondition {ThrottlingMode = throttlingMode};

                // Shift 8 bits to truncate throttling mode.
                var groupCode = reasonCode >> 8;

                // Determine throttling type for all well-known resources that may be subject to throttling conditions.
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.PhysicalDatabaseSpace,
                    (ThrottlingType) (groupCode & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.PhysicalLogSpace,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.LogWriteIoDelay,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.DataReadIoDelay,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.Cpu,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.DatabaseSize,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.Internal,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.WorkerThreads,
                    (ThrottlingType) ((groupCode = groupCode >> 2) & 3)));
                condition._throttledResources.Add(Tuple.Create(ThrottledResourceType.Internal,
                    (ThrottlingType) ((groupCode >> 2) & 3)));

                return condition;
            }

            return Unknown;
        }

        /// <summary>
        ///     Returns a textual representation of the current ThrottlingCondition object, including the information held with
        ///     respect to throttled resources.
        /// </summary>
        /// <returns>A string that represents the current ThrottlingCondition object.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();

            result.AppendFormat(CultureInfo.CurrentCulture, "Mode: {0} | ", ThrottlingMode);

            var resources =
                _throttledResources
                    .Where(x => x.Item1 != ThrottledResourceType.Internal)
                    .Select(x => string.Format(CultureInfo.CurrentCulture, "{0}: {1}", x.Item1, x.Item2))
                    .OrderBy(x => x).ToArray();

            result.Append(string.Join(", ", resources));

            return result.ToString();
        }
    }
}