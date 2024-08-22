using Crayon.API.Domain.DTOs.Requests.Orders.Parts;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class UpdateQuantityPartValidator : AbstractValidator<UpdateQuantityPart>
{
    public UpdateQuantityPartValidator()
    {
        RuleFor(x => x.NewQuantity)
            .NotNull()
            .InclusiveBetween(1, Int32.MaxValue);
    }
}