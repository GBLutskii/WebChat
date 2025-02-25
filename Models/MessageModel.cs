using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebChatV1.Models;

public class MessageModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime TimeSent { get; set; }
    
    [Required]
    [JsonPropertyName("sequenceNumber")]
    public int OrderNumber { get; set; }
}