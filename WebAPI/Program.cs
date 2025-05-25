using FastEndpoints;
using FastEndpoints.Swagger;
using Features;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureInfrastructure();
builder.ConfigureFeatures();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app
    .UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();