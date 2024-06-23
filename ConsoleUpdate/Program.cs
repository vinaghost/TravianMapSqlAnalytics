using ConsoleUpdate.Commands;
using ConsoleUpdate.Services;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host
    .CreateApplicationBuilder(args);

builder.BindConfiguration();

builder.Services.AddCore();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services
    .AddHttpClient<GetServerListCommand>()
    .ConfigurePrimaryHttpMessageHandler(builder =>
         new HttpClientHandler
         {
             ClientCertificateOptions = ClientCertificateOption.Manual
             ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
         }
    );

builder.Services.AddHttpClient();

builder.Services.AddHostedService<StartUpService>();

var host = builder.Build();
await host.RunAsync();
