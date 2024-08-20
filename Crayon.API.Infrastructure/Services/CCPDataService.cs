using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Crayon.API.Domain.DTOs.Requests.External;
using Crayon.API.Domain.DTOs.Requests.Internal.CCP;
using Crayon.API.Domain.DTOs.Requests.Orders;
using Crayon.API.Domain.DTOs.Requests.Orders.Parts;
using Crayon.API.Domain.DTOs.Responses.External;
using Crayon.API.Domain.DTOs.Responses.External.CCP;
using Crayon.API.Domain.DTOs.Responses.External.CCP.Parts;
using Crayon.API.Domain.DTOs.Responses.External.Orders;
using Crayon.API.Domain.DTOs.Responses.External.Orders.Parts;
using Crayon.API.Domain.DTOs.Responses.External.Parts;
using Crayon.API.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Crayon.API.Infrastructure.Services;

public class CCPDataService : ICCPDataService
{
    private readonly HttpClient _ccpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<CCPDataService> _logger;
    private readonly bool _shouldUseMock;
    
    public CCPDataService(IConfiguration configuration, IHostEnvironment environment, IMapper mapper, ILogger<CCPDataService> logger)
    {
        _mapper = mapper;
        _logger = logger;
        
        var endpoint = configuration.GetSection("Security:CCP").GetValue<string?>("Endpoint");
        // var clientId = configuration.GetSection("Security:CCP").GetValue<string?>("ClientId"); // We don't need this at the moment
        var clientSecret = configuration.GetSection("Security:CCP").GetValue<string?>("ClientSecret");
        var shouldMockRequests = configuration.GetSection("Security:CCP").GetValue<bool?>("Mock");
        if (shouldMockRequests.HasValue)
        {
            _shouldUseMock = shouldMockRequests.Value;
        }
        
        var handler = new HttpClientHandler();
        if (environment.IsDevelopment())
        {
            // Disabling the certificate validation for local env testing
            handler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
        }
        _ccpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30),
            BaseAddress = Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? ccpEndpoint) 
                ? ccpEndpoint 
                : throw new ArgumentException($"Invalid Endpoint configured for CCP API access: '{endpoint}'")
        };
        _ccpClient.DefaultRequestHeaders.Add("x-api-key", clientSecret);
    }

    public async Task<ListServicesResponse> GetAvailableServices(ListServicesRequest request)
    {
        try
        {
            if (_shouldUseMock)
            {
                var mockedData = GetMockServicesList(new CCPListServicesRequest()
                {
                    FilterString = request.FilterString,
                    CurrentPage = request.CurrentPage,
                    ItemsPerPage = request.ItemsPerPage
                });
                return new ListServicesResponse()
                {
                    Success = true,
                    Data = _mapper.Map<List<ListServicesResponseLine>>(mockedData.Data!),
                    CurrentPage = request.CurrentPage,
                    ItemsPerPage = request.ItemsPerPage,
                    TotalPages = mockedData.TotalPages
                };
            }

            CCPServicesResponse ccpApiResponse = await GetServicesList(new CCPListServicesRequest()
            {
                FilterString = request.FilterString,
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage
            });
            return new()
            {
                Success = ccpApiResponse.Success,
                Message = ccpApiResponse.Message,
                Data = ccpApiResponse is { Success: true, Data: not null }
                    ? _mapper.Map<List<ListServicesResponseLine>>(ccpApiResponse.Data!)
                    : [],
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage,
                TotalPages = 10
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred in communication with CCP (cloud provider): {ex}", ex);
            return new()
            {
                Success = false,
                Message =
                    "An error occurred while trying to communicate with the service provider api, please try again later.",
                CurrentPage = 1,
                TotalPages = 1,
                ItemsPerPage = request.ItemsPerPage
            };
        }
    }

    public async Task<OrderServicesResponse> SubmitOrder(OrderServicesRequest request, Guid orderId)
    {
        try
        {
            CCPServicesResponse services = _shouldUseMock
                ? GetMockServicesList(new() { CurrentPage = 1, ItemsPerPage = Int32.MaxValue })
                : await GetServicesList(new() { CurrentPage = 1, ItemsPerPage = Int32.MaxValue });
            
            IEnumerable<CCPServiceLine> filteredOrderServiceList =
                services.Data!.Where(x => request.Lines.Any(y => y.ServiceId == x.ServiceId))
                    .ToList();
            
            if (request.Lines.Count < 1 || filteredOrderServiceList.Count() != request.Lines.Count)
            {
                return new()
                {
                    OrderId = orderId,
                    Success = false,
                    Message = "Ordered services aren't valid or cart is empty!",
                    TransactionFee = 0,
                    Data = []
                };
            }
            
            if (_shouldUseMock)
            {
                return new()
                {
                    OrderId = orderId,
                    Success = true,
                    Data = MapRequestLines(request.Lines, filteredOrderServiceList),
                    TransactionFee = new Random().NextDouble() * 5,
                };
            }

            CCPOrderServicesRequest mappedRequest = _mapper.Map<CCPOrderServicesRequest>(request);
            CCPOrderServicesResponse orderServicesResponse = await OrderServices(mappedRequest, orderId);
            return _mapper.Map<OrderServicesResponse>(orderServicesResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred in communication with CCP (cloud provider): {ex}", ex);
            return new()
            {
                OrderId = orderId,
                Success = false,
                Message =
                    "An error occurred while trying to communicate with the service provider api, please try again later.",
                Data = [],
                TransactionFee = 0,
            };
        }
    }

    private List<SubmittedOrderLinePart>? MapRequestLines(List<OrderLinePart> requestLines,
        IEnumerable<CCPServiceLine> filteredOrderServiceList)
    {
        return requestLines.Select(x =>
        {
            var item = filteredOrderServiceList.First(y => y.ServiceId == x.ServiceId);
            return _mapper.Map(x, _mapper.Map<SubmittedOrderLinePart>(item));
        }).ToList();
    }

    private async Task<CCPOrderServicesResponse> OrderServices(CCPOrderServicesRequest request, Guid orderId)
    {
        CCPOrderServicesResponse? deserializedResponse 
            = await Request<CCPOrderServicesRequest, CCPOrderServicesResponse>(request, HttpMethod.Post, "/services/order");
        return deserializedResponse
               ?? new()
               {
                   Success = false,
                   Message = "An error occurred while trying to parse the service provider api response!",
                   Data = [],
                   OrderId = orderId,
                   TransactionFee = 0
               };
    }

    private async Task<CCPServicesResponse> GetServicesList(CCPListServicesRequest request)
    {
        CCPServicesResponse? deserializedResponse 
            = await Request<CCPListServicesRequest, CCPServicesResponse>(request, HttpMethod.Post, "/services/list");
        return deserializedResponse
               ?? new()
               {
                   Success = false,
                   Message = "An error occurred while trying to parse the service provider api response!",
                   CurrentPage = request.CurrentPage,
                   ItemsPerPage = request.ItemsPerPage,
                   TotalPages = 0,
                   Data = []
               };
    }

    private async Task<TResponse?> Request<TRequest, TResponse>(TRequest request, HttpMethod method, string endpoint)
    {
        var responseMessage = method switch
        {
            {} when method.Method == HttpMethod.Post.Method => await PostJsonData(request, endpoint),
            _ => throw new NotImplementedException($"Method {method} is not supported yet!")
        };

        await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TResponse>(responseStream);
    }

    private async Task<HttpResponseMessage> PostJsonData<T>(T request, string endpoint)
    {
        using var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, request);
        
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(endpoint, UriKind.Absolute));
        httpRequestMessage.Content = new StringContent(Encoding.UTF8.GetString(ms.ToArray()), Encoding.UTF8);
        httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await _ccpClient.SendAsync(httpRequestMessage);
    }
    

    private CCPServicesResponse GetMockServicesList(CCPListServicesRequest request)
    {
        List<CCPServiceLine> availableServices =
        [
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Microsoft Office",
                Price = 10.99,
                IsAvailable = true,
                TaxMultiplier = 0.2
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Microsoft Windows 11",
                Price = 59.99,
                IsAvailable = true,
                TaxMultiplier = 0.2
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Microsoft OneDrive Tier 1",
                Price = 2.99,
                IsAvailable = false,
                TaxMultiplier = 0.15
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Microsoft OneDrive Tier 2",
                Price = 4.99,
                IsAvailable = false,
                TaxMultiplier = 0.2
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Microsoft OneDrive Tier 3",
                Price = 9.99,
                IsAvailable = false,
                TaxMultiplier = 0.1
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Name = "Microsoft XBox Live Subscription (monthly)",
                Price = 9.99,
                IsAvailable = false,
                TaxMultiplier = 0.2
            },
            new()
            {
                ServiceId = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                Name = "Microsoft Azure Kubernetes Service (Standard) (monthly)",
                Price = 4.99,
                IsAvailable = false,
                TaxMultiplier = 0.2
            }
        ];

        var queryableSrc = availableServices
            .AsQueryable()
            .Where(x => string.IsNullOrEmpty(request.FilterString)
                        || x.Name.StartsWith(request.FilterString));


        var totalCount = queryableSrc.Count();

        var totalPagesCount =
            Convert.ToInt32(Math.Max(Math.Ceiling(totalCount / (double)request.ItemsPerPage), 1));
        
        return new()
        {
            Data = queryableSrc
                .Skip((request.CurrentPage - 1) * request.ItemsPerPage)
                .Take(request.ItemsPerPage)
                .ToList(),
            Success = true,
            CurrentPage = request.CurrentPage,
            ItemsPerPage = request.ItemsPerPage,
            TotalPages = totalPagesCount,
        };
    }

    public void Dispose()
    {
        _ccpClient.Dispose();
    }
}