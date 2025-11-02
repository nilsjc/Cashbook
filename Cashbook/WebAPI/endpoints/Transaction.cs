using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.endpoints
{
    public static class Transaction
    {
        public static void RegisterTransactionEndpoints(this WebApplication app)
        {
            app.MapPost("/transaction", CreateTransaction)
            .WithName("CreateAccount")
            .WithOpenApi();
        }
        public static async Task<IResult> CreateTransaction(TransactionRequest request)
        {
            // Transaction logic to be implemented
            return Results.Ok(request);
        }
    }
}