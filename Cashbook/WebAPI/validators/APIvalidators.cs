using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebAPI.endpoints;

namespace WebAPI.validators
{
    public class AccountCreateValidator : AbstractValidator<AccountDTO>
    {
        public AccountCreateValidator()
        {
            RuleFor(account => account.Name)
                .NotEmpty().WithMessage("Account name is required.")
                .MaximumLength(255).WithMessage("Account name must not exceed 255 characters.");

            RuleFor(account => account.Type)
                .IsInEnum().WithMessage("Invalid account type.");
        }
    }
}