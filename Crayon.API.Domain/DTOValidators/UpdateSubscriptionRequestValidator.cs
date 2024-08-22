using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.Enums;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class UpdateSubscriptionRequestValidator : AbstractValidator<UpdateSubscriptionRequest>
{
    public UpdateSubscriptionRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.RequestType)
            .NotNull()
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.ServiceId)
            .NotNull()
            .NotEmpty();

        When(x => x.RequestType == SubscriptionRequestOperation.Update,
            () =>
            {
                RuleFor(x => x.QuantityPart)
                    .NotNull();
            });
    }
}