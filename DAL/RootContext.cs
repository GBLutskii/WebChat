using Microsoft.EntityFrameworkCore;
using WebChatV1.Models;

namespace WebChatV1.DAL;

public class RootContext : DbContext
{
    public RootContext(DbContextOptions option) : base(option) {}
    
    public DbSet<MessageModel> Messages { get; set; }
}