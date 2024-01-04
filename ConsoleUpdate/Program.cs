using ConsoleUpdate.Services;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.BindConfiguration();

builder.Services.AddCore();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddHttpClient();
builder.Services.AddHostedService<StartUpService>();

var host = builder.Build();
await host.RunAsync();