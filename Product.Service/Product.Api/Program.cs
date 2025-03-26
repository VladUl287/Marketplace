using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Api;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddVault();

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
