using WebChatV1.Models;

namespace WebChatV1.Services;

public interface IWebSocketHandler
{
    Task HandleWebSocketAsync(HttpContext context);
    
    Task SendMessageToAllAsync(MessageModel message);
}