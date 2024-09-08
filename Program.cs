using Microsoft.EntityFrameworkCore;
using ChatingApp.Context;
using ChatingApp.Helpers;
using ChatingApp.Services;
using ChatingApp.Middlewares;
using ChatingApp.Services.Implements;
using ChatingApp.Services.Interfaces;
using ChatingApp;

var builder = WebApplication.CreateBuilder(args);

// Add CORS for React.JS
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

ChattingModule.ConfigureServices(builder.Services, builder.Configuration);

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
