using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;
using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Crayon.API.Domain.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        
        // var order = await orderRepository.Create(new()
        // {
        //     Identifier = orderId,
        //     CustomerId = request.UserContext!.InternalCustomerId,
        //     CustomerAccountId = customerAccount!.Id,
        //     Total = Math.Round(orderAttemptResponse.Total, 2),
        //     TotalTax = Math.Round(orderAttemptResponse.TotalTax, 2),
        //     GrandTotal = Math.Round(orderAttemptResponse.GrandTotal, 2),
        //     TransactionFee = Math.Round(orderAttemptResponse.TransactionFee, 2),
        //
        //     Lines = [],
        //
        //     Status = EntityStatus.Active,
        //     CreatedAt = DateTime.UtcNow,
        //     UpdatedAt = DateTime.UtcNow,
        //
        //     OrderStatus = OrderStatus.Processed
        // });

        CreateMap<OrderServicesResponse, OrderEntity>()
            .ForMember(x => x.Total, o => o.MapFrom(src => Math.Round(src.Total, 2)))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => Math.Round(src.TotalTax, 2)))
            .ForMember(x => x.GrandTotal, o => o.MapFrom(src => Math.Round(src.GrandTotal, 2)))
            .ForMember(x => x.TransactionFee, o => o.MapFrom(src => Math.Round(src.TransactionFee, 2)))
            .ForMember(x => x.Lines, o => o.MapFrom(src => new List<OrderLineEntity>()))
            .ForMember(x => x.Status, o => o.MapFrom(src => EntityStatus.Active))
            .ForMember(x => x.CreatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.OrderStatus, o => o.MapFrom(src => OrderStatus.Processed));
        
        CreateMap<OrderServicesRequest, OrderEntity>()
            .ForMember(x => x.CustomerId, o => o.MapFrom(src => src.UserContext!.InternalCustomerId))
            .ForMember(x => x.CustomerAccountId, o => o.MapFrom(src => src.AccountId));
            
        CreateMap<SubmittedOrderLinePart, OrderLineEntity>()
            .ForMember(x => x.ServiceId, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => Math.Round(src.TotalTax, 2)))
            .ForMember(x => x.BaseTotalPrice, o => o.MapFrom(src => Math.Round(src.BaseTotalPrice, 2)))
            .ForMember(x => x.ServiceName, o => o.MapFrom(src => src.ServiceName))
            .ForMember(x => x.UnitPrice, o => o.MapFrom(src => Math.Round(src.UnitPrice, 2)))
            .ForMember(x => x.TaxPerUnitMultiplier, o => o.MapFrom(src => Math.Round(src.TaxPerUnitMultiplier, 2)))
            .ForMember(x => x.TotalPrice, o => o.MapFrom(src => Math.Round(src.TotalPrice, 2)))
            .ForMember(x => x.Identifier, o => o.MapFrom(src => Guid.NewGuid()))
            .ForMember(x => x.LicenseStatus, o => o.MapFrom(src => LicenseStatus.Active))
            .ForMember(x => x.Status, o => o.MapFrom(src => EntityStatus.Active))
            .ForMember(x => x.CreatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedAt, o => o.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.ValidUntil, o => o.MapFrom(src => DateTime.UtcNow.AddMonths(1)));

        CreateMap<OrderLineEntity, OrderEntryLineModel>()
            .ForMember(x => x.ServiceId, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.ServiceName, o => o.MapFrom(src => src.ServiceName))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.UnitPrice, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.TaxPerUnitMultiplier, o => o.MapFrom(src => src.TaxPerUnitMultiplier))
            .ForMember(x => x.BaseTotalPrice, o => o.MapFrom(src => src.BaseTotalPrice))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => src.TotalTax))
            .ForMember(x => x.TotalPrice, o => o.MapFrom(src => src.TotalPrice))
            .ForMember(x => x.LicenseStatus, o => o.MapFrom(src => src.LicenseStatus))
            .ForMember(x => x.ExpiresOn, o => o.MapFrom(src => src.ValidUntil));
        
        CreateMap<OrderEntity, OrderEntryModel>()
            .ForMember(x => x.Lines, o => o.MapFrom(src => src.Lines))
            .ForMember(x => x.OrderId, o => o.MapFrom(src => src.Identifier))
            .ForMember(x => x.TransactionFee, o => o.MapFrom(src => src.TransactionFee))
            .ForMember(x => x.Total, o => o.MapFrom(src => src.Total))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => src.TotalTax))
            .ForMember(x => x.GrandTotal, o => o.MapFrom(src => src.GrandTotal))
            .ForMember(x => x.Status, o => o.MapFrom(src => src.OrderStatus))
            .ForMember(x => x.PostedOn, o => o.MapFrom(src => src.CreatedAt));
        
    }
}