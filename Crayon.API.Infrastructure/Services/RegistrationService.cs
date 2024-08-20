using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Parts;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Crayon.API.Infrastructure.Services.Interfaces;
using CrayonCCP.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Crayon.API.Infrastructure.Services;

public class RegistrationService(ICustomerRepository customerRepository, ICustomerAccountRepository accountRepository, ILogger<RegistrationService> logger,
    IMapper mapper) : IRegistrationService
{
    public async Task<CustomerRegistrationResponse> Register(CustomerRegistrationRequest request)
    {
        if (request is { RegistrationRequest.CustomerId: not null, SubAccountRegistration: true })
        {
            return await RegisterSubAccount(request);
        }

        return await RegisterPrimaryAccount(request);
    }

    private async Task<CustomerRegistrationResponse> RegisterSubAccount(CustomerRegistrationRequest request)
    {
        Guid customerId = request.RegistrationRequest.CustomerId ?? Guid.Empty;
        
        var customerExists = await customerRepository.CustomerExists(request.RegistrationRequest.Email);
        var customerByIdentifier =
            await customerRepository.GetByIdentifierAsync(customerId);
        if (customerExists || customerByIdentifier is null)
        {
            logger.LogWarning("Customer with CustomerId {customerId} doesn't exist!", customerId);
            return new CustomerRegistrationResponse()
            {
                Success = false,
                Message = "Customer with that E-Mail or Id isn't valid!"
            };
        }

        CustomerAccountEntity? customerMgmtAccount = await accountRepository.GetByIdentifierAsync(request.UserContext!.CustomerAccountId);
        if (customerMgmtAccount is null 
            || (!customerMgmtAccount.IsManagementAccount || customerMgmtAccount.CustomerId != customerByIdentifier.Id) && !request.UserContext.IsAdmin)
        {
            logger.LogWarning("Current user {userId} isn't allowed to create new sub-accounts for the specified customer {customerId}!", 
                request.UserContext.UserId, customerId);
            throw new UnauthorizedAccessException(
                "Current user isn't allowed to create new sub-accounts for the specified customer!");
        }

        if (!customerByIdentifier.IsServiceroker)
        {
            logger.LogWarning("Customer with CustomerId {customerId} isn't a service broker, so they can't create multiple accounts!", customerId);
            throw new UnauthorizedAccessException("Current customer isn't a service broker, and can't have multiple accounts!");
        }

        var customerAccount = await CreateCustomerAccEntity(request, false, customerByIdentifier);
        await accountRepository.SaveChangesAsync();

        return new CustomerRegistrationResponse()
        {
            Success = true,
            Message = "Registration successful!",
            Data = mapper.Map<CustomerRegistrationEntryPart>(customerAccount)
        };
    }

    private async Task<CustomerRegistrationResponse> RegisterPrimaryAccount(CustomerRegistrationRequest request)
    {
        var accountExists =
            await accountRepository.AccountExists(request.RegistrationRequest.Username,
                request.RegistrationRequest.Email);
        if (accountExists)
        {
            logger.LogWarning("Account with Email {email} / Username {username} already exists!", 
                request.RegistrationRequest.Email, request.RegistrationRequest.Username);
            return new()
            {
                Success = false,
                Message = "Account with these credentials already exists!",
                Data = null
            };
        }

        var customerExists = await customerRepository.CustomerExists(request.RegistrationRequest.Email);
        if (customerExists)
        {
            logger.LogWarning("Customer with Email {email} / Username {username} already exists!", 
                request.RegistrationRequest.Email, request.RegistrationRequest.Username);
            return new CustomerRegistrationResponse()
            {
                Success = false,
                Message = "Customer with that E-Mail address already exists!"
            };
        }
        
        var customerEntity = await CreateCustomerEntity(request);
        await customerRepository.SaveChangesAsync();
        
        var customerAccEntity = await CreateCustomerAccEntity(request, true, customerEntity);
        await accountRepository.SaveChangesAsync();

        return new CustomerRegistrationResponse()
        {
            Success = true,
            Message = "Registration successful!",
            Data = mapper.Map<CustomerRegistrationEntryPart>(customerAccEntity)
        };
    }

    private async Task<CustomerAccountEntity> CreateCustomerAccEntity(CustomerRegistrationRequest request, bool isManagementAccount, CustomerEntity customerEntity)
    {
        byte[] bytes = Sha256Extensions.Hash(request.RegistrationRequest.Password);
        var customerManagementAccount = mapper.Map<CustomerAccountEntity>(request);
        customerManagementAccount.CustomerId = customerEntity.Id;
        customerManagementAccount.IsManagementAccount = isManagementAccount;
        customerManagementAccount.Password = bytes;
        customerManagementAccount = await accountRepository.Create(customerManagementAccount);
        return customerManagementAccount;
    }

    private async Task<CustomerEntity> CreateCustomerEntity(CustomerRegistrationRequest request)
    {
        var customerEntity = mapper.Map<CustomerEntity>(request);
        customerEntity = await customerRepository.Create(customerEntity);
        return customerEntity;
    }
}