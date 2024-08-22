using Crayon.API.Domain.DTOs.Requests.External;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50)
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);
    }
}