using MassTransit;
using Microsoft.EntityFrameworkCore;
using Product.Api.Options;
using Product.Core.Events;
using Product.Infrastructure.Data;
using Vault.AspNet;

namespace Product.Api.Extensions;

public static class ServicesExtensions
{
    public static IHostApplicationBuilder AddVault(this IHostApplicationBuilder builder)
    {
        builder.Configuration.AddVaultKeyValueSource((options) =>
        {
            options.Token = "";
            options.Url = "http://localhost:8200";
            options.Path = "product";
            options.MountPoint = "secret";
        });

        builder.Services.AddVaultHostedService((schedule) => schedule
            .WithIntervalInSeconds(60)
            .RepeatForever());

        return builder;
    }

    public static IHostApplicationBuilder AddEventBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<ConsumerSecond>();
                rider.AddConsumer<KafkaMessageConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    k.TopicEndpoint<ProductCreatedEvent>("test-topic", "product-group-name", e =>
                    {
                        e.ConfigureConsumer<ConsumerSecond>(context);
                        e.ConfigureConsumer<KafkaMessageConsumer>(context);
                    });
                });
            });
        });

        return builder;
    }

    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        var dbConnection = builder.Configuration.GetValue<string>("Database:ConnectionString");
        ArgumentNullException.ThrowIfNull(dbConnection);

        builder.Services.AddDbContext<ProductsDbContext>(options =>
        {
            options.UseSqlite(dbConnection);
        });

        return builder;
    }

    public static IHostApplicationBuilder AddOptions(this IHostApplicationBuilder builder)
    {
        var dbOptions = builder.Configuration.GetSection(DbOptions.Position);
        builder.Services.Configure<DbOptions>(dbOptions);

        return builder;
    }
}
