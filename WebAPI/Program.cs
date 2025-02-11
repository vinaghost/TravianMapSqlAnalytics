using Application.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Features;

var builder = WebApplication.CreateBuilder(args);

builder.BindConfiguration();

builder.Services
    .AddCore()
    .AddFeatures();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app
    .UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();