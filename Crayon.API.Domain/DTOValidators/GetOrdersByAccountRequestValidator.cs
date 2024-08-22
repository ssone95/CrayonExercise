using Crayon.API.Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class GetOrdersByAccountRequestValidator : AbstractValidator<GetOrdersByAccountRequest>
{
    public GetOrdersByAccountRequestValidator()
    {
        List<int> allowedItemsPerPageList = [1, 5, 10, 25, 50];
        RuleFor(x => x.AccountId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.CurrentPage)
            .NotNull()
            .InclusiveBetween(1, Int32.MaxValue);
        
        RuleFor(x => x.ItemsPerPage)
            .NotNull()
            .Must(x => allowedItemsPerPageList.Contains(x))
            .WithMessage("Value provided for {PropertyName} is not in '" + string.Join(", ", allowedItemsPerPageList) + "'!");
    }
}