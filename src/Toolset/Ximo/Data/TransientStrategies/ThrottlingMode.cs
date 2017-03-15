namespace Ximo.Data.TransientStrategies
{
    /// <summary>
    ///     Defines the possible throttling modes in SQL Database.
    /// </summary>
    public enum ThrottlingMode
    {
        /// <summary>
        ///     Corresponds to the "No Throttling" throttling mode, in which all SQL statements can be processed.
        /// </summary>
        NoThrottling = 0,

        /// <summary>
        ///     Corresponds to the "Reject Update / Insert" throttling mode, in which SQL statements such as INSERT, UPDATE, CREATE
        ///     TABLE, and CREATE INDEX are rejected.
        /// </summary>
        RejectUpdateInsert = 1,

        /// <summary>
        ///     Corresponds to the "Reject All Writes" throttling mode, in which SQL statements such as INSERT, UPDATE, DELETE,
        ///     CREATE, and DROP are rejected.
        /// </summary>
        RejectAllWrites = 2,

        /// <summary>
        ///     Corresponds to the "Reject All" throttling mode, in which all SQL statements are rejected.
        /// </summary>
        RejectAll = 3,

        /// <summary>
        ///     Corresponds to an unknown throttling mode whereby throttling mode cannot be determined with certainty.
        /// </summary>
        Unknown = -1
    }
}