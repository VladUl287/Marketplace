using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Product.Api.Consumers;
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

    public static IHostApplicationBuilder AddKafkaBus(this IHostApplicationBuilder builder)
    {
        //builder.Services.AddTransient<IProducer<Null, string>>(serviceProvider =>
        //{
        //    var config = new ProducerConfig
        //    {
        //        BootstrapServers = "localhost:9092"  // replace with your Kafka server
        //    };
        //    return new ProducerBuilder<Null, string>(config).Build();
        //});

        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<ProductCreatedConsumer>();
                rider.AddConsumer<ProductUpdatedConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:9092");

                    k.TopicEndpoint<ProductCreatedEvent>("product-created", "product-group-name", e =>
                    {
                        e.ConfigureConsumer<ProductCreatedConsumer>(context);
                        e.UseDelayedRedelivery(r => r.Intervals([
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(15),
                            TimeSpan.FromMinutes(30),
                            TimeSpan.FromMinutes(60)
                        ]));
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
                    });

                    k.TopicEndpoint<ProductUpdatedEvent>("product-updated", "product-group-name", e =>
                    {
                        e.ConfigureConsumer<ProductUpdatedConsumer>(context);
                        e.UseDelayedRedelivery(r => r.Intervals([
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(15),
                            TimeSpan.FromMinutes(30),
                            TimeSpan.FromMinutes(60)
                        ]));
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)));
                    });
                });
            });
        });

        return builder;
    }

    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        //var dbConnection = builder.Configuration.GetValue<string>("Database:ConnectionString");
        var dbConnection = "Data Source = custom.db";
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
