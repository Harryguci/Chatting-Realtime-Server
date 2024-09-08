using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Services.Implements;
using ChatingApp.Services.Interfaces;
using ChatingApp.Services;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp;

public static class ChattingModule
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<IWebsocketHandler, WebsocketHandler>();

        // configure strongly typed settings object
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        // configure DI for application services
        services.AddScoped<IUserService, UserService>();

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

        // Add the context
        services.AddDbContext<ChatingContext>(
                   dbContextOptions => dbContextOptions
                       .UseMySql(configuration.GetConnectionString("ChatingDb"), serverVersion)
                       // The following three options help with debugging, but should
                       // be changed or removed for production.
                       .LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors()
               );

    }
}