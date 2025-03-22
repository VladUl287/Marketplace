using Product.Api;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddVault();

    builder.AddOptions();

    builder.AddDatabase();

    builder.AddEventBus();

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
