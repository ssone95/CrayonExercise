using System.Security.Authentication;
using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.Common;
using Crayon.API.Domain.DTOs.Requests.Internal.Customers;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Responses.Base;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers;
using Crayon.API.Domain.DTOs.Responses.Internal.Customers.Parts;
using Crayon.API.Domain.Entities.Customers;
using Crayon.API.Domain.Entities.Orders;
using Crayon.API.Domain.Enums;
using Crayon.API.Infrastructure.Repositories.Interfaces;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;
using NRedisStack;

namespace Crayon.API.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ICCPDataService ccpDataService;
    private readonly IOrderRepository orderRepository;
    private readonly IOrderLineRepository orderLineRepository;
    private readonly ICustomerAccountRepository customerAccountRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> logger;

    public OrderService(ICCPDataService ccpDataService, IOrderRepository orderRepository,
        IOrderLineRepository orderLineRepository,
        ICustomerAccountRepository customerAccountRepository, IMapper mapper, ILogger<OrderService> logger)
    {
        this.ccpDataService = ccpDataService;
        this.orderRepository = orderRepository;
        this.orderLineRepository = orderLineRepository;
        this.customerAccountRepository = customerAccountRepository;
        _mapper = mapper;
        this.logger = logger;
    }
    public async Task<OrderServicesResponse> Post(OrderServicesRequest request)
    {
        if (!ValidateUserContext(request, out var response))
            return response!;

        CustomerAccountEntity? customerAccount = await customerAccountRepository.GetByIdentifierAsync(request.AccountId);
        if (!ValidateAccessToCustomerAccount(request.UserContext!, customerAccount, out response))
            return response!;
        
        Guid orderId = Guid.NewGuid();
        var orderAttemptResponse = await ccpDataService.SubmitOrder(request, orderId);
        if (!orderAttemptResponse.Success)
            return orderAttemptResponse;

        OrderEntity order = _mapper.Map<OrderEntity>(orderAttemptResponse);
        order.Identifier = orderId;
        order.CustomerId = customerAccount!.CustomerId;
        order.CustomerAccountId = customerAccount.Id;
        
        order = await orderRepository.Create(order);
        await orderRepository.SaveChangesAsync();

        foreach (SubmittedOrderLinePart ol in orderAttemptResponse.Data!)
        {
            var olEntity = _mapper.Map<OrderLineEntity>(ol);
            olEntity.OrderId = order.Id;
            _ = await orderLineRepository.Create(olEntity);
        }
        await orderLineRepository.SaveChangesAsync();
        return orderAttemptResponse;
    }

    public async Task<GetOrdersByAccountResponse> GetByAccount(GetOrdersByAccountRequest request)
    {
        if (!request.UserContext.IsManager && request.AccountId != request.UserContext.UserId && !request.UserContext.IsAdmin)
        {
            return new GetOrdersByAccountResponse()
            {
                Success = false,
                Message = "You don't have permissions to view another user's data!"
            };
        }
        
        CustomerAccountEntity? customerAccount = await customerAccountRepository.GetByIdentifierAsync(request.AccountId);
        if (!ValidateAccessToCustomerAccount(request.UserContext, customerAccount, out _))
        {
            return new()
            {
                Success = false,
                Message = "You don't have permissions to view another user's data!"
            };
        }
        
        int currentPage = Math.Max(request.CurrentPage, 1);
        int itemsPerPage = Math.Max(request.ItemsPerPage, 10);

        (List<OrderEntity> orders, int totalPages) = await orderRepository.GetByAccountIdentifierAsync(customerAccount!.Identifier, currentPage, itemsPerPage);

        return new()
        {
            Success = true,
            Data = _mapper.Map<List<OrderEntryModel>>(orders),
            CurrentPage = currentPage,
            ItemsPerPage = itemsPerPage,
            TotalPages = totalPages
        };
    }

    public async Task<UpdateSubscriptionResponse> UpdateSubscription(UpdateSubscriptionRequest request)
    {
        if (request.UserContext is null ||
            !request.UserContext!.IsManager && !request.UserContext.IsAdmin)
        {
            return new UpdateSubscriptionResponse()
            {
                Success = false,
                Message = "You don't have permissions to modify another user's data!",
                ServiceId = request.ServiceId,
                OrderId = request.OrderId,
                Data = null
            };
        }

        var orderById = await orderRepository.GetByIdentifierAsync(request.OrderId);
        var line = orderById?.Lines.FirstOrDefault(x => x.ServiceId == request.ServiceId);
        if (orderById?.CustomerAccount is null || line is null)
        {
            return new UpdateSubscriptionResponse()
            {
                Success = false,
                Message = "You don't have permissions to modify another user's data!",
                ServiceId = request.ServiceId,
                OrderId = request.OrderId,
                Data = null
            };
        }
        
        CustomerAccountEntity? customerAccount = await customerAccountRepository.GetByIdentifierAsync(orderById.CustomerAccount.Identifier);
        if (!ValidateAccessToCustomerAccount(request.UserContext, customerAccount, out _)
            || request is { RequestType: SubscriptionRequestOperation.Update, QuantityPart: null }
            || request is { RequestType: SubscriptionRequestOperation.Update, QuantityPart.NewQuantity: < 1 })
        {
            return new()
            {
                Success = false,
                Message = "You don't have permissions to modify another user's data or input is invalid!"
            };
        }

        switch (request.RequestType)
        {
            case SubscriptionRequestOperation.Cancel:
                line.LicenseStatus = EntityStatus.Cancelled;
                break;
            case SubscriptionRequestOperation.Extend:
                line.ValidUntil = line.ValidUntil < DateTime.UtcNow 
                    ? DateTime.UtcNow.AddMonths(1) 
                    : line.ValidUntil.AddMonths(1);
                break;
            case SubscriptionRequestOperation.Update:
                line.Quantity = request.QuantityPart!.NewQuantity;
                line.BaseTotalPrice = line.Quantity * line.UnitPrice;
                line.TotalTax = line.BaseTotalPrice * line.TaxPerUnitMultiplier;
                line.TotalPrice = line.BaseTotalPrice + line.TotalTax;

                orderById.Total = orderById.Lines.Sum(x => x.BaseTotalPrice);
                orderById.TotalTax = orderById.Lines.Sum(x => x.TotalTax);
                orderById.GrandTotal = orderById.Total + orderById.TotalTax + orderById.TransactionFee;
                break;
        }
        line.UpdatedAt = DateTime.UtcNow;
        orderById.UpdatedAt = DateTime.UtcNow;
        
        await orderLineRepository.Update(line);
        await orderLineRepository.SaveChangesAsync();
        
        await orderRepository.Update(orderById);
        await orderRepository.SaveChangesAsync();

        return new()
        {
            ServiceId = request.ServiceId,
            OrderId = request.OrderId,
            Success = true,
            Data = _mapper.Map<OrderEntryLineModel>(line)
        };
    }

    private static bool ValidateAccessToCustomerAccount(CustomerAccountDetailsContextModel context, CustomerAccountEntity? customerAccount, out OrderServicesResponse? response)
    {
        var hasValidAccess = !(customerAccount?.Customer is null
               || (customerAccount.CustomerId != context!.InternalCustomerId 
                    && !context.IsAdmin));

        response = null;
        if (!hasValidAccess)
            throw new UnauthorizedAccessException("You can't order services on behalf of another account!");

        return hasValidAccess;
    }

    private static bool ValidateUserContext(OrderServicesRequest request, out OrderServicesResponse? result)
    {
        bool isContextValid = !(request.UserContext is null
               || !request.UserContext.IsManager && !request.UserContext.IsAdmin);

        result = null;
        if (!isContextValid)
            throw new AuthenticationException("To submit orders you must be logged in with valid credentials and have access to that option!");

        return isContextValid;
    }
}