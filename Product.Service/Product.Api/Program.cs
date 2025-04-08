using FastEndpoints;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddHybridCache(options =>
    {
        options.DefaultEntryOptions = new()
        {
            Expiration = TimeSpan.FromHours(3),
            LocalCacheExpiration = TimeSpan.FromHours(3)
        };
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379,password=password,abortConnect=false";
    });

    builder.Services.AddControllers();

    //builder.AddVault();

    builder.AddOtpl();

    builder.AddOptions();

    builder.AddDatabase();

    builder.AddKafkaBus();

    builder.Services.AddAuthorization();
    builder.Services.AddFastEndpoints();
}

var app = builder.Build();
{
    app.MapPrometheusScrapingEndpoint();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseFastEndpoints();

    app.MapControllers();
}
app.Run();
