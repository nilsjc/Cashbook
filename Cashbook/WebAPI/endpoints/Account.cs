using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

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

        public static async Task<IResult> CreateAccount(AccountDTO account, IValidator<AccountDTO> validator)
        {
            var validationResult = await validator.ValidateAsync(account);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }
            // check duplicates
            if(account.Name=="TestingError")
            {
                return Results.BadRequest("Invalid account name.");
            }
            
            return Results.Ok(account);
        }
    }
}