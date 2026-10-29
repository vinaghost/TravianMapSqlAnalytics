using FastEndpoints;
using FastEndpoints.Swagger;

using Features;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureFeatures();
builder.ConfigureFeatures();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app
    .UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();