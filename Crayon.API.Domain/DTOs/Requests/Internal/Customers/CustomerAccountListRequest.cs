using System.Text.Json.Serialization;
using Crayon.API.Domain.DTOs.Base;
using Crayon.API.Domain.DTOs.Requests.Common;

namespace Crayon.API.Domain.DTOs.Requests.Internal.Customers;

public class CustomerAccountListRequest : BaseFilterRequest
{
    [JsonIgnore] public CustomerAccountDetailsContextModel? UserContext { get; set; } = null;
}