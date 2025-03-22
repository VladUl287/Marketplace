namespace Product.Api.Options;

public sealed class DbOptions
{
    public const string Position = "Database";
    public required string ConnectionString { get; init; }
}
