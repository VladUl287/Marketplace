using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Vault.AspNet.Jobs;

[DisallowConcurrentExecution]
public sealed class RefreshConfigJob(ILogger<RefreshConfigJob> logger, IConfiguration configuration) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (configuration is not IConfigurationRoot configurationRoot)
            {
                return Task.CompletedTask;
            }

            logger.LogInformation("Reload configuration start '{now}'.", DateTime.UtcNow);
            configurationRoot.Reload();
            logger.LogInformation("Reload configuration end.");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Reload configuration fail.");
            return Task.CompletedTask;
        }
    }
}
