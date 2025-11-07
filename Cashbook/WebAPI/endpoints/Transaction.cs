using WebAPI.database;

namespace WebAPI.endpoints
{
    public static class TransactionAPI
    {
        public static void RegisterTransactionEndpoints(this WebApplication app)
        {
            app.MapPost("/transaction", CreateTransaction)
            .WithName("CreateTransaction")
            .WithOpenApi();
        }
        public static async Task<IResult> CreateTransaction(TransactionRequestDTO request, IDatabaseService dbService)
        {
            if(request.FromAccount == request.ToAccount)
            {
                return Results.BadRequest("FromAccount and ToAccount cannot be the same.");
            }
            var result = await dbService.CreateTransactionAsync(request.FromAccount, request.ToAccount, request.Amount);
            if (!result.Success)
            {
                return Results.Conflict(result.Error);
            }
            
            return Results.Ok(request);
        }
    }
}