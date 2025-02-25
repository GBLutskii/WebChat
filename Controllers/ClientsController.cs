using Microsoft.AspNetCore.Mvc;

namespace WebChatV1.Controllers;

public class ClientsController : Controller
{
    public IActionResult Sender() => View();
    
    public IActionResult Recipient() => View();
    
    public IActionResult History() => View();
}