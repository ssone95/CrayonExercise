using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers.Parts;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Crayon.API.Infrastructure.Services.Interfaces;

namespace Crayon.API.Infrastructure.Services;

public class CustomerDataService(ICustomerAccountRepository accountRepository, IMapper mapper) : ICustomerDataService
{
    public async Task<CommonUserContextModel?> GetCustomerEntityAsCtxModelById(Guid id)
    {
        var customerAccountEntity = await accountRepository.GetByIdentifierAsync(id);
        return customerAccountEntity is not null 
            ? mapper.Map<CustomerAccountDetailsContextModel>(customerAccountEntity)
                : null;
    }
    
    public async Task<CustomerAccountListResponse> GetCustomerAccounts(CustomerAccountListRequest request)
    {
        (List<CustomerAccountEntity> customerAccountEntities, int totalPages) = await accountRepository.GetAccountsByCustomerIdentifier(request.UserContext!.CustomerId, request.FilterString, request.CurrentPage, request.ItemsPerPage);
        return new()
        {
            Data = mapper.Map<List<CustomerAccountEntryModel>>(customerAccountEntities),
            Success = true,
            CurrentPage = request.CurrentPage,
            ItemsPerPage = request.ItemsPerPage,
            TotalPages = totalPages
        };
    }
}