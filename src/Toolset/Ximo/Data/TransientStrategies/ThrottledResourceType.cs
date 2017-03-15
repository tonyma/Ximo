using System.Diagnostics.CodeAnalysis;

namespace Ximo.Data.TransientStrategies
{
    /// <summary>
    ///     Defines the types of resources in SQL Database that may be subject to throttling conditions.
    /// </summary>
    public enum ThrottledResourceType
    {
        /// <summary>
        ///     Corresponds to the "Physical Database Space" resource, which may be subject to throttling.
        /// </summary>
        PhysicalDatabaseSpace = 0,

        /// <summary>
        ///     Corresponds to the "Physical Log File Space" resource, which may be subject to throttling.
        /// </summary>
        PhysicalLogSpace = 1,

        /// <summary>
        ///     Corresponds to the "Transaction Log Write IO Delay" resource, which may be subject to throttling.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Io",
            Justification = "As designed")] LogWriteIoDelay = 2,

        /// <summary>
        ///     Corresponds to the "Database Read IO Delay" resource, which may be subject to throttling.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Io",
            Justification = "As designed")] DataReadIoDelay = 3,

        /// <summary>
        ///     Corresponds to the "CPU" resource, which may be subject to throttling.
        /// </summary>
        Cpu = 4,

        /// <summary>
        ///     Corresponds to the "Database Size" resource, which may be subject to throttling.
        /// </summary>
        DatabaseSize = 5,

        /// <summary>
        ///     Corresponds to the "SQL Worker Thread Pool" resource, which may be subject to throttling.
        /// </summary>
        WorkerThreads = 7,

        /// <summary>
        ///     Corresponds to an internal resource that may be subject to throttling.
        /// </summary>
        Internal = 6,

        /// <summary>
        ///     Corresponds to an unknown resource type in the event that the actual resource cannot be determined with certainty.
        /// </summary>
        Unknown = -1
    }
}