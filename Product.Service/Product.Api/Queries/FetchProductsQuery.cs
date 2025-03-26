namespace Product.Api.Queries;

public sealed class FetchProductsQuery {
    public int Page { get; init; }
    public int Size { get; init; }
}
