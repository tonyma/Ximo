using Ximo.Data.TransientStrategies;
using Ximo.TransientFaultHandling;

namespace Ximo.Cqrs.Decorators
{
    /// <summary>
    ///     Decorator adding retry logic for SQL Azure / SQL Server transient errors.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public class TransientSqlErrorCommandHandlerDecorator<TCommand> : ICommandDecorator<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransientSqlErrorCommandHandlerDecorator{TCommand}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated.</param>
        public TransientSqlErrorCommandHandlerDecorator(ICommandHandler<TCommand> decorated)
        {
            _decorated = decorated;
        }

        /// <summary>
        ///     Handles the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void Handle(TCommand command)
        {
            var policy = RetryPolicy.CreateRetryPolicy(5, 1);
            policy.ExecuteAction<SqlTransientExceptionStrategy>(() => _decorated.Handle(command));
        }
    }
}