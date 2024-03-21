using Microsoft.EntityFrameworkCore;
using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Services;
using ChatingApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IWebsocketHandler, WebsocketHandler>();

// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// configure DI for application services
builder.Services.AddScoped<IUserService, UserService>();

// Add Cors for React Js
var myAllowSpecificOrigins = "chating-application";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
    builder =>
    {
        builder.WithOrigins(
            "http://localhost:3000",
            "https://localhost:3000").AllowAnyHeader().AllowAnyMethod();
    });
});

// Add the context
builder.Services.AddDbContextPool<ChatingContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("ChatingDb")));

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseWebSockets();

app.UseAuthorization();

app.MapControllers();

// Terminate Middleware
app.Run();
