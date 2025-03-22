
using Scalar.AspNetCore;
using Serilog;
using WebAPI.Infrastructure;
using WebAPI.Presentation.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddInfrastructure(builder.Configuration, builder.Host);
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });

    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.BluePlanet;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var apiV1 =  app.MapGroup("/api/v1");

apiV1.MapGroup("users").MapUsers();

app.Run();
