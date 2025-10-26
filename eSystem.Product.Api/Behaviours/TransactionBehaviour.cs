namespace eSystem.Product.Api.Behaviours;

public class TransactionBehaviour<TRequest, TResponse>(
    AppDbContext context,
    ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly AppDbContext context = context;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var executionStrategy = context.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            logger.LogInformation("Executing transaction");
            try
            {
                var result = await next(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Committed transaction");
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogInformation("Rollback transaction with error message: {ex}", ex.Message);
                throw;
            }
        });
    }
}