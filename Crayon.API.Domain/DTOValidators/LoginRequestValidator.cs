using Crayon.API.Domain.DTOs.Requests.External;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(50);
    }
}