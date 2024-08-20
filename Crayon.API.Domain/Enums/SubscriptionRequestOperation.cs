using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Crayon.API.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionRequestOperation
{
    [Description("Update")]
    Update = 0,
    
    [Description("Cancel")]
    Cancel = 10,
    
    [Description("Extend")]
    Extend = 20
}