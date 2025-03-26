using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Api.Consumers;
using Product.Api.Options;
using Product.Core.Events;
using Product.Infrastructure.Data;
using Vault.AspNet;

namespace Product.Api.Extensions;

public static class ServicesExtensions
{
    public static IHostApplicationBuilder AddOtpl(this IHostApplicationBuilder builder)
    {
        //builder.Logging.ClearProviders();

        static void OtlpConfig(OtlpExporterOptions exOptions, IConfiguration configuration)
        {
            var config = configuration.GetSection("OpenTelemetry");
            var url = config.GetValue<string>("Connection") ?? throw new NullReferenceException();
            var headers = config.GetValue<string>("Headers") ?? throw new NullReferenceException();

            exOptions.Endpoint = new Uri(url);
            exOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            exOptions.Headers = headers;
        }

        builder.Logging.AddOpenTelemetry(options =>
        {
            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("aspnet"))
                .AddConsoleExporter()
                .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration));
        });
        var configuration = builder.Configuration;
        builder.Services.AddOpenTelemetry()
              .ConfigureResource(resource => resource.AddService("aspnet"))
              .WithTracing(tracing => tracing
                  .AddSource("aspnet")
                  .AddAspNetCoreInstrumentation()
                  .AddConsoleExporter()
                  .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration)))
              .WithMetrics(metrics => metrics
                  .AddMeter("aspnet")
                  .AddAspNetCoreInstrumentation()
                  .AddConsoleExporter()
                  .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration)));

        return builder;
    }

    public static IHostApplicationBuilder AddVault(this IHostApplicationBuilder builder)
    {
        builder.Configuration.AddVaultKeyValueSource((options) =>
        {
            options.Token = "root";
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
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                    });

                    k.TopicEndpoint<ProductUpdatedEvent>("product-updated", "product-group-name", e =>
                    {
                        e.ConfigureConsumer<ProductUpdatedConsumer>(context);
                        //e.UseDelayedRedelivery(r => r.Intervals([
                        //    TimeSpan.FromMinutes(5),
                        //    TimeSpan.FromMinutes(15),
                        //    TimeSpan.FromMinutes(30),
                        //    TimeSpan.FromMinutes(60)
                        //]));
                        e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                    });
                });
            });
        });

        return builder;
    }

    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        var dbConnection = builder.Configuration.GetValue<string>("Database:Connection");
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
