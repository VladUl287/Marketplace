using FastEndpoints;
using Product.Api;
using Product.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddFastEndpoints();

    builder.AddVault();

    builder.AddOtpl();
    
    builder.AddOptions();

    builder.AddDatabase();

    builder.AddKafkaBus();

    builder.Services.AddHostedService<Hosted>();

    builder.Services.AddControllers();

    builder.Services.AddOpenApi();
}

var app = builder.Build();
{
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
