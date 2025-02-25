using System.Text.Json.Serialization;

namespace WebChatV1.Models;

public class MessageRequest
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("sequenceNumber")]
    public int OrderNumber { get; set; }
}