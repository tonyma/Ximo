using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ximo.TransientFaultHandling
{
    /// <summary>
    ///     Defines a policy for retrying operations.
    /// </summary>
    public class RetryPolicy
    {
        private RetryPolicy(int numberOfRetries, TimeSpan retryWaitTime)
        {
            NumberOfRetries = numberOfRetries;
            RetryWaitTime = retryWaitTime;
            Exceptions = new Stack<Exception>();
        }

        /// <summary>
        ///     Gets the number of retries.
        /// </summary>
        /// <value>The number of retries.</value>
        public int NumberOfRetries { get; }

        /// <summary>
        ///     Gets the retry wait time.
        /// </summary>
        /// <value>The retry wait time.</value>
        public TimeSpan RetryWaitTime { get; }

        /// <summary>
        ///     Gets the exceptions that occurred during the retries.
        /// </summary>
        /// <value>The exceptions.</value>
        public Stack<Exception> Exceptions { get; }

        /// <summary>
        ///     Creates the retry policy.
        /// </summary>
        /// <param name="retryWaitTime">The retry wait time.</param>
        /// <param name="numberOfRetries">The number of retries.</param>
        /// <returns>RetryPolicy.</returns>
        /// <exception cref="System.ArgumentException">The number of retries cannot be less than 2.</exception>
        public static RetryPolicy CreateRetryPolicy(TimeSpan retryWaitTime, int numberOfRetries = 6)
        {
            if (numberOfRetries < 2)
            {
                throw new ArgumentException("The number of retries cannot be less than 2.");
            }
            return new RetryPolicy(numberOfRetries, retryWaitTime);
        }

        /// <summary>
        ///     Creates the retry policy.
        /// </summary>
        /// <param name="numberOfRetries">The number of retries.</param>
        /// <param name="retryWaitTimeInSeconds">The retry wait time in seconds.</param>
        /// <returns>RetryPolicy.</returns>
        public static RetryPolicy CreateRetryPolicy(int numberOfRetries = 6,
            int retryWaitTimeInSeconds = 1)
        {
            return CreateRetryPolicy(TimeSpan.FromSeconds(retryWaitTimeInSeconds), numberOfRetries);
        }

        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <typeparam name="TStrategy">The type of the t strategy.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>TResult.</returns>
        public TResult ExecuteAction<TResult, TStrategy>(Func<TResult> function)
            where TStrategy : ITransientExceptionStrategy, new()
        {
            var strategy = new TStrategy();
            var counter = 0;
            var result = default(TResult);
            while (counter < NumberOfRetries)
            {
                try
                {
                    result = function.Invoke();
                    break;
                }
                catch (Exception exception)
                {
                    counter = HandleException(strategy, exception, counter);
                }
            }
            return result;
        }

        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <typeparam name="TStrategy">The type of the t strategy.</typeparam>
        /// <param name="action">The action.</param>
        public void ExecuteAction<TStrategy>(Action action)
            where TStrategy : ITransientExceptionStrategy, new()
        {
            var strategy = new TStrategy();
            var counter = 0;
            while (counter < NumberOfRetries)
            {
                try
                {
                    action.Invoke();
                    break;
                }
                catch (Exception exception)
                {
                    counter = HandleException(strategy, exception, counter);
                }
            }
        }

        /// <summary>
        ///     Executes the function.
        /// </summary>
        /// <typeparam name="TStrategy">The type of the t strategy.</typeparam>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>TResponse.</returns>
        public TResponse ExecuteFunction<TStrategy, TResponse>(Func<TResponse> func)
            where TStrategy : ITransientExceptionStrategy, new()
        {
            var strategy = new TStrategy();
            var counter = 0;
            var result = default(TResponse);
            while (counter < NumberOfRetries)
            {
                try
                {
                    result = func.Invoke();
                    break;
                }
                catch (Exception exception)
                {
                    counter = HandleException(strategy, exception, counter);
                }
            }
            return result;
        }

        private int HandleException<TStrategy>(TStrategy strategy, Exception exception, int counter)
            where TStrategy : ITransientExceptionStrategy, new()
        {
            if (strategy.IsTransient(exception))
            {
                counter++;
                Exceptions.Push(exception);
                if (counter == NumberOfRetries)
                {
                    Exceptions.Push(new RetryLimitExceededException(Exceptions));
                    throw new AggregateException($"Operation retry limit '{NumberOfRetries}' exceeded.",
                        Exceptions.ToList());
                }
                Thread.Sleep(RetryWaitTime.Milliseconds);
            }
            else
            {
                throw exception;
            }
            return counter;
        }
    }
}