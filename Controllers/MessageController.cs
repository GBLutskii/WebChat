using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChatV1.DAL;
using WebChatV1.Models;
using WebChatV1.Services;

namespace WebChatV1.Controllers;

[Route("api/messages")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly RootContext _context;
    private readonly ILogger<MessageController> _logger;
    private readonly IWebSocketHandler _webSocketHandler;

    public MessageController(RootContext context, ILogger<MessageController> logger, IWebSocketHandler webSocketHandler)
    {
        _context = context;
        _logger = logger;
        _webSocketHandler = webSocketHandler;
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
    {
        if (request.Text.Length > 128)
        {
            _logger.LogWarning("Message length exceeds 128 characters!");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return BadRequest("Message text should not exceed 128 characters!");
        }

        var message = new MessageModel
        {
            Text = request.Text,
            OrderNumber = request.OrderNumber,
            TimeSent = DateTime.UtcNow
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Message saved: {Text}, OrderNumber: {OrderNumber}", message.Text, message.OrderNumber);

        await _webSocketHandler.SendMessageToAllAsync(message);

        return Ok();
    }
    
    [HttpGet("history")]
    public IActionResult GetMessageHistory([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var messages = _context.Messages
            .Where(msg => msg.TimeSent >= start && msg.TimeSent <= end)
            .OrderBy(msg => msg.TimeSent)
            .ToList();

        _logger.LogInformation("Retrieved {Count} messages for range {Start} to {End}", messages.Count, start, end);
        
        return Ok(messages);
    }
}
