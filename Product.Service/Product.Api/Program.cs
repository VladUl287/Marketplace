using FastEndpoints;
using OpenTelemetry.Trace;
using Product.Api;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddHybridCache();

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379,password=password,abortConnect=false";
    });

    builder.Services.AddControllers();

    builder.Services.AddFastEndpoints();

    //builder.AddVault();

    builder.AddOtpl();
    
    //builder.AddOptions();

    //builder.AddDatabase();

    //builder.AddKafkaBus();

    //builder.Services.AddHostedService<Hosted>();

    //builder.Services.AddOpenApi();
}

var app = builder.Build();
{
    app.MapPrometheusScrapingEndpoint();
    app.MapControllers();

    app.UseFastEndpoints()
        .UseSwaggerUi();

    //if (app.Environment.IsDevelopment())
    //{
    //    app.MapOpenApi();
    //}

    //app.UseHttpsRedirection();

    //app.UseAuthorization();

    //app.MapControllers();
}
app.Run();
