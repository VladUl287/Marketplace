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

        var serviceName = "aspnet";
        var serviceVersion = "1.0.0";
        var configuration = builder.Configuration;
        var resource = ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion);
        builder.Services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();

                builder.AddMeter("Microsoft.AspNetCore.Hosting",
                                 "Microsoft.AspNetCore.Server.Kestrel");
                builder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                    });

                //tcb
                //    .AddMeter(serviceName)
                //    .SetResourceBuilder(resource)
                //    .AddAspNetCoreInstrumentation()
                //    .AddConsoleExporter()
                //    .AddPrometheusExporter()
                //;
                //tcb.AddView("http.server.request.duration",
                //    new ExplicitBucketHistogramConfiguration
                //    {
                //        Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                //                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                //    });
            })
            ;
        //builder.Services.AddOpenTelemetry()
        //      .ConfigureResource(resource => resource.AddService("aspnet"))
        //      .WithTracing(tracing => tracing
        //          .AddSource("aspnet")
        //          .AddAspNetCoreInstrumentation()
        //          .AddConsoleExporter()
        //          .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration)))
        //      .WithMetrics(metrics => metrics
        //          .AddMeter("aspnet")
        //          .AddAspNetCoreInstrumentation()
        //          .AddConsoleExporter()
        //          .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration)));
        //builder.Logging.AddOpenTelemetry(options =>
        //{
        //    options
        //        .SetResourceBuilder(resource)
        //        .AddConsoleExporter()
        //        .AddOtlpExporter((options) => OtlpConfig(options, builder.Configuration))
        //        ;
        //});

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

        //builder.Services.AddVaultHostedService((schedule) => schedule
        //    .WithIntervalInSeconds(60)
        //    .RepeatForever());

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
