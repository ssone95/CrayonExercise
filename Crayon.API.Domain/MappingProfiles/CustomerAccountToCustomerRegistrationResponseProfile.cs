using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers.Parts;
using Crayon.API.Domain.DTOs.Responses.Internal.Parts;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Domain.Enums;

namespace Crayon.API.Domain.MappingProfiles;

public class CustomerAccountToCustomerRegistrationResponseProfile : Profile
{
    public CustomerAccountToCustomerRegistrationResponseProfile()
    {
        CreateMap<CustomerAccountEntity, CustomerRegistrationEntryPart>()
            .ForMember(dst => dst.CustomerId, o => o.MapFrom(src => src.Customer!.Identifier))
            .ForMember(dst => dst.CustomerAccountId, o => o.MapFrom(src => src.Identifier));

        CreateMap<CustomerRegistrationRequest, CustomerEntity>()
            .ForMember(x => x.Identifier, o => o.MapFrom(src => Guid.NewGuid()))
            .ForMember(x => x.IsServiceroker, o => o.MapFrom(src => src.IsServiceBroker))
            .ForMember(x => x.CustomerName, o => o.MapFrom(src =>src.RegistrationRequest.Name))
            .ForMember(x => x.ContactEmail, o => o.MapFrom(src => src.RegistrationRequest.Email))
            .ForMember(x => x.Status, o => o.MapFrom(src => EntityStatus.Active))
            .ForMember(x => x.CreatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.CustomerOrders, o => o.MapFrom(src => new List<OrderEntity>()))
            .ForMember(x => x.CustomerAccounts, o => o.MapFrom(src => new List<CustomerAccountEntity>()));

        CreateMap<CustomerRegistrationRequest, CustomerAccountEntity>()
            .ForMember(x => x.Identifier, o => o.MapFrom(src => Guid.NewGuid()))
            .ForMember(x => x.Verified, o => o.MapFrom(src => true))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.RegistrationRequest.Name))
            .ForMember(x => x.Email, o => o.MapFrom(src => src.RegistrationRequest.Email))
            .ForMember(x => x.Username, o => o.MapFrom(src => src.RegistrationRequest.Username))
            .ForMember(x => x.Status, o => o.MapFrom(src => EntityStatus.Active))
            .ForMember(x => x.CreatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.AccountOrders, o => o.MapFrom(src => new List<OrderEntity>()));

        CreateMap<CustomerAccountEntity, CustomerAccountEntryModel>()
            .ForMember(x => x.Id, o => o.MapFrom(src => src.Identifier))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.Email, o => o.MapFrom(src => src.Email));


        CreateMap<CustomerAccountEntity, CustomerAccountDetailsContextModel>()
            .ForMember(x => x.CustomerId, o => o.MapFrom(src => src.Customer!.Identifier))
            .ForMember(x => x.CustomerAccountId, o => o.MapFrom(src => src.Identifier))
            .ForMember(x => x.InternalCustomerId, o => o.MapFrom(src => src.Customer!.Id))
            .ForMember(x => x.InternalCustomerAccountId, o => o.MapFrom(src => src.Id))
            .ForMember(x => x.IsBroker, o => o.MapFrom(src => src.Customer!.IsServiceroker))
            .ForMember(x => x.IsManager, o => o.MapFrom(src => src.IsManagementAccount))
            .ForMember(x => x.UserEmail, o => o.MapFrom(src => src.Email))
            .ForMember(x => x.IsCustomer, o => o.MapFrom(src => src.Customer != null))
            .ForMember(x => x.IsAdmin, o => o.MapFrom(src => false))
            .ForMember(x => x.IsAuthenticated, o => o.MapFrom(src => src != null))
            .ForMember(x => x.Roles, o => o.MapFrom(src => new List<string>()));
    }
}