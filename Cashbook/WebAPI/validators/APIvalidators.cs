using FluentValidation;
using WebAPI.endpoints;

namespace WebAPI.validators
{
    public class AccountCreateValidator : AbstractValidator<AccountDTO>
    {
        public AccountCreateValidator()
        {
            RuleFor(account => account.Name)
                .NotEmpty().WithMessage("Account name is required.");

            RuleFor(account => account.Name)
                .MaximumLength(20).WithMessage("Account name must not exceed 20 characters.");

            RuleFor(account => account.Type)
                .IsInEnum().WithMessage("Invalid account type.");
        }
    }
}