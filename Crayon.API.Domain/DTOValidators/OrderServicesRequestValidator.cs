using Crayon.API.Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class OrderServicesRequestValidator : AbstractValidator<OrderServicesRequest>
{
    public OrderServicesRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Lines)
            .NotNull()
            .NotEmpty();
    }
}