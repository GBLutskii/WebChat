using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebChatV1.Models;

namespace WebChatV1.Services;

public class WebSocketHandler : IWebSocketHandler
{
    private readonly List<WebSocket> _sockets = new();
    private readonly ILogger<WebSocketHandler> _logger;

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(socket);
            _logger.LogInformation("WebSocket client connected. Total clients: {Count}", _sockets.Count);

            while (socket.State == WebSocketState.Open)
            {
                await Task.Delay(1000);
            }

            _sockets.Remove(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            _logger.LogInformation("WebSocket client disconnected. Total clients: {Count}", _sockets.Count);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    public async Task SendMessageToAllAsync(MessageModel message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(messageJson);

        foreach (var socket in _sockets.Where(s => s.State == WebSocketState.Open))
        {
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        _logger.LogInformation("Message sent to {Count} WebSocket clients: {Text}", _sockets.Count, message.Text);
    }
}