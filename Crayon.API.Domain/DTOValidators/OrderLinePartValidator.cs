using Crayon.API.Domain.DTOs.Requests.Orders.Parts;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class OrderLinePartValidator : AbstractValidator<OrderLinePart>
{
    public OrderLinePartValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotNull()
            .InclusiveBetween(1, Int32.MaxValue);

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();
    }
}