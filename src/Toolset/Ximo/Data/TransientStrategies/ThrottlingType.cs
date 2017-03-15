namespace Ximo.Data.TransientStrategies
{
    /// <summary>
    ///     Defines the possible throttling types in SQL Database.
    /// </summary>
    public enum ThrottlingType
    {
        /// <summary>
        ///     Indicates that no throttling was applied to a given resource.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Corresponds to a soft throttling type. Soft throttling is applied when machine resources such as, CPU, I/O,
        ///     storage, and worker threads exceed
        ///     predefined safety thresholds despite the load balancer’s best efforts.
        /// </summary>
        Soft = 1,

        /// <summary>
        ///     Corresponds to a hard throttling type. Hard throttling is applied when the machine is out of resources, for example
        ///     storage space.
        ///     With hard throttling, no new connections are allowed to the databases hosted on the machine until resources are
        ///     freed up.
        /// </summary>
        Hard = 2,

        /// <summary>
        ///     Corresponds to an unknown throttling type in the event that the throttling type cannot be determined with
        ///     certainty.
        /// </summary>
        Unknown = 3
    }
}