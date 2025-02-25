using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebChatV1.DAL;
using Serilog;
using WebChatV1.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u4}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<MessageCleanupService>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "WebChat API", Version = "v1" });
});

// builder.Services.AddDbContext<RootContext>(builder =>
// {
//     builder.UseNpgsql("User ID=postgres;Password=12345;Host=localhost;Port=5432;Database=Web_Chat;");
//     builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
// });

builder.Services.AddDbContext<RootContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

await Task.Delay(5000);
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RootContext>();
    await dbContext.Database.MigrateAsync();
    dbContext.Dispose();
}

app.UseSwagger();
app.UseRouting();
app.UseWebSockets();
app.MapControllers();
app.UseSerilogRequestLogging();

app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "WebChat API v1"));

app.Map("/ws", async webSocketsContext =>
{
    var myHandler = webSocketsContext.RequestServices.GetRequiredService<IWebSocketHandler>();
    await myHandler.HandleWebSocketAsync(webSocketsContext);
});

app.UseEndpoints(ep =>
{
    ep.MapControllers();
    ep.MapControllerRoute(name: "default", pattern: "{controller=Clients}/{action=Index}/{id?}");
});

// var scope = app.Services.CreateScope();
// var migrationContext = scope.ServiceProvider.GetRequiredService<RootContext>();
// await migrationContext.Database.MigrateAsync();
// scope.Dispose();

app.Run();
