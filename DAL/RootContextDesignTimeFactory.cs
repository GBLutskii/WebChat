using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebChatV1.DAL;

public class RootContextDesignTimeFactory : IDesignTimeDbContextFactory<RootContext>
{
    public RootContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RootContext>();
        optionsBuilder.UseNpgsql();
        
        return new RootContext(optionsBuilder.Options);
    }
}