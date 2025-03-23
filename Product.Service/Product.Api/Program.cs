using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Api;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    //builder.Logging.ClearProviders();

    static void OtlpConfig(OtlpExporterOptions exOptions)
    {
        exOptions.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        exOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        exOptions.Headers = "";
    }

    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("aspnet"))
            .AddConsoleExporter()
            .AddOtlpExporter(OtlpConfig)
            ;
    });
    builder.Services.AddOpenTelemetry()
          .ConfigureResource(resource => resource.AddService("aspnet"))
          .WithTracing(tracing => tracing
              .AddSource("aspnet")
              .AddAspNetCoreInstrumentation()
              .AddConsoleExporter()
              .AddOtlpExporter(OtlpConfig))
          .WithMetrics(metrics => metrics
              .AddMeter("aspnet")
              .AddAspNetCoreInstrumentation()
              .AddConsoleExporter()
              .AddOtlpExporter(OtlpConfig));

    //builder.AddVault();

    builder.AddOptions();

    builder.AddDatabase();

    builder.AddKafkaBus();

    builder.Services.AddHostedService<Hosted>();

    builder.Services.AddMediatR(options =>
    {
        options.RegisterServicesFromAssembly(typeof(IApiMarker).Assembly);
    });

    builder.Services.AddControllers();

    builder.Services.AddOpenApi();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}
app.Run();
