using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.endpoints
{
    public static class Account
    {
        public static void RegisterAccountEndpoints(this WebApplication app)
        {
            app.MapPost("/account", CreateAccount)
            .WithName("CreateAccount")
            .WithOpenApi();
        }

        static async Task<IResult> CreateAccount(AccountDTO account)
        {
            // check duplicates
            return Results.Ok(account);
        }
    }
}