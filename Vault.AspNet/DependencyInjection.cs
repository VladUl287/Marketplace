using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Vault.AspNet.Jobs;

namespace Vault.AspNet;

public static class DependencyInjection
{
    public static IConfigurationBuilder AddVaultKeyValueSource(this IConfigurationBuilder builder, Action<VaultConfigurationSource.VaultSettings> options)
    {
        var settings = new VaultConfigurationSource.VaultSettings();
        options(settings);
        return builder.Add(new VaultConfigurationSource(settings));
    }

    public static IServiceCollection AddVaultHostedService(this IServiceCollection services, Action<SimpleScheduleBuilder> schedule)
    {
        services.AddQuartz(options =>
        {
            var refreshJobKey = JobKey.Create(nameof(RefreshConfigJob));
            options.AddJob<RefreshConfigJob>(refreshJobKey)
                .AddTrigger(c => c.ForJob(refreshJobKey)
                    .WithSimpleSchedule(schedule));
        });
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
