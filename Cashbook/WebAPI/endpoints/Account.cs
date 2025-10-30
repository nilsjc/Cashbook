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

        public static async Task<IResult> CreateAccount(AccountDTO account)
        {
            if(account.Name=="TestingError")
            {
                return Results.BadRequest("Invalid account name.");
            }
            // check duplicates
            return Results.Ok(account);
        }
    }
}