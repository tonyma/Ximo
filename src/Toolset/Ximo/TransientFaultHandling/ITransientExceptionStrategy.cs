using System;

namespace Ximo.TransientFaultHandling
{
    /// <summary>
    ///     Defines the contract of a utility class that checks if an <see cref="Exception" /> is transient (requires retries).
    /// </summary>
    public interface ITransientExceptionStrategy
    {
        /// <summary>
        ///     Determines whether the specified exception is transient.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns><c>true</c> if the specified exception is transient; otherwise, <c>false</c>.</returns>
        bool IsTransient(Exception exception);
    }
}