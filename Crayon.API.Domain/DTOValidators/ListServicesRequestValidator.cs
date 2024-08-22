using Crayon.API.Domain.DTOs.Requests.External;
using FluentValidation;

namespace Crayon.API.Domain.DTOValidators;

public class ListServicesRequestValidator : AbstractValidator<ListServicesRequest>
{
    public ListServicesRequestValidator()
    {
        List<int> allowedItemsPerPageList = [1, 5, 10, 25, 50];
        RuleFor(x => x.CurrentPage)
            .NotNull()
            .InclusiveBetween(1, Int32.MaxValue);
        
        RuleFor(x => x.ItemsPerPage)
            .NotNull()
            .Must(x => allowedItemsPerPageList.Contains(x))
            .WithMessage("Value provided for {PropertyName} is not in '" + string.Join(", ", allowedItemsPerPageList) + "'!");

        When(x => !string.IsNullOrEmpty(x.FilterString),
            () => RuleFor(x => x.FilterString)
                .MinimumLength(2)
                .MaximumLength(50));
    }
}