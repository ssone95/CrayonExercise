using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Internal.CCP;
using Crayon.API.Domain.DTOs.Requests.Internal.CCP.Parts;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Requests.Orders.Parts;
using Crayon.API.Domain.DTOs.Responses.External.CCP;
using Crayon.API.Domain.DTOs.Responses.External.CCP.Parts;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;
using Crayon.API.Domain.DTOs.Responses.External.Parts;

namespace Crayon.API.Domain.MappingProfiles;

public class CCPMappingProfiles : Profile
{
    public CCPMappingProfiles()
    {
        CreateMap<CCPServiceLine, SubmittedOrderLinePart>()
            .ForMember(x => x.ServiceName, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.UnitPrice, o => o.MapFrom(src => src.Price))
            .ForMember(x => x.TaxPerUnitMultiplier, o => o.MapFrom(src => src.TaxMultiplier));
        CreateMap<OrderLinePart, SubmittedOrderLinePart>()
            .ForMember(x => x.ServiceId, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity));

        CreateMap<CCPServiceLine, ListServicesResponseLine>()
            .ForMember(x => x.ServiceId, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ForMember(x => x.Price, o => o.MapFrom(src => src.Price))
            .ForMember(x => x.IsAvailable, o => o.MapFrom(src => src.IsAvailable));

        CreateMap<CCPSubmittedOrderLinePart, SubmittedOrderLinePart>()
            .ForMember(x => x.ServiceId, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.ServiceName, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.UnitPrice, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.TotalPrice, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.BaseTotalPrice, o => o.MapFrom(src => src.UnitPrice))
            .ForMember(x => x.TaxPerUnitMultiplier, o => o.MapFrom(src => src.UnitPrice));

        CreateMap<CCPOrderServicesResponse, OrderServicesResponse>()
            .ForMember(x => x.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(x => x.Data, o => o.MapFrom(src => src.Data))
            .ForMember(x => x.Success, o => o.MapFrom(src => src.Success))
            .ForMember(x => x.GrandTotal, o => o.MapFrom(src => src.GrandTotal))
            .ForMember(x => x.TotalTax, o => o.MapFrom(src => src.TotalTax))
            .ForMember(x => x.TransactionFee, o => o.MapFrom(src => src.TransactionFee))
            .ForMember(x => x.Total, o => o.MapFrom(src => src.Total))
            .ForMember(x => x.Message, o => o.MapFrom(src => src.Message));

        CreateMap<OrderLinePart, CCPOrderLinePart>()
            .ForMember(x => x.Quantity, o => o.MapFrom(src => src.Quantity))
            .ForMember(x => x.Id, o => o.MapFrom(src => src.ServiceId))
            .ForMember(x => x.Name, o => o.MapFrom(src => src.Name))
            .ReverseMap();

        CreateMap<OrderServicesRequest, CCPOrderServicesRequest>()
            .ForMember(x => x.Lines, o => o.MapFrom(src => src.Lines));
    }
}