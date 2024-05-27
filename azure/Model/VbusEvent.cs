using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace Visma.Business.WebhookExample
{

    public class VbusEvent
    {
        [JsonConstructor]
        public VbusEvent(string tableIdentifier, int customerNo, int companyNo, JsonArray primaryKeys) { 
            TableIdentifier = tableIdentifier;
            CustomerNo = customerNo;
            CompanyNo = companyNo;
            PrimaryKeys = primaryKeys;
        }
        [JsonPropertyName("tableIdentifier")]
        public string TableIdentifier { get; set; }
        [JsonPropertyName("customerNo")]
        public int CustomerNo { get; set; }
        [JsonPropertyName("companyNo")]
        public int CompanyNo { get; set; }
        [JsonPropertyName("primaryKeys")]
        public JsonArray PrimaryKeys { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("event")]
        public EventType Event { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}
