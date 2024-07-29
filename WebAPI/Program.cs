using Application.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Features;
using WebAPI.Extensions;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.BindConfiguration();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMiddleware();

builder.Services
    .AddCore()
    .AddFeatures();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();
app.UseMiddleware<ServerMiddleware>();
app
    .UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();