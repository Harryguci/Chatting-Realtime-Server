using Microsoft.EntityFrameworkCore;
using New.Namespace;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add Cors for React Js
var myAllowSpecificOrigins = "harryguci";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
    builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://localhost:3000").AllowAnyHeader().AllowAnyMethod();
    });
});

// Add the context
builder.Services.AddDbContextPool<ChatingContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("ChatingDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
