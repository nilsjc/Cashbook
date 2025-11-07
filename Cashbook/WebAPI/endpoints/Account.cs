using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebAPI.database;

namespace WebAPI.endpoints
{
    public static class AccountAPI
    {
        public static void RegisterAccountEndpoints(this WebApplication app)
        {
            app.MapPost("/account", CreateAccount)
            .WithName("CreateAccount")
            .WithOpenApi();

            app.MapGet("/accounts", GetAccounts)
            .WithName("GetAccounts")
            .WithOpenApi();
        }

        public static async Task<IResult> CreateAccount(PostAccountDTO account, IValidator<PostAccountDTO> validator, IDatabaseService dbService)
        {
            var validationResult = await validator.ValidateAsync(account);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }

            string name = (account.Name.Length > 10 ? account.Name.Substring(0, 10) : account.Name)
                                .ToLower();
            // create account
            var result = await dbService.CreateAccountAsync(name, account.Type);
            if (!result.Success)
            {
                return Results.Conflict(result.Error);
            }
            
            return Results.Ok(account);
        }
        
        public static async Task<IResult> GetAccounts(IDatabaseService dbService)
        {
            try
            {
                var accounts = await dbService.GetAllAccountsAsync();
                return Results.Ok(accounts);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}