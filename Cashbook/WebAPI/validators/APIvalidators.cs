using FluentValidation;
using WebAPI.endpoints;

namespace WebAPI.validators
{
    public class AccountCreateValidator : AbstractValidator<PostAccountDTO>
    {
        public AccountCreateValidator()
        {
            RuleFor(account => account.Name)
                .NotEmpty().WithMessage("Account name is required.");

            RuleFor(account => account.Type)
                .IsInEnum().WithMessage("Invalid account type.");
        }
    }
}