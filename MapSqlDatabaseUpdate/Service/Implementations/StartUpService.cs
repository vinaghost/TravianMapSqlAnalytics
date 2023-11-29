using MapSqlDatabaseUpdate.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class StartUpService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StartUpService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly IMediator _mediator;

        public StartUpService(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, ILogger<StartUpService> logger, IMediator mediator)
        {
            _configuration = configuration;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var servers = await _mediator.Send(new GetServerListCommand(), cancellationToken);
            await _mediator.Send(new UpdateServerListCommand(servers), cancellationToken);
            _hostApplicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}