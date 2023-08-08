using Quartz;

namespace MapSqlQuery.Jobs
{
    public class UpdateDatabaseJob : IJob
    {
        public static readonly JobKey Key = new(nameof(UpdateDatabaseJob));

        private readonly ILogger<UpdateDatabaseJob> _logger;

        public UpdateDatabaseJob(ILogger<UpdateDatabaseJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Started {job}", nameof(UpdateDatabaseJob));
            return Task.FromResult(true);
        }
    }
}