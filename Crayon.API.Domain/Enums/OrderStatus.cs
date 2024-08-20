using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Crayon.API.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    [Description("Declined")]
    Declined = -2,
    [Description("Cancelled")]
    Cancelled,
    [Description("Pending")]
    Pending,
    [Description("Processing")]
    Processing,
    [Description("Processed")]
    Processed
}